using UnityEngine;

public class Coinget : MonoBehaviour, IInteractable
{
    private Animator animator;
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
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

        animator.SetBool("isDead", true);
        
    }
    public void DestroySelf()
    {
        Destroy(transform.root.gameObject);
    }
}