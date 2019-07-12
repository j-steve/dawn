using UnityEngine;

public enum UnitAnimationType
{
    IDLE,
    EAT,
    DRINK,
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
