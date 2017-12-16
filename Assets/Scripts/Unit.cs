﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

abstract public class Unit : MonoBehaviour
{
    #region Static

    static HexPathfinder pathfinder = new HexPathfinder();

    static public Unit Create(Unit prefab, HexCell cell)
    {
        Unit obj = Instantiate(prefab);
        obj.Initialize(cell);
        return obj;
    }

    #endregion

    public string UnitName { get { return unitName; } }

    [SerializeField] string unitName;

    HexCell location {
        get {
            return _location;
        }
        set {
            if (_location != null) {
                _location.units.Remove(this);
            }
            _location = value;
            _location.units.Add(this);
        }
    }

    HexCell _location;

    protected bool isMoving = false;

    float timeTilDeparture;

    void Initialize(HexCell cell)
    {
        StartCoroutine(StartIdleAnimation());
        location = cell;
        transform.localPosition = cell.Center;
        transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    void Update()
    {
        if (!isMoving) {
            timeTilDeparture -= Time.deltaTime;
            if (timeTilDeparture <= 0) {
                var path = GetNewTravelPath();
                SetMovement(true);
                StopAllCoroutines();
                StartCoroutine(TravelToCell(path));
            }
        }
    }

    IEnumerator TravelToCell(IList<HexCell> path)
    {
        foreach (var cell in path.Skip(1)) {
            var lastLocation = location;
            location = cell;
            var travelSpeed = .5f;
            for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed) {
                var rotation = t * 2;
                if (rotation < 1f) {
                    Quaternion fromRotation = transform.localRotation;
                    Quaternion toRotation =
                        Quaternion.LookRotation(cell.Center - transform.localPosition);
                    transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, rotation);
                }
                transform.localPosition = Vector3.Lerp(lastLocation.Center, cell.Center, t);
                yield return null;
            }
        }
        // Reset any vertical rotation so unit is level on map.
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.ScaledBy(Vector3.up));
        SetMovement(false);
        timeTilDeparture = Random.Range(5f, 20f);
    }

    IList<HexCell> GetNewTravelPath()
    {
        return pathfinder.FindNearest(
            location,
            c => c != location &&
            c.Coordinates.DistanceTo(location.Coordinates) >= 5 &&
            c.GetNeighbors().FirstOrDefault(n => n.Elevation == 0) != null);
    }

    /// <summary>
    /// Wait a short random interval before starting the idle animation loop,
    /// to prevent all units from having the exact same idle animation schedule.
    /// </summary>
    IEnumerator StartIdleAnimation()
    {
        var secs = Random.Range(0f, 2f);
        yield return new WaitForSeconds(secs);
        if (!isMoving) {
            SetMovement(false);
        }
    }

    IEnumerator TriggerMoveAaimation()
    {
        while (true) {
            var secs = Random.Range(0f, 10f);
            yield return new WaitForSeconds(secs);
            SetMovement(!isMoving);
        }
    }

    abstract protected void SetMovement(bool moving);
}