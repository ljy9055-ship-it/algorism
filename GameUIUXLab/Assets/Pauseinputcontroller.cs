using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInputController : MonoBehaviour
{
    [SerializeField] private UIScreenFlowController screenFlow;

    private InputAction pauseAction;
    private bool isPaused;

    void Awake()
    {
        pauseAction = new InputAction("Pause", InputActionType.Button);
        pauseAction.AddBinding("<Keyboard>/escape");
        pauseAction.AddBinding("<Gamepad>/start");
    }

    void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPausePerformed;
    }

    void OnDisable()
    {
        pauseAction.performed -= OnPausePerformed;
        pauseAction.Disable();
    }

    void OnDestroy()
    {
        pauseAction.Dispose();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            screenFlow.PauseGame();
        }
        else
        {
            screenFlow.ResumeGame();
        }
    }
}