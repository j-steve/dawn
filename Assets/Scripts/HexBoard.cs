﻿using UnityEngine;
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
    public static HexBoard ActiveBoard;

    public event Action<HexCellClickedEventArgs> HexCellClickedEvent;

    /// <summary>
    /// The size of the map, as a number of HexMeshChunks in the X and Y axes.
    /// </summary>
    public RectangleInt mapSize;

    public float continentsPerChunk;

    public Material terrainMaterial;

    public HexChunk hexChunkPrefab;
    public HexCell hexCellPrefab;
    public Text hexLabelPrefab;

    public readonly Dictionary<HexCellCoordinates, HexCell> hexCells = new Dictionary<HexCellCoordinates, HexCell>();

    public Unit[] unitPrefabs;

    /// <summary>
    /// Sets active board, on initialization or after script recompilation. 
    /// </summary>
    void OnEnable()
    {
        ActiveBoard = this;
    }

    void Start()
    {
#if UNITY_EDITOR
        mapSize = new RectangleInt(8, 8);
        continentsPerChunk = 0.5f;
#endif
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
            UIInGame.Instance.SetSelected(null);
        } else if (!UIInGame.Instance.IsSelected(clickedCell)) {
            UIInGame.Instance.SetSelected(clickedCell);
        } else if (clickedCell.units.Count > 0) {
            UIInGame.Instance.SetSelected(clickedCell.units.First());
        } else {
            UIInGame.Instance.SetSelected(null);
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
