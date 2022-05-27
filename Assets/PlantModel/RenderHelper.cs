using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RenderHelper
{

    public static void CreateCylinder(MeshData data, Vector3 translation, Quaternion rotation, Growable g, float height)
    {        
        var botR = g.CalcRadius(g.Age);
        if (g.Child != null)
        {
            var topR = g.CalcRadius(g.Child.Age);
            RenderHelper.CreateCylinder(data, translation, rotation, botR: botR, topR: topR, height: height);
        }
        else
        {
            RenderHelper.CreateCylinder(data, translation, rotation, botR: botR, topR: 0.0f, height: height);
        }

    }

    public static void CreateCylinder(MeshData data, Vector3 translation, Quaternion rotation, int faces = 5, float height = 1, float topR = 1, float botR = 1)
    {
        int countBefore = data.Vertices.Count;

        var facePos = 2 * Mathf.PI / faces;

        //add bottom circle
        for (int i = 0; i < faces; i++)
        {
            var newBot = translation + rotation * (
                 new Vector3(
                    Mathf.Sin(facePos * i) * botR,
                    0,
                    Mathf.Cos(facePos * i) * botR));
            data.Vertices.Add(newBot);
        }

        if (topR == 0.0f)
        {
            //add top point
            var newTop = translation + rotation *
                (new Vector3(
                    topR,
                    height,
                    topR));
            data.Vertices.Add(newTop);
        }



        for (int i = 0; i < faces; i++)
        {
            var botRight = countBefore + ((i + 1) % faces);
            var bot = countBefore + i;
            int top;
            int topRight;
            
            if (topR != 0.0f)
            {
                top = countBefore + i + faces;
                topRight = countBefore + ((i + 1) % faces) + faces;
            }
            else
            {
                top = topRight = countBefore + faces;
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

        data.LeafTriangles.AddRange(new List<int>
            {
                countBefore, countBefore + 1, countBefore + 2,
                countBefore + 1, countBefore + 3, countBefore + 2
            });
    }

}
