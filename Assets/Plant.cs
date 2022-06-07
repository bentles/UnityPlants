using Assets.PlantModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class Plant : MonoBehaviour
{
    public Mesh mesh;
    public MeshData meshData;
    public AnimationCurve radiusCurve;
    public AnimationCurve heightCurve;
    public Camera cameraReference;

    Stem Child;
    public int MaxAge = 30;

    public int StartAge = 20;
    public int RandomSeed;
    public float GrowthStepTime = 0.3f;
    public bool LevelOfDetail = false;
    public float LevelOfDetailScale = 0.2f;
    public float LevelOfDetailMinAge = 8;


    private Task RecreateMeshTask;
    CancellationTokenSource MeshTokenSource;

    public int LodCutOff { get; set; } = int.MaxValue;

    // Start is called before the first frame update
    void Start()
    {
        meshData = new MeshData()
        {
            Vertices = new List<Vector3>(),
            Triangles = new List<int>(),
            Uvs = new List<Vector2>(),
            LeafTriangles = new List<int>(),
        };

        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.subMeshCount = 2;
        mesh.MarkDynamic();

        GetComponent<MeshFilter>().mesh = mesh;
        RandomSeed = 112;
        Child = new Stem(this);

        MeshTokenSource = new CancellationTokenSource();

        //grow to start age
        Child.Grow(GrowthStepTime * StartAge);


    }

    internal int CalcLodCutoff()
    {
        if (cameraReference == null)
        {
            return 0;
        }

        float distToCamera = LevelOfDetailScale * Vector3.Distance(transform.position, cameraReference.transform.position);
        return Mathf.FloorToInt(distToCamera);
    }

    private void Update()
    {
        if (Child == null)
        {
            Start();
        }

        var newLod = CalcLodCutoff();
        var hasNewLodLevel = newLod != LodCutOff;
        LodCutOff = newLod;

        var hasGrown = Child.Grow(Time.deltaTime);

        if (hasGrown || hasNewLodLevel)
        {
            //cancel previous token's task
            MeshTokenSource.Cancel();

            MeshTokenSource = new CancellationTokenSource();
            var ct = MeshTokenSource.Token;

            RecreateMeshTask = Task.Run(() => CreateMeshData(ct), ct);
        }

        if (RecreateMeshTask != null && RecreateMeshTask.IsCompleted)
        {
            UpdateMesh();
            RecreateMeshTask = null;
        }
    }

    private void CreateMeshData(CancellationToken ct)
    {
        meshData.Clear();
        RenderGrowable(Child, new System.Random(RandomSeed), ct);
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.subMeshCount = 2;
        mesh.SetVertices(meshData.Vertices);
        mesh.SetTriangles(meshData.Triangles, 0);
        mesh.SetUVs(0, meshData.Uvs);

        mesh.SetTriangles(meshData.LeafTriangles, 1);
        mesh.RecalculateNormals();
    }

    //I would like for this to be a pure function (must take a seed for rng stuff)
    //I would like to be able to lerp between growables in any state and see the render transform... somehow
    private void RenderGrowable(Growable g, System.Random random, CancellationToken ct)
    {
        if (g is null || ct.IsCancellationRequested)
        {
            return;
        }

        var renderContext = new RenderContext
        {
            Distance = 0,
            Translation = Vector3.zero,
            Rotation = Quaternion.identity
        };

        g.Render(meshData, random, renderContext, ct);
    }





}
