using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A HexChunk is a group of <see cref="HexCell"/>s which share the same mesh(es).
/// and are therefore essentially rendered as a single GameObject.
/// </summary>
public class HexChunk : MonoBehaviour
{
    #region Static

    static readonly EdgeDirection[] EASTERLY_DIRECTIONS =
        new EdgeDirection[] { EdgeDirection.NE, EdgeDirection.E, EdgeDirection.SE };

    public static HexChunk Create(HexChunk prefab, Transform parent, int row, int column)
    {
        HexChunk obj = Instantiate(prefab);
        obj.Initialize(parent, row, column);
        return obj;
    }

    #endregion

    public Canvas hexCanvas;
    public HexMeshTerrain terrainMesh;
    public HexMesh oceanMesh;
    public HexMesh lakeMesh;

    public int row { get; private set; }
    public int column { get; private set; }

    void Initialize(Transform parent, int row, int column)
    {
        this.row = row;
        this.column = column;
        name = "HexChunk ({0}, {1})".Format(row, column);
        transform.SetParent(parent);
    }

    internal void Triangulate(IEnumerable<HexCell> hexCells)
    {
        terrainMesh.InitOrReset();
        oceanMesh.InitOrReset();
        lakeMesh.InitOrReset();
        foreach (HexCell cell in hexCells) {
            TriangulateHexCell(cell);
            if (cell.Elevation == 0)
                TriangulateWater(cell);
            foreach (EdgeDirection direction in EASTERLY_DIRECTIONS) {
                HexCell neighbor = cell.GetNeighbor(direction);
                if (neighbor) {
                    TriangulateBridge(direction, cell, neighbor);
                    if (direction != EdgeDirection.SE) {
                        HexCell neighbor2 = cell.GetNeighbor(direction.Next());
                        if (neighbor2) {
                            TriangulateCorner(direction, cell, neighbor, neighbor2);
                        }
                    }
                }
            }
        }
        foreach (var v in terrainMesh.vertices) {
            vertices[v] = Color.red;
        }
        terrainMesh.Apply();
        oceanMesh.Apply();
        lakeMesh.Apply();
    }

    static readonly Vector3 WATERLEVEL = new Vector3(0, HexConstants.ELEVATION_STEP * 1.5f, 0);

    static readonly Vector3[] HEX_VERTEX_OFFSETS = new Vector3[]  {
        HexConstants.HEX_CELL_SEPERATION * new Vector3(0, 0, 1),
        HexConstants.HEX_CELL_SEPERATION * new Vector3((float)HexConstants.HEX_RADIUS, 0f, 0.5f),
        HexConstants.HEX_CELL_SEPERATION * new Vector3((float)HexConstants.HEX_RADIUS, 0f, -0.5f),
    };

    void TriangulateWater(HexCell cell)
    {
        HexMesh mesh = cell.TerrainType == TerrainTexture.BLUEWATER ? oceanMesh : lakeMesh;
        int v0 = mesh.vertices.Count;
        Vector3 center = cell.Center + WATERLEVEL;
        mesh.AddVertex(cell.Center + WATERLEVEL);
        foreach (Vector3 vertexOffset in HEX_VERTEX_OFFSETS) {
            mesh.AddVertex(center + vertexOffset);
            mesh.AddVertex(center - vertexOffset);
        }
        for (int i = 1; i <= 4; i++) {
            mesh.triangles.AddRange(new int[] { v0, v0 + i, v0 + i + 2 });
        }
        mesh.triangles.AddRange(new int[] { v0, v0 + 5, v0 + 2 });
        mesh.triangles.AddRange(new int[] { v0, v0 + 6, v0 + 1 });
    }

    /// <summary>
    /// The width of a river relative to a hex cell edge.
    /// </summary>
    const float RIVER_WIDTH = 0.4f;
    const float RIVER_LEFT = (1 - RIVER_WIDTH) / 2;
    const float RIVER_RIGHT = (1 + RIVER_WIDTH) / 2;

    /// <summary>
    /// Creates the vertices and triangles for a single hexagonal HexCell.
    /// </summary>
    void TriangulateHexCell(HexCell cell)
    {
        int center = terrainMesh.vertices.Count;
        var triangles = new List<Triangle>(48); // size is 8 (triangles per direction) * 6 (num of directions)
        foreach (EdgeDirection direction in EnumClass.GetAll<EdgeDirection>()) {
            // Create the outer portion of the hex.
            Edge farEdge = cell.GetEdge(direction);
            var farEdgeRiverLeft = farEdge.Lerp(RIVER_LEFT);
            var farEdgeRiverRight = farEdge.Lerp(RIVER_RIGHT);
            var farEdgeMidpoint = farEdge.Lerp(0.5f);
            var innerHexLeft = Vector3.Lerp(cell.Center, farEdge.vertex1, RIVER_WIDTH);
            var innerHexRight = Vector3.Lerp(cell.Center, farEdge.vertex2, RIVER_WIDTH);
            var innerHexMidpoint = Vector3.Lerp(innerHexLeft, innerHexRight, 0.5f);
            triangles.Add(new Triangle(farEdge.vertex1, farEdgeRiverLeft, innerHexLeft));
            triangles.Add(new Triangle(farEdgeRiverRight, farEdge.vertex2, innerHexRight));
            triangles.AddRange(new Rectangle(farEdgeRiverLeft, farEdgeMidpoint, innerHexMidpoint, innerHexLeft).AsTriangles());
            triangles.AddRange(new Rectangle(farEdgeMidpoint, farEdgeRiverRight, innerHexRight, innerHexMidpoint).AsTriangles());
            // Create the cenral inner portion where the river (if present) changes direction.
        }
        foreach(var triangle in triangles) {
            terrainMesh.AddVertices(triangle);
            terrainMesh.triangles.AddRange(new int[] {
                terrainMesh.vertices.Count - 3, terrainMesh.vertices.Count - 2, terrainMesh.vertices.Count - 1});
            terrainMesh.AddColors(Color.red, Color.red, Color.red);
            terrainMesh.AddTerrainType(cell.TerrainType, 0, 0, 3);
        }
    }

    IEnumerable<Triangle> Split(Triangle initialTriangle, int splitTimes)
    {
        var triangles = new List<Triangle>(1) { initialTriangle };
        for (int i = 0; i < splitTimes; i++) {
            var results = new List<Triangle>(triangles.Count * 3);
            foreach (Triangle triangle in triangles) {
                Vector3 center = (triangle.vertex1 + triangle.vertex2 + triangle.vertex3) / 3;
                results.Add(new Triangle(triangle.vertex1, triangle.vertex2, center));
                results.Add(new Triangle(triangle.vertex2, triangle.vertex3, center));
                results.Add(new Triangle(triangle.vertex3, triangle.vertex1, center));
            }
            triangles = results;
        }
        return triangles;
    }

    public const int TERRACES_PER_SLOPE = 2;
    public const int TERRACE_STEPS = TERRACES_PER_SLOPE * 2 + 1;
    public const float TERRACE_STEP_LERP = 1f / TERRACE_STEPS;
    public const float TERRACE_EXTRA_WIDTH = 0.1f;

    readonly float[] splits = new float[] { 0, RIVER_LEFT, 0.5f, RIVER_RIGHT, 1 };


    /// <summary>
    /// Creates the vertices and triangles for a "bridge": the rectangular
    /// region which fills the gap between two adjacent HexCells.
    /// </summary>
    /// <param name="direction">The edge direction relative to cell1.</param>
    void TriangulateBridge(EdgeDirection direction, HexCell cell1, HexCell cell2)
    {
        TexturedEdge tile1edge = cell1.GetEdge(direction).Reversed();
        TexturedEdge tile2edge = cell2.GetEdge(direction.Opposite());
        for (int i = 0 ; i < splits.Length - 1; i++) {
            terrainMesh.AddQuad(tile1edge.Lerp(splits[i]), tile1edge.Lerp(splits[i + 1]), tile2edge.Lerp(splits[i + 1]), tile2edge.Lerp(splits[i]));
            terrainMesh.AddColors(Color.red, Color.red, Color.green, Color.green);
            terrainMesh.AddTerrainType(tile1edge.texture, tile2edge.texture, 0, 4);
        }
        //TransitionType transitionType = GetTransitionType(cell1, cell2);
        //if (transitionType == TransitionType.Flat) {
        //    terrainMesh.AddQuadWithTerrain(tile1edge, tile2edge);
        //} else if (transitionType == TransitionType.Cliff) {
        //    var e1offset = new TexturedEdge(tile1edge.Slerp(tile2edge, .10f), TerrainTexture.CLIFF);
        //    var e2offset = new TexturedEdge(tile2edge.Slerp(tile1edge, .10f), TerrainTexture.CLIFF);
        //    terrainMesh.AddQuadWithTerrain(tile1edge, e1offset);
        //    terrainMesh.AddQuadWithTerrain(e1offset, e2offset);
        //    terrainMesh.AddQuadWithTerrain(e2offset, tile2edge);
        //} else if (transitionType == TransitionType.Terraced) {
        //    Edge lastTerraceStop = tile1edge;
        //    Color lastColor = Color.green;
        //    for (int i = 0; i < TERRACES_PER_SLOPE; i++) {
        //        // TODO: Fix terrain blending, by using the slerp of the color gradient between the two textures.
        //        float e1slerp = (i * 2 + 1) * TERRACE_STEP_LERP - TERRACE_EXTRA_WIDTH;
        //        float e2slerp = (i * 2 + 2) * TERRACE_STEP_LERP + TERRACE_EXTRA_WIDTH;
        //        Edge terraceStartEdge = tile1edge.Slerp(tile2edge, e1slerp);
        //        Edge terraceStopEdge = tile1edge.Slerp(tile2edge, e2slerp);
        //        Edge midpoint = tile1edge.Slerp(tile2edge, (1f + i) / (TERRACES_PER_SLOPE + 1));
        //        terraceStartEdge.vertex1.y = terraceStopEdge.vertex1.y = midpoint.vertex1.y;
        //        terraceStartEdge.vertex2.y = terraceStopEdge.vertex2.y = midpoint.vertex2.y;
        //        terrainMesh.AddQuad(lastTerraceStop.vertex1, lastTerraceStop.vertex2, terraceStartEdge.vertex2, terraceStartEdge.vertex1);
        //        var terraceStartColor = Color.Lerp(Color.green, Color.red, e1slerp);
        //        var terraceStopColor = Color.Lerp(Color.green, Color.red, e2slerp);
        //        terrainMesh.AddColors(lastColor, lastColor, terraceStartColor, terraceStartColor);
        //        terrainMesh.AddQuad(terraceStartEdge.vertex1, terraceStartEdge.vertex2, terraceStopEdge.vertex2, terraceStopEdge.vertex1);
        //        terrainMesh.AddColors(terraceStartColor, terraceStartColor, terraceStopColor, terraceStopColor);
        //        lastColor = terraceStopColor;
        //        lastTerraceStop = terraceStopEdge;
        //    }
        //    terrainMesh.AddQuad(lastTerraceStop.vertex1, lastTerraceStop.vertex2, tile2edge.vertex2, tile2edge.vertex1);
        //    terrainMesh.AddColors(lastColor, lastColor, Color.red, Color.red);
        //    terrainMesh.AddTerrainType(tile2edge.texture, tile1edge.texture, 0, TERRACES_PER_SLOPE * 2 * 4 + 4);
        //}
    }

    IEnumerable<TexturedEdge> SplitEdge(TexturedEdge edge, IEnumerable<float> splits)
    {
        Vector3 lastVertex = edge.vertex1;
        foreach (var split in splits) {
            Vector3 nextVertex = edge.Lerp(split);
            yield return new TexturedEdge(lastVertex, nextVertex, edge.texture);
        }
        yield return new TexturedEdge(lastVertex, edge.vertex2, edge.texture);
    }

    //enum TransitionType { Flat, Terraced, Cliff }

    //TransitionType GetTransitionType(HexCell cell1, HexCell cell2)
    //{
    //    switch (Math.Abs(cell1.Elevation - cell2.Elevation)) {
    //        case 0:
    //            return TransitionType.Flat;
    //        case 1:
    //            return TransitionType.Terraced;
    //        default:
    //            return TransitionType.Cliff;
    //    }
    //}

    public Dictionary<Vector3, Color> vertices = new Dictionary<Vector3, Color>();

    void OnDrawGizmos()
    {
        foreach (var vert in vertices) {
            Gizmos.color = vert.Value;
            Gizmos.DrawWireSphere(transform.TransformPoint(vert.Key), 0.2f);
        }
    }

    /// <summary>
    /// Creates the vertices and triangles for a corner triangle, in the 
    /// region between the bridges of three adjacent HexCells.
    /// </summary>
    /// <param name="direction">The edge direction relative to cell1.</param>
    void TriangulateCorner(EdgeDirection direction, HexCell cell1, HexCell cell2, HexCell cell3)
    {
        terrainMesh.AddTriangle(
            cell1.Vertices[direction.Next().vertex1],
            cell2.Vertices[direction.Opposite().vertex1],
            cell3.Vertices[direction.Previous().vertex1]);
        terrainMesh.AddColors(Color.red, Color.green, Color.blue);
        IEnumerable<int> elevations = new HexCell[] { cell1, cell2, cell3 }.Select(c => c.Elevation);
        if (Math.Abs(elevations.Max() - elevations.Min()) <= 1) {
            terrainMesh.AddTerrainType(cell1.TerrainType, cell2.TerrainType, cell3.TerrainType, 3);
        } else {
            terrainMesh.AddTerrainType(TerrainTexture.CLIFF, TerrainTexture.CLIFF, TerrainTexture.CLIFF, 3);
        }
    }
}

