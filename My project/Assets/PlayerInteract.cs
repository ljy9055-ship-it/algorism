using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public Transform cameraTransform;
    public float interactDistance = 5f;
    public float interactRadius = 0.5f;

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("F 입력됨");
            TryInteract();
        }
    }

    void TryInteract()
    {
        Vector3 origin = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;

        Debug.DrawRay(origin, direction * interactDistance, Color.red, 1f);

        RaycastHit[] hits = Physics.SphereCastAll(
            origin,
            interactRadius,
            direction,
            interactDistance,
            ~0,
            QueryTriggerInteraction.Collide
        );
     

        foreach (RaycastHit hit in hits)
        {
            

            if (hit.collider.transform.root == transform.root)
            {
               
                continue;
            }

            

            IInteractable interactable =
                hit.collider.GetComponentInParent<IInteractable>();

            if (interactable == null)
            {
                interactable = hit.collider.GetComponentInChildren<IInteractable>();
            }

            if (interactable != null)
            {
                Debug.Log("상호작용 성공");
                interactable.Interact();
                Animator animator = new Animator();
                animator = GetComponentInChildren<Animator>();
                animator.SetTrigger("Interact");
                return;
            }
        }

        Debug.Log("상호작용 가능한 대상 없음");

        
    }
    }