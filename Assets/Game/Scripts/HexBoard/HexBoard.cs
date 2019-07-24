using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using System.IO;

public class HexCellClickedEventArgs : System.ComponentModel.CancelEventArgs
{
    public readonly HexCell Cell;
    public HexCellClickedEventArgs(HexCell cell) { Cell = cell; }
}

public class HexBoard : MonoBehaviour
{
    static public HexBoard Active;

    public event Action<HexCellClickedEventArgs> HexCellClickedEvent;

    /// <summary>
    /// The size of the map, as a number of HexMeshChunks in the X and Y axes.
    /// </summary>
    public RectangleInt mapSize;
    public float continentsPerChunk;
    /// <summary>
    /// The minimum size of a biome, which also influences its max size. Multiplied by each biome's min tile count.
    /// </summary>
    public float biomeMinSize;
    /// <summary>
    /// The maximum size of a biome relative to its minimum size.  Multiplied by effective min size to get the max value.
    /// Determines the range of sizes available. Setting to 1 would mean no variation in biome size.
    /// </summary>
    public float biomeMaxSize;

    public Material terrainMaterial;
    public Texture2D noiseSource;
    public HexChunk hexChunkPrefab;
    public HexCell hexCellPrefab;
    public Text hexLabelPrefab;

    public UnitPlayer playerPrefab;
    public int playerStartingUnits;

    public readonly Dictionary<HexCellCoordinates, HexCell> hexCells = new Dictionary<HexCellCoordinates, HexCell>();

    /// <summary>
    /// Sets active board, on initialization or after script recompilation. 
    /// </summary>
    void OnEnable()
    {
        Active = this;
    }

    void Start()
    {
#if UNITY_EDITOR
        mapSize = new RectangleInt(8, 8);
        continentsPerChunk = 0.5f;
#endif
        GameDataLoader.Load<ResourceType>("TileResources/resourceTypes");
        GameDataLoader.Load<TileType>("Biomes/tileTypes");
        GameDataLoader.Load<AnimalType>("Animals/animalTypes");
        StartCoroutine(new HexBoardGenerator(this).CreateMap());
    }

    void Update()
    {
        if (Game.Paused)
            return;
        if (Input.GetButtonDown("Show Gridlines")) {
            terrainMaterial.ToggleKeyword("GRIDLINES_ON");
        }

    }

    public void OnMapClick()
    {
        HexCell clickedCell = GetCellUnderCursor();
        if (HexCellClickedEvent != null) {
            var eventArgs = new HexCellClickedEventArgs(clickedCell);
            HexCellClickedEvent.Invoke(eventArgs);
            if (eventArgs.Cancel) { return; }
        }
        if (clickedCell == null) {
            InGameUI.Instance.SetSelected(null);
        } else if (!InGameUI.Instance.IsSelected(clickedCell)) {
            InGameUI.Instance.SetSelected(clickedCell);
        } else if (clickedCell.units.Count > 0) {
            InGameUI.Instance.SetSelected(clickedCell.units.First());
        } else {
            InGameUI.Instance.SetSelected(null);
        }
    }

    public HexCell GetCellUnderCursor()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            Vector3 position = transform.InverseTransformPoint(hit.point);
            var coordinates = HexCellCoordinates.FromPosition(position);
            if (hexCells.ContainsKey(coordinates)) {
                return hexCells[coordinates];
            }
        }
        return null;
    }

    public void SaveMap(BinaryWriter writer)
    {
        writer.Write((short)mapSize.Height);
        writer.Write((short)mapSize.Width);
        foreach (var cell in hexCells.Values) {
            cell.Save(writer);
        }
    }

    public void LoadMap(string path)
    {
        var mapGenerator = new HexBoardGenerator(this);
        StartCoroutine(mapGenerator.LoadMap(path));
    }
}
