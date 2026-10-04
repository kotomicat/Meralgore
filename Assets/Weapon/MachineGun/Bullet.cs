using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;

    void Start()
    {
        
    }
    public void Init(float weaponDamage)
    {
        damage = weaponDamage;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Health health))
        {
            health.Damage(damage);
        }
        else if (collision.transform.parent != null && collision.transform.parent.TryGetComponent(out health))
        {
            health.Damage(damage);
        }

        Destroy(gameObject, 1);
    }
}
