using UnityEngine;
using System.Collections;

public class UnitAnimal : Unit
{
    float timeTilDeparture;

    protected override float TravelSpeed { get { return 0.75f; } }
    protected override string SelectedDescription { get { return Goal; } }

    bool isSelected;

    string Goal {
        get { return _goal; }
        set {
            if (isSelected) {
                UIInGame.ActiveInGameUI.UpdateDescription(value);
            }
            _goal = value;
        }
    }
    string _goal;

    void Update()
    {
        if (!IsMoving) {
            timeTilDeparture -= Time.deltaTime;
            if (timeTilDeparture <= 0) {
                Goal = "Seeking water";
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
        Goal = "Drinking";
        timeTilDeparture = Random.Range(5f, 20f);
    }


    public override void Select()
    {
        base.Select();
        isSelected = true;
    }

    protected override void onBlur()
    {
        isSelected = false;
    }

}
