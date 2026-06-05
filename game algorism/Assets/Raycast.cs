using UnityEngine;
using UnityEngine.InputSystem;

public class Raycast : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody rb;
    public float jumpForce = 5f;
    public float checkDistance = 0.6f;

    void Start() { rb = GetComponent<Rigidbody>(); }

    void Update()
    {
        // Physics.Raycast는 시작점에서 방향으로 보이지 않는 선을 쏴 충돌 여부를 검사하는 메서드입니다.
        // Vector3.down은 월드 기준 아래 방향인 (0, -1, 0) 벡터입니다.
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, checkDistance);

        // Debug.DrawRay는 씬 뷰에 디버그용 광선을 그려 Raycast 방향을 눈으로 확인하게 해 줍니다.
        Debug.DrawRay(transform.position, Vector3.down * checkDistance, isGrounded ? Color.green : Color.red);

        // Input System: 스페이스바를 누른 순간 확인
        // wasPressedThisFrame은 해당 키가 이번 프레임에 막 눌렸을 때만 true가 되는 프로퍼티입니다.
        if (isGrounded && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Vector3.up은 월드 기준 위 방향인 (0, 1, 0) 벡터입니다.
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
