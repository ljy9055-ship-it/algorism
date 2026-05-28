using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class NewBehaviourScript : MonoBehaviour
{

    public float moveSpeed = 5f;
    
    void Start()
    {
        Vector3 Ip=transform.localPosition;
        Quaternion Ir = transform.localRotation;
        Vector3 Is = transform.localScale;

        Vector3 wp = transform.position;
        Quaternion wr = transform .rotation;
        Vector3 ws = transform .lossyScale;

        Transform p = transform.parent;
        var childCount = transform.childCount;

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = Vector2.zero;
        if (Keyboard.current is not null)
        {
            float h = 0;
            float v = 0;
            if (Keyboard.current.aKey.isPressed) h = -1;
            if (Keyboard.current.dKey.isPressed) h = 1;
            if (Keyboard.current.wKey.isPressed) v = 1;
            if(Keyboard.current.sKey.isPressed) v = -1;
            inputVector = new Vector2(h, v);
        }
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y).normalized;
        if (moveDir.magnitude > 0)
        {
            // transform.Translate는 현재 오브젝트를 지정한 방향과 거리만큼 이동시키는 메서드입니다.
            // Time.deltaTime은 직전 프레임부터 현재 프레임까지 걸린 시간입니다.
            // Space.World는 이동 방향을 월드 좌표 기준으로 해석하겠다는 옵션입니다.
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        }
    }
    
}
