using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button confirmButton;

    private Action onConfirm;
    private bool initialized;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized)
            return;

        initialized = true;

        confirmButton.onClick.RemoveListener(Confirm);
        confirmButton.onClick.AddListener(Confirm);
    }

    public void ShowVictoryResult(
        int experience,
        int gold,
        string itemName,
        Action confirmAction)
    {
        Initialize();

        onConfirm = confirmAction;

        titleText.text = "ÀüÅõ ½Â¸®!";

        string result = "";

        if (experience > 0)
            result += $"°æÇèÄ¡ +{experience}\n";

        if (gold > 0)
            result += $"°ñµå +{gold}\n";

        if (!string.IsNullOrWhiteSpace(itemName))
            result += $"È¹µæ ¾ÆÀÌÅÛ: {itemName}\n";

        if (string.IsNullOrWhiteSpace(result))
            result = "È¹µæÇÑ º¸»óÀÌ ¾ø½À´Ï´Ù.";

        resultText.text = result.TrimEnd();

        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Debug.Log(
            $"°á°ú ÆË¾÷ È°¼ºÈ­: {gameObject.activeInHierarchy}"
        );
    }

    private void Confirm()
    {
        gameObject.SetActive(false);

        Action callback = onConfirm;
        onConfirm = null;

        callback?.Invoke();
    }
    
}