using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class UnitAnimal : Unit
{
    public override string InGameUITitle {
        get {
            return string.Format("{0} hunger:{1:00}% thirst:{2:00}%", base.InGameUITitle, Hunger, Thirst);
        }
    }
    public override string InGameUIDescription {
        get {
            return IsDead ? "Dead" : CombatOpponent ? "Fighting {0}".Format(CombatOpponent) : goal;
        }
    }

    protected override float TravelSpeed { get { return 0.75f; } }

    protected float timeTilDeparture;

    protected string goal;

    protected HexCell Destination {
        get { return _destination; }
        set {
            if (UIInGame.ActiveInGameUI.selection == (ISelectable)this) {
                if (_destination)
                    _destination.UnHighlight();
                if (value)
                    value.Highlight(Color.blue);
            }
            _destination = value;
        }
    }
    HexCell _destination;

    protected override void TakeAction()
    {
        Hunger -= Time.deltaTime * .005f * Hunger;
        Thirst -= Time.deltaTime * .005f * Thirst;
        if (!CombatOpponent) {
            timeTilDeparture -= Time.deltaTime;
            if (timeTilDeparture <= 0) {
                goal = "Seeking water";
                var path = GetNewGoal();
                if (path != null && path.Count > 0) {
                    Destination = path.Last();
                    StartCoroutine(TravelToCell(path));
                }
            }
        }
    }

    protected virtual IList<HexCell> GetNewGoal()
    {
        goal = "Seeking water";
        return GetPathToWater().Select(c => c.cell).ToList();
    }


    //protected virtual Dictionary<string, Func<HexCell, HexCell, bool>> GetGoals()
    //{
    //    return new Dictionary<string, Func<HexCell, HexCell, bool>>() {
    //        {
    //            "water",
    //            (HexCell c1, HexCell c2) => c1.Coordinates.DistanceTo(c2.Coordinates) >= 5 && c2.GetNeighbors().FirstOrDefault(n => n.Elevation == 0) != null
    //        }
    //    };
    //}

    IList<PathStep> GetPathToWater()
    {
        return pathfinder.FindNearest(
            Location,
            c => c != Location &&
            c.Coordinates.DistanceTo(Location.Coordinates) >= 5 &&
            c.GetNeighbors().Contains(n => n.Elevation == 0)) ?? new List<PathStep>();
    }

    protected override void ArrivedAtCell()
    {
        Destination = null;
        goal = "Drinking";
        timeTilDeparture = UnityEngine.Random.Range(5f, 10f);
    }

    public override void OnFocus()
    {
        base.OnFocus();
        if (Destination) {
            Destination.Highlight(Color.blue);
        }
    }

    public override void OnBlur()
    {
        base.OnBlur();
        if (Destination) {
            Destination.UnHighlight();
        }
    }
}