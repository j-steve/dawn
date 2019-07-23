using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// An Edge is simply a pair of two vertices, with each vertex representing a single point in space.
/// This abstraction simplifies some operations around manipulating vertices, 
/// e.g. during prgrammatic mesh creation/modification.
/// </summary>
public class Edge : IEnumerable<Vector3>
{
    #region Static

    static public Edge operator +(Edge e1, Edge e2)
    {
        return new Edge(e1.vertex1 + e2.vertex1, e1.vertex2 + e2.vertex2);
    }

    static public Edge operator +(Edge e1, Vector3 v1)
    {
        return new Edge(e1.vertex1 + v1, e1.vertex2 + v1);
    }

    /// <summary>
    /// Returns a group of edges which together form a single polygonal shape. 
    /// For example, if given vectors A, B, and C, it will return three edges: A-B, B-C, and C-A.
    /// </summary>
    static public Edge[] Polygon(params Vector3[] vectors)
    {
        Edge[] edges = new Edge[vectors.Length];
        int i, j;
        for (i = 0, j = 1; j < vectors.Length; i++, j++) {
            edges[i] = new Edge(vectors[i], vectors[j]);
        }
        edges[i] = new Edge(vectors[vectors.Length - 1], vectors[0]);
        return edges;
    }

    #endregion

    public Vector3 vertex1;
    public Vector3 vertex2;

    public Edge(Vector3 vertex1, Vector3 vertex2)
    {
        this.vertex1 = vertex1;
        this.vertex2 = vertex2;
    }

    public Edge Reversed()
    {
        return new Edge(vertex2, vertex1);
    }

    public Edge Slerp(Edge e2, float amount)
    {
        return new Edge(
            Vector3.Slerp(vertex1, e2.vertex1, amount),
            Vector3.Slerp(vertex2, e2.vertex2, amount));
    }

    public IEnumerator<Vector3> GetEnumerator()
    {
        yield return vertex1;
        yield return vertex2;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return "Edge<" + vertex1.ToString() + " TO " + vertex2.ToString() + ">";
    }
}