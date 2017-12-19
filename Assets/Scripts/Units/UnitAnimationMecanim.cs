using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimationMecanim : UnitAnimation
{
    static readonly int triggerMoving = Animator.StringToHash("Moving");
    static readonly int triggerIdle = Animator.StringToHash("Idle");

    public override void SetAnimation(UnitAnimationType animationType)
    {
        GetComponent<Animator>().SetTrigger(GetTriggerName(animationType));
    }

    int GetTriggerName(UnitAnimationType animationType)
    {
        switch (animationType) {
            case UnitAnimationType.MOVE:
                return triggerMoving;
            case UnitAnimationType.IDLE:
            default:
                return triggerIdle;
        }
    }
}