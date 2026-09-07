using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] public LayerMask enemyLayer;
    [Header("Weapon Settings")]
    public float damage = 15f;
    public float cooldown = 0.5f;

    [Header("References")]
    [SerializeField] protected Transform firePoint;

    protected float nextAttackTime;
    public bool attack;

    public virtual bool CanAttack => Time.time >= nextAttackTime;

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

    public virtual void Attack() { }
    public virtual void Reload() { }

    protected void ResetLastAttack()
    {
        nextAttackTime = Time.time + cooldown;
    }
    
}
