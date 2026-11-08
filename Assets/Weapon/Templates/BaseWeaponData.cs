using UnityEngine;

public abstract class BaseWeaponData : ScriptableObject, IWeaponStats
{
    public abstract float Damage { get; }
    public abstract float Cooldown { get; }
    public abstract float Range { get; }
}
