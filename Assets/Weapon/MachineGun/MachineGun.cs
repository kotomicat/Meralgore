using UnityEngine;

public class MachineGun : Weapon
{
    [Header("Machine Gun Settings")]
    [SerializeField] float spread;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootForce;

    void Start()
    {
        
    }

    void Update()
    {
        if (CanAttack && attack)
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

        ResetLastAttack();
    }


}
