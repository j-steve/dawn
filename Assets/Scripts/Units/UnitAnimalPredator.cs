using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class UnitAnimalPredator : UnitAnimal
{
    Unit target;

    HexCell prevTargetCell;

    //IList<HexCell> Stalk()
    //{
    //    var path = GetPathToPrey().Select(c => c.cell).ToList();
    //    if (path.Count > 0) {
    //        var lastCell = path.Last();
    //        target = lastCell.units.Where(u => u.UnitName != UnitName).FirstOrDefault();
    //        goal = "Stalking {0}".Format(target);
    //        path.RemoveAt(path.Count - 1);  // Goal is to get next to target.
    //        if (prevTargetCell)
    //            prevTargetCell.UnHighlight();
    //        prevTargetCell = path.Last();
    //    }
    //    return path;
    //}

    //IList<PathStep> GetPathToPrey()
    //{
    //    return pathfinder.FindNearest(
    //        Location,
    //        c => c != Location && c.units.Contains(u => !u.IsDead && u.UnitName != UnitName)
    //        ) ?? new List<PathStep>();
    //}

    //protected override void ArrivedAtCell()
    //{
    //    if (goal == "Seeking water") {
    //        base.ArrivedAtCell();
    //    } else {
    //        if (!Attack(target)) {
    //            // The stalked prey is gone, stalk again.
    //            Debug.LogFormat(this, "Prey is gone ({0} -> {1}), restalking {2}", Location, target.Location, target);
    //            var path = Stalk();
    //            Destination = path.LastOrDefault();
    //            StartCoroutine(TravelToCell(path));
    //        }
    //    }
    //}

    /// <summary>
    /// Attacks the given victim unit. Returns false if unable to attack.
    /// </summary>
    //bool Attack(Unit victim)
    //{
    //    if (target.IsDead || target.Location.Coordinates.DistanceTo(Location.Coordinates) != 1) {
    //        return false;
    //    }
    //    Debug.LogFormat(this, "Attacking target: {0}", victim);
    //    //goal = "Attacking {0}".Format(target);
    //    AttackedBy(victim);
    //    victim.AttackedBy(this);
    //    timeTilDeparture = Random.Range(1f, 10f);
    //    return true;
    //}

    protected override void CombatWon(Unit opponent)
    {
        base.CombatWon(opponent);
        Debug.LogFormat(this, "{0} won combat against {1}", name, opponent.name);
        timeTilDeparture = 0;
    }
}
