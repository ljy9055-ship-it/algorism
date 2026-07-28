using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("전투 UI")]
    public GameObject battlePanel;
    public TMP_Text battleText;
    public TMP_Text playerHpText;
    public TMP_Text enemyHpText;

    public Button attackButton;
    public Button defendButton;
    

    [Header("전투 결과 팝업")]
    [SerializeField] private BattleResultPopup resultPopup;

    private StoryManager storyManager;
    private EnemyData currentEnemy;

    private int enemyHp;
    private bool playerDefending;
    private bool battleEnded;
    [Header("전투 배경")]
    [SerializeField] private Image battleBackgroundImage;

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

        if (manager == null)
        {
            Debug.LogError("StoryManager가 없습니다.");
            return;
        }

        storyManager = manager;
        currentEnemy = enemy;
        enemyHp = enemy.maxHp;
        UpdateBattleBackground(currentEnemy.battleBackground);

        playerDefending = false;
        battleEnded = false;

        battlePanel.SetActive(true);

        battleText.text =
            $"{currentEnemy.description}\n" +
            $"{currentEnemy.enemyName}과 전투가 시작되었다!";

        /*
         * 전투를 여러 번 시작하면 리스너가 중복 등록될 수 있으므로
         * 기존 리스너를 제거한 뒤 다시 등록한다.
         */
        attackButton.onClick.RemoveListener(PlayerAttack);
        defendButton.onClick.RemoveListener(PlayerDefend);
        

        attackButton.onClick.AddListener(PlayerAttack);
        defendButton.onClick.AddListener(PlayerDefend);
        

        SetButtonsInteractable(true);
        UpdateBattleUI();
    }

    private void PlayerAttack()
    {
        if (battleEnded)
            return;

        PlayerData player = PlayerData.Instance;

        if (player == null)
        {
            Debug.LogError("PlayerData.Instance가 없습니다.");
            return;
        }

        int damage = player.Attack;

        enemyHp -= damage;
        enemyHp = Mathf.Max(0, enemyHp);

        battleText.text =
            $"{currentEnemy.enemyName}에게 " +
            $"{damage}의 피해를 주었다!";

        Debug.Log(
            $"기본 공격력: {player.attack}, " +
            $"최종 공격력: {player.Attack}, " +
            $"장착 무기: " +
            $"{(player.equippedWeapon != null ? player.equippedWeapon.equipmentName : "없음")}"
        );

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
            "방어 자세를 취했다.\n" +
            "이번 적 공격의 피해가 절반으로 감소한다.";

        EnemyTurn();
    }
    private void EnemyTurn()
    {
        if (battleEnded || currentEnemy == null)
            return;

        PlayerData player = PlayerData.Instance;

        if (player == null)
        {
            Debug.LogError("PlayerData.Instance가 없습니다.");
            return;
        }

        // 장비 보너스를 포함한 최종 방어력 적용
        int finalDamage = Mathf.Max(
            1,
            currentEnemy.attack - player.Defense
        );

        // 방어 중이면 최종 피해 절반
        if (playerDefending)
        {
            finalDamage = Mathf.Max(1, finalDamage / 2);
            playerDefending = false;
        }

        // 피해는 한 번만 적용
        player.ChangeHp(-finalDamage);

        battleText.text +=
            $"\n{currentEnemy.enemyName}의 공격!" +
            $"\n{finalDamage}의 피해를 받았다.";

        UpdateBattleUI();

        if (player.hp <= 0)
        {
            Defeat();
        }
    }
    private void UpdateBattleBackground(Sprite backgroundSprite)
    {
        if (battleBackgroundImage == null)
        {
            Debug.LogWarning("전투 배경 Image가 연결되지 않았습니다.");
            return;
        }

        if (backgroundSprite == null)
        {
            battleBackgroundImage.enabled = false;
            return;
        }

        battleBackgroundImage.sprite = backgroundSprite;
        battleBackgroundImage.enabled = true;
    }
    

    private void Victory()
    {
        if (battleEnded)
            return;

        battleEnded = true;
        SetButtonsInteractable(false);

        PlayerData player = PlayerData.Instance;

        int gainedExperience =
            currentEnemy.experienceReward;

        int gainedGold =
            currentEnemy.goldReward;

        string gainedItem =
            currentEnemy.itemReward;

        // 보상 지급
        player.experience += gainedExperience;
        player.ChangeGold(gainedGold);

        if (!string.IsNullOrWhiteSpace(gainedItem))
        {
            player.AddItem(gainedItem);
        }

        /*
         * PlayerData에 DefeatEnemy 함수가 실제로 있을 때만 사용한다.
         * 함수가 없다면 이 부분은 삭제한다.
         */
        if (!string.IsNullOrWhiteSpace(currentEnemy.enemyId))
        {
            player.DefeatEnemy(currentEnemy.enemyId);
        }

        battleText.text =
            $"{currentEnemy.enemyName}을 쓰러뜨렸다!";

        if (resultPopup == null)
        {
            Debug.LogError("BattleResultPopup이 연결되지 않았습니다.");

            // 팝업이 없어도 전투가 멈추지 않도록 바로 종료
            FinishVictory();
            return;
        }

        resultPopup.ShowVictoryResult(
            gainedExperience,
            gainedGold,
            gainedItem,
            FinishVictory
        );
    }

    private void FinishVictory()
    {
        CloseBattlePanel();

        if (storyManager != null)
        {
            storyManager.FinishBattleVictory();
        }

        currentEnemy = null;
        enemyHp = 0;
    }

    private void Defeat()
    {
        if (battleEnded)
            return;

        battleEnded = true;
        SetButtonsInteractable(false);

        battleText.text = "체력이 모두 떨어졌다.";

        storyManager.FinishBattleDefeat();
    }

    private void SetButtonsInteractable(bool value)
    {
        if (attackButton != null)
            attackButton.interactable = value;

        if (defendButton != null)
            defendButton.interactable = value;

        
    }

    private void UpdateBattleUI()
    {
        if (PlayerData.Instance == null ||
            currentEnemy == null)
        {
            return;
        }

        playerHpText.text =
            $"플레이어 HP: {PlayerData.Instance.hp}" +
            $"/{PlayerData.Instance.MaxHp}";

        enemyHpText.text =
            $"{currentEnemy.enemyName} HP: " +
            $"{enemyHp}/{currentEnemy.maxHp}";
    }

    public void CloseBattlePanel()
    {
        if (battlePanel != null)
        {
            battlePanel.SetActive(false);
        }
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