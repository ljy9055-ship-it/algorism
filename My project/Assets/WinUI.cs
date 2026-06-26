using UnityEngine;

public class WinUI : MonoBehaviour
{
    public GameObject winUI;

    public void ShowWin()
    {
        winUI.SetActive(true);
    }
}