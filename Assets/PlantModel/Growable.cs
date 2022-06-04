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

    private float ElapsedTime { get; set; } = 0f;

    /// <param name="time">Elapsed time</param>
    /// <returns>If we should re-render</returns>
    public bool Grow(float time)
    {
        ElapsedTime += time;

        var startAge = Age;

        if (ElapsedTime > Plant.GrowthStepTime && Age < Plant.MaxAge)
        {
            var div = Mathf.FloorToInt(ElapsedTime / Plant.GrowthStepTime);
            div = Mathf.Min(div, Plant.MaxAge - Age);

            Age += div;
            ElapsedTime -= Plant.GrowthStepTime * div;
        }

        return Age < Plant.MaxAge && ChildGrowth(time);
    }

    public abstract bool ChildGrowth(float time);

    private bool _justSprouted;
    public bool JustSprouted { get { return _justSprouted; } }

    private bool _sprouted;
    public bool Sprouted
    {
        get { return _sprouted; }
        set
        {
            _justSprouted = false;
            if (!_sprouted && value)
            {
                _justSprouted = true;
            }
            _sprouted = value;
        }
    }

    //remember angle so it does not change on re-render
    private Dictionary<int, float> randomAngles = new Dictionary<int, float>();
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
        ElapsedTime = 0;
        _justSprouted = false;
        _sprouted = false;
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
