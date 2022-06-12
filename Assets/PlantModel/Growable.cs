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
    }

    public Plant Plant { get; }

    public int Age { get; set; }

    private float ElapsedTime { get; set; }

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

    //remember angle so it does not change on re-render
    private readonly Dictionary<int, float> randomAngles = new();
    public float CachedRandomValue(int i, System.Random random) {
        if (!randomAngles.ContainsKey(i))
        {
            return randomAngles[i] = (float)random.NextDouble();
        }
        return randomAngles[i];
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

    public abstract void Render(MeshData meshData, System.Random random, RenderContext renderContext, CancellationToken ct);

    public abstract Growable Child { get; set; }

}
