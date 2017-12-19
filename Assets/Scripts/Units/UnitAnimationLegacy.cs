using UnityEngine;

public class UnitAnimationLegacy : UnitAnimation
{
    [SerializeField] AnimationClip idle;
    [SerializeField] AnimationClip eating;
    [SerializeField] AnimationClip moving;
    [SerializeField] AnimationClip stalking;
    [SerializeField] AnimationClip fighting;
    [SerializeField] AnimationClip death;

    public override void SetAnimation(UnitAnimationType animationType)
    {
        GetComponentInChildren<Animation>().CrossFade(GetAnimationName(animationType));
    }

    string GetAnimationName(UnitAnimationType animationType)
    {
        switch (animationType) {
            case UnitAnimationType.MOVE:
                return moving != null ? moving.name : "Walking";
            case UnitAnimationType.STALK:
                return stalking != null ? stalking.name : GetAnimationName(UnitAnimationType.MOVE);
            case UnitAnimationType.FIGHT:
                return fighting != null ? fighting.name : "Fighting";
            case UnitAnimationType.DEATH:
                return death != null ? death.name : "Death";
            case UnitAnimationType.EAT:
                return eating != null ? eating.name : GetAnimationName(UnitAnimationType.IDLE);
            case UnitAnimationType.IDLE:
            default:
                return idle != null ? idle.name : "Idle";
        }
    }
}