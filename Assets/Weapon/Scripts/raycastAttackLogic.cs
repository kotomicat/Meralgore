using System.Collections;
using System.Collections.Specialized;
using System.Net;
using UnityEngine;

public class raycastAttackLogic : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private WeaponConfig config;
    [SerializeField] private BulletTrace tracePrefab;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
    }
    void TryHit(Vector3 originPosition, Vector3 direction,float range)
    {
        if (weapon.reload)
        {
            return;
        }

        if (Physics.Raycast(originPosition, direction, out RaycastHit hit, range))
        {
            Vector3 endPoint = hit.point;
            Transform hitTransform = hit.collider.transform;
            if (hitTransform.TryGetComponent(out Health health) || 
                (hitTransform.parent && hitTransform.parent.TryGetComponent(out health))
                )
            {
                health.Damage(weapon.damage);
            }
            weapon.SpawnBulletHole(hit.normal, endPoint, hitTransform); 
        }
        // Тут с монетками надо подумать, так что пока не доделываю
        //else if (Physics.SphereCast(originPosition, config.CoinHitRadius, direction, out RaycastHit sphereHit, config.CoinCastRange))
        //{
        //    Transform target = (Near)
        //}
        SpawnTrace(originPosition, direction);
    }
    private void SpawnTrace(params Vector3[] points)
    {
        BulletTrace trace = Instantiate(tracePrefab);
        trace.Show(points);
    }
   
}
