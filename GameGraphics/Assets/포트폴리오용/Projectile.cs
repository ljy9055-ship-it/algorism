using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 5f;

    [SerializeField] private GameObject impactEffectPrefab;

    private bool hasHit = false;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (hasHit)
            return;

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit)
            return;

        hasHit = true;

        ContactPoint contact = collision.contacts[0];

        Instantiate(
            impactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );

        Destroy(gameObject);
    }
}