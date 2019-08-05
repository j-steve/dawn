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

    IList<PathStep> pathSteps;

    #region ISelectable

    public override void OnFocus(InGameUI ui)
    {
        base.OnFocus(ui);
        ui.labelDescription.text = "";
        ui.labelDetails.text = "";
        ui.addBuildingButton.gameObject.SetActive(true);
        ui.addBuildingButton.onClick.AddListener(ShowCreateVillageDialog);
        HexBoard.Active.HexCellClickedEvent += OnHexCellClick;
    }

    public override void OnBlur(InGameUI ui)
    {
        base.OnBlur(ui);
        ui.addBuildingButton.gameObject.SetActive(false);
        ui.addBuildingButton.onClick.RemoveListener(ShowCreateVillageDialog);
        HexBoard.Active.HexCellClickedEvent -= OnHexCellClick;
        UnHighlightPath();
    }

    void ShowCreateVillageDialog()
    {
        InGameUI.Instance.createVillageDialog.Show(() => {
            Village.CreateVillage(this, InGameUI.Instance.createVillageName.text);
        });
    }

    #endregion

    protected override void TakeAction()
    {
        if (InGameUI.Instance.IsSelected(this)) {
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
            InGameUI.Instance.SetSelected(null);

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
