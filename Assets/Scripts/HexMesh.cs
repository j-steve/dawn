using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    protected Mesh mesh;

    [NonSerialized] public List<Vector3> vertices = new List<Vector3>();
    [NonSerialized] public List<int> triangles = new List<int>();
    [NonSerialized] public List<Color32> colors = new List<Color32>();
    [NonSerialized] public List<Vector2> uvs = new List<Vector2>();

    protected virtual void Awake()
    {
        this.GetRequiredComponent<MeshFilter>().mesh = mesh = new Mesh();
    }

    public virtual void Clear()
    {
        mesh.Clear();
        var lists = new IList[] { vertices, triangles, colors, uvs };
        foreach (var list in lists) {
            list.Clear();
        }
    }

    public virtual void Apply()
    {
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetColors(colors);
        mesh.RecalculateNormals();
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int index = vertices.Count;
        vertices.AddRange(new Vector3[] { v1, v2, v3 });
        triangles.AddRange(new int[] { index, index + 1, index + 2 });
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int i1 = vertices.Count;
        vertices.AddRange(new Vector3[] { v1, v2, v3, v4 });
        triangles.AddRange(new int[] { i1, i1 + 3, i1 + 2 });
        triangles.AddRange(new int[] { i1, i1 + 1, i1 + 3 });
    }

    public void AddColors(params Color32[] colorsToAdd)
    {
        colors.AddRange(colorsToAdd);
    }
}
