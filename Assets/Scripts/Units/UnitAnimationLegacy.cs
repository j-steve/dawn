using UnityEngine;

public class UnitAnimationLegacy : UnitAnimation
{
    public override void SetAnimation(UnitAnimationType animationType)
    {
        GetComponentInChildren<Animation>().CrossFade(GetAnimationName(animationType));
    }

    string GetAnimationName(UnitAnimationType animationType)
    {
        switch (animationType) {
            case UnitAnimationType.MOVE:
                return "Walking";
            case UnitAnimationType.IDLE:
            default:
                return "Idle";
        }
    }
}