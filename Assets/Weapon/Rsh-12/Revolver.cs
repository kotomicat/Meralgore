using System.Collections;
using UnityEngine;

public class Revolver : Weapon
{
    LineRenderer lineRenderer;

    [Header("Animation")]
    public float reloadStep = 1f;
    public float shootStep = 1f;
    [SerializeField] float traceThinningStep = 0.001f;

    [SerializeField] private float reloadRotationAngle = 15f;
    
    [SerializeField] private float shootRotationAngle = 20f;
    [SerializeField] private float shootPositionOffset = 10f;

    Quaternion shootRotation;
    Vector3 shootPosition;

    private Quaternion reloadRotation;

    [Header("Revolver")]
    
    [SerializeField] float range = 10f;
    Camera mainCamera;
    [SerializeField] CameraController cameraController;

    [SerializeField] int maxAmmo;
    private int currentAmmo;
    private bool reload = false;

    private Coroutine showTraceCoroutine;
    private Coroutine reloadCoroutine;
    private Coroutine shootCoroutine;


    [Header("Alternative Attack")]
    [SerializeField] GameObject coin;
    [SerializeField] private LayerMask coinLayer;
    [SerializeField] float coinThrowUpForce = 5f;
    [SerializeField] float coinThrowForwardForce = 7f;
    [SerializeField] float coinCastRange = 30f;
    [SerializeField] float coinHitRadius = 0.5f;

    [SerializeField] CharacterController plCharacterController;

    [Header("Nearest Enemy Params")]
    [SerializeField] float searchRadius = 10f;
    [SerializeField] int maxColliders = 10;

    private Collider[] enemiesBuffer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        enemiesBuffer = new Collider[maxColliders];

        currentAmmo = maxAmmo;

        mainCamera = Camera.main;

        reloadRotation = Quaternion.Euler(0, 270, reloadRotationAngle);
        shootRotation = Quaternion.Euler(0, 270, shootRotationAngle);
        shootPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -shootPositionOffset);
    }

    public override void StartAttack()
    {
        if (CanAttack && !reload)
        {
            if (currentAmmo > 0)
            {
                cameraController.Pulse();
                Attack();
            }
            else
                Reload();
        }
    }

    public override void EndAttack() { }

    public override void Attack()
    {
        shootCoroutine = StartCoroutine(ShootAnimation());

        // ТОЧКА В ЦЕНТРЕ КАМЕРЫ
        Ray cameraRay = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 endPoint;

        // МЕХАНИКА БАЗОВОГО ВЫСТРЕЛА
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, range)) // при попадании рейкастом
        {
            endPoint = hit.point; // точка попадания

            //нанесение урона по объекту если у него есть здоровье
            if (hit.collider.TryGetComponent(out Health health))
            {
                health.Damage(damage);
            }
            else if (hit.transform.parent != null && hit.transform.parent.TryGetComponent(out health))
            {
                // Замена GetComponentInParent на явный шаг вверх (работает быстрее)
                health.Damage(damage);
            }

        }
        else
        {
            endPoint = cameraRay.GetPoint(range);
            StartCoroutine(ShowTrace(firePoint.position, endPoint));
        }

        // МЕХАНИКА ПОПАДАНИЯ В МОНЕТУ
        if (Physics.SphereCast(mainCamera.transform.position, coinHitRadius, mainCamera.transform.forward, out RaycastHit sphereHit, coinCastRange, coinLayer)) // при попадании сферой
        {
            Transform target = (NearestEnemy(sphereHit.transform));

            if (target != null)
            { 
                StartCoroutine(ShowTrace(firePoint.position, sphereHit.transform.position, target.position));

                if (target.TryGetComponent(out Health health))
                    health.Damage(damage * 1.5f);
            }

            Destroy(sphereHit.transform.gameObject);

        }
        currentAmmo -= 1;
        ResetLastAttack();
    }

    public override void StartAltAttack()
    {
        //монета появляется на сцене, записывается ее rigidbody
        GameObject spawnedCoin = Instantiate(coin, transform.position, transform.rotation);
        Rigidbody coinRb = spawnedCoin.GetComponent<Rigidbody>();

        //получение скорости игрока для добавления ее к силе броска
        Vector3 playerVelocity = plCharacterController.velocity;

        //рассчет силы броска монеты с учетом направления камеры
        Vector3 throwForce = Vector3.up * coinThrowUpForce + mainCamera.transform.forward * coinThrowForwardForce;



        //применение рассчитаной силы к монетке
        coinRb.AddForce(throwForce + (playerVelocity / 2), ForceMode.Impulse);

        Destroy(spawnedCoin, 5f);
    }

    Transform NearestEnemy(Transform coin)
    {
        int count = Physics.OverlapSphereNonAlloc(coin.position, searchRadius, enemiesBuffer, enemyLayer); // вызываем функцию чтобы в кеш записались враги в радиусе

        Transform nearestEnemy = null; // переменная для хранения ближайшего врага
        float nearestDistance = Mathf.Infinity; // переменная для хранения ближайшей дистанции
        // она сразу с гигантским числом чтобы любой враг был ближе

        for (int i = 0; i < count; i++) // перебираем всех врагов в радиусе, пока счетчик меньше количества найденных врагов
        {
            //вычисление "втупую" будет быстрее чем встроенная функция distance
            Vector3 direction = enemiesBuffer[i].transform.position - coin.position;
            float sqrtDistance = direction.sqrMagnitude; // вычисление квадрата дистанции до врага

            if (sqrtDistance < nearestDistance) // если квадрат дистанции меньше ближайшей дистанции
            {
                nearestDistance = sqrtDistance; // то ближайшая дистанция становится равной квадрату дистанции
                nearestEnemy = enemiesBuffer[i].transform; // а ближайший враг становится текущим врагом
            }
        }

        return nearestEnemy; // возвращаем ближайшего врага
    }

    public override void Reload()
    {
        if (!reload)
            reloadCoroutine = StartCoroutine(ReloadAnimation());
    }

    private IEnumerator ReloadAnimation() // анимация перезарядки
    {
        Quaternion startRotation = transform.localRotation;

        reload = true; // помечается для блокировки выстрела

        while (Quaternion.Angle(transform.localRotation, reloadRotation) > 0.1f) // пока истинный поворот не приблизился к конечной точке
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, reloadRotation, Time.deltaTime * reloadStep);
            yield return null;
        }

        while (Quaternion.Angle(transform.localRotation, startRotation) > 0.1f) // пока истинный поворот не приблизился к конечной точке
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, startRotation, Time.deltaTime * reloadStep);
            yield return null;
        }

        currentAmmo = maxAmmo;
        reload = false;

        yield break;
    }

    private IEnumerator ShootAnimation() // анимация выстрела
    {
        Quaternion startRotation = transform.localRotation;
        Vector3 startPosition = transform.localPosition;

        while (Quaternion.Angle(transform.localRotation, shootRotation) > 0.1f) // пока истинный поворот не приблизился к конечной точке
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, shootRotation, Time.deltaTime * shootStep);
            transform.localPosition = Vector3.Lerp(transform.localPosition, shootPosition, Time.deltaTime * shootStep);
            yield return null;
        }

        while (Quaternion.Angle(transform.localRotation, startRotation) > 0.1f) // пока истинный поворот не приблизился к конечной точке
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, startRotation, Time.deltaTime * (shootStep / 1.5f));
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * shootStep);
            yield return null;
        }

        yield break;
    }
    private IEnumerator ShowTrace(params Vector3[] points) // анимация трассировки выстрела
    {
        lineRenderer.positionCount = points.Length;

        float width = 0.2f;


        for (int i = 0; i < points.Length; i++)
            lineRenderer.SetPosition(i, points[i]);

        lineRenderer.enabled = true;

        while (width > 0)
        {
            width -= traceThinningStep;
            lineRenderer.startWidth = width;

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        lineRenderer.enabled = false;
    }
}