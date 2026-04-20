using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//時間管理用フェーズ
public enum DayPhase
{
    Morning,
    DayTime,
    Night
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("プレイヤーのステータス情報")]
    public PlayerData playerData;
    public int currentHp;

    [Header("エンカウント時に保存するデータ")]
    public string lasstSceneName;
    public Vector3 lastEncountPosition;
    public List<EnemyData> nextBattleEnemies = new List<EnemyData>();

    public bool isInvincible; 

    [Header("FadeManagerの参照")]
    public FadeManager fadeManager;
    public FadeManager Fade => fadeManager;

    [Header("時間管理")]
    public DayPhase currentPhase = DayPhase.Morning;

    [Header("スクリプト参照")]
    public InventoryManager inventoryManager;
    public SymbolEncounterManager symbolEncounterManager;

    [Header("所持金ゴールド")]
    public int gold;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            inventoryManager = GetComponent<InventoryManager>();
            symbolEncounterManager = GetComponent<SymbolEncounterManager>();

            currentHp = playerData.maxHP;
        }
        else
        {
            Destroy(gameObject);
        }

        if (fadeManager == null)
        {
            fadeManager = GetComponent<FadeManager>();
        }
    }

    //エンカウントしたときの情報を保存する処理
    public void SetEncountData(string sceneName, Vector3 position, List<EnemyData> enemies,
                               string symbolId, Vector3 symbolPos)
    {
        lasstSceneName = sceneName;
        lastEncountPosition = position;
        nextBattleEnemies = enemies;

        //シンボルエンカウントなら、SymbolEncounterManagerにデータを送る
        if (!string.IsNullOrEmpty(symbolId))
        {
            symbolEncounterManager.SetEncountData(symbolId, enemies, symbolPos);
        }
    }

    //バトル後のHPを保存する処理
    public void SaveBattleResult(UnitStatus status)
    {
        this.currentHp = status.currentHP;

        playerData.maxHP = status.maxHP;
        playerData.attack = status.baseAttack;
        playerData.defense = status.baseDefense;
        playerData.speed = status.baseSpeed;
    }

    //シンボルエネミー撃破時処理
    public void DefeatedSymbolEnemy()
    {
        symbolEncounterManager.DefeatedSymbolEnemy();
    }

    //指定時間エンカウントを無効化する処理
    public async UniTaskVoid StartInvincibleTimer(float duration)
    {
        isInvincible = true;

        await UniTask.Delay((int)(duration * 1000));

        isInvincible = false;
        symbolEncounterManager.isSymbolEncounter = false;
    }
}
