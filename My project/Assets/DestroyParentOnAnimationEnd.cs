using UnityEngine;

public class DestroyParentOnAnimationEnd : MonoBehaviour
{
    public void DestroyParent()
    {
        Destroy(transform.root.gameObject);
    }
}