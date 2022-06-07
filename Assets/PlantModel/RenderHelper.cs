using Assets.PlantModel;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public static class RenderHelper
{
    public static List<Vector3> CylinderVerticesTemplate;
    public static List<Vector2> CylinderUvsTemplate;

    const int faces = 6;

    static RenderHelper()
    {
        var facePos = 2 * Mathf.PI / faces;
        var facePosUv = 1f / faces;

        Vector3[] Vertices = new Vector3[faces + 1];
        Vector2[] Uvs = new Vector2[faces + 1];

        for (int i = 0; i < faces; i++)
        {
            var newBottomVertex =
                 new Vector3(
                    Mathf.Sin(facePos * i),
                    0,
                    Mathf.Cos(facePos * i));


            Vertices[i] = newBottomVertex;
            Uvs[i] = new Vector2(facePosUv * i, 0f);
        }

        //add another vertex here for the UV to be nicer
        var endBottomVertex =
                 new Vector3(
                    Mathf.Sin(0),
                    0,
                    Mathf.Cos(0));


        Vertices[faces] = endBottomVertex;
        Uvs[faces] = new Vector2(1f, 0f);

        CylinderVerticesTemplate = Vertices.ToList();
        CylinderUvsTemplate = Uvs.ToList();
    }



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

    public static void CreateBranchSegment(MeshData data, RenderContext renderContext, float height = 1, float topR = 1, float botR = 1)
    {
        int countBefore = data.Vertices.Count;

        //add bottom circle
        for (int i = 0; i < CylinderVerticesTemplate.Count; i++)
        {
            var newBottomVertex = renderContext.Translation + renderContext.Rotation * 
                CylinderVerticesTemplate[i] * botR;
            data.Vertices.Add(newBottomVertex);
            data.Uvs.Add(CylinderUvsTemplate[i] + new Vector2( 0f , renderContext.Distance));
        }

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
                    translation + rotation * new Vector3(0, 0, 0) * size,
                    translation + rotation * new Vector3(0,0,0.5f)  * size,
                    translation + rotation * new Vector3(0.5f,0,0) * size,
                    translation + rotation * new Vector3(0.2f,0,0.3f)  * size,
                    translation + rotation * new Vector3(0.3f,0,0.2f) * size,
                    translation + rotation * new Vector3(0.5f,0,0.5f)  * size,
                });

        data.Uvs.AddRange(new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(0, 0.8f) ,
                    new Vector2(0.8f, 0),
                    new Vector2(0.2f, 0.6f) ,
                    new Vector2(0.6f, 0.2f),
                    new Vector2(0.8f, 0.8f) ,
                });

        data.LeafTriangles.AddRange(new List<int>
            {
                countBefore, countBefore + 1, countBefore + 2,
                countBefore + 1, countBefore + 3, countBefore + 2,
                countBefore + 2, countBefore + 3, countBefore + 4,
                countBefore + 3, countBefore + 5, countBefore + 4
            });
    }

}
