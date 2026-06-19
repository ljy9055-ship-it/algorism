using UnityEngine;

public class Coinget : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("ÄÚÀÎ È¹µæ!");
        Destroy(gameObject);
        Debug.Log("°¨Áö ÈÄº¸ : ");
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
