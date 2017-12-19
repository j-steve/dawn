using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class UnitAnimal : Unit
{
    public override string InGameUIDescription { get { return goal; } }

    protected override float TravelSpeed { get { return 0.75f; } }

    protected float timeTilDeparture;

    protected string goal;

    void Update()
    {
        if (!IsMoving) {
            timeTilDeparture -= Time.deltaTime;
            if (timeTilDeparture <= 0) {
                goal = "Seeking water";
                var path = GetNewGoal();
                if (path != null) {
                    StopAllCoroutines();
                    StartCoroutine(TravelToCell(path));
                }
            }
        }
    }

    protected virtual IList<HexCell> GetNewGoal()
    {
        goal = "Seeking water";
        return GetPathToWater();
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

    IList<HexCell> GetPathToWater()
    {
        return pathfinder.FindNearest(
            Location,
            c => c != Location &&
            c.Coordinates.DistanceTo(Location.Coordinates) >= 5 &&
            c.GetNeighbors().Contains(n => n.Elevation == 0));
    }

    protected override void ArrivedAtCell()
    {
        goal = "Drinking";
        timeTilDeparture = UnityEngine.Random.Range(5f, 20f);
    }
}