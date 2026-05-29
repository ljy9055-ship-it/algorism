using UnityEngine;
using UnityEngine.InputSystem;

public class rigidbody : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
        private Rigidbody rb;
    public float pushForce = 10f;

    void Start()
    {
        // GetComponent<T>()는 같은 게임 오브젝트에 붙은 T 타입 컴포넌트를 가져오는 메서드입니다.
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate는 물리 계산 주기에 맞춰 자동 호출되므로 Rigidbody 이동에 적합합니다.
    void FixedUpdate()
    {
        float h = 0;
        float v = 0;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h = -1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h = 1;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) v = 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) v = -1;
        }

        Vector3 forceDir = new Vector3(h, 0, v).normalized;

        // Rigidbody.AddForce는 Rigidbody에 힘을 가해 물리 엔진이 속도를 바꾸게 하는 메서드입니다.
        rb.AddForce(forceDir * pushForce, ForceMode.Force);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
