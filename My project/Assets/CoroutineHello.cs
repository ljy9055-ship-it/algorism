using System.Collections;
using UnityEngine;

public class CoroutineHello : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(PrintAfterDelay());
    }

    private IEnumerator PrintAfterDelay()
    {
        Debug.Log("코루틴 시작");

        yield return new WaitForSeconds(2f);

        Debug.Log("2초 뒤에 실행");
    }
}