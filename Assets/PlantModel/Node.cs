using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : Growable
{
    public Node(Growable child, Plant plant) : base(plant)
    {
        Child = child;
    }

    public override Growable Child { get; set; }
    public List<Growable> Children { get; set; } = new List<Growable> { };

    public override bool Grow(float time)
    {
        base.Grow(time);

        //logic to add more children
        NaiveGrowthLogic();

        var childJustSprouted = Child.Grow(time);

        foreach (var child in Children)
        {
            childJustSprouted |= child.Grow(time);
        }

        return JustSprouted || childJustSprouted;
    }

    private void NaiveGrowthLogic()
    {
        var rand = UnityEngine.Random.Range(0f, 1f);

        if (Age >= 1 && !Sprouted)
        {
            Sprouted = true;
           
            if (rand < 0.02f)
            {
                Flowers(3);
            }
            else if (rand < 0.95f)
            {
                Leaves(3);
                if (rand < 0.4)
                {
                    StemTips(3);
                }
            }
            else
            {
                StemTips(3);
            }
        }
    }

    private void Flowers(int n)
    {
        Children = new List<Growable>();
        for (int i = 0; i < n; i++)
        {
            Children.Add(new Flower(Plant));
        }
    }

    private void Leaves(int n)
    {
        Children = new List<Growable>();
        for (int i = 0; i < n; i++)
        {
            Children.Add(new Leaf(Plant));
        }
    }

    private void StemTips(int n)
    {
        Children = new List<Growable>();
        for (int i = 0; i < n; i++)
        {
            Children.Add(new Stem(Plant));
        }
    }

    public override void Render(MeshData data, System.Random random, Vector3 translation, Quaternion rotation)
    {
        var radial = Quaternion.AngleAxis(CachedRandomValue(0, random) * 360f, rotation * Vector3.up);
        var ang = Quaternion.AngleAxis(CachedRandomValue(1, random) * 25f, rotation * Vector3.forward);

        var offset = 0.5f * Height;
        RenderHelper.CreateCylinder(data, translation, rotation, this, height: offset);
        
        Child.Render(data, random, translation + rotation * new Vector3(0f, offset, 0f), radial * ang * rotation);
        

        var angle = 360f / Children.Count;
        var child_ang = Quaternion.AngleAxis(CachedRandomValue(2, random) * 45f, rotation * Vector3.forward);
        //iterate over children 
        for (int i = 0; i < Children.Count; i++)
        {
            var rad = Quaternion.AngleAxis(CachedRandomValue(3 + i, random) + angle * i, rotation * Vector3.up);
            Growable child = Children[i];
            child.Render(data, random, translation, rad * child_ang * rotation);
        }
    }
}