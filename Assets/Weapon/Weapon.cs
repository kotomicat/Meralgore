using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] public LayerMask enemyLayer;
    [Header("Weapon Settings")]
    public float range = 10f;
    public float damage = 15f;
    public float cooldown = 0.5f;

    [Header("Ammo")]
    public int maxAmmo;
    public int currentAmmo;
    public bool reload = false;

    [Header("References")]
    [SerializeField] protected Transform firePoint;

    [Header("Visual")]
    [SerializeField] private GameObject bulletHoleDecalPrefab;

    public Camera mainCamera;

    protected float nextAttackTime;
    public bool attack;

    public virtual bool CanAttack => Time.time >= nextAttackTime;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public virtual void StartAttack()
    {
        
        Debug.Log("StartAttack");
        attack = true;
    }

    public virtual void EndAttack()
    {
        Debug.Log("EndAttack");
        attack = false;
    }

    public virtual void StartAltAttack()
    {
        
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

    public virtual void Attack() { }
    public virtual void Reload() { }

    protected void ResetLastAttack()
    {
        nextAttackTime = Time.time + cooldown;
    }
    
}
