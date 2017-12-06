using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class HexBoard : MonoBehaviour
{
    public static HexBoard ActiveBoard;

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

    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR
        mapSize = new RectangleInt(4, 4);
        continentsPerChunk = 0.5f;
#endif
        ActiveBoard = this;
        new HexBoardGenerator(this).CreateMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Show Gridlines")) {
            terrainMaterial.ToggleKeyword("GRIDLINES_ON");
        }
        if (Input.GetMouseButton(0)) {
            HandleCellClick();
        }
    }

    HexCell highlightedCell;

    void HandleCellClick()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            Vector3 position = transform.InverseTransformPoint(hit.point);
            var coordinates = HexCellCoordinates.FromPosition(position);
            if (hexCells.ContainsKey(coordinates)) {
                if (highlightedCell) { highlightedCell.Highlight(false); }
                highlightedCell = hexCells[coordinates];
                highlightedCell.Highlight(true);
            }
        }
    }

    void OnEnable()
    {
        ActiveBoard = this;
    }

}
