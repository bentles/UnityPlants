using Assets.PlantModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


//[ExecuteInEditMode]
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
    public int LevelOfDetailAgeSteps = 5;

    private readonly Dictionary<string, float> renderRandomnessCache = new();
    private readonly Dictionary<string, float> growthRandomnessCache = new();
    private System.Random renderRandom;
    private System.Random growthRandom;

    private Task RecreateMeshTask;
    CancellationTokenSource MeshTokenSource;

    //everything younger than this will not be rendered
    public int LodCutOffAge { get; set; } = int.MaxValue;

    // Start is called before the first frame update
    void Start()
    {
        renderRandom = new System.Random(RandomSeed);
        growthRandom = new System.Random(RandomSeed);

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
        Child = new Stem(this);

        MeshTokenSource = new CancellationTokenSource();

        //grow to start age
        GrowBySteps(StartAge);
    }

    internal bool CalcLodCutoff()
    {
        if (cameraReference == null)
        {
            return false;
        }

        float distToCamera = LevelOfDetailScale * Vector3.Distance(transform.position, cameraReference.transform.position);

        var newCutoff = Mathf.RoundToInt(distToCamera / LevelOfDetailAgeSteps) * LevelOfDetailAgeSteps;
        if (newCutoff != LodCutOffAge)
        {
            Debug.Log($"LOD {LodCutOffAge}");
            LodCutOffAge = newCutoff;
            return true;
        }

        return false;
    }

    public float GetRandom(Guid growableId, int i)
    {
        var lookup = $"{growableId}{i}";
        if (!renderRandomnessCache.ContainsKey(lookup))
        {
            return renderRandomnessCache[lookup] = (float)renderRandom.NextDouble();
        }
        return renderRandomnessCache[lookup];
    }

    public float GetGrowthRandom(Guid growableId, int i)
    {
        var lookup = $"{growableId}{i}";
        if (!growthRandomnessCache.ContainsKey(lookup))
        {
            return growthRandomnessCache[lookup] = (float)growthRandom.NextDouble();
        }
        return growthRandomnessCache[lookup];
    }

    private float elapsedTime = 0f;

    private void Update()
    {
        if (Child == null)
        {
            Start();
        }

        var lodChanged = CalcLodCutoff();
        bool ageChanged = ApplyGrowthSteps();


        if (ageChanged || lodChanged)
        {
            //cancel previous token's task
            MeshTokenSource.Cancel();
            MeshTokenSource.Dispose();

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

    private bool ApplyGrowthSteps()
    {
        elapsedTime += Time.deltaTime;
        var steps = Mathf.FloorToInt(elapsedTime / GrowthStepTime);
        steps = Mathf.Min(steps, MaxAge - Child.Age);
        bool hasGrown = GrowBySteps(steps);
        elapsedTime -= GrowthStepTime * steps;

        return hasGrown;
    }

    private bool GrowBySteps(int steps)
    {
        bool hasGrown = false;
        for (int i = 0; i < steps; i++)
        {
            hasGrown |= Child.Grow();
        }

        return hasGrown;
    }

    private void CreateMeshData(CancellationToken ct)
    {
        Debug.Log($"Start render! at age: {Child.Age}");
        meshData.Clear();
        RenderGrowable(Child, ct);
    }

    private void UpdateMesh()
    {
        System.Diagnostics.Stopwatch s = new();
        s.Start();
        mesh.Clear();
        mesh.subMeshCount = 2;
        mesh.SetVertices(meshData.Vertices);
        mesh.SetTriangles(meshData.Triangles, 0);
        mesh.SetUVs(0, meshData.Uvs);

        mesh.SetTriangles(meshData.LeafTriangles, 1);
        mesh.RecalculateNormals();
        s.Stop();
        Debug.Log($"time: {s.ElapsedMilliseconds}");
    }

    //I would like for this to be a pure function (must take a seed for rng stuff)
    //I would like to be able to lerp between growables in any state and see the render transform... somehow
    private void RenderGrowable(Growable g, CancellationToken ct)
    {
        if (g is null || ct.IsCancellationRequested)
        {
            Debug.Log("Cancelled before render!");
            return;
        }

        var renderContext = new RenderContext
        {
            Distance = 0,
            Translation = Vector3.zero,
            Rotation = Quaternion.identity
        };

        g.Render(meshData, renderContext, ct);

        if (ct.IsCancellationRequested)
        {
            Debug.Log("Cancelled during render!");
        }
        else
        {
            Debug.Log("Render completed!");
        }
    }





}
