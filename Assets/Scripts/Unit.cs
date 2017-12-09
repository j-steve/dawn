using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class Unit : MonoBehaviour
{
    #region Static

    static HexPathfinder pathfinder = new HexPathfinder();

    static readonly int triggerMoving = Animator.StringToHash("Moving");
    static readonly int triggerIdle = Animator.StringToHash("Idle");

    static public Unit Create(Unit prefab, HexCell cell)
    {
        Unit obj = Instantiate(prefab);
        obj.Initialize(cell);
        return obj;
    }

    #endregion

    Animator animator;

    bool isMoving = false;

    public HexCellCoordinates location;
    float timeTilDeparture;

    void Initialize(HexCell cell)
    {
        animator = this.GetRequiredComponent<Animator>();
        StartCoroutine(StartIdleAnimation());
        SetPosition(cell);
        transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    void SetPosition(HexCell cell)
    {
        transform.localPosition = cell.Center;
        location = cell.Coordinates;
    }

    void Update()
    {
        if (isMoving)
            return;
        timeTilDeparture -= Time.deltaTime;
        if (timeTilDeparture <= 0) {
            var path = GetNewTravelPath();
            animator.SetTrigger(triggerMoving);
            isMoving = true;
            StopAllCoroutines();
            StartCoroutine(TravelToCell(path));
        }
    }

    IEnumerator TravelToCell(IList<HexCell> path)
    {
        var origin = HexBoard.ActiveBoard.hexCells[location];
        foreach (var cell in path.Skip(1)) {
            var travelSpeed = .5f;
            for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed) {
                var rotation = t * 2;
                if (rotation < 1f) {
                    Quaternion fromRotation = transform.localRotation;
                    Quaternion toRotation =
                        Quaternion.LookRotation(cell.Center - transform.localPosition);
                    transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, rotation);
                }
                transform.localPosition = Vector3.Lerp(origin.Center, cell.Center, t);
                yield return null;
            }
            origin = cell;
        }
        // Reset any vertical rotation so unit is level on map.
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.ScaledBy(Vector3.up));

        location = origin.Coordinates;
        animator.SetTrigger(triggerIdle);
        isMoving = false;
        timeTilDeparture = Random.Range(5f, 20f);
    }

    IList<HexCell> GetNewTravelPath()
    {
        var currentCell = HexBoard.ActiveBoard.hexCells[location];
        var path = pathfinder.FindNearest(
            currentCell,
            c => c != currentCell &&
            c.Coordinates.DistanceTo(currentCell.Coordinates) >= 5 &&
            c.GetNeighbors().FirstOrDefault(n => n.Elevation == 0) != null);
        path[0].Highlight(Color.green);
        path.Last().Highlight(Color.blue);
        return path;
    }

    /// <summary>
    /// Wait a short random interval before starting the idle animation loop,
    /// to prevent all units from having the exact same idle animation schedule.
    /// </summary>
    IEnumerator StartIdleAnimation()
    {
        var secs = Random.Range(0f, 2f);
        yield return new WaitForSeconds(secs);
        if (!isMoving)
            animator.SetTrigger(triggerIdle);
    }

    IEnumerator TriggerMoveAaimation()
    {
        while (true) {
            var secs = Random.Range(0f, 10f);
            yield return new WaitForSeconds(secs);
            Debug.LogFormat("Done waiing {0} secs", secs);
            if (isMoving) {
                animator.SetTrigger(triggerIdle);
                isMoving = false;
            }
            else {
                animator.SetTrigger(triggerMoving);
                isMoving = true;
            }
        }
    }
}