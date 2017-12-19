using UnityEngine;

public enum UnitAnimationType
{
    IDLE,
    MOVE,
    ATTACK,
    HIT,
    DEATH,
}

public abstract class UnitAnimation : MonoBehaviour
{
    public abstract void SetAnimation(UnitAnimationType animationType);
}
