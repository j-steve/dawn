using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class UnitAnimalPredator : UnitAnimal
{
    Unit target;

    HexCell prevTargetCell;

    protected override bool IsMoving {
        get { return base.IsMoving; }
        set {
            base.IsMoving = value;
            if (value && goal.StartsWith("Stalking")) {
                SetAnimation(UnitAnimationType.STALK);
            }
        }
    }

    protected override IList<HexCell> GetNewGoal()
    {
        if (Random.value > 0.9995f) { // TODO: FIX THIS!!!!!!!!!!!!!!!!!!!!!!!!!!! (lower value back to 0.5f)
            return base.GetNewGoal();
        } else {
            return Stalk();
        }
    }

    IList<HexCell> Stalk()
    {
        var path = GetPathToPrey();
        if (path.Count > 0) {
            var lastCell = path.Last();
            target = lastCell.units.Where(u => u.UnitName != UnitName).FirstOrDefault();
            goal = "Stalking {0}".Format(target);
            path.RemoveAt(path.Count - 1);  // Goal is to get next to target.
            if (prevTargetCell != null)
                prevTargetCell.UnHighlight();
            prevTargetCell = path.Last();
        }
        return path;
    }

    IList<HexCell> GetPathToPrey()
    {
        return pathfinder.FindNearest(
            Location,
            c => c != Location && c.units.Contains(u => !u.IsDead && u.UnitName != UnitName)
            ) ?? new List<HexCell>();
    }

    protected override void ArrivedAtCell()
    {
        if (goal == "Seeking water") {
            base.ArrivedAtCell();
        } else {
            if (!Attack(target)) {
                // The stalked prey is gone, stalk again.
                Debug.LogFormat(gameObject, "Prey is gone ({0} -> {1}), restalking {2}", Location, target.Location, target);
                var path = Stalk();
                Destination = path.LastOrDefault();
                StopAllCoroutines();
                StartCoroutine(TravelToCell(path));
            }
        }
    }

    /// <summary>
    /// Attacks the given victim unit. Returns false if unable to attack.
    /// </summary>
    bool Attack(Unit victim)
    {
        if (target.IsDead || target.Location.Coordinates.DistanceTo(Location.Coordinates) != 1) {
            return false;
        }
        Debug.LogFormat(gameObject, "Attacking target: {0}", victim);
        goal = "Attacking {0}".Format(target);
        AttackedBy(victim);
        victim.AttackedBy(this);
        timeTilDeparture = Random.Range(1f, 10f);
        return true;
    }

    protected override void CombatWon(Unit opponent)
    {
        base.CombatWon(opponent);
        SetAnimation(UnitAnimationType.EAT);
        goal = "Eating {0}".Format(opponent);
        timeTilDeparture = Random.Range(5f, 10f);
    }
}
