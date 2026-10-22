
using System.Collections;
using UnityEngine;

public enum WeaponFireMode
{
    SemiAutomatic,
    Automatic
}

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private WeaponConfig config;
    [SerializeField] private WeaponFireMode fireMode;

    [Header("References")]
    [SerializeField] private WeaponAttackLogic attackLogic;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Camera mainCamera;

    public float Damage => config != null ? config.Damage : 0f;
    public float Range => config != null ? config.Range : 0f;
    public float Cooldown => config != null ? config.Cooldown : 0f;

    public int CurrentAmmo { get; private set; }
    public int MaxAmmo => config != null ? config.MaxAmmo : 0;

    public bool IsAttacking { get; private set; }
    public bool IsReloading { get; private set; }

    public Transform FirePoint => firePoint;
    public Camera MainCamera => mainCamera;

    private float nextAttackTime;
    private Coroutine reloadCoroutine;

    public bool CanAttack =>
        config != null &&
        attackLogic != null &&
        !IsReloading &&
        CurrentAmmo > 0 &&
        Time.time >= nextAttackTime;

    protected virtual void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (attackLogic == null)
            attackLogic = GetComponent<WeaponAttackLogic>();

        if (config == null)
        {
            Debug.LogError(
                $"{name}: WeaponConfig is not assigned.", this);
            enabled = false;
            return;
        }

        if (attackLogic == null)
        {
            Debug.LogError(
                $"{name}: WeaponAttackLogic is missing.", this);
            enabled = false;
            return;
        }

        CurrentAmmo = config.MaxAmmo;
    }

    private void Update()
    {
        if (IsAttacking &&
            fireMode == WeaponFireMode.Automatic)
        {
            TryAttack();
        }
    }

    public virtual void StartAttack()
    {
        if (IsReloading)
            return;

        IsAttacking = true;

        OnAttackStarted();

        // Первый выстрел происходит сразу при нажатии.
        TryAttack();
    }

    public virtual void EndAttack()
    {
        if (!IsAttacking)
            return;

        IsAttacking = false;

        OnAttackEnded();
    }

    public virtual void StartAltAttack()
    {
        // Реализуется конкретным оружием при необходимости.
    }

    private void TryAttack()
    {
        if (!CanAttack)
        {
            if (CurrentAmmo <= 0 && !IsReloading)
                OnAmmoEmpty();

            return;
        }

        // Фиксируем выстрел до выполнения логики атаки.
        CurrentAmmo--;
        nextAttackTime = Time.time + config.Cooldown;

        attackLogic.ExecuteAttack(this);

        OnAttackExecuted();
    }

    public virtual void Reload()
    {
        if (config == null ||
            IsReloading ||
            CurrentAmmo >= config.MaxAmmo)
        {
            return;
        }

        IsAttacking = false;
        IsReloading = true;

        OnReloadStarted();

        reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(config.ReloadDuration);

        CurrentAmmo = config.MaxAmmo;
        IsReloading = false;
        reloadCoroutine = null;

        OnReloadFinished();
    }

    protected virtual void OnAttackStarted() { }

    protected virtual void OnAttackEnded() { }

    protected virtual void OnAttackExecuted() { }

    protected virtual void OnAmmoEmpty() { }

    protected virtual void OnReloadStarted() { }

    protected virtual void OnReloadFinished() { }
}
