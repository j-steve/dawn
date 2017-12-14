using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

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

    HexCell highlightedCell;
    HexCell searchFromCell;
    internal List<HexCell> pathCells = new List<HexCell>();

    public Unit moosePrefab;
    public Unit wolfPrefab;
    public int mooseCount;

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
        if (Input.GetMouseButtonDown(0)) {
            UIInGame.ActiveInGameUI.HideUI();
            HexCell clickedCell = GetCellClickTarget();
            if (clickedCell != null) {
                if (clickedCell == highlightedCell) {
                    clickedCell.Highlight(null);
                    highlightedCell = null;
                    UIInGame.ActiveInGameUI.HideUI();
                    return;
                }
                if (clickedCell.units.Count > 0) {
                    var msg = clickedCell.units.Select(x => x.UnitName).Join(", ");
                    UIInGame.ActiveInGameUI.ShowUI(msg);
                }
                foreach (var cell in pathCells) { cell.Highlight(null); }
                pathCells.Clear();
                Color color = Color.green;
                if (Input.GetKey(KeyCode.LeftShift)) {
                    if (searchFromCell) { searchFromCell.Highlight(null); }
                    searchFromCell = clickedCell;
                    searchFromCell.Highlight(Color.blue);
                }
                else {
                    if (highlightedCell) { highlightedCell.Highlight(null); }
                    highlightedCell = clickedCell;
                    clickedCell.Highlight(Color.green);
                }
                if (searchFromCell && highlightedCell) {
                    var path = new HexPathfinder().Search(searchFromCell, highlightedCell);
                    for (int i = 1; i < path.Count - 1; i++) {
                        path[i].Highlight(Color.yellow);
                        pathCells.Add(path[i]);
                    }
                }
            }
        }
    }

    HexCell GetCellClickTarget()
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
