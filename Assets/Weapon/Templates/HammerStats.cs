using UnityEngine;

[CreateAssetMenu(fileName = "HammerStats", menuName = "Weapons/Hammer")]
public class HammerStats : BaseWeaponData, IChargeableStats
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float cooldown = 0.2f;
    [SerializeField] private float range = 50f;

    [Header("Для молота")]
    [SerializeField] private int maxChargeTime = 30;
    [SerializeField] private float damageScale = 0.1f;
    [SerializeField] private int minChargeThreshold = 5;

    public override float Damage => damage;
    public override float Cooldown => cooldown;
    public override float Range => range;

    public float MaxChargeTime => maxChargeTime;
    public float DamageScale => damageScale;
    public float MinChargeThreshold => minChargeThreshold;

}
