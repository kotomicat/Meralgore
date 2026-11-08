using UnityEngine;

[CreateAssetMenu(fileName = "NewShotgunStats", menuName = "Weapons/Shotgun")]
public class ShotgunStats : BaseWeaponData, IAmmoStats, IShotgunStats
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float cooldown = 0.2f;
    [SerializeField] private float range = 50f;

    [Header("Для стрелкового")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float spread = 0.1f;

    [Header("Для дробовиков")]
    [SerializeField] private int pelletsCount = 5;

    public override float Damage => damage;
    public override float Cooldown => cooldown;
    public override float Range => range;

    public int MaxAmmo => maxAmmo;
    public float Spread => spread;
    public int PelletsCount => pelletsCount;

}
