using UnityEngine;
using UnityEngine.InputSystem;

public class hardtest4 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }
        Vector2 input = Vector2.zero;
        if (keyboard.aKey.isPressed)
        {
            input.x -= 1f;
        }
        if (keyboard.dKey.isPressed)
        {
            input.x += 1f;
        }
        if(keyboard.sKey.isPressed)
        {
            input.y -= 1f;

        }
        if (keyboard.wKey.isPressed)
        {
            input.y += 1f;
        }
    }
}
