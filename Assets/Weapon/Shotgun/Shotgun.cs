using UnityEngine;

public class Shotgun : Weapon
{
    Transform firePointStarPos;
    [SerializeField] float minSpreadRotation;
    [SerializeField] float maxSpreadRotation;
    Camera mainCamera;

    [SerializeField] float range = 10f;
    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        firePointStarPos = firePoint;
    }

    public override void StartAttack()
    {
        Attack();
    }

    public override void EndAttack() { }

    public override void Attack() 
    {
        float mobDistance;

        for (int bulletCounter = 10; bulletCounter > 0; bulletCounter--)
        {
            firePoint.localRotation = Quaternion.identity;
            firePoint.localRotation = Quaternion.Euler(
                firePointStarPos.localRotation.x + Random.Range(minSpreadRotation, maxSpreadRotation),
                firePointStarPos.localRotation.y + Random.Range(minSpreadRotation, maxSpreadRotation),
                firePointStarPos.localRotation.z + Random.Range(minSpreadRotation, maxSpreadRotation));

            Vector3 fwd = firePoint.TransformDirection(Vector3.forward);

            if (Physics.Raycast(firePoint.position, fwd, out RaycastHit hit, range))
            {
                Debug.Log($"попадание в {hit.collider.gameObject.name}");

                mobDistance = Vector3.Distance(transform.position, hit.collider.transform.position);

                float currentDamage = damage / mobDistance / 10;

                if (hit.collider.TryGetComponent(out Health health))
                    health.Damage(currentDamage);
                else if (hit.transform.parent != null && hit.transform.parent.TryGetComponent(out health)) 
                    health.Damage(currentDamage);
            }
        }
    }

}
