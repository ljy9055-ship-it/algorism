using Unity.VisualScripting;
using UnityEngine;

public class test5 : MonoBehaviour
{
    Vector3 vec = new Vector3(10, 0, 10);
    Rigidbody rb;
    public float pushForce = 10f;
    ForceMode forc = new ForceMode();
    
    void Nockback()
    {

        
    }
    void Start()
    {
        
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rb.AddComponent<Rigidbody>();
        rb.AddForce(10f, 0, 10f);
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(vec, forc);
    }
}
