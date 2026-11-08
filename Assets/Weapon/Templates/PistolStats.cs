using UnityEngine;

[CreateAssetMenu(fileName = "NewPistolStats", menuName = "Weapons/Pistol")]
public class PistolStats : BaseWeaponData, IAmmoStats
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float cooldown = 0.2f;
    [SerializeField] private float range = 50f;

    [Header("Для стрелкового")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float spread = 0.1f;

    // Реализация базового BaseWeaponData (IWeaponStats)
    public override float Damage => damage;
    public override float Cooldown => cooldown;
    public override float Range => range;

    // Реализация специфичного IRangedStats
    public int MaxAmmo => maxAmmo;
    public float Spread => spread;
}