using UnityEngine;

public class Coinget : MonoBehaviour, IInteractable
{
    private Animator animator;
    private SimpleCharacterControllerMover playerMover;
    private ItemPickup itemData;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerMover =
                player.GetComponent<SimpleCharacterControllerMover>();

            if (playerMover == null)
            {
                Debug.LogError(
                    "Player 오브젝트에 SimpleCharacterControllerMover가 없습니다."
                );
            }
        }
        else
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾지 못했습니다.");
        }

        itemData = GetComponent<ItemPickup>();

        if (itemData == null)
        {
            Debug.LogError(
                "코인 오브젝트에 ItemPickup 컴포넌트가 없습니다."
            );
        }
    }

    public void Interact()
    {
        Debug.Log("코인 획득!");

        if (playerMover != null && itemData != null)
        {
            Debug.Log("속도 증가량: " + itemData.speedbonus);

            playerMover.IncreaseSpeed(itemData.speedbonus);
        }

        AutoItemCollector collector =
            FindFirstObjectByType<AutoItemCollector>();

        if (collector != null)
        {
            collector.CollectFromMonster(gameObject);
        }
        else
        {
            Debug.LogError("AutoItemCollector를 찾지 못했습니다.");
        }

        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }
    }

    public void DestroySelf()
    {
        Destroy(transform.root.gameObject);
    }
}