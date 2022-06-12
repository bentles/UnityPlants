using Assets.PlantModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Node : Growable
{
    public Node(Growable child, Plant plant) : base(plant)
    {
        Child = child;
    }

    public override Growable Child { get; set; }
    public List<Growable> Children { get; set; } = new List<Growable> { };


    public override bool ChildGrowth()
    {
        //logic to add more children
        SproutingLogic();

        var descendentGrowth = Child.Grow();

        foreach (var child in Children)
        {
            descendentGrowth |= child.Grow();
        }

        return descendentGrowth;
    }

    private void SproutingLogic()
    {
        var rand = UnityEngine.Random.Range(0f, 1f);

        if (Age >= 1)
        {
            if (!Children.Any())
            {
                if (rand < 0.02f)
                {
                    Flowers(3);
                }
                else if (rand < 0.5f)
                {
                    Leaves(3);
                }
                else if (rand < 1f)
                {
                    StemTips(3);
                }

            }
        }
    }

    private void Flowers(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Children.Add(new Flower(Plant));
        }
    }

    private void Leaves(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Children.Add(new Leaf(Plant));
        }
    }

    private void StemTips(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Children.Add(new Stem(Plant)
            {
                Age = -3
            });
        }
    }

    public override void Render(MeshData data, RenderContext renderContext, CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
        {
            return;
        }

        var rotation = renderContext.Rotation;
        var translation = renderContext.Translation;

        var radial = Quaternion.AngleAxis(Plant.GetRandom(Id ,0) * 360f, rotation * Vector3.up);
        var ang = Quaternion.AngleAxis(Plant.GetRandom(Id, 1) * 25f, radial * Vector3.forward);

        var offset = 0.5f * Height;
        RenderHelper.CreateBranchSegment(data, renderContext, this, height: offset);

        var childRenderContext = new RenderContext
        {
            Translation = translation + rotation * new Vector3(0f, offset, 0f),
            Rotation = ang * rotation,
        };
        childRenderContext.Distance = renderContext.Distance + Vector3.Distance(translation, childRenderContext.Translation);

        Child.Render(data, childRenderContext, ct);

        var angle = 360f / Children.Count;
        var child_ang = Quaternion.AngleAxis(Plant.GetRandom(Id, 2) * 45f, rotation * Vector3.forward);
        //iterate over children 
        for (int i = 0; i < Children.Count; i++)
        {
            var rad = Quaternion.AngleAxis(Plant.GetRandom(Id, 3 + i) + angle * i, rotation * Vector3.up);
            Growable child = Children[i];

            var childiRenderContext = new RenderContext
            {
                Translation = translation,
                Rotation = rad * child_ang * rotation,
            };
            childRenderContext.Distance = renderContext.Distance + Vector3.Distance(translation, childRenderContext.Translation);

            child.Render(data, childiRenderContext, ct);
        }
    }

    public override int TotalChildren()
    {
        return Children.Count + base.TotalChildren();
    }
}
