using System.Collections;
using System.Collections.Specialized;
using System.Net;
using UnityEngine;

public class rastraycastAttackLogic : MonoBehaviour
{
    private Weapon weapon;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
    }
    void TryHit(Vector3 origin, Vector3 direction,float range)
    {

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range))
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

        StartCoroutine(ShowTrace());
    }

    IEnumerator ShowTrace (params Vector3[] points)
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        return null;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
