using UnityEngine;

public enum UnitAnimationType
{
    IDLE,
    MOVE,
    FIGHT,
    HIT,
    DEATH,
}

public abstract class UnitAnimation : MonoBehaviour
{
    public abstract void SetAnimation(UnitAnimationType animationType);
}
