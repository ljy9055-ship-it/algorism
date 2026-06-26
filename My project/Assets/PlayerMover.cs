using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 0.1f;

    private CharacterController controller;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        transform.Rotate(
            Vector3.up,
            lookInput.x * mouseSensitivity
        );

        Vector3 move =
            transform.forward * moveInput.y +
            transform.right * moveInput.x;

        controller.Move(
            move * moveSpeed * Time.deltaTime
        );
    }
}