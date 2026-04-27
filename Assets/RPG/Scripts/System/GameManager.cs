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

//階層情報をまとめて管理するためのクラス
[System.Serializable]
public class FloorData
{
    public string floorName;
    public string sceneName;
    public string markerId;
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

    [Header("スクリプト参照")]
    public InventoryManager inventoryManager;
    public SymbolEncounterManager symbolEncounterManager;
    public FadeManager fadeManager;

    [Header("時間管理")]
    public DayPhase currentPhase = DayPhase.Morning;

    [Header("所持金ゴールド")]
    public int gold;

    [Header("到達済み階層")]
    public List<FloorData> visitedFloorData = new List<FloorData>();

    [Header("バトル設定")]
    public bool isEscapeDisabled = false;

    [Header("イベント用フラグ")]
    public bool isNewGame = false;
    public FadeManager Fade => fadeManager;
    //ラスボスを倒したフラグ
    public bool IslastBossDefated { get; set; } = false;
    
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

    //ラスボス撃破フラグを立てるための関数
    public void SetLastBossDefated()
    {
        IslastBossDefated = true;
    }

    //指定時間エンカウントを無効化する処理
    public async UniTaskVoid StartInvincibleTimer(float duration)
    {
        isInvincible = true;

        await UniTask.Delay((int)(duration * 1000));

        isInvincible = false;
        symbolEncounterManager.isSymbolEncounter = false;
    }

    //すでに同じシーンが登録されていないか確認する処理
    public bool IsFloorRegistered(string fName)
    {
        return visitedFloorData.Exists(f => f.markerId == fName);
    }

    //新しい階層を記録する処理
    public void RegisterVisitedFloor(string fName,string sName,string mId)
    {
        if (!IsFloorRegistered(sName))
        {
            //新しく到達した階層を保存
            FloorData newFloor = new FloorData
            {
                floorName = fName,
                sceneName = sName,
                markerId = mId
            };

            visitedFloorData.Add(newFloor);
            Debug.Log($"<color=cyan>階層を記録しました:{fName}</color>");
        }
    }
}
