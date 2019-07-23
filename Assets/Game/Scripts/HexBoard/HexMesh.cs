using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// A HexMesh is a container object for a single mesh, used to depict the HexBoard within space.
/// Each 
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    /// <summary>
    /// Determines the irregularity of programmatic meshes.
    /// Increase this value for less "boxy" hills/slopes & hex shapes.
    /// </summary>
    const float perturbStrength = 5f;

    static private Vector4 SampleNoise(Vector3 position)
    {
        const float noiseScale = 0.003f;
        return HexBoard.Active.noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
    }

    static private Vector3 Perturb(Vector3 position)
    {
        Vector4 sample = SampleNoise(position);
        position.x += (sample.x * 2f - 1f) * perturbStrength;
        //position.y += (sample.y * 2f - 1f) * perturbStrength;
        position.z += (sample.z * 2f - 1f) * perturbStrength;
        return position;
    }

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
        mesh.RecalculateNormals(110);
        mesh.RecalculateTangents();
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int index = vertices.Count;
        AddVertices(new Vector3[] { v1, v2, v3 });
        triangles.AddRange(new int[] { index, index + 1, index + 2 });
    }

    public void AddQuad(Edge e1, Edge e2)
    {
        AddQuad(e1.vertex1, e1.vertex2, e2.vertex2, e2.vertex1);
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int i1 = vertices.Count;
        AddVertices(new Vector3[] { v1, v2, v3, v4 });
        triangles.AddRange(new int[] { i1, i1 + 1, i1 + 2 });
        triangles.AddRange(new int[] { i1, i1 + 2, i1 + 3 });
    }

    public void AddColors(params Color32[] colorsToAdd)
    {
        colors.AddRange(colorsToAdd);
    }

    public void AddVertices(IEnumerable<Vector3> newVertices)
    {
        foreach (var vertex in newVertices) {
            AddVertex(vertex);
        }
    }
    public void AddVertex(Vector3 vertex)
    {
        vertices.Add(Perturb(vertex));
    }
}
