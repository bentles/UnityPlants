using Assets.PlantModel;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class RenderHelper
{

    public static void CreateBranchSegment(MeshData data, RenderContext renderContext, Growable g, float height)
    {        
        var botR = g.CalcRadius(g.Age);
        if (g.Child != null)
        {
            var topR = g.CalcRadius(g.Child.Age);
            RenderHelper.CreateBranchSegment(data, renderContext, botR: botR, topR: topR, height: height);
        }
        else
        {
            RenderHelper.CreateBranchSegment(data, renderContext, botR: botR, topR: 0.0f, height: height);
        }

    }

    public static void CreateBranchSegment(MeshData data, RenderContext renderContext, int faces = 5, float height = 1, float topR = 1, float botR = 1)
    {
        int countBefore = data.Vertices.Count;

        var facePos = 2 * Mathf.PI / faces;

        //add bottom circle
        for (int i = 0; i < faces; i++)
        {
            var newBottomVertex = renderContext.Translation + renderContext.Rotation * (
                 new Vector3(
                    Sin(facePos * i) * botR,
                    0,
                    Cos(facePos * i) * botR));

            
            data.Vertices.Add(newBottomVertex);
            data.Uvs.Add(new Vector2((1f / (faces)) * i, renderContext.Distance ));
        }

        //add another vertex here for the UV to be nice
        var endBottomVertex = renderContext.Translation + renderContext.Rotation * (
                 new Vector3(
                    Sin(0) * botR,
                    0,
                    Cos(0) * botR));


        data.Vertices.Add(endBottomVertex);
        data.Uvs.Add(new Vector2(1f , renderContext.Distance ));

        if (topR == 0.0f)
        {
            //add top point
            var newTop = renderContext.Translation + renderContext.Rotation *
                (new Vector3(
                    topR,
                    height,
                    topR));
            data.Vertices.Add(newTop);
            data.Uvs.Add(new Vector2(0.5f, (renderContext.Distance + height)));
        }



        for (int i = 0; i < faces; i++)
        {
            var botRight = countBefore + (i + 1);
            var bot = countBefore + i;
            int top;
            int topRight;
            
            if (topR != 0.0f)
            {
                top = countBefore + i + (faces + 1);
                topRight = countBefore + (i + 1) + (faces + 1);
            }
            else
            {
                top = topRight = countBefore + (faces + 1);
            }

            data.Triangles.AddRange(new[] { bot, botRight, top, });
            data.Triangles.AddRange(new[] { top, botRight, topRight });
        }        
    }

    public static void CreateLeaf(MeshData data, Vector3 translation, Quaternion rotation, float size)
    {
        var countBefore = data.Vertices.Count;

        data.Vertices.AddRange(new List<Vector3>
                {
                    translation + rotation * new Vector3(0,0,0) * size,
                    translation + rotation * new Vector3(0,0,0.5f)  * size,
                    translation + rotation * new Vector3(0.5f,0,0) * size,
                    translation + rotation * new Vector3(0.7f,0,0.7f)  * size,
                });

        data.Uvs.AddRange(new List<Vector2>
                {
                    translation + rotation * new Vector3(0,0,0) * size,
                    translation + rotation * new Vector3(0,0,0.5f)  * size,
                    translation + rotation * new Vector3(0.5f,0,0) * size,
                    translation + rotation * new Vector3(0.7f,0,0.7f)  * size,
                });

        data.LeafTriangles.AddRange(new List<int>
            {
                countBefore, countBefore + 1, countBefore + 2,
                countBefore + 1, countBefore + 3, countBefore + 2
            });
    }

    //Since each cylinder is the same sin and cos can be memoised
    private static ConcurrentDictionary<float, float> SinMemo = new ConcurrentDictionary<float, float>();
    private static ConcurrentDictionary<float, float> CosMemo = new ConcurrentDictionary<float, float>();

    private static float Sin(float f)
    {
        if (!SinMemo.ContainsKey(f))
        {
            SinMemo[f] = Mathf.Sin(f);
        }
        return SinMemo[f];
    }

    private static float Cos(float f)
    {
        if (!CosMemo.ContainsKey(f))
        {
            CosMemo[f] = Mathf.Cos(f);
        }
        return CosMemo[f];
    }


}
