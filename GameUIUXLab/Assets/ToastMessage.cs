using System.Collections;
using TMPro;
using UnityEngine;

public class ToastMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float lifeTime = 2f;

    public void Show(string message)
    {
        messageText.text = message;
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        // WaitForSeconds는 Time.timeScale의 영향을 받습니다.
        // Pause 중에도 Toast가 사라져야 할 때만 WaitForSecondsRealtime으로 바꿉니다.
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}