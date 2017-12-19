using UnityEngine;

public enum UnitAnimationType
{
    IDLE,
    EAT,
    MOVE,
    STALK,
    FIGHT,
    HIT,
    DEATH,
}

public abstract class UnitAnimation : MonoBehaviour
{
    public abstract void SetAnimation(UnitAnimationType animationType);
}
