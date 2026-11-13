using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public sealed class PlatformConfigLoader : MonoBehaviour
{
    [SerializeField] private GameSession session;
    [SerializeField] private string remoteUrl;

    public void ApplySettings()
    {
        // 진행 중인 라운드의 규칙을 바꾸거나, 스테이지 준비와 설정 읽기가 겹치지 않게 합니다.
        if (session.IsPlaying || session.IsLoading || !session.IsReady)
        {
            return;
        }

        session.SetBusy(true);
        StartCoroutine(LoadSettings());
    }

    private IEnumerator LoadSettings()
    {
        string url = remoteUrl;
        if (string.IsNullOrWhiteSpace(url))
        {
            string path = Application.streamingAssetsPath + "/platform-config.json";
            // 이미 URL 형태이면 그대로 쓰고, PC의 파일 경로는 file:// 형식의 URL로 바꿉니다.
            url = path.Contains("://") ? path : new Uri(path).AbsoluteUri;
        }

        // 실패 경로마다 값을 다시 넣지 않아도 되도록, 기본값 30초를 먼저 준비합니다.
        int seconds = 30;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = 10;

            // 응답을 기다리는 동안 제어를 Unity에 돌려줍니다. 요청이 끝난 뒤 아래 코드를 이어 실행합니다.
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    // HTTP 성공만으로는 충분하지 않습니다. 본문을 JSON으로 읽고 값의 범위까지 확인합니다.
                    PlatformConfig config = JsonUtility.FromJson<PlatformConfig>(request.downloadHandler.text);
                    // &&는 왼쪽부터 확인하므로 config가 null이면 그 안의 roundSeconds를 읽지 않습니다.
                    if (config != null
                        && config.roundSeconds >= 10
                        && config.roundSeconds <= 120)
                    {
                        seconds = config.roundSeconds;
                    }
                    else
                    {
                        Debug.LogWarning("Invalid roundSeconds; use default 30.");
                    }
                }
                catch (Exception error)
                {
                    Debug.LogWarning($"Invalid JSON: {error.Message}; use default 30.");
                }
            }
            else
            {
                Debug.LogWarning($"Config failed: {request.error}; use default 30.");
            }
        }

        // 설정값 또는 기본값을 다음 라운드에 적용한 뒤 시작 잠금을 풉니다.
        session.SetRoundSeconds(seconds);
        session.SetBusy(false);
        session.SetStatus($"Next round: {seconds} seconds");
    }
}