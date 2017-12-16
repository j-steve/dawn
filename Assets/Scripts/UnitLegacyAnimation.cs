using UnityEngine;

public class UnitLegacyAnimation : Unit
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
            animation.CrossFade("Walking");
        } else {
            animation.CrossFade("Idle");
        }
        isMoving = moving;
    }
}