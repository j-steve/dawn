using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class UnitAnimalPredator : UnitAnimal
{
    protected override IList<HexCell> GetNewGoal()
    {
        if (Random.value > 0.5f) {
            return base.GetNewGoal();
        } else {
            var path = GetPathToPrey();
            var unit = path.Last().units.FirstOrDefault();
            goal = string.Format("Stalking {0}", unit == null ? "" : unit.UnitName);
            return path;
        }
    }

    IList<HexCell> GetPathToPrey()
    {
        return pathfinder.FindNearest(
            Location,
            c => c != Location &&
            c.GetNeighbors().Contains(cellContainsPrey));
    }

    protected override void ArrivedAtCell()
    {
        if (goal == "Seeking water") {
            base.ArrivedAtCell();
        } else {
            if (Location.GetNeighbors().FirstOrDefault(n => n.units.Count > 0) == null) {
                // The stalked prey is gone, stalk again.
                StopAllCoroutines();
                StartCoroutine(TravelToCell(GetPathToPrey()));
            } else {
                goal = "Hunting";
                timeTilDeparture = Random.Range(1f, 10f);
            }
        }
    }

    bool cellContainsPrey(HexCell cell)
    {
        return cell.units.Contains(u => u.UnitName != UnitName);
    }
}
