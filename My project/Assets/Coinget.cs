using UnityEngine;

public class Coinget : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("코인 획득!");

        AutoItemCollector collector = FindFirstObjectByType<AutoItemCollector>();

        if (collector != null)
        {
            collector.CollectFromMonster(gameObject);
        }
        else
        {
            Debug.LogError("AutoItemCollector를 찾지 못했습니다.");
        }

        Destroy(gameObject);
    }
}