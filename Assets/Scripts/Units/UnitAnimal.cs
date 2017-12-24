using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class UnitAnimal : Unit
{
    static Dictionary<string, MoveGoal[]> goals = new Dictionary<string, MoveGoal[]> {
        {"Moose",  new MoveGoal[] { MoveGoal.DRINK, MoveGoal.GRAZE, MoveGoal.WANDER} },
        {"Wolf",  new MoveGoal[] { MoveGoal.DRINK, MoveGoal.EAT_CORPSE, MoveGoal.HUNT, MoveGoal.WANDER } },
    };

    public override string InGameUITitle {
        get {
            return string.Format("{0} hunger:{1:00}% thirst:{2:00}%", base.InGameUITitle, Hunger, Thirst);
        }
    }
    public override string InGameUIDescription {
        get {
            return IsDead ? "Dead" : CombatOpponent ? "Fighting {0}".Format(CombatOpponent) : goal.goalName;
        }
    }

    protected override float TravelSpeed { get { return 0.75f; } }

    protected float timeTilDeparture;

    protected MoveGoal goal;

    protected bool atGoal;

    protected HexCell Destination {
        get { return _destination; }
        set {
            if (UIInGame.Instance.selection == (ISelectable)this) {
                if (_destination)
                    _destination.UnHighlight();
                if (value)
                    value.Highlight(Color.blue);
            }
            _destination = value;
        }
    }
    HexCell _destination;

    protected override void Update()
    {
        if (!IsDead) {
            Hunger -= Time.deltaTime * .005f * Hunger;
            Thirst -= Time.deltaTime * .010f * Thirst;
        }
        base.Update();
    }

    protected override void TakeAction()
    {
        if (!CombatOpponent) {
            timeTilDeparture -= Time.deltaTime;
            if (atGoal) {
                if (goal == MoveGoal.GRAZE || goal == MoveGoal.EAT_CORPSE) {
                    Hunger += Time.deltaTime * 5;
                } else if (goal == MoveGoal.DRINK) {
                    Thirst += Time.deltaTime * 10;
                }
            }
            if (timeTilDeparture <= 0) {
                atGoal = false;
                var path = GetNewGoalPath();
                Destination = path.LastOrDefault();
                if (path.Count == 1) {
                    ArrivedAtCell();
                } else {
                    StartCoroutine(TravelToCell(path));
                }
            }
        }
    }

    IList<HexCell> GetNewGoalPath()
    {
        var rankedGoals = new Priority_Queue.SimplePriorityQueue<MoveGoal>();
        foreach (var potentialGoal in goals[UnitName]) {
            float priority = potentialGoal.priority(this) * Random.value;
            rankedGoals.Enqueue(potentialGoal, priority);
        }
        while (rankedGoals.TryDequeue(out goal)) {
            var path = pathfinder.FindNearest(
                Location,
                c => c.GetNeighbors().Contains(n => goal.neighborContains(this, n)));
            if (path.Count > 0) {
                return path.Select(c => c.cell).ToList();
            }
        }
        Debug.LogWarning(name + ": No valid goals.", this);
        goal = null;
        timeTilDeparture = 10; // Check again in 10 seconds.
        return new List<HexCell>();
    }

    protected override void ArrivedAtCell()
    {
        if (goal == MoveGoal.WANDER) {
            // No action required on reaching this destination. Move on.
            timeTilDeparture = 0;
            return;
        }
        var target = Location.GetNeighbors().FirstOrDefault(n => goal.neighborContains(this, n));
        if (target) {
            transform.LookAt(target.Center);
            atGoal = true;
            timeTilDeparture = Random.Range(5f, 10f);
            Debug.LogFormat(this, "{0} arrived at cell for {1}, til {2}", name, goal, timeTilDeparture);
            if (goal == MoveGoal.HUNT) {
                var victim = target.units.FirstOrDefault(u => u.UnitName != UnitName);
                AttackedBy(victim);
                victim.AttackedBy(this);
                timeTilDeparture = 0;
            } else if (goal == MoveGoal.DRINK) {
                SetAnimation(UnitAnimationType.DRINK);
            } else {
                SetAnimation(UnitAnimationType.EAT);
            }
        } else {
            Debug.LogWarningFormat(this, "{0} reached destination for {1} but no good!", name, goal);
        }

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