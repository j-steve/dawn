using UnityEngine;

public class UnitAnimationLegacy : UnitAnimation
{
    [SerializeField] AnimationClip idle;
    [SerializeField] AnimationClip moving;
    [SerializeField] AnimationClip fighting;

    public override void SetAnimation(UnitAnimationType animationType)
    {
        GetComponentInChildren<Animation>().CrossFade(GetAnimationName(animationType));
    }

    string GetAnimationName(UnitAnimationType animationType)
    {
        switch (animationType) {
            case UnitAnimationType.MOVE:
                return moving != null ? moving.name : "Walking";
            case UnitAnimationType.FIGHT:
                return fighting != null ? fighting.name : "Fighting";
            case UnitAnimationType.IDLE:
            default:
                return idle != null ? idle.name : "Idle";
        }
    }
}