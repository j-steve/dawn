using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitMecanimAnimation : Unit
{
    static readonly int triggerMoving = Animator.StringToHash("Moving");
    static readonly int triggerIdle = Animator.StringToHash("Idle");

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        SetMovement(false);
    }

    protected override void SetMovement(bool moving)
    {
        if (moving) {
            animator.SetTrigger(triggerMoving);
        } else {
            animator.SetTrigger(triggerIdle);
        }
        isMoving = moving;
    }
}