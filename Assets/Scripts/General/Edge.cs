using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Edge : IEnumerable<Vector3>
{
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

}