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
        StopAllCoroutines();
        StartCoroutine(Search(fromCell, toCell));
    }

    IEnumerator Search(HexCell fromCell, HexCell toCell)
    {
        // The set of nodes already evaluated
        var closedSet = new HashSet<HexCell>();

        // The set of currently discovered nodes that are not evaluated yet.
        // Initially, only the start node is known.
        var openSet = new HashSet<HexCell>();

        // For each node, which node it can most efficiently be reached from.
        // If a node can be reached from many nodes, cameFrom will eventually contain the
        // most efficient previous step.
        var cameFrom = new Dictionary<HexCell, HexCell>();

        // For each node, the cost of getting from the start node to that node.
        var gScore = new Dictionary<HexCell, float>() { { fromCell, 0 } };

        // For each node, the total cost of getting from the start node to the goal
        // by passing by that node. That value is partly known, partly heuristic.
        var fScore = new Priority_Queue.SimplePriorityQueue<HexCell>();
        fScore.Enqueue(toCell, HeuristicCostEstimate(fromCell, toCell));

        while (openSet.Count > 0) {
            //    current:= the node in openSet having the lowest fScore[] value
            //    if current = goal
            //        return reconstruct_path(cameFrom, current)

            //    openSet.Remove(current)
            //    closedSet.Add(current)

            //    for each neighbor of current
            //        if neighbor in closedSet
            //            continue		// Ignore the neighbor which is already evaluated.

            //        if neighbor not in openSet	// Discover a new node
            //            openSet.Add(neighbor)

            //        // The distance from start to a neighbor
            //        //the "dist_between" function may vary as per the solution requirements.
            //    tentative_gScore:= gScore[current] + dist_between(current, neighbor)
            //        if tentative_gScore >= gScore[neighbor]
            //            continue		// This is not a better path.

            //        // This path is the best until now. Record it!
            //        cameFrom[neighbor] := current
            //        gScore[neighbor] := tentative_gScore
            //        fScore[neighbor] := gScore[neighbor] + heuristic_cost_estimate(neighbor, goal)

            //return failure
            return null;


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
