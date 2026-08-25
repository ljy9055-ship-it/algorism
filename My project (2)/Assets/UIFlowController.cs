using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIFlowController : MonoBehaviour
{
    [Header("Default Selection")]
    [SerializeField] private GameObject titleFirstButton;
    [SerializeField] private GameObject playFirstButton;
    [SerializeField] private GameObject resultFirstButton;

    [Header("Screens")]
    [SerializeField] private GameObject screenTitle;
    [SerializeField] private GameObject screenPlay;
    [SerializeField] private GameObject screenResult;
    [SerializeField] private GameObject panelInventory;

    private void Start()
    {
        ShowTitle();
    }

    private void Update()
    {
        // 플레이 화면일 때만 인벤토리 키 사용
        if (!screenPlay.activeSelf)
            return;

        if (Keyboard.current == null)
            return;

        // I키 = 열기/닫기
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }

        // ESC = 인벤토리 닫기
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (panelInventory.activeSelf)
            {
                CloseInventory();
            }
        }
    }

    public void ShowTitle()
    {
        screenTitle.SetActive(true);
        screenPlay.SetActive(false);
        screenResult.SetActive(false);
        panelInventory.SetActive(false);
        SelectButton(titleFirstButton);
    }

    public void StartGame()
    {
        screenTitle.SetActive(false);
        screenPlay.SetActive(true);
        screenResult.SetActive(false);
        panelInventory.SetActive(false);
        SelectButton(playFirstButton);
    }

    public void ShowResult()
    {
        screenTitle.SetActive(false);
        screenPlay.SetActive(false);
        screenResult.SetActive(true);
        panelInventory.SetActive(false);
        SelectButton(resultFirstButton);
    }

    public void OpenInventory()
    {
        panelInventory.SetActive(true);
    }

    public void CloseInventory()
    {
        panelInventory.SetActive(false);
    }

    public void ToggleInventory()
    {
        panelInventory.SetActive(!panelInventory.activeSelf);
    }

    public void RetryGame()
    {
        StartGame();
    }
    private void SelectButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}