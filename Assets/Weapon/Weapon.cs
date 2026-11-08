using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected Animator animator;
    public Camera mainCamera;

    [Header("Data System")]
    [SerializeField] private BaseWeaponData baseStatsAsset;
    public IWeaponStats CurrentStats { get; private set; }

    [Header("Ammo")]
    public int currentAmmo;
    public bool reload = false;

    [Header("References")]
    [SerializeField] protected Transform firePoint;
    [SerializeField] public LayerMask enemyLayer;

    [Header("Visual")]
    [SerializeField] private GameObject bulletHoleDecalPrefab;

    protected Coroutine reloadCoroutine;

    protected float nextAttackTime;
    public bool attack;

    public virtual bool CanAttack => Time.time >= nextAttackTime;

    void Awake()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        ResetStats();
    }
    public void ResetStats() 
    { 
        CurrentStats = baseStatsAsset;
        ResetAmmo();
    }

    private void OnEnable() //всегда при активации оружия, сбрасываем старые триггеры и запускаем анимацию экипировки
    {
        ResetAllTriggers();
        animator.SetTrigger("Equip");
    }

    public void ResetAllTriggers() //сбрасывает любые триггеры в Animator
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger)
                animator.ResetTrigger(parameter.name);
        }
    }
    protected void ResetLastAttack()
    {
        nextAttackTime = Time.time + CurrentStats.Cooldown;
    }

    protected void ResetAmmo()
    {
        // если оружие поддерживает интерфейс стрелкового оружия, то 
        if (CurrentStats is IAmmoStats ammo)
        {
            // берем максимальный боезапас в качество текущего
            currentAmmo = ammo.MaxAmmo;
        }
    }

    public void SpawnBulletHole(Vector3 normal, Vector3 point, Transform parent)
    {
        Vector3 decalUp = Mathf.Abs(normal.y) > 0.9f ? Vector3.forward : Vector3.up;

        // hit.normal отрицателен чтобы проектор декалей смотрел внутрь поверхности
        Quaternion decalRotation = Quaternion.LookRotation(-normal, decalUp);

        GameObject decal = Instantiate(bulletHoleDecalPrefab, point, decalRotation);

        // чтобы дырки двигались с перемещающимся объектом, делаем их дочерними
        // true вторым аргументом значит что мировые координаты не меняются
        decal.transform.SetParent(parent, true);

        Destroy(decal, 10f); //самоуничтожение
    }

    public virtual void StartAttack() { attack = true; }
    public virtual void EndAttack() { attack = false; }
    public virtual void StartAltAttack() { }
    public virtual void Attack() { }
    public virtual void Reload() { }    
}
