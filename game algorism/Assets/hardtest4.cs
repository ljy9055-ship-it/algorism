using UnityEngine;
using UnityEngine.InputSystem;

public class hardtest4 : MonoBehaviour
{
    public Transform crash;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public float gravity = -9.81f;

    [SerializeField]
    // 초기 속도를 에디터에서 지정할 수 있도록 직렬화합니다. (예: 점프는 양수값 입력)
    // 방향(+/-) 정보를 포함하므로 '속도(Velocity)' 변수명을 사용합니다.
    private float currentVelocityY = 0f;

    void Update()
    {
        // 1. 중력 때문에 아래쪽 속도가 조금씩 커집니다. (v = v0 + at)
        // 가속도의 방향에 따라 속도가 어느 방향으로 바뀔지 결정됩니다.
        currentVelocityY += gravity * Time.deltaTime;

        // 2. 지금 속도만큼 위치를 조금 이동합니다.
        // Time.deltaTime을 곱하면 프레임 속도와 상관없이 비슷한 움직임이 됩니다.
        Vector3 pos = transform.position;
        pos.y += currentVelocityY * Time.deltaTime;

        // 3. 지면 충돌 시 정지 (임시 로직)
        if (pos.y < 0)
        {
            pos.y = 0;

            crash.transform.position =transform.TransformPoint(0, 0, 0);
            currentVelocityY = 0; // 속도를 0으로 만들어 정지시킴

        }

        transform.position = pos;
    }
}
