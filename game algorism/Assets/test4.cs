using UnityEngine;
using UnityEngine.InputSystem;

public class test4 : MonoBehaviour
{
    public float rotationSpeed = 4f;
    public float targetMoveSpeed = 3f;
    public float targetDistance = 4f;
    public float targetRange = 3f;

    Vector3 targetOffset = new Vector3(0f, 0f, 4f);

    void Update()
    {
        // Keyboard.current는 Input System에서 현재 키보드 장치를 가져오는 프로퍼티입니다.
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        // Vector2.zero는 (0, 0)을 뜻하는 2D 벡터 기본값입니다.
        Vector2 input = Vector2.zero;

        // aKey, leftArrowKey는 각각 A 키와 왼쪽 방향키를 나타내는 입력 버튼입니다.
        // isPressed는 해당 키가 지금 눌려 있는 동안 true가 되는 프로퍼티입니다.
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            input.x -= 1f;
        }

        // dKey, rightArrowKey는 각각 D 키와 오른쪽 방향키를 나타냅니다.
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            input.x += 1f;
        }

        // sKey, downArrowKey는 각각 S 키와 아래 방향키를 나타냅니다.
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            input.y -= 1f;
        }

        // wKey, upArrowKey는 각각 W 키와 위 방향키를 나타냅니다.
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            input.y += 1f;
        }

        // spaceKey는 스페이스바 입력 버튼입니다.
        // wasPressedThisFrame은 이번 프레임에 막 눌렸을 때만 true가 되는 프로퍼티입니다.
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            targetOffset = new Vector3(0f, 0f, targetDistance);
        }

        // Time.deltaTime은 이전 프레임에서 현재 프레임까지 걸린 시간입니다. 프레임 속도가 달라도 이동 속도를 일정하게 맞출 때 사용합니다.
        targetOffset += new Vector3(input.x, input.y, 0f) * targetMoveSpeed * Time.deltaTime;
        // Mathf.Clamp는 값을 최소값과 최대값 사이로 제한하는 메서드입니다.
        targetOffset.x = Mathf.Clamp(targetOffset.x, -targetRange, targetRange);
        targetOffset.y = Mathf.Clamp(targetOffset.y, -targetRange, targetRange);
        targetOffset.z = targetDistance;

        // normalized는 벡터의 방향은 유지하고 길이만 1로 만든 값을 돌려주는 프로퍼티입니다.
        Vector3 targetDirection = targetOffset.normalized;

        // Quaternion.LookRotation은 지정한 방향을 바라보는 회전값을 만들어 주는 메서드입니다.
        // Vector3.up은 월드 기준 위쪽 방향인 (0, 1, 0)을 뜻합니다.
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        // transform.rotation은 현재 오브젝트의 회전값이고, Quaternion.Slerp는 두 회전 사이를 부드럽게 섞습니다.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        Vector3 targetposition = origin + targetOffset;
        Vector3 targetDirection = targetOffset.normalized;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetposition, 0.15f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin, origin + transform.forward * targetDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + targetDirection*targetDistance);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    
}
