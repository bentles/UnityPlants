using Assets.PlantModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Stem : Growable
{
    public bool IsTip { get; set; } = true;

    public Stem(Growable child, Plant plant) : base(plant)
    {
        Child = child;
        child.Reset();
        IsTip = false;
    }

    public Stem(Plant plant) : base(plant)
    {
    }

    private Growable _child;

    public override Growable Child { get { return _child; } set { _child = value; IsTip = value == null; } }


    public override bool ChildGrowth()
    {
        NaiveGrowthLogic();
        if (Child != null)
        {
            //grandchild grew
            return Child.Grow();
        }

        return false;
    }

    private void NaiveGrowthLogic()
    {
        if (!IsTip)
        {
            return; //only the tip can grow
        }

        // it is the tip so can grow:
        if (Age >= 2 && Child == null)
        {
            var rand = UnityEngine.Random.Range(0, 1f);
            if (rand < 0.1f)
            {
                StemTip();
            }
            else
            {
                NodeStemTip();
            }

        }
    }

    private void StemTip()
    {
        var stem = new Stem(Plant);
        Child = stem;
    }

    private void NodeStemTip()
    {
        var stem = new Stem(Plant);
        var node = new Node(stem, Plant);
        Child = node;
    }

    public override void Render(MeshData data, System.Random random, RenderContext renderContext, CancellationToken ct)
    {
        var translation = renderContext.Translation;
        var rotation = renderContext.Rotation;

        if (Age < 0 || ct.IsCancellationRequested)
        {
            return;
        }

        if (IsTip)
        {
            RenderTip(data, renderContext);
            return;
        }


        if (!Plant.LevelOfDetail ||
            (Plant.LevelOfDetail && Age > Mathf.Min(Plant.LodCutOff, Plant.LevelOfDetailMinAge)))
        {
            RenderBranchSegment(data, random, renderContext, ct);
        }
        else
        {
            //TODO: possibly can make this more sophisticated
            RenderTip(data, renderContext);
        }

    }

    private void RenderBranchSegment(MeshData data, System.Random random, RenderContext renderContext, CancellationToken ct)
    {
        var rotation = renderContext.Rotation;
        var translation = renderContext.Translation;

        var radial = Quaternion.AngleAxis(CachedRandomValue(0, random) * 360f, rotation * Vector3.up);
        var ang = Quaternion.AngleAxis(CachedRandomValue(1, random) * 25f, radial * Vector3.forward);

        var offset = Height;
        RenderHelper.CreateBranchSegment(data, renderContext, this, height: offset);

        var childRenderContext = new RenderContext
        {
            Translation = translation + rotation * new Vector3(0f, offset, 0f),
            Rotation = ang * rotation
        };
        childRenderContext.Distance = renderContext.Distance + Vector3.Distance(translation, childRenderContext.Translation);

        //Debug.Log("stem");
        //Debug.Log(CachedRandomValue(0, random));
        //Debug.Log(CachedRandomValue(1, random));

        Child.Render(data, random, childRenderContext, ct);
    }

    private void RenderTip(MeshData data, RenderContext renderContext)
    {
       // Debug.Log("tip");

        var offset = Height;
        RenderHelper.CreateBranchSegment(data, renderContext, topR: 0f, botR: CalcRadius(Age), height: offset);
    }
}
