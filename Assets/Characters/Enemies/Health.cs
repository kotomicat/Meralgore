using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip deathSound;

    [SerializeField] float health;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void Damage(float damage)
    { 
        health -= damage;

        if (health <= 0)
            Death();
    }

    void Death()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(deathSound);
        
        Destroy(gameObject, deathSound.length);
    }
}
