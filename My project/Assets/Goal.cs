using UnityEngine;

public class Goal : MonoBehaviour, IInteractable
{
    public WinUI winUI;

    public void Interact()
    {
        winUI.ShowWin();
    }
}