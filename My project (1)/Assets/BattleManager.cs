using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject battlePanel;
    public TMP_Text battleText;
    public TMP_Text playerHpText;
    public TMP_Text enemyHpText;

    public Button attackButton;
    public Button defendButton;
    public Button escapeButton;

    private StoryManager storyManager;
    private EnemyData currentEnemy;

    private int enemyHp;
    private bool playerDefending;
    private bool battleEnded;

    public void StartBattle(
        EnemyData enemy,
        StoryManager manager
    )
    {
        if (enemy == null)
        {
            Debug.LogError("전투 적 데이터가 없습니다.");
            return;
        }

        storyManager = manager;
        currentEnemy = enemy;
        enemyHp = enemy.maxHp;

        playerDefending = false;
        battleEnded = false;

        battlePanel.SetActive(true);

        battleText.text =
            $"{currentEnemy.description}\n" +
            $"{currentEnemy.enemyName}과 전투가 시작되었다!";

        

        attackButton.onClick.AddListener(PlayerAttack);
        defendButton.onClick.AddListener(PlayerDefend);
        escapeButton.onClick.AddListener(TryEscape);

        SetButtonsInteractable(true);
        UpdateBattleUI();
    }

    private void PlayerAttack()
    {
        if (battleEnded)
            return;

        int damage = PlayerData.Instance.attack;
        enemyHp -= damage;
        enemyHp = Mathf.Max(0, enemyHp);

        battleText.text =
            $"{currentEnemy.enemyName}에게 " +
            $"{damage}의 피해를 주었다!";

        UpdateBattleUI();

        if (enemyHp <= 0)
        {
            Victory();
            return;
        }

        EnemyTurn();
    }

    private void PlayerDefend()
    {
        if (battleEnded)
            return;

        playerDefending = true;

        battleText.text =
            "방어 자세를 취했다. 이번 공격의 피해가 감소한다.";

        EnemyTurn();
    }

    private void EnemyTurn()
    {
        int damage = currentEnemy.attack;

        if (playerDefending)
        {
            damage = Mathf.Max(1, damage / 2);
            playerDefending = false;
        }

        int finalDamage = Mathf.Max(
            1,
            damage - PlayerData.Instance.defense
        );

        PlayerData.Instance.ChangeHp(-finalDamage);

        battleText.text +=
            $"\n{currentEnemy.enemyName}의 공격!" +
            $"\n{finalDamage}의 피해를 받았다.";

        UpdateBattleUI();

        if (PlayerData.Instance.hp <= 0)
        {
            Defeat();
        }
    }

    private void TryEscape()
    {
        if (battleEnded)
            return;

        bool escaped = Random.value <= 0.5f;

        if (escaped)
        {
            battleEnded = true;

            battleText.text = "전투에서 도망쳤다.";
            SetButtonsInteractable(false);

            storyManager.FinishBattleEscape();
        }
        else
        {
            battleText.text = "도망에 실패했다!";
            EnemyTurn();
        }
    }

    private void Victory()
    {
        battleEnded = true;
        SetButtonsInteractable(false);

        PlayerData player = PlayerData.Instance;

        player.ChangeGold(currentEnemy.goldReward);

        if (!string.IsNullOrWhiteSpace(currentEnemy.itemReward))
        {
            player.AddItem(currentEnemy.itemReward);
        }

        battleText.text =
            $"{currentEnemy.enemyName}을 쓰러뜨렸다!\n" +
            $"{currentEnemy.goldReward} 골드를 획득했다.";

        if (!string.IsNullOrWhiteSpace(currentEnemy.itemReward))
        {
            battleText.text +=
                $"\n{currentEnemy.itemReward}을 획득했다.";
        }

        storyManager.FinishBattleVictory();
    }

    private void Defeat()
    {
        battleEnded = true;
        SetButtonsInteractable(false);

        battleText.text = "체력이 모두 떨어졌다.";
        storyManager.FinishBattleDefeat();
    }

    private void SetButtonsInteractable(bool value)
    {
        attackButton.interactable = value;
        defendButton.interactable = value;
        escapeButton.interactable = value;
    }

    private void UpdateBattleUI()
    {
        playerHpText.text =
            $"플레이어 HP: {PlayerData.Instance.hp}" +
            $"/{PlayerData.Instance.maxHp}";

        enemyHpText.text =
            $"{currentEnemy.enemyName} HP: " +
            $"{enemyHp}/{currentEnemy.maxHp}";
    }

    public void CloseBattlePanel()
    {
        battlePanel.SetActive(false);
    }
    public bool IsInBattle
    {
        get
        {
            return !battleEnded && currentEnemy != null;
        }
    }

    public string CurrentEnemyId
    {
        get
        {
            if (currentEnemy == null)
                return string.Empty;

            return currentEnemy.enemyId;
        }
    }

    public int CurrentEnemyHp
    {
        get
        {
            return enemyHp;
        }
    }

}