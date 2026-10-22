using UnityEngine;

[CreateAssetMenu(
    fileName = "WeaponConfig",
    menuName = "Weapons/Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    [Header("Weapon settings")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float range = 10f;
    [SerializeField] private float cooldown = 0.5f;

    [Header("Ammo settings")]
    [SerializeField] private int maxAmmo = 12;
    [SerializeField] private float reloadDuration = 1.5f;

    [Header("Coin settings")]
    [SerializeField] private float coinHitRadius = 0.5f;
    [SerializeField] private float coinCastRange = 30f;

    [Header("Shoot trace settings")]
    [SerializeField] private float shootTraceWidth = 0.2f;
    [SerializeField] private float shootTraceThinningStep = 0.001f;

    public float Damage => damage;
    public float Range => range;
    public float Cooldown => cooldown;

    public int MaxAmmo => maxAmmo;
    public float ReloadDuration => reloadDuration;

    public float CoinHitRadius => coinHitRadius;
    public float CoinCastRange => coinCastRange;

    public float ShootTraceWidth => shootTraceWidth;
    public float ShootTraceThinningStep => shootTraceThinningStep;
}