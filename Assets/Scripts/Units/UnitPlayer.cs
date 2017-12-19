using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class UnitPlayer : Unit
{
    const float MOVEMENT_POINTS = 3;

    protected override float TravelSpeed { get { return 1f; } }

    HexCell currentHoverTarget;

    IList<HexCell> path;

    public override void OnFocus()
    {
        base.OnFocus();
        HexBoard.ActiveBoard.HexCellClickedEvent += OnHexCellClick;
    }

    public override void OnBlur()
    {
        base.OnBlur();
        HexBoard.ActiveBoard.HexCellClickedEvent -= OnHexCellClick;
        UnHighlightPath();
    }

    protected override void TakeAction()
    {
        if (UIInGame.ActiveInGameUI.selection == (ISelectable)this) {
            var cell = HexBoard.ActiveBoard.GetCellUnderCursor();
            if (cell != currentHoverTarget) {
                UnHighlightPath();
                if (cell != Location) {
                    currentHoverTarget = cell;
                    MapPathToTarget(cell);
                }
            }
        }
    }

    void MapPathToTarget(HexCell target)
    {
        path = pathfinder.Search(Location, target);
        float cost = 0;
        HexCell prevCell = Location;
        HexCell lastCell = path.Last();
        foreach (var cell in path) {
            cost += pathfinder.MovementCost(prevCell, cell);
            int turns = Mathf.FloorToInt(cost / MOVEMENT_POINTS);
            var color = cell == lastCell ? Color.green : Color.white;
            cell.Highlight(color, turns.ToString());
            prevCell = cell;
        }
    }

    void OnHexCellClick(HexCellClickedEventArgs e)
    {
        Debug.LogFormat("{0} knows you clicked {1}", UnitName, e.Cell);
        if (e.Cell != Location) {
            StopAllCoroutines();
            StartCoroutine(TravelToCell(path));
            UnHighlightPath();
            e.Cancel = true;
            UIInGame.ActiveInGameUI.SetSelected(null);

        }
    }

    void UnHighlightPath()
    {
        if (path != null) {
            foreach (var cell in path) {
                cell.UnHighlight();
            }
            path = null;
        }
    }
}
