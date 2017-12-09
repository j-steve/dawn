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
        obj.SetPosition(cell);
        return obj;
    }

    #endregion

    private Animator animator;

    private bool isMoving = false;

    public HexCellCoordinates location;
    private Queue<HexCellCoordinates> goalPath = new Queue<HexCellCoordinates>();
    private float timeAtLocation = 0;
    private float stayAtLocationUntil;

    private void Awake()
    {
        animator = this.GetRequiredComponent<Animator>();
        StartCoroutine(StartIdleAnimation());
    }

    private void SetPosition(HexCell cell)
    {
        transform.localPosition = cell.Center;
        location = cell.Coordinates;
    }

    private void Update()
    {
        timeAtLocation += Time.deltaTime;
        if (goalPath.Count == 0) {
            NewGoal();
        }
        if (timeAtLocation > 2) {
            var coords = goalPath.Dequeue();
            var cell = HexBoard.ActiveBoard.hexCells[coords];
            SetPosition(cell);
            timeAtLocation = 0;
        }
    }

    private void NewGoal()
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