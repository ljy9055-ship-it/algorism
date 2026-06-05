using UnityEngine;
using UnityEngine.InputSystem;

public class cannon2 : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float linearDamping = 0f;
    [SerializeField] private float launchSpeed = 12f;
    [SerializeField] private float launchAngle = 35f;
    [SerializeField] private float timeStep = 0.08f;

    [SerializeField] private float yawAngle = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current == null)
            return;
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            FireProjectile();
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null)
        {
            return;
        }
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody body = projectile.GetComponent<Rigidbody>();
        if (body == null)
        {
            return;
        }
        body.linearDamping = linearDamping;
        body.useGravity = true;
        body.linearVelocity = GetLaunchVelocity();
    }
    private Vector3 GetLaunchVelocity()
    {
        // Quaternion.Euler는 각도 값을 회전으로 바꾸는 메서드입니다.
        // Unity에서 X축 양수 회전은 앞 방향을 아래로 기울입니다.
        // 그래서 "위로 launchAngle도"를 만들기 위해 X축에는 음수 각도를 넣습니다.
        Quaternion rotation = Quaternion.Euler(-launchAngle, yawAngle, 0f);

        // Vector3.forward는 월드 기준 앞 방향 벡터입니다.
        return rotation * Vector3.forward * launchSpeed;
    }
}
