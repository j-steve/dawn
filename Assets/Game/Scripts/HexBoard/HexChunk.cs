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

    public static HexChunk Create(HexChunk prefab, int row, int column)
    {
        HexChunk obj = Instantiate(prefab);
        obj.Initialize(row, column);
        return obj;
    }

    #endregion

    public Canvas hexCanvas;
    public HexMeshTerrain terrainMesh;
    public HexMesh treeMesh;
    public HexMesh oceanMesh;
    public HexMesh lakeMesh;

    public int row { get; private set; }
    public int column { get; private set; }

    void Initialize(int row, int column)
    {
        this.row = row;
        this.column = column;
        name = "HexChunk ({0}, {1})".Format(row, column);
        transform.SetParent(HexBoard.Active.transform);
    }

    internal void Triangulate(IEnumerable<HexCell> hexCells)
    {
        terrainMesh.Clear();
        oceanMesh.Clear();
        lakeMesh.Clear();
        if (treeMesh)
            treeMesh.Clear();
        foreach (HexCell cell in hexCells) {
            TriangulateHexCell(cell);
            if (cell.Elevation == 0)
                TriangulateWater(cell);
            if (treeMesh && cell.TerrainType == TerrainTexture.MIXEDTREES)
                TriangulateTrees(cell);
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
        terrainMesh.Apply();
        oceanMesh.Apply();
        lakeMesh.Apply();
        if (treeMesh)
            treeMesh.Apply();
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
        mesh.vertices.Add(cell.Center + WATERLEVEL);
        foreach (Vector3 vertexOffset in HEX_VERTEX_OFFSETS) {
            mesh.vertices.Add(center + vertexOffset);
            mesh.vertices.Add(center - vertexOffset);
        }
        for (int i = 1; i <= 4; i++) {
            mesh.triangles.AddRange(new int[] { v0, v0 + i, v0 + i + 2 });
        }
        mesh.triangles.AddRange(new int[] { v0, v0 + 5, v0 + 2 });
        mesh.triangles.AddRange(new int[] { v0, v0 + 6, v0 + 1 });
    }

    void TriangulateTrees(HexCell cell)
    {
        HexMesh mesh = treeMesh;
        int v0 = mesh.vertices.Count;
        Vector3 center = cell.Center + (WATERLEVEL / 2);
        mesh.vertices.Add(center + (WATERLEVEL / 2));
        foreach (Vector3 vertexOffset in HEX_VERTEX_OFFSETS) {
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
        terrainMesh.AddColors(Color.red);
        foreach (EdgeDirection direction in EnumClass.GetAll<EdgeDirection>()) {
            terrainMesh.vertices.AddRange(hexCell.GetEdge(direction));
            terrainMesh.AddColors(Color.red, Color.red);
            terrainMesh.triangles.AddRange(new int[] {
                terrainMesh.vertices.Count - 2, terrainMesh.vertices.Count - 1, center});
        }
        terrainMesh.AddTerrainType(hexCell.TerrainType, 0, 0, 13);
    }

    public const int terracesPerSlope = 2;
    public const int terraceSteps = terracesPerSlope * 2 + 1;
    public const float terraceStepLerp = 1f / terraceSteps;
    public const float terraceExtraWidth = 0.1f;


    /// <summary>
    /// Creates the vertices and triangles for a "bridge": the rectangular
    /// region which fills the gap between two adjacent HexCells.
    /// </summary>
    /// <param name="direction">The edge direction relative to cell1.</param>
    void TriangulateBridge(EdgeDirection direction, HexCell cell1, HexCell cell2)
    {
        TexturedEdge tile1edge = cell1.GetEdge(direction).Reversed();
        TexturedEdge tile2edge = cell2.GetEdge(direction.Opposite());
        TransitionType transitionType = GetTransitionType(cell1, cell2);
        if (transitionType == TransitionType.Flat) {
            terrainMesh.AddQuadWithTerrain(tile1edge, tile2edge);
        } else if (transitionType == TransitionType.Cliff) {
            var e1offset = new TexturedEdge(tile1edge.Slerp(tile2edge, .10f), TerrainTexture.CLIFF);
            var e2offset = new TexturedEdge(tile2edge.Slerp(tile1edge, .10f), TerrainTexture.CLIFF);
            terrainMesh.AddQuadWithTerrain(tile1edge, e1offset);
            terrainMesh.AddQuadWithTerrain(e1offset, e2offset);
            terrainMesh.AddQuadWithTerrain(e2offset, tile2edge);
        } else if (transitionType == TransitionType.Terraced) {
            Edge lastTerraceStop = tile1edge;
            Color lastColor = Color.green;
            for (int i = 0; i < terracesPerSlope; i++) {
                // TODO: Fix terrain blending, by using the slerp of the color gradient between the two textures.
                float e1slerp = (i * 2 + 1) * terraceStepLerp - terraceExtraWidth;
                float e2slerp = (i * 2 + 2) * terraceStepLerp + terraceExtraWidth;
                Edge terraceStartEdge = tile1edge.Slerp(tile2edge, e1slerp);
                Edge terraceStopEdge = tile1edge.Slerp(tile2edge, e2slerp);
                Edge midpoint = tile1edge.Slerp(tile2edge, (1f + i) / (terracesPerSlope + 1));
                terraceStartEdge.vertex1.y = terraceStopEdge.vertex1.y = midpoint.vertex1.y;
                terraceStartEdge.vertex2.y = terraceStopEdge.vertex2.y = midpoint.vertex2.y;
                terrainMesh.AddQuad(lastTerraceStop.vertex1, lastTerraceStop.vertex2, terraceStartEdge.vertex2, terraceStartEdge.vertex1);
                var terraceStartColor = Color.Lerp(Color.green, Color.red, e1slerp);
                var terraceStopColor = Color.Lerp(Color.green, Color.red, e2slerp);
                terrainMesh.AddColors(lastColor, lastColor, terraceStartColor, terraceStartColor);
                terrainMesh.AddQuad(terraceStartEdge.vertex1, terraceStartEdge.vertex2, terraceStopEdge.vertex2, terraceStopEdge.vertex1);
                terrainMesh.AddColors(terraceStartColor, terraceStartColor, terraceStopColor, terraceStopColor);
                lastColor = terraceStopColor;
                lastTerraceStop = terraceStopEdge;
            }
            terrainMesh.AddQuad(lastTerraceStop.vertex1, lastTerraceStop.vertex2, tile2edge.vertex2, tile2edge.vertex1);
            terrainMesh.AddColors(lastColor, lastColor, Color.red, Color.red);
            terrainMesh.AddTerrainType(tile2edge.texture, tile1edge.texture, 0, terracesPerSlope * 2 * 4 + 4);
        }
    }

    enum TransitionType { Flat, Terraced, Cliff }

    TransitionType GetTransitionType(HexCell cell1, HexCell cell2)
    {
        switch (Math.Abs(cell1.Elevation - cell2.Elevation)) {
            case 0:
                return TransitionType.Flat;
            case 1:
                return TransitionType.Terraced;
            default:
                return TransitionType.Cliff;
        }
    }

    //public Dictionary<Vector3, Color> vertices = new Dictionary<Vector3, Color>();

    //void OnDrawGizmos()
    //{
    //    foreach (var vert in vertices) {
    //        Gizmos.color = vert.Value;
    //        Gizmos.DrawWireSphere(transform.TransformPoint(vert.Key), 0.2f);
    //    }
    //}

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

