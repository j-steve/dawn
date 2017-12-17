using UnityEngine;

public class UnitLegacyAnimation : Unit
{
    new Animation animation;

    protected override void Awake()
    {
        animation = GetComponentInChildren<Animation>();
        base.Awake();
    }

    protected override void SetMovement(bool moving)
    {
        if (moving) {
            animation.CrossFade("Walking");
        } else {
            animation.CrossFade("Idle");
        }
        isMoving = moving;
    }
}