using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Unit2 : Unit
{
    new Animation animation;

    private void Awake()
    {
        animation = GetComponentInChildren<Animation>();
        SetMovement(false);
    }

    protected override void SetMovement(bool moving)
    {
        if (moving) {
            animation.Play("Walking");
        }
        else {
            animation.Play("Idle");
        }
        isMoving = moving;
    }
}