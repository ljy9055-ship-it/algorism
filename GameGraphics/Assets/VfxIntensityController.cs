using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class VfxIntensityController : MonoBehaviour
{
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private string spawnRateName = "SpawnRate";
    [SerializeField] private float lowRate = 20f;
    [SerializeField] private float highRate = 100f;

    public void OnLowIntensity(InputValue value)
    {
        if (value.isPressed)
        {
            visualEffect.SetFloat(spawnRateName, lowRate);
        }
    }

    public void OnHighIntensity(InputValue value)
    {
        if (value.isPressed)
        {
            visualEffect.SetFloat(spawnRateName, highRate);
        }
    }
}