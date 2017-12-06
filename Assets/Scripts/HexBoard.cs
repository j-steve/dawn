using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

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
    List<HexCell> pathCells = new List<HexCell>();

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
            HexCell clickedCell = GetCellClickTarget();
            if (clickedCell != null) {
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
                    var path = Search(searchFromCell, highlightedCell);
                    for (int i = 1; i < path.Count - 1; i++) {
                        path[i].Highlight(Color.white);
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

    public void FindPath(HexCell fromCell, HexCell toCell)
    {
        //StopAllCoroutines();
        //StartCoroutine(Search(fromCell, toCell));
    }

    List<HexCell> Search(HexCell start, HexCell goal)
    {
        // The set of nodes already evaluated
        var closedSet = new HashSet<HexCell>();

        // The set of currently discovered nodes that are not evaluated yet.
        // Initially, only the start node is known.
        var openSet = new Priority_Queue.SimplePriorityQueue<HexCell>();
        openSet.Enqueue(start, 0);

        // For each node, which node it can most efficiently be reached from.
        // If a node can be reached from many nodes, cameFrom will eventually contain the
        // most efficient previous step.
        var cameFrom = new Dictionary<HexCell, HexCell>();

        // For each node, the cost of getting from the start node to that node.
        var gScore = new Dictionary<HexCell, float>() { { start, 0 } };

        // For each node, the total cost of getting from the start node to the goal
        // by passing by that node. That value is partly known, partly heuristic.
        var fScore = new Dictionary<HexCell, float>() {
            { goal, HeuristicCostEstimate(start, goal) }
        };

        HexCell current;
        while (openSet.TryDequeue(out current)) {
            if (current == goal) {
                UnityUtils.Log("MATCH! Checked {0} for {1} to {2}, current is {3}", closedSet.Count, start, goal, current);
                return ReconstructPath(cameFrom, goal);
            }
            if (current != start) {
                current.Highlight(Color.yellow);
                pathCells.Add(current);
            }

            foreach (var neighbor in current.GetNeighbors()) {
                if (closedSet.Contains(neighbor)) { UnityUtils.Log("{0} ar openSet", openSet); continue; }
                UnityUtils.Log("Adding {0} to openSet", openSet);

                // The distance from start to a neighbor
                //the "dist_between" function may vary as per the solution requirements.
                var tentative_gScore = gScore[current] + DistanceBetween(current, neighbor);
                if (!gScore.ContainsKey(neighbor) || tentative_gScore < gScore[neighbor]) {
                    // This path is the best until now. Record it!
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentative_gScore;
                    fScore[neighbor] = tentative_gScore + HeuristicCostEstimate(neighbor, goal);
                    openSet.Enqueue(neighbor, fScore[neighbor]); // Discover a new node.
                }
            }

            closedSet.Add(current);
        }
        UnityUtils.Log("No matches! Checked {0}", closedSet.Count);

        return null;
    }

    private float DistanceBetween(HexCell current, HexCell neighbor)
    {
        if ((current.Elevation == 0) != (neighbor.Elevation == 0)) {
            return 100;
        }
        return Math.Abs(current.Elevation - neighbor.Elevation) * 2 + 1;
    }

    private List<HexCell> ReconstructPath(Dictionary<HexCell, HexCell> cameFrom, HexCell current)
    {
        var path = new List<HexCell>() { current };
        while (cameFrom.TryGetValue(current, out current)) {
            path.Add(current);
        }
        return path;
    }

    float HeuristicCostEstimate(HexCell c1, HexCell c2)
    {
        int sumOfDiff =
            Math.Abs(c1.Coordinates.X - c2.Coordinates.X) +
            Math.Abs(c1.Coordinates.Y - c2.Coordinates.Y) +
            Math.Abs(c1.Coordinates.Z - c2.Coordinates.Z);
        return sumOfDiff / 2;
    }

    void OnEnable()
    {
        ActiveBoard = this;
    }


}
