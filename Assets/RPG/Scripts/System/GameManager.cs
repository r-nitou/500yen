using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("シンボルエンカウント用")]
    public List<string> defeatedSymbolId = new List<string>();
    public string currentSymbolId;
    public bool isSymbolEncounter;
    public bool isInvincible; 

    [Header("FadeManagerの参照")]
    public FadeManager fadeManager;
    public FadeManager Fade => fadeManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
    public void SetEncountData(string sceneName, Vector3 position, List<EnemyData> enemies, string symbolId = "")
    {
        lasstSceneName = sceneName;
        lastEncountPosition = position;
        nextBattleEnemies = enemies;
        currentSymbolId = symbolId;
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
        if (isSymbolEncounter && !string.IsNullOrEmpty(currentSymbolId))
        {
            //撃破済みリストに追加
            if (!defeatedSymbolId.Contains(currentSymbolId))
            {
                defeatedSymbolId.Add(currentSymbolId);
            }
        }

        //フラグとIDをリセット
        isSymbolEncounter = false;
        currentSymbolId = "";
    }

    //指定時間エンカウントを無効化する処理
    public async UniTaskVoid StartInvincibleTimer(float duration)
    {
        isInvincible = true;

        await UniTask.Delay((int)(duration * 1000));

        isInvincible = false;
        isSymbolEncounter = false;
    }
}
