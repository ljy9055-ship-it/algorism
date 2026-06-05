using UnityEngine;
using UnityEngine.InputSystem;

public class raycast2 : MonoBehaviour
{
    public Rigidbody rb;
    public float jumpforce = 5f;
    Vector3 halfextend = new Vector3(0.5f, 0.5f, 0.5f);
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
       rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = transform.rotation;
        bool isHit = Physics.BoxCast(transform.position, halfextend, Vector3.down, rotation);
        Debug.DrawRay(transform.position, Vector3.down, isHit ? Color.yellow : Color.blue);
        
        
        if (isHit && Keyboard.current != null&&Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.AddForce(Vector3.up *jumpforce, ForceMode.Impulse);
        }
    }
}
