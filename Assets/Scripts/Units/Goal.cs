using UnityEngine;
using System.Collections;
using System;

public interface IGoal { }

public struct MoveGoal : IGoal
{
    static public MoveGoal DRINK = new MoveGoal(1, "Moving to drink", (unit, cell) => cell.TerrainType == TerrainTexture.TEALWATER);
    static public MoveGoal EAT_CORPSE = new MoveGoal(2, "Moving to feed", (unit, cell) => cell.units.Contains(u => u.IsDead && u.UnitName != unit.UnitName));
    static public MoveGoal GRAZE = new MoveGoal(3, "Moving to feed", (unit, cell) => unit.Location.DistanceTo(cell) > 10);
    static public MoveGoal HUNT = new MoveGoal(4, "Moving to hunt", (unit, cell) => cell.units.Contains(u => u.UnitName != unit.UnitName));

    public delegate bool HexCellContains(Unit unit, HexCell cell);

    private readonly int priority;
    private readonly string goalName;
    private readonly HexCellContains neighborContains;

    MoveGoal(int priority, string goalName, HexCellContains neighborContains)
    {
        this.priority = priority;
        this.goalName = goalName;
        this.neighborContains = neighborContains;
    }

    public override string ToString() { return goalName; }
}
