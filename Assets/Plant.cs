using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Plant : MonoBehaviour
{
    public Mesh mesh;
    public MeshData meshData;
    public AnimationCurve radiusCurve;
    public AnimationCurve heightCurve;

    Stem Child;
    public int MaxAge = 30;
    public int RandomSeed;
    public float GrowthStepTime = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        meshData = new MeshData()
        {
            Vertices = new List<Vector3>(),
            Triangles = new List<int>(),
            LeafTriangles = new List<int>(),
        };

        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.subMeshCount = 2;
        GetComponent<MeshFilter>().mesh = mesh;
        RandomSeed = 112;
        Child = new Stem(this)
        {
            Sprouted = true,
        };
        
    }


    private void Update()
    {
        var hasGrown = Child.Grow(Time.deltaTime);
        if (hasGrown)
        {
            Reset();
            RenderGrowable(Child, new System.Random(RandomSeed));
            UpdateMesh();
        }
    }

    private void Reset()
    {
        meshData.Clear();
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.subMeshCount = 2;
        mesh.SetVertices(meshData.Vertices);
        mesh.SetTriangles(meshData.Triangles, 0);
        mesh.SetTriangles(meshData.LeafTriangles, 1);
        mesh.RecalculateNormals();
    }

    //I would like for this to be a pure function (must take a seed for rng stuff)
    //I would like to be able to lerp between growables in any state and see the render transform... somehow
    private void RenderGrowable(Growable g, System.Random random)
    {
        RenderGrowable(g, random, Vector3.zero, Quaternion.identity);
    }

    private void RenderGrowable(Growable g, System.Random random, Vector3 translation, Quaternion rotation)
    {
        if (g is null)
        {
            return;
        }

        g.Render(meshData, random, translation, rotation);
    }





}
