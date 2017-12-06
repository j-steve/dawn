using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class HexChunk : MonoBehaviour
{
    #region Static

    static readonly EdgeDirection[] EASTERLY_DIRECTIONS =
        new EdgeDirection[] { EdgeDirection.NE, EdgeDirection.E, EdgeDirection.SE };

    public static HexChunk Create(HexChunk prefab, int row, int column)
    {
        HexChunk obj = Instantiate(prefab);
        obj.Initialize(row, column);
        return obj;
    }

    #endregion

    public Canvas hexCanvas;
    public HexMeshTerrain terrainMesh;
    public HexMesh oceanMesh;
    public HexMesh lakeMesh;

    public int row { get; private set; }
    public int column { get; private set; }

    void Initialize(int row, int column)
    {
        this.row = row;
        this.column = column;
        name = "HexChunk ({0}, {1})".Format(row, column);
        transform.SetParent(HexBoard.ActiveBoard.transform);
    }

    internal void Triangulate(IEnumerable<HexCell> hexCells)
    {
        terrainMesh.Clear();
        oceanMesh.Clear();
        lakeMesh.Clear();
        foreach (HexCell cell in hexCells) {
            TriangulateHexCell(cell);
            if (cell.Elevation == 0) { TriangulateWater(cell); }
            foreach (EdgeDirection direction in EASTERLY_DIRECTIONS) {
                HexCell neighbor = cell.GetNeighbor(direction);
                if (neighbor != null) {
                    TriangulateBridge(direction, cell, neighbor);
                    if (direction != EdgeDirection.SE) {
                        HexCell neighbor2 = cell.GetNeighbor(direction.Next());
                        if (neighbor2 != null) {
                            TriangulateCorner(direction, cell, neighbor, neighbor2);
                        }
                    }
                }
            }
        }
        terrainMesh.Apply();
        oceanMesh.Apply();
        lakeMesh.Apply();
    }


    static private readonly Vector3 WATERLEVEL = new Vector3(0, HexConstants.ELEVATION_STEP * 1.5f, 0);
    static private readonly Vector3[] HEX_VERTEX_OFFSETS = new Vector3[]  {
        HexConstants.HEX_CELL_SEPERATION * new Vector3(0, 0, 1),
        HexConstants.HEX_CELL_SEPERATION * new Vector3((float)HexConstants.HEX_RADIUS, 0f, 0.5f),
        HexConstants.HEX_CELL_SEPERATION * new Vector3((float)HexConstants.HEX_RADIUS, 0f, -0.5f),
    };

    private void TriangulateWater(HexCell cell)
    {
        var mesh = cell.TerrainType == TerrainTexture.BLUEWATER ? oceanMesh : lakeMesh;
        int v0 = mesh.vertices.Count;
        Vector3 center = cell.Center + WATERLEVEL;
        mesh.vertices.Add(cell.Center + WATERLEVEL);
        foreach (var vertexOffset in HEX_VERTEX_OFFSETS) {
            mesh.vertices.Add(center + vertexOffset);
            mesh.vertices.Add(center - vertexOffset);
        }
        for (int i = 1; i <= 4; i++) {
            mesh.triangles.AddRange(new int[] { v0, v0 + i, v0 + i + 2 });
        }
        mesh.triangles.AddRange(new int[] { v0, v0 + 5, v0 + 2 });
        mesh.triangles.AddRange(new int[] { v0, v0 + 6, v0 + 1 });
    }

    /// <summary>
    /// Creates the vertices and triangles for a single hexagonal HexCell.
    /// </summary>
    void TriangulateHexCell(HexCell hexCell)
    {
        int center = terrainMesh.vertices.Count;
        terrainMesh.vertices.Add(hexCell.Center);
        terrainMesh.AddColors(Colors.RED);
        foreach (EdgeDirection direction in EnumClass.GetAll<EdgeDirection>()) {
            terrainMesh.vertices.AddRange(hexCell.GetEdge(direction));
            terrainMesh.AddColors(Colors.RED, Colors.RED);
            terrainMesh.triangles.AddRange(new int[] {
                terrainMesh.vertices.Count - 2, terrainMesh.vertices.Count - 1, center});
        }
        terrainMesh.AddTerrainType(hexCell.TerrainType, 0, 0, 13);
    }

    /// <summary>
    /// Creates the vertices and triangles for a "bridge": the rectangular
    /// region which fills the gap between two adjacent HexCells.
    /// </summary>
    /// <param name="direction">The edge direction relative to cell1.</param>
    void TriangulateBridge(EdgeDirection direction, HexCell cell1, HexCell cell2)
    {
        TexturedEdge e1 = cell1.GetEdge(direction).Reversed();
        TexturedEdge e2 = cell2.GetEdge(direction.Opposite());
        if (cell1.Elevation == cell2.Elevation) {
            terrainMesh.AddQuadWithTerrain(e1, e2);
        }
        else {
            var e1offset = new TexturedEdge(e1.Slerp(e2, .15f), TerrainTexture.CLIFF);
            var e2offset = new TexturedEdge(e2.Slerp(e1, .15f), TerrainTexture.CLIFF);
            if (Math.Abs(cell1.Elevation - cell2.Elevation) == 1) {
                float y = (e1offset.vertex1.y + e2offset.vertex1.y) / 2;
                e1offset.vertex1.y = e1offset.vertex2.y = y;
                e2offset.vertex1.y = e2offset.vertex2.y = y;
                e1offset.texture = cell1.TerrainType;
                e2offset.texture = cell2.TerrainType;
            }
            terrainMesh.AddQuadWithTerrain(e1, e1offset);
            terrainMesh.AddQuadWithTerrain(e1offset, e2offset);
            terrainMesh.AddQuadWithTerrain(e2offset, e2);
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
        terrainMesh.AddColors(Colors.RED, Colors.GREEN, Colors.BLUE);
        var elevations = new HexCell[] { cell1, cell2, cell3 }.Select(c => c.Elevation);
        if (Math.Abs(elevations.Max() - elevations.Min()) <= 1) {
            terrainMesh.AddTerrainType(cell1.TerrainType, cell2.TerrainType, cell3.TerrainType, 3);
        }
        else {
            terrainMesh.AddTerrainType(TerrainTexture.CLIFF, TerrainTexture.CLIFF, TerrainTexture.CLIFF, 3);
        }
    }

}

