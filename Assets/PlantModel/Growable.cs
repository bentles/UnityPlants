using Assets.PlantModel;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class Growable : IRenderable
{
    public Growable(Plant plant)
    {
        Plant = plant;
        Id = Guid.NewGuid();
    }

    public Plant Plant { get; }

    public int Age { get; set; }

    public Guid Id { get; private set; }


    /// <param name="time">Total Elapsed time</param>
    /// <returns>If we should re-render</returns>
    public bool Grow()
    {
        if (Age > Plant.MaxAge)
        {
            return false;
        }

        Age += 1;
        var before = TotalChildren();
        var newDescedents = ChildGrowth();
        var after = TotalChildren();
        var newChild = before != after;

        return newChild || newDescedents;

    }

    public int BranchLength()
    {
        return Child == null ? 1 : 1 + Child.BranchLength();
    }

    public abstract bool ChildGrowth();
    public virtual int TotalChildren()
    {
        return Child == null ? 0 : 1;
    }

    public void Reset()
    {
        Age = 0;
    }

    public float Height { get { return CalcHeight(); } }

    private float CalcHeight()
    {
        return Plant.heightCurve.Evaluate(Age);
    }

    public float CalcRadius(float age)
    {
        return Plant.radiusCurve.Evaluate(age);
    }

    public abstract void Render(MeshData meshData, RenderContext renderContext, CancellationToken ct);

    public abstract Growable Child { get; set; }

}
