using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Triangle : IEnumerable<Vector3>
{
    public readonly Vector3 vertex1;
    public readonly Vector3 vertex2;
    public readonly Vector3 vertex3;

    public Triangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        this.vertex1 = vertex1;
        this.vertex2 = vertex2;
        this.vertex3 = vertex3;
    }

    public IEnumerator<Vector3> GetEnumerator()
    {
        yield return vertex1;
        yield return vertex2;
        yield return vertex3;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public struct Rectangle //: IEnumerable<Vector3>
{

    public readonly Vector3 vertex1;
    public readonly Vector3 vertex2;
    public readonly Vector3 vertex3;
    public readonly Vector3 vertex4;

    public Rectangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 vertex4)
    {
        this.vertex1 = vertex1;
        this.vertex2 = vertex2;
        this.vertex3 = vertex3;
        this.vertex4 = vertex4;
    }

    public IEnumerable<Triangle> AsTriangles()
    {
        yield return new Triangle(vertex1, vertex2, vertex4);
        yield return new Triangle(vertex4, vertex2, vertex3);
    }

    //public IEnumerator<Vector3> GetEnumerator()
    //{
    //    // Triangle #1
    //    yield return vertex1;
    //    yield return vertex2;
    //    yield return vertex4;
    //    // Triangle #2
    //    yield return vertex4;
    //    yield return vertex2;
    //    yield return vertex3;
    //}

    //IEnumerator IEnumerable.GetEnumerator()
    //{
    //    return GetEnumerator();
    //}
}