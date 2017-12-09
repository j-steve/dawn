using UnityEngine;
using System.Collections.Generic;
using System;
using Priority_Queue;

public class HexPathfinder
{
    /// <summary>
    /// Initiates an A* pathfinding search to determine the cheapest path from
    /// origin to goal, based on the current movement cost function.
    /// </summary>
    /// <returns>
    /// A list of the optimal path for moving from origin to destination,
    /// where each entry represents the next step in the journey, starting with
    /// the origin cell itself and ending with the goal cell.
    /// </returns>
    public IList<HexCell> Search(HexCell origin, HexCell goal)
    {
        // The set of nodes already evaluated
        var closedSet = new HashSet<HexCell>();

        // The set of currently discovered nodes that are not evaluated yet.
        // Initially, only the start node is known.
        var openSet = new SimplePriorityQueue<HexCell>();
        openSet.Enqueue(origin, 0);

        // For each node, which node it can most efficiently be reached from.
        // If a node can be reached from many nodes, cameFrom will eventually contain the
        // most efficient previous step.
        var cameFrom = new Dictionary<HexCell, HexCell>();

        // For each node, the cost of getting from the start node to that node.
        var gScore = new Dictionary<HexCell, float>() { { origin, 0 } };

        // For each node, the total cost of getting from the start node to the goal
        // by passing by that node. That value is partly known, partly heuristic.
        var fScore = new Dictionary<HexCell, float>() {
            { goal, MovementCostEstimate(origin, goal) }
        };

        HexCell current;
        while (openSet.TryDequeue(out current)) {
            if (current == goal) {
                return ReconstructPath(cameFrom, goal);
            }
            if (current != origin) {
                current.Highlight(Color.gray);
                HexBoard.ActiveBoard.pathCells.Add(current);
            }
            foreach (var neighbor in current.GetNeighbors()) {
                if (closedSet.Contains(neighbor)) {
                    continue;
                }
                // The movement cost for moving from current to this neighbor.
                var existingScore = gScore.ContainsKey(neighbor);
                var tentativeGScore = gScore[current] + MovementCost(current, neighbor);
                if (!existingScore || tentativeGScore < gScore[neighbor]) {
                    // This path is cheapest yet seen for this neighbor.
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + MovementCostEstimate(neighbor, goal);

                    if (existingScore)
                        openSet.UpdatePriority(neighbor, fScore[neighbor]);
                    else
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
            closedSet.Add(current);
        }
        Debug.LogWarningFormat("No path from {0} to {1}! Checked {2}", origin, goal, closedSet.Count);
        return null;
    }

    /// <summary>
    /// A heuristic estimate of the cost of moving between the two given cells,
    /// which may not neccesarily be adjacent (aka neighbors).  May be as simple
    /// as the distance between the two cells.
    /// </summary>
    float MovementCostEstimate(HexCell c1, HexCell c2)
    {
        int sumOfDiff =
            Math.Abs(c1.Coordinates.X - c2.Coordinates.X) +
            Math.Abs(c1.Coordinates.Y - c2.Coordinates.Y) +
            Math.Abs(c1.Coordinates.Z - c2.Coordinates.Z);
        return sumOfDiff / 2;
    }

    /// <summary>
    /// The exact cost of moving between the two given cells, which will always
    /// be adjacent (neighboring) cells.
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns></returns>
    float MovementCost(HexCell c1, HexCell c2)
    {
        if ((c1.Elevation == 0) != (c2.Elevation == 0)) {
            return 100;
        }
        return Math.Abs(c1.Elevation - c2.Elevation) * 2 + 1;
    }

    /// <summary>
    /// Reconstructs the complete (cheapest) path for moving to the given goal
    /// cell, based on the given list of movement costs.
    /// </summary>
    /// <param name="cameFrom">
    /// A mapping of cells to the cheapest cell from which they can be reached,
    /// where the key is the arrival cell and the key is the departure cell.
    /// </param>
    IList<HexCell> ReconstructPath(Dictionary<HexCell, HexCell> cameFrom, HexCell destination)
    {
        var path = new List<HexCell>() { destination };
        while (cameFrom.TryGetValue(destination, out destination)) {
            path.Add(destination);
        }
        path.Reverse();
        return path;
    }
}
