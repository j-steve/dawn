﻿using UnityEngine;
using System.Collections;

public class UnitAnimal : Unit
{
    float timeTilDeparture;

    protected override float TravelSpeed { get { return 0.75f; } }

    void Update()
    {
        if (!IsMoving) {
            timeTilDeparture -= Time.deltaTime;
            if (timeTilDeparture <= 0) {
                var path = GetNewTravelPath();
                if (path != null) {
                    StopAllCoroutines();
                    StartCoroutine(TravelToCell(path));
                }
            }
        }
    }

    protected override void ArrivedAtCell()
    {
        timeTilDeparture = Random.Range(5f, 20f);
    }
}
