using System.Net;
using UnityEngine;

public class Kick : MonoBehaviour
{
    [Header("Kick Settings")]
    [SerializeField] private float range = 2f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float kickForce = 5f;
    [SerializeField] private float downOffset = 0.6f;

    public Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
    }

    public void PerformKick()
    {
        Ray cameraRay = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 kickOrigin = mainCamera.transform.position - (Vector3.up * downOffset);

        if (Physics.Raycast(kickOrigin, mainCamera.transform.forward, out RaycastHit hit, range)) // при попадании рейкастом
        { 
            //нанесение урона по объекту если у него есть здоровье
            if (hit.collider.TryGetComponent(out Health health) || (hit.transform.parent != null && hit.transform.parent.TryGetComponent(out health)))
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddForce(cameraRay.direction * kickForce, ForceMode.Impulse);
                }

                health.Damage(damage);
            }
        }

        Debug.DrawRay(kickOrigin, cameraRay.direction * range, Color.red, 1.0f);
    }

}
