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

    void Start()
    {
        
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

        // ПЕРЕДАЧА УРОНА: Получаем скрипт пули и передаем урон
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

    private IEnumerator ReloadProcess() // анимация перезарядки
    {
        isReloading = true;
        yield return new WaitForSeconds(2f);
        currentAmmo = maxAmmo;
        isReloading = false;
        yield break;
    }
}
