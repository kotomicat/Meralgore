using UnityEngine;

public class Shotgun : Weapon
{
    Transform firePointStarPos;
    [SerializeField] float minSpreadRotation;
    [SerializeField] float maxSpreadRotation;

    void Start()
    {
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
        
        // 1. ПОИСК ТОЧКИ ПРИЦЕЛА
        Ray cameraForward = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //луч из центра камеры
        Vector3 targetPoint;

        if (Physics.Raycast(cameraForward, out RaycastHit cameraHit, range))
            targetPoint = cameraHit.point; //при попадания взгляда игрока на объект в пределах дальности, определяем его
        else
            targetPoint = cameraForward.GetPoint(range); // при взгляде игрока в пустоту, берем точку на максимальной дистанции выстрела

        // 2. ЦИКЛ РАЗБРОСА ДРОБИНОК
        for (int bulletCounter = 10; bulletCounter > 0; bulletCounter--)
        {
            // направляем дуло в точку прицела
            firePoint.LookAt(targetPoint);

            // определяем разброс дроби
            float spreadX = Random.Range(minSpreadRotation, maxSpreadRotation);
            float spreadY = Random.Range(minSpreadRotation, maxSpreadRotation);

            // поворот дула
            // Space.Self значит поворот по локали (если по мировым то World)
            firePoint.Rotate(spreadX, spreadY, 0f, Space.Self);

            Vector3 fwd = firePoint.forward;

            // нанесение урона, отрисовка декалей (пуль)
            if (Physics.Raycast(firePoint.position, fwd, out RaycastHit hit, range))
            {
                Debug.Log($"попадание в {hit.collider.gameObject.name}");

                SpawnBulletHole(hit.normal, hit.point, hit.transform);

                // для распределения урона определяем насколько  
                mobDistance = Vector3.Distance(transform.position, hit.collider.transform.position);

                float currentDamage = damage / mobDistance / 10;

                // урон
                if (hit.collider.TryGetComponent(out Health health))
                    health.Damage(currentDamage);
                else if (hit.transform.parent != null && hit.transform.parent.TryGetComponent(out health)) 
                    health.Damage(currentDamage);
            }
        }
    }

}
