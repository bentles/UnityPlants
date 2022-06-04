using Assets.PlantModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Flower : Growable
{
    public Flower(Plant plant) : base(plant)
    {

    }

    public override Growable Child { get; set; } = null;

    public override bool ChildGrowth(float time)
    {
        return false;
    }

    public override void Render(MeshData data, System.Random random, RenderContext renderContext, CancellationToken ct)
    {
        //TODO:
    }
}
