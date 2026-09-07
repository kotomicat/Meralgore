using Unity.VisualScripting;
using UnityEngine;

public class HammerHit : MonoBehaviour
{
    Hammer hammer;

    void Start()
    {
        hammer = transform.parent.gameObject.GetComponent<Hammer>();
    }

    void OnTriggerEnter(Collider other)
    {
        hammer.Hit(other);
    }
}