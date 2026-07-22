using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public float masterVolume = 1f;
    public float bgmVolume = 1f;
    public float effectVolume = 1f;

    public bool fullscreen = true;
    public int textSpeed = 30;

    public void ApplySettings()
    {
        AudioListener.volume = masterVolume;
        Screen.fullScreen = fullscreen;
    }
}