using UnityEngine;
using System.Collections;

public class Goal { }

public class MoveGoal : Goal
{
    #region Delegates

    public delegate bool HexCellContains(Unit unit, HexCell cell);

    public delegate float UnitNeedsDelegate(Unit unit);

    #endregion

    static UnitNeedsDelegate thirstPriority = (u) => u.Thirst / 100;
    static UnitNeedsDelegate hungerPriority = (u) => u.Hunger / 100;

    static public MoveGoal DRINK = new MoveGoal(thirstPriority, "Drinking", (unit, cell) => cell.TerrainType == TerrainTexture.TEALWATER);
    static public MoveGoal EAT_CORPSE = new MoveGoal((u) => hungerPriority(u) / 10, "Feeding", (unit, cell) => cell.units.Contains(u => u.IsDead && u.UnitName != unit.UnitName));
    static public MoveGoal WANDER = new MoveGoal((u) => Random.value * 10, "Wandering", (unit, cell) => unit.Location.DistanceTo(cell) > 10);
    static public MoveGoal GRAZE = new MoveGoal(hungerPriority, "Grazing", (unit, cell) => true);
    static public MoveGoal HUNT = new MoveGoal((u) => hungerPriority(u) * 2, "Hunting", (unit, cell) => cell.units.Contains(u => u.UnitName != unit.UnitName));

    /** The method for calculating the priority value of this goal for a particular unit. */
    public readonly UnitNeedsDelegate priority;
    /** The human-readable name for this goal to display while the action is being performed. */
    public readonly string goalName;
    /** The condition for determining suitable destination cells for this goal. */
    public readonly HexCellContains neighborContains;

    MoveGoal(UnitNeedsDelegate priority, string goalName, HexCellContains neighborContains)
    {
        this.priority = priority;
        this.goalName = goalName;
        this.neighborContains = neighborContains;
    }

    public override string ToString() { return goalName; }
}
