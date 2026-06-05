using UnityEngine;
using UnityEngine.InputSystem;

public class cannon : MonoBehaviour
{
    [Header("Launch")]
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float launchSpeed = 12f;
    [SerializeField] private float launchAngle = 35f;
    [SerializeField] private float yawAngle = 0f;
    [Header("Prediction")]

    [SerializeField] private int maxSteps = 40;
    [SerializeField] private float timeStep = 0.08f;
    [SerializeField] private float linearDamping = 0f;

    private Vector3 hitPoint;
    private Vector3 lastPredictedPoint;
    private bool hasHit;
    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            yawAngle -= 60f * Time.deltaTime;
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            yawAngle += 60f * Time.deltaTime;
        }

        // Q/E 키로 발사 각도를 낮추거나 높입니다.
        if (Keyboard.current.qKey.isPressed)
        {
            launchAngle -= 40f * Time.deltaTime;
        }

        if (Keyboard.current.eKey.isPressed)
        {
            launchAngle += 40f * Time.deltaTime;
        }

        // Mathf.Clamp는 값을 지정한 최소/최대 범위 안에 가두는 메서드입니다.
        launchAngle = Mathf.Clamp(launchAngle, 5f, 80f);

        // Space 키를 누른 순간 실제 투사체를 발사합니다.
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            FireProjectile();
        }

    }
    private Vector3 GetLaunchVelocity()
    {
        Quaternion rotation = Quaternion.Euler(-launchAngle, yawAngle, 0f);
        return rotation * Vector3.forward * launchSpeed;
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

        // Unity 6에서 linearDamping은 Inspector의 Linear Damping 값과 대응합니다.
        body.linearDamping = linearDamping;
        body.useGravity = true;
        body.linearVelocity = GetLaunchVelocity();
    }

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;
        Vector3 velocity = GetLaunchVelocity();

        hasHit = false;
        lastPredictedPoint = position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, 0.15f);

        for (int i = 0; i < maxSteps; i++)
        {
            Vector3 previousPosition = position;

            // linearDamping이 0보다 크면 속도가 조금씩 줄어듭니다.
            velocity *= 1f - linearDamping * timeStep;

            // Physics.gravity는 현재 프로젝트에 설정된 중력 벡터입니다.
            velocity += Physics.gravity * timeStep;
            position += velocity * timeStep;

            Vector3 move = position - previousPosition;
            float distance = move.magnitude;

            // Physics.Raycast는 이전 점에서 다음 점 방향으로 선을 쏴 Collider와 닿는지 검사합니다.
            if (Physics.Raycast(previousPosition, move.normalized, out RaycastHit hit, distance))
            {
                hasHit = true;
                hitPoint = hit.point;

                Gizmos.color = Color.red;
                Gizmos.DrawLine(previousPosition, hit.point);
                Gizmos.DrawWireSphere(hit.point, 0.25f);
                break;
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(previousPosition, position);
            Gizmos.DrawWireSphere(position, 0.05f);
            lastPredictedPoint = position;
        }

        if (!hasHit)
        {
            // maxSteps 안에서 Collider를 만나지 못했다면 마지막 예측 지점을 표시합니다.
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(lastPredictedPoint, 0.25f);
            return;
        }

        // 착탄 지점을 한 번 더 크게 표시합니다.
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitPoint, 0.08f);

    }
}
