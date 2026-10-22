using System.Collections;
using System.Collections.Specialized;
using System.Net;
using UnityEngine;

public class RaycastAttackLogic : WeaponAttackLogic
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private BulletTrace tracePrefab;


    public override void ExecuteAttack(BaseWeapon weapon)
    {
        Camera cam = weapon.MainCamera;

        if (cam == null)
        {
            return;
        }

        Ray aimRay = cam.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0));

        Vector3 targetPoint = aimRay.origin + aimRay.direction * weapon.Range;
        Vector3 hitPoint = targetPoint;

        if (Physics.Raycast(aimRay,out RaycastHit aimHit, weapon.Range, hitLayer))
        {
            targetPoint = aimHit.point;
        }

        Vector3 origin = weapon.FirePoint != null ? weapon.FirePoint.position : aimRay.origin;

        Vector3 direction = (targetPoint - origin).normalized;

        if (Physics.Raycast(origin,direction,out RaycastHit hit, weapon.Range, hitLayer))
        {
            Health health = hit.collider.GetComponentInParent<Health>();
            hitPoint = hit.point;
            if ( health != null)
            {
                health.Damage(weapon.Damage);
            }
        }

        SpawnTrace(origin, hitPoint);
    }   
   
    private void SpawnTrace(params Vector3[] points)
    {
        BulletTrace trace = Instantiate(tracePrefab);
        trace.Show(points);
    }
   
}
