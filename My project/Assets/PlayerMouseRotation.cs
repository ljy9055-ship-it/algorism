using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouseRotation : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.15f;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        float mouseX = Mouse.current.delta.ReadValue().x;

        transform.Rotate(Vector3.up * mouseX * mouseSensitivity);
    }
}