using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

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

    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR
        mapSize = new RectangleInt(8, 8);
        continentsPerChunk = 0.5f;
#endif
        ActiveBoard = this;
        StartCoroutine(new HexBoardGenerator(this).CreateMap());

    }

    // Update is called once per frame
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
            UIInGame.ActiveInGameUI.HideUI();
        } else if ((ISelectable)clickedCell != UIInGame.ActiveInGameUI.selection) {
            UIInGame.ActiveInGameUI.SetSelected(clickedCell);
        } else if (clickedCell.units.Count > 0) {
            UIInGame.ActiveInGameUI.SetSelected(clickedCell.units.First());
        } else {
            UIInGame.ActiveInGameUI.HideUI();
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

    void OnEnable()
    {
        ActiveBoard = this;
    }

}
