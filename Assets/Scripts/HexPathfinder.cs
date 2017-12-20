using UnityEngine;
using System.Collections.Generic;
using System;
using Priority_Queue;

public class HexPathfinder
{
    readonly float maxDistance;

    public HexPathfinder(float maxDistance)
    {
        this.maxDistance = maxDistance;
    }

    public IList<PathStep> FindNearest(HexCell origin, Func<HexCell, bool> goalCondition)
    {
        var cameFrom = new Dictionary<HexCell, HexCell>();
        var gScore = new Dictionary<HexCell, float>() { { origin, 0 } };
        var closedSet = new HashSet<HexCell>();
        var openSet = new SimplePriorityQueue<HexCell>();
        openSet.Enqueue(origin, 0);

        HexCell current;
        while (openSet.TryDequeue(out current)) {
            if (gScore[current] > maxDistance)
                break;
            if (goalCondition(current)) {
                return ReconstructPath(cameFrom, current, gScore);
            }
            foreach (var neighbor in current.GetNeighbors()) {
                if (closedSet.Contains(neighbor))
                    continue;
                var existingScore = gScore.ContainsKey(neighbor);
                var tentativeGScore = gScore[current] + MovementCost(current, neighbor);
                if (!existingScore || tentativeGScore < gScore[neighbor]) {
                    // This path is cheapest yet seen for this neighbor.
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;

                    if (existingScore)
                        openSet.UpdatePriority(neighbor, tentativeGScore);
                    else
                        openSet.Enqueue(neighbor, tentativeGScore);
                }
            }
        }
        Debug.LogWarningFormat("No suitable path from {0} to goal condition cell! Checked {1}", origin, closedSet.Count);
        return new List<PathStep>();
    }

    /// <summary>
    /// Initiates an A* pathfinding search to determine the cheapest path from
    /// origin to goal, based on the current movement cost function.
    /// </summary>
    /// <returns>
    /// A list of the optimal path for moving from origin to destination,
    /// where each entry represents the next step in the journey, starting with
    /// the origin cell itself and ending with the goal cell.
    /// </returns>
    public IList<PathStep> Search(HexCell origin, HexCell goal)
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
            { goal, origin.DistanceTo(goal) }
        };

        HexCell current;
        while (openSet.TryDequeue(out current)) {
            if (current == goal) {
                return ReconstructPath(cameFrom, current, gScore);
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
                    fScore[neighbor] = tentativeGScore + neighbor.DistanceTo(goal);

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
    /// The exact cost of moving between the two given cells, which will always
    /// be adjacent (neighboring) cells.
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns></returns>
    public float MovementCost(HexCell c1, HexCell c2)
    {
        if (c1.Elevation == 0 || c2.Elevation == 0) {
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
    IList<PathStep> ReconstructPath(Dictionary<HexCell, HexCell> cameFrom, HexCell destination, Dictionary<HexCell, float> gScore)
    {
        var path = new List<PathStep>() { new PathStep(destination, gScore[destination]) };
        while (cameFrom.TryGetValue(destination, out destination)) {
            path.Add(new PathStep(destination, gScore[destination]));
        }
        path.Reverse();
        return path;
    }

}


public class PathStep
{
    public readonly HexCell cell;
    public readonly float cost;
    internal PathStep(HexCell cell, float cost)
    {
        this.cell = cell;
        this.cost = cost;
    }
}