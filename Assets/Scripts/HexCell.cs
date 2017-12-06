using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
    static public HexCell Create(HexCell prefab, int row, int column)
    {
        HexCell obj = Instantiate(prefab);
        obj.Initialize(row, column);
        return obj;
    }

    static private readonly Vector3
        v1 = HexConstants.HEX_SIZE * new Vector3(0, 0, 1),
        v2 = HexConstants.HEX_SIZE * new Vector3((float)HexConstants.HEX_RADIUS, 0f, 0.5f),
        v3 = HexConstants.HEX_SIZE * new Vector3((float)HexConstants.HEX_RADIUS, 0f, -0.5f);


    /// <summary>
    /// Returns the vector cooresponding to the very center of the hexagon cell.
    /// </summary>
    public Vector3 Center {
        get {
            return transform.localPosition;
        }
        private set {
            transform.localPosition = value;
        }
    }

    public int Elevation {
        get {
            return elevation;
        }
        set {
            elevation = value;
            Vector3 position = Center;
            position.y = value * HexConstants.ELEVATION_STEP;
            Center = position;
            SetVertices();
            if (label != null) {
                var labelPosition = label.rectTransform.localPosition;
                labelPosition.z = -position.y;
                label.rectTransform.localPosition = labelPosition;
            }
        }
    }


    public float Latitude {
        get {
            return (float)Coordinates.Z / (HexBoard.ActiveBoard.mapSize.Height * HexConstants.CELLS_PER_CHUNK_ROW / 2) - 1f;
        }
    }

    public Color32 Color;

    public TerrainTexture TerrainType;

    public int Biome = 0;

    public int Continent = 0;

    int elevation = 0;

    Text label;

    /// <summary>
    /// Returns the hex cell's <code>HexCellCoordinates</code>, which is a 
    /// set of three integer values representing the hexagon's position along
    /// the three directions (E/W, NE/SW, and NW/SE).
    /// </summary>
    public HexCellCoordinates Coordinates { get; private set; }

    ///// <summary>
    ///// Returns the six edges of the hexagon cell, each represented as a pair
    ///// of vectors.  There are 12 vectors in all and 6 unique vectors among them.
    ///// </summary>
    //public Edge[] Edges { get; private set; }

    public readonly Vector3[] Vertices = new Vector3[6];

    /// <summary>
    /// Instantiates the <code>HexCell</code>. Should be called immediately.
    /// </summary>
    /// <param name="row">The horizantal (x-axis) row for this cell.</param>
    /// <param name="column">The vertical (z-axis) column for this cell.</param>
    void Initialize(int row, int column)
    {
        Coordinates = HexCellCoordinates.FromOffsetCoordinates(column, row);
        name = "HexCell " + Coordinates.ToString();

        transform.SetParent(HexBoard.ActiveBoard.transform, false);
        float x = column + (row % 2 * 0.5f); // Offset odd-numbered rows.
        Center = new Vector3(x * (float)HexConstants.HEX_DIAMETER, 0f, row * 1.5f)
            * (float)HexConstants.HEX_CELL_SEPERATION;
        SetVertices();
        CreateLabel();

        TerrainType = TerrainTexture.TEALWATER;
    }

    void CreateLabel()
    {
        label = Instantiate(HexBoard.ActiveBoard.hexCellLabelPrefab);
        var canvas = HexBoard.ActiveBoard.hexBoardLabelCanvas;
        label.rectTransform.SetParent(canvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(Center.x, Center.z);
        //label.text = Coordinates.ToStringOnSeparateLines();
        label.text = Mathf.RoundToInt(Latitude * 100).ToString();
    }

    void SetVertices()
    {
        Vertices[VertexDirection.N] = Center + v1;
        Vertices[VertexDirection.ENE] = Center + v2;
        Vertices[VertexDirection.ESE] = Center + v3;
        Vertices[VertexDirection.S] = Center - v1;
        Vertices[VertexDirection.WNW] = Center - v2;
        Vertices[VertexDirection.WSW] = Center - v3;
    }

    /// <summary>
    /// Returns the neighboring <code>HexCell</code> for the given 
    /// <code>HexDirection</code>.
    /// </summary>
    /// <param name="direction">
    /// The direction of the neighboring cell,
    /// relative to this cell.
    /// </param>
    /// <returns>The neighboring cell.</returns>
    public HexCell GetNeighbor(EdgeDirection direction)
    {
        HexCellCoordinates offset = HexCellCoordinates.OFFSET[direction];
        HexCell neighbor;
        HexBoard.ActiveBoard.hexCells.TryGetValue(Coordinates + offset, out neighbor);
        return neighbor;
    }

    public IEnumerable<HexCell> GetNeighbors()
    {
        foreach (EdgeDirection direction in EdgeDirection.Values) {
            var neighbor = GetNeighbor(direction);
            if (neighbor) { yield return GetNeighbor(direction); }
        }
    }


    /// <summary>
    /// Returns the <code>Edge</code> corresponding to the given
    /// <code>HexDirection</code>.
    /// </summary>
    /// <param name="direction">
    /// The direction of the edge (relative to the
    /// center of the hexagon).
    /// </param>
    /// <returns>The edge for the given direction.</returns>
    public TexturedEdge GetEdge(EdgeDirection direction)
    {
        return new TexturedEdge(Vertices[direction.vertex1], Vertices[direction.vertex2], this.TerrainType);
    }

    public void Highlight(bool toggleOn)
    {
        var highlight = label.rectTransform.GetComponentInChildren<Image>();
        highlight.color = Colors.GREEN;
        highlight.enabled = toggleOn;
    }
}