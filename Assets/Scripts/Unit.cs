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
    Queue<HexCellCoordinates> goalPath = new Queue<HexCellCoordinates>();
    float timeAtLocation = 0;
    float stayAtLocationUntil;

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
        timeAtLocation += Time.deltaTime;
        if (isMoving)
            return;
        if (goalPath.Count == 0) {
            timeAtLocation = Random.Range(-15f, -2f);
            NewGoal();
        }
        if (timeAtLocation > 5) {
            timeAtLocation = 0;
            var coords = goalPath.Dequeue();
            var destination = HexBoard.ActiveBoard.hexCells[coords];
            transform.LookAt(destination.Center);
            animator.SetTrigger(triggerMoving);
            isMoving = true;
            StopAllCoroutines();
            StartCoroutine(TravelToCell(destination));
        }
    }

    IEnumerator TravelToCell(HexCell destination)
    {
        var origin = HexBoard.ActiveBoard.hexCells[location];
        var travelSpeed = .5f;
        for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed) {
            transform.localPosition = Vector3.Lerp(origin.Center, destination.Center, t);
            yield return null;
        }
        location = destination.Coordinates;
        timeAtLocation = 0;
        animator.SetTrigger(triggerIdle);
        isMoving = false;
    }

    void NewGoal()
    {
        var currentCell = HexBoard.ActiveBoard.hexCells[location];
        var path = pathfinder.FindNearest(
            currentCell,
            c => c != currentCell && c.GetNeighbors().FirstOrDefault(n => n.Elevation == 0) != null);
        Debug.LogFormat("Path length is {0}", path.Count);
        goalPath = new Queue<HexCellCoordinates>();
        for (var i = 1; i < path.Count; i++) {
            goalPath.Enqueue(path[i].Coordinates);
            path[i].Highlight(Color.white);
        }
        path[0].Highlight(Color.green);
        path.Last().Highlight(Color.blue);
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