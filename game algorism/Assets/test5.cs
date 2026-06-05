using UnityEngine;

public class test5 : MonoBehaviour
{
    Vector3 vec = new Vector3(10, 0, 10).normalized;

    public float pushForce = 10f;
    public ForceMode forc = ForceMode.Impulse;

    void Start()
    {
        Vector3 forcepower = new Vector3(1, 1, 1).normalized;

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.AddForce(Vector3.up * 100f, ForceMode.Impulse);
    }

    void Update()
    {

    }

    void Knockback()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.AddForce(vec * pushForce, forc);
    }
}