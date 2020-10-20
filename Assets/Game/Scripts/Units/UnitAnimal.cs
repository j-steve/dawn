using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class UnitAnimal : Unit
{
    static Dictionary<Behavior, MoveGoal[]> goals = new Dictionary<Behavior, MoveGoal[]> {
        {Behavior.Grazer, new MoveGoal[] { MoveGoal.DRINK, MoveGoal.GRAZE, MoveGoal.WANDER} },
        {Behavior.SmallGameHunter, new MoveGoal[] { MoveGoal.DRINK, MoveGoal.GRAZE, MoveGoal.WANDER} },
        {Behavior.Predator,  new MoveGoal[] { MoveGoal.DRINK, MoveGoal.EAT_CORPSE, MoveGoal.HUNT, MoveGoal.WANDER } },
    };

    static public UnitAnimal Create(HexCell cell, AnimalType animalType)
    {
        var prefab = Resources.Load<GameObject>("Animals/Prefabs/" + animalType.prefabs.GetRandom());
        var gameObject = Instantiate(prefab);
        var animalUnit = gameObject.AddComponent<UnitAnimal>();
        animalUnit.animalType = animalType;
        animalUnit._unitName = animalType.name;
        animalUnit._attackPower = animalType.ferocity;
        animalUnit.Initialize(cell);
        return animalUnit;
    }

    public AnimalType animalType;

    protected override float TravelSpeed { get { return 0.75f; } }

    protected override float calculateMovementCost(HexCell c1, HexCell c2)
    {
        if (c2.Elevation == 0) { return float.MaxValue; }
        float cost = System.Math.Abs(c1.Elevation - c2.Elevation) * 2 + 1;
        if (!animalType.biomes.Contains(c2.biome)) {
            cost *= 2; // More expensive to travel through non-preferred biomes.
        }
        return cost;
    }

    protected float timeTilDeparture;

    protected MoveGoal goal;

    protected bool atGoal;

    protected HexCell Destination {
        get { return _destination; }
        set {
            if (InGameUI.Instance.selection == (ISelectable)this) {
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
        foreach (var potentialGoal in goals[animalType.behavior[0]]) {
            float priority = potentialGoal.priority(this) * Random.value;
            rankedGoals.Enqueue(potentialGoal, priority);
        }
        while (rankedGoals.TryDequeue(out goal)) {
            var path = pathfinder.BreadthFirstSearch(
                Location,
                c => c.GetNeighbors().Contains(n => goal.neighborContains(this, n)),
                20);
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
            LookAt(target.Center);
            atGoal = true;
            timeTilDeparture = Random.Range(5f, 10f);
            //Debug.LogFormat(this, "{0} arrived at cell for {1}, til {2}", name, goal, timeTilDeparture);
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
            Debug.LogWarningFormat(this, "{0} reached destination for {1} but target is gone!", name, goal);
        }

    }

    #region ISelectable

    public override void OnFocus(InGameUI ui)
    {
        base.OnFocus(ui);
        if (Destination) {
            Destination.Highlight(Color.blue);
        }
    }

    public override void OnBlur(InGameUI ui)
    {
        base.OnBlur(ui);
        if (Destination) {
            Destination.UnHighlight();
        }
    }

    public override void OnUpdateWhileSelected(InGameUI ui)
    {
        base.OnUpdateWhileSelected(ui);
        ui.labelDescription.text = string.Format("hunger:{0:00}% thirst:{1:00}%", Hunger, Thirst);
        ui.labelDetails.text = IsDead ? "Dead" : CombatOpponent ? "Fighting {0}".Format(CombatOpponent) : goal != null ? goal.goalName : "";
    }

    #endregion

}