using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class UnitPlayer : Unit
{
    const float MOVEMENT_POINTS = 3;

    protected override float TravelSpeed { get { return 1f; } }

    HexCell currentHoverTarget;

    IList<PathStep> pathSteps;

    public override void OnFocus()
    {
        base.OnFocus();
        HexBoard.Active.HexCellClickedEvent += OnHexCellClick;
    }

    public override void OnBlur()
    {
        base.OnBlur();
        HexBoard.Active.HexCellClickedEvent -= OnHexCellClick;
        UnHighlightPath();
    }

    protected override void TakeAction()
    {
        if (SelectionInfoPanel.Instance.IsSelected(this)) {
            var cell = HexBoard.Active.GetCellUnderCursor();
            if (cell != currentHoverTarget) {
                UnHighlightPath();
                if (cell != null && cell != Location) {
                    currentHoverTarget = cell;
                    MapPathToTarget(cell);
                }
            }
        }
    }

    internal void CreateVillage()
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.transform.localScale *= 5;
        cube.transform.parent = Location.transform;
        var newpos  = Location.transform.position;
        newpos.y += cube.transform.localScale.y / 2;
        cube.transform.position = newpos;
        Destroy(gameObject);
    }

    void MapPathToTarget(HexCell target)
    {
        pathSteps = pathfinder.AStarSearch(Location, target);
        var lastStep = pathSteps.LastOrDefault();
        foreach (var step in pathSteps) {
            int turns = Mathf.FloorToInt(step.cost / MOVEMENT_POINTS);
            var color = step == lastStep ? Color.green : Color.white;
            step.cell.Highlight(color, turns.ToString());
        }
    }

    void OnHexCellClick(HexCellClickedEventArgs e)
    {
        Debug.LogFormat("{0} knows you clicked {1}", UnitName, e.Cell);
        if (!IsDead && !IsMoving && e.Cell != Location) {
            StartCoroutine(TravelToCell(pathSteps.Select(p => p.cell).ToList()));
            UnHighlightPath();
            e.Cancel = true;
            SelectionInfoPanel.Instance.SetSelected(null);

        }
    }

    void UnHighlightPath()
    {
        if (pathSteps != null) {
            foreach (var step in pathSteps) {
                step.cell.UnHighlight();
            }
            pathSteps = null;
        }
    }

    protected override float calculateMovementCost(HexCell c1, HexCell c2)
    {
        if (c2.Elevation == 0) { return 100; }
        return Math.Abs(c1.Elevation - c2.Elevation) * 2 + 1;
    }

}
