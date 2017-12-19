using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimationMecanim : UnitAnimation
{
    static readonly int triggerMoving = Animator.StringToHash("Moving");
    static readonly int triggerIdle = Animator.StringToHash("Idle");
    static readonly int triggerEating = Animator.StringToHash("Eating");
    static readonly int triggerFighting = Animator.StringToHash("Fighting");
    static readonly int triggerDeath = Animator.StringToHash("Death");

    public override void SetAnimation(UnitAnimationType animationType)
    {
        GetComponent<Animator>().SetTrigger(GetTriggerName(animationType));
    }

    int GetTriggerName(UnitAnimationType animationType)
    {
        switch (animationType) {
            case UnitAnimationType.EAT:
                return triggerEating;
            case UnitAnimationType.MOVE:
            case UnitAnimationType.STALK:
                return triggerMoving;
            case UnitAnimationType.FIGHT:
                return triggerFighting;
            case UnitAnimationType.DEATH:
                return triggerDeath;
            case UnitAnimationType.IDLE:
            default:
                return triggerIdle;
        }
    }
}