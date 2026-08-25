using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform toastRoot;
    [SerializeField] private ToastMessage toastPrefab;

    public void ShowToast(string message)
    {
        ToastMessage toast = Instantiate(toastPrefab, toastRoot);
        toast.Show(message);
    }

    public void ShowItemToast()
    {
        ShowToast("æ∆¿Ã≈€¿ª »πµÊ«ﬂΩ¿¥œ¥Ÿ.");
    }
}