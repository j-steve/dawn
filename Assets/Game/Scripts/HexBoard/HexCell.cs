using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class HexCell : MonoBehaviour, ISelectable, ISaveable
{
    #region Static

    static public HexCell Create(HexCell prefab, HexChunk chunk, int row, int column)
    {
        HexCell obj = Instantiate(prefab);
        obj.Initialize(chunk, row, column);
        return obj;
    }

    static private readonly Vector3
        v1 = HexConstants.HEX_SIZE * new Vector3(0, 0, 1),
        v2 = HexConstants.HEX_SIZE * new Vector3((float)HexConstants.HEX_RADIUS, 0f, 0.5f),
        v3 = HexConstants.HEX_SIZE * new Vector3((float)HexConstants.HEX_RADIUS, 0f, -0.5f);

    #endregion

    /// <summary>
    /// Returns the vector cooresponding to the very center of the hexagon cell.
    /// </summary>
    public Vector3 Center {
        get { return transform.localPosition; }
        private set { transform.localPosition = value; }
    }

    public int Elevation {
        get { return _elevation; }
        set {
            _elevation = value;
            Vector3 position = Center;
            position.y = value * HexConstants.ELEVATION_STEP;
            Center = position;
            SetVertices();
            if (label) {
                var labelPosition = label.rectTransform.localPosition;
                labelPosition.z = -position.y;
                label.rectTransform.localPosition = labelPosition;
            }
        }
    }
    int _elevation = 0;

    public float Latitude {
        get {
            return (float)Coordinates.Z / (HexBoard.Active.mapSize.Height * HexConstants.CELLS_PER_CHUNK_ROW / 2) - 1f;
        }
    }

    public int ContinentNumber = 0;

    public int BiomeNumber = 0;

    public Biome Biome { get; internal set; }

    public TerrainTexture TerrainType { get { return Biome.terrainTexture; } }

    public readonly List<Unit> units = new List<Unit>();

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
    void Initialize(HexChunk chunk, int row, int column)
    {
        Coordinates = HexCellCoordinates.FromOffsetCoordinates(column, row);
        name = "HexCell " + Coordinates.ToString();

        transform.SetParent(chunk.transform, false);
        float x = column + (row % 2 * 0.5f); // Offset odd-numbered rows.
        Center = new Vector3(x * (float)HexConstants.HEX_DIAMETER, 0f, row * 1.5f)
            * (float)HexConstants.HEX_CELL_SEPERATION;
        SetVertices();
        CreateLabel(chunk.hexCanvas);

        Biome = Biome.LAKE;
    }

    void CreateLabel(Canvas hexCanvas)
    {
        label = Instantiate(HexBoard.Active.hexLabelPrefab);
        label.name = "HexLabel " + Coordinates.ToString();
        label.rectTransform.SetParent(hexCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(Center.x, Center.z);
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
        HexBoard.Active.hexCells.TryGetValue(Coordinates + offset, out neighbor);
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
        return new TexturedEdge(Vertices[direction.vertex1], Vertices[direction.vertex2], TerrainType);
    }

    /// <summary>
    /// Returns the distance to the given cell.
    /// </summary>
    public int DistanceTo(HexCell other)
    {
        return Coordinates.DistanceTo(other.Coordinates);
    }

    #region ISelectable

    string ISelectable.InfoPanelTitle {
        get {
            return Biome.ToString();
        }
    }

    string ISelectable.InfoPanelDescription {
        get {
            return "Continent #{0}, Biome #{1}".Format(ContinentNumber + 1, BiomeNumber + 1);
        }
    }

    string ISelectable.InfoPanelDetails {
        get {
            return units.Count == 0 ? "" : "UNITS: " + units.Select(x => x.UnitName).Join(", ") ;
        }
    }

    void ISelectable.OnFocus()
    {
        Highlight(Color.green);
    }

    void ISelectable.OnBlur()
    {
        UnHighlight();
    }

    #endregion

    #region Cell Highlighting

    public void Highlight(Color? color, string labelText)
    {
        Highlight(color);
        label.text = labelText;
    }

    public void Highlight(Color? color)
    {
        var highlight = label.rectTransform.GetComponentInChildren<Image>();
        if (color.HasValue) { highlight.color = color.Value; }
        highlight.enabled = color.HasValue;
    }

    public void UnHighlight()
    {
        Highlight(null);
        label.text = "";
    }

    public bool HasHighlight()
    {
        var highlight = label.rectTransform.GetComponentInChildren<Image>();
        return highlight.enabled;
    }

    #endregion

    #region HashCode

    public override int GetHashCode()
    {
        return Coordinates.GetHashCode();
    }

    public override bool Equals(System.Object obj)
    {
        return obj != null && obj.GetType() == GetType() &&
            ((HexCell)obj).Coordinates.Equals(Coordinates);
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((Int16)Biome.id);
        writer.Write((Int16)Elevation);
    }

    public void Load(BinaryReader reader)
    {
        Biome = Biome.GetByID(reader.ReadInt16());
        Elevation = reader.ReadInt16();
    }

    #endregion
}