using UnityEngine;
using System.Collections;

public class Shotgun : Weapon
{
    Transform firePointStarPos;
    [SerializeField] float minSpreadRotation;
    [SerializeField] float maxSpreadRotation;
    Coroutine reloadCoroutine;

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
        animator.ResetTrigger("Shoot");
        animator.SetTrigger("Shoot");

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

    public override void Reload()
    {
        if (!reload)
            reloadCoroutine = StartCoroutine(ReloadProcess());
    }

    private IEnumerator ReloadProcess() // анимация перезарядки
    {
        reload = true;
        animator.SetTrigger("Reload");

        // ждем окончания текущего кадра, чтобы Animator переключился на состояние перезарядки
        yield return null;

        // счетчик превышения времени ожидания анимации
        float timeout = 0f;

        // данный цикл работает пока не включится анимация, и для защиты от вечного зависания накручивает timeout
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && timeout < 1.0f)
        {
            timeout += Time.deltaTime;
            yield return null;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); //берем информацию о текущем состоянии на слое 0
        float animationLength = stateInfo.length; // берем длину анимации в секундах
        yield return new WaitForSeconds(animationLength - 0.001f); // делаем паузу на время анимации, чтобы дать ей завершиться

        currentAmmo = maxAmmo;
        reload = false;
    }
}
