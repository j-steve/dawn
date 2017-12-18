using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class UnitAnimal : Unit
{
    float timeTilDeparture;

    protected override float TravelSpeed { get { return 0.75f; } }

    public override string Description { get { return goal; } }

    string goal;

    void Update()
    {
        if (!IsMoving) {
            timeTilDeparture -= Time.deltaTime;
            if (timeTilDeparture <= 0) {
                goal = "Seeking water";
                var path = GetNewTravelPath();
                if (path != null) {
                    StopAllCoroutines();
                    StartCoroutine(TravelToCell(path));
                }
            }
        }
    }

    IList<HexCell> GetNewTravelPath()
    {
        return pathfinder.FindNearest(
            Location,
            c => c != Location &&
            c.Coordinates.DistanceTo(Location.Coordinates) >= 5 &&
            c.GetNeighbors().FirstOrDefault(n => n.Elevation == 0) != null);
    }

    protected override void ArrivedAtCell()
    {
        goal = "Drinking";
        timeTilDeparture = Random.Range(5f, 20f);
    }


}
