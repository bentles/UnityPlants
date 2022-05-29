using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stem : Growable
{
    public bool IsTip { get; set; } = true;

    public Stem(Growable child, Plant plant): base(plant)
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


    public override bool ChildGrowth(float time)
    {
        var timeLeft = NaiveGrowthLogic(time);

        if (Child != null)
        {
            return Child.Grow(timeLeft);
        }

        return false;
    }

    private float NaiveGrowthLogic(float time)
    {
        if (!IsTip)
        {
            return time; //only the tip can grow
        }

        if (Age >= 2 && !Sprouted)
        {
            Sprouted = true;
            var rand = UnityEngine.Random.Range(0, 1f);
            if (rand < 0.1f)
            {
                StemTip();
            }
            else
            {
                NodeStemTip();
            }

            return time - 2 * Plant.GrowthStepTime;
        }

        return time;
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

    public override void Render(MeshData data, System.Random random, Vector3 translation, Quaternion rotation)
    {
        if (Age < 0)
        {
            return;
        }

        if(IsTip)
        {
            RenderTip(data, translation, rotation);
            return;
        }


        if (!Plant.LevelOfDetail ||
            (Plant.LevelOfDetail && Age > Mathf.Min(Plant.LodCutOff, Plant.LevelOfDetailMinAge)))
        {
            RenderBranchSegment(data, random, translation, rotation);
        }
        else
        {
            //TODO: possibly can make this more sophisticated
            RenderTip(data, translation, rotation);
        }

    }

    private void RenderBranchSegment(MeshData data, System.Random random, Vector3 translation, Quaternion rotation)
    {
        var radial = Quaternion.AngleAxis(CachedRandomValue(0, random) * 360f, rotation * Vector3.up);
        var ang = Quaternion.AngleAxis(CachedRandomValue(1, random) * 25f, rotation * radial * Vector3.forward);

        var offset = Height;
        RenderHelper.CreateBranchSegment(data, translation, rotation, this, height: offset);

        Child.Render(data, random, translation + rotation * new Vector3(0f, offset, 0f), ang * rotation);
    }

    private void RenderTip(MeshData data, Vector3 translation, Quaternion rotation)
    {
        var offset = Height;
        RenderHelper.CreateBranchSegment(data, translation, rotation, topR: 0f, botR: CalcRadius(Age), height: offset);
    }
}
