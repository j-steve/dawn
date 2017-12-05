using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMeshChunk : MonoBehaviour
{
    static private readonly EdgeDirection[] EASTERLY_DIRECTIONS =
        new EdgeDirection[] { EdgeDirection.NE, EdgeDirection.E, EdgeDirection.SE };

    private Mesh mesh;
    private MeshCollider meshCollider;

    public int row { get; private set; }
    public int column { get; private set; }

    static List<Vector3> vertices = new List<Vector3>();
    static List<int> triangles = new List<int>();
    static List<Color32> colors = new List<Color32>();
    static List<Vector3> terrainTypes = new List<Vector3>();


    internal void Initialize(int row, int column)
    {
        this.row = row;
        this.column = column;
        name = "HexMeshChunk ({0}, {1})".Format(row, column);
        transform.SetParent(HexBoard.ActiveBoard.transform);
    }

    // Use this for initialization
    void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
    }


    private void OnDrawGizmos()
    {
        if (mesh != null) {
            Gizmos.color = Color.black;
            for (int i = 0; i < vertices.Count; i++) {
                Gizmos.DrawSphere(vertices[i], 0.1f);
            }
        }
    }

    internal void Triangulate(IEnumerable<HexCell> hexCells)
    {
        mesh.name = "Procedural Grid";
        vertices.Clear();
        triangles.Clear();
        terrainTypes.Clear();
        colors.Clear();
        var bridges = new Dictionary<HexCell, Dictionary<EdgeDirection, HexMeshBridge>>();
        UnityUtils.Log("Rendering hexes...");
        foreach (HexCell cell in hexCells) {
            bridges.Add(cell, new Dictionary<EdgeDirection, HexMeshBridge>());
            TriangulateHexCell(cell);
            foreach (EdgeDirection direction in EASTERLY_DIRECTIONS) {
                HexCell neighbor = cell.GetNeighbor(direction);
                if (neighbor != null) {
                    bridges[cell][direction] = TriangulateBridge(direction, cell, neighbor);
                    //HexCell neighbor2 = cell.GetNeighbor(direction.Next());
                    //if (neighbor2 != null) {
                    //    TriangulateCorner(direction, cell, neighbor, neighbor2);
                    //}
                }
            }
        }
        UnityUtils.Log("Adding triangles...");
        foreach (var cell in bridges) {
            var neighborNE = cell.Key.GetNeighbor(EdgeDirection.NE);
            var neighborE = cell.Key.GetNeighbor(EdgeDirection.E);
            if (neighborNE != null && neighborE != null && bridges.ContainsKey(neighborNE)) {
                UnityUtils.Log("Adding corner for {0}", cell);
                TringulateCorner(
                    cell.Value[EdgeDirection.NE],
                    cell.Value[EdgeDirection.E],
                    bridges[neighborNE][EdgeDirection.SE]);
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors32 = colors.ToArray();

        mesh.SetUVs(2, terrainTypes);

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;
    }

    /// <summary>
    /// Creates the vertices and triangles for a single hexagonal HexCell.
    /// </summary>
    void TriangulateHexCell(HexCell hexCell)
    {
        int center = vertices.Count;
        vertices.Add(hexCell.Center);
        AddColors(Colors.RED);
        foreach (EdgeDirection direction in EnumClass.GetAll<EdgeDirection>()) {
            vertices.AddRange(hexCell.GetEdge(direction));
            AddColors(Colors.RED, Colors.RED);
            triangles.AddRange(new int[] {
                vertices.Count - 2, vertices.Count - 1, center});
        }
        AddTerrainType(hexCell.TerrainType, 0, 0, 13);
    }

    /// <summary>
    /// Creates the vertices and triangles for a "bridge": the rectangular
    /// region which fills the gap between two adjacent HexCells.
    /// </summary>
    /// <param name="direction">The edge direction relative to cell1.</param>
    HexMeshBridge TriangulateBridge(EdgeDirection direction, HexCell cell1, HexCell cell2)
    {
        int v0 = vertices.Count;
        TexturedEdge e1 = cell1.GetEdge(direction).Reversed();
        TexturedEdge e2 = cell2.GetEdge(direction.Opposite());
        if (cell1.Elevation == cell2.Elevation && false) {
            AddQuadWithTerrain(e1, e2);
            return new HexMeshBridge(e1, e2);
        }
        else {
            var e1offset = new TexturedEdge(e1.Slerp(e2, .15f), TerrainTexture.CLIFF);
            var e2offset = new TexturedEdge(e2.Slerp(e1, .15f), TerrainTexture.CLIFF);
            if (Math.Abs(cell1.Elevation - cell2.Elevation) <= 1) {
                float y = (e1offset.vertex1.y + e2offset.vertex1.y) / 2;
                e1offset.vertex1.y = e1offset.vertex2.y = y;
                e2offset.vertex1.y = e2offset.vertex2.y = y;
                e1offset.texture = cell1.TerrainType;
                e2offset.texture = cell2.TerrainType;
            }
            AddQuadWithTerrain(e1, e1offset);
            AddQuadWithTerrain(e1offset, e2offset);
            AddQuadWithTerrain(e2offset, e2);
            return new HexMeshBridge(e1, e1offset, e2offset, e2);
        }
    }

    void TringulateCorner(HexMeshBridge b1, HexMeshBridge b2, HexMeshBridge b3)
    {
        UnityUtils.Log("Adding bridges");
        AddTriangle(
            b1.edges[0].vertex2, b1.edges[1].vertex2, b3.edges[1].vertex1);
        AddColors(Colors.RED, Colors.GREEN, Colors.BLUE);
        //AddTerrainType(b1.edges[0].texture, b3.edges[1].texture, b1.edges[1].texture, 3);
        AddTerrainType(TerrainTexture.TEALWATER, TerrainTexture.TEALWATER, TerrainTexture.TEALWATER, 3);
    }

    /// <summary>
    /// Creates the vertices and triangles for a corner triangle, in the 
    /// region between the bridges of three adjacent HexCells.
    /// </summary>
    /// <param name="direction">The edge direction relative to cell1.</param>
    void TriangulateCorner(EdgeDirection direction, HexCell cell1, HexCell cell2, HexCell cell3)
    {
        AddTriangle(
            cell1.Vertices[direction.Next().vertex1],
            cell2.Vertices[direction.Opposite().vertex1],
            cell3.Vertices[direction.Previous().vertex1]);
        AddColors(Colors.RED, Colors.GREEN, Colors.BLUE);
        if (cell1.Elevation == cell2.Elevation && cell2.Elevation == cell3.Elevation) {
            AddTerrainType(cell1.TerrainType, cell2.TerrainType, cell3.TerrainType, 3);
        }
        else {
            AddTerrainType(TerrainTexture.CLIFF, TerrainTexture.CLIFF, TerrainTexture.CLIFF, 3);
        }
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int index = vertices.Count;
        vertices.AddRange(new Vector3[] { v1, v2, v3 });
        triangles.AddRange(new int[] { index, index + 1, index + 2 });
    }

    void AddQuadWithTerrain(TexturedEdge e1, TexturedEdge e2)
    {
        AddQuad(e1.vertex1, e1.vertex2, e2.vertex1, e2.vertex2);
        AddColors(Colors.RED, Colors.RED, Colors.GREEN, Colors.GREEN);
        AddTerrainType(e1.texture, e2.texture, 0, 4);
    }

    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int i1 = vertices.Count;
        vertices.AddRange(new Vector3[] { v1, v2, v3, v4 });
        triangles.AddRange(new int[] { i1, i1 + 3, i1 + 2 });
        triangles.AddRange(new int[] { i1, i1 + 1, i1 + 3 });
    }

    void AddColors(params Color32[] colorsToAdd)
    {
        colors.AddRange(colorsToAdd);
    }

    void AddTerrainType(
        TerrainTexture redChannel, TerrainTexture blueChannel,
        TerrainTexture greenChannel, int count)
    {
        Vector3 terrainType = new Vector3(
            (int)redChannel, (int)blueChannel, (int)greenChannel);
        for (int i = 0; i < count; i++) {
            terrainTypes.Add(terrainType);
        }
    }

}


class HexMeshBridge
{
    public TexturedEdge[] edges;

    public HexMeshBridge(params TexturedEdge[] edges)
    {
        this.edges = edges;
    }
}