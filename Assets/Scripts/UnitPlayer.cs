using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UnitPlayer : Unit
{
    private bool isSelected;
    private HexCell currentHoverTarget;
    private IList<HexCell> path;

    private void Update()
    {
        if (isSelected) {
            var cell = HexBoard.ActiveBoard.GetCellUnderCursor();
            if (cell != currentHoverTarget) {
                UnHighlightPath();
                currentHoverTarget = cell;
                MapPathToTarget(cell);
            }
        }
    }

    private void MapPathToTarget(HexCell cell)
    {
        path = pathfinder.Search(Location, cell);
        for (int i = 0; i < path.Count - 1; i++) {
            path[i].Highlight(Color.yellow);
        }
        path[path.Count - 1].Highlight(Color.green);
    }

    public override void Select()
    {
        base.Select();
        HexBoard.ActiveBoard.HexCellClickedEvent += OnHexCellClick;
        isSelected = true;
    }

    void OnHexCellClick(HexCellClickedEventArgs e)
    {
        Debug.LogFormat("{0} knows you clicked {1}", UnitName, e.Cell);
    }

    protected override void onBlur()
    {
        HexBoard.ActiveBoard.HexCellClickedEvent -= OnHexCellClick;
        UnHighlightPath();
        isSelected = false;
    }

    void UnHighlightPath()
    {
        if (path != null) {
            foreach (var cell in path) {
                cell.Highlight(null);
            }
            path = null;
        }
    }
}
