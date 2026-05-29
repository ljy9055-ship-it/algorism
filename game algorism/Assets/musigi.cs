using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
public class musigi : MonoBehaviour
{
    private Rigidbody rb;
    public float pushForce = 10f;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 forceDir = new Vector3(0,0,0);
        knockback(forceDir);
        
    }
    private void FixedUpdate()
    {
        float h = 0;
        float v = 0;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) h = -1;
            if (Keyboard.current.dKey.isPressed) h = 1;
            if (Keyboard.current.wKey.isPressed) v = 1;
            if (Keyboard.current.sKey.isPressed) v = -1;
        }
        Vector3 forceDir = new Vector3(h, 0, v).normalized;

        rb.AddForce(forceDir*pushForce,ForceMode.Force);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void knockback(Vector3 forceDir)
    {
        
        ForceMode mode = ForceMode.Impulse;
        
        rb.AddForce(forceDir.normalized,mode);
    }
}
