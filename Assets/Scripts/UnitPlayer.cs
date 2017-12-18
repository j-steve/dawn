using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class UnitPlayer : Unit
{
    private bool isSelected;
    private HexCell currentHoverTarget;
    private IList<HexCell> path;

    const float MOVEMENT_POINTS = 3;

    protected override float TravelSpeed { get { return 1f; } }
    protected override string SelectedDescription { get { return "(player)"; } }

    private void Update()
    {
        if (isSelected) {
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

    private void MapPathToTarget(HexCell target)
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

    public override void Select()
    {
        base.Select();
        HexBoard.ActiveBoard.HexCellClickedEvent += OnHexCellClick;
        isSelected = true;
    }

    void OnHexCellClick(HexCellClickedEventArgs e)
    {
        Debug.LogFormat("{0} knows you clicked {1}", UnitName, e.Cell);
        if (e.Cell != Location) {
            StopAllCoroutines();
            StartCoroutine(TravelToCell(path));
            UnHighlightPath();
            e.Cancel = true;
            UIInGame.ActiveInGameUI.HideUI();
        }
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
                cell.UnHighlight();
            }
            path = null;
        }
    }
}
