using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder;

public class MachineGun : Weapon
{
    [Header("Machine Gun Settings")]
    [SerializeField] float spread;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootForce;
    Coroutine reloadCoroutine;
    bool isReloading = false;

    private void OnEnable()
    {
        // защита от ошибки в случае неициалированного Animator
        if (animator != null)
        {
            // сброс старых триггеров
            animator.ResetTrigger("Shoot");
            animator.ResetTrigger("Reload");

            // анимация экипировки оружия
            animator.SetTrigger("Equip");
        }
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (CanAttack && !isReloading && attack)
        {
            Attack();
        }
    }

    public override void Attack()
    {
        animator.ResetTrigger("Shoot");
        animator.SetTrigger("Shoot");

        // луч по камере
        Ray cameraRay = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 endPoint; // точка выстрела

        if (Physics.Raycast(cameraRay, out RaycastHit hit, range))
            endPoint = hit.point;
        else 
            endPoint = cameraRay.GetPoint(75);
        
        // направление из дула к центру камеры
        Vector3 directionWithoutSpread = endPoint - firePoint.position;

        // рассчет разброса
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // направление с учетом разброса 
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // спавним пулю и присваиваем в переменную
        GameObject currentBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
       
        currentBullet.transform.forward = directionWithSpread.normalized;
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        // получаем скрипт пули и передаем урон
        if (currentBullet.TryGetComponent(out Bullet bulletScript))
        {
            bulletScript.Init(damage); // damage — переменная урона из скрипта оружия
        }
        
        currentAmmo--;
        
        ResetLastAttack();

        if (currentAmmo <= 0)
        {
            reloadCoroutine = StartCoroutine(ReloadProcess());
        }
    }
    public override void Reload() 
    {
        reloadCoroutine = StartCoroutine(ReloadProcess());
    }

    private IEnumerator ReloadProcess() // анимация перезарядки
    {
        isReloading = true;
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
        isReloading = false;
    }
}
