using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : Growable
{
    public Flower(Plant plant) : base(plant)
    {

    }

    public override Growable Child { get; set; } = null;

    public override void Render(MeshData data, System.Random random, Vector3 translation, Quaternion rotation)
    {
        //TODO:
    }
}
