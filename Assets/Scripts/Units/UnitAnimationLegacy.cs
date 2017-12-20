using UnityEngine;

public class UnitAnimationLegacy : UnitAnimation
{
    [SerializeField] AnimationClip idle;
    [SerializeField] AnimationClip eating;
    [SerializeField] AnimationClip drinking;
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
                return moving ? moving.name : "Walking";
            case UnitAnimationType.STALK:
                return stalking ? stalking.name : GetAnimationName(UnitAnimationType.MOVE);
            case UnitAnimationType.FIGHT:
                return fighting ? fighting.name : "Fighting";
            case UnitAnimationType.DEATH:
                return death ? death.name : "Death";
            case UnitAnimationType.EAT:
                return eating ? eating.name : GetAnimationName(UnitAnimationType.IDLE);
            case UnitAnimationType.DRINK:
                return drinking ? drinking.name : GetAnimationName(UnitAnimationType.EAT);
            case UnitAnimationType.IDLE:
            default:
                return idle ? idle.name : "Idle";
        }
    }
}