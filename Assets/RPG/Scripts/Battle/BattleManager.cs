using Cysharp.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//バトルの状態を管理する
public enum BattlePhase
{
    SetUp,
    CutIn,
    PlayerSelect,
    PlayerAction,
    EnemyAction,
    CheckAlive,
    Result
}

public class BattleManager : MonoBehaviour
{
    //定数
    private const float SLIDEIN_DURATION = 0.4f;        //スライドインする時間
    private const float FADEOUT_DURATION = 2f;          //フェードアウトする時間    
    private const float SAFE_TIME = 2.0f;               //シンボルエンカウントの無敵時間
    public const int NOMAL_USE_ITEM_AMOUNT = 1;         //通常使用するアイテムの量

    public static BattleManager instance { get; set; }

    [Header("生成設定")]
    [SerializeField] private UnitStatus playerUnitStatus;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemyContainer;

    [Header("バトルの状態管理")]
    [SerializeField] private BattlePhase currentPhase;

    [Header("スクリプト参照")]
    [SerializeField] private CommandManager commandManager;
    [SerializeField] private HeroineCutInManager heroineCutInManager;
    [SerializeField] private BattleItemWindowManager itemWindowManager;

    [Header("演出用設定")]
    [SerializeField] private RectTransform battleContent;
    [SerializeField] private Image effectImage;

    [Header("敗北時復帰設定")]
    [SerializeField] private string homeSceneName = "VillageScene";
    [SerializeField] private string homeMarkerId = "HomeEntrance";

    private bool isExiting = false;
    private PlayerInputAction inputAction;
    private List<UnitStatus> activeEnemyObject = new List<UnitStatus>();
    private List<EnemyData> defatedEnemyDataList = new List<EnemyData>();

    private void Awake()
    {
        instance = this;
        //InputActionのインスタンスを取得
        inputAction = new PlayerInputAction();

        //フェードイメージのアルファ値を0にしておく
        effectImage.color = new Color(0, 0, 0, 0);
    }

    private void OnEnable()
    {
        //バトル用のInputActionを有効化
        inputAction.Battle.Enable();

        //イベントの紐づけ
        inputAction.Battle.ReturnScene.performed += OnDicision;
    }

    private void OnDisable()
    {
        //バトル用のInputActionを無効化
        inputAction.Battle.Disable();
        //イベントの紐づけ解除
        inputAction.Battle.ReturnScene.performed -= OnDicision;
    }

    private void OnDicision(InputAction.CallbackContext context)
    {
        if (isExiting) return;
        ReturnToDungeon().Forget();
    }

    private async UniTaskVoid ReturnToDungeon()
    {
        isExiting = true;

        await SceneLoader.instance.ExcuteSceneTransition("CaveScene", "CaveEntrance", PlayerMove.instance);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //バトル開始
        StartBattle().Forget();
    }

    //バトルを開始する処理
    private async UniTaskVoid StartBattle()
    {
        //フェーズ変更
        currentPhase = BattlePhase.SetUp;
        if (GameManager.instance != null)
        {
            playerUnitStatus.currentHP = GameManager.instance.currentHp;
            playerUnitStatus.PlayerInitialize(GameManager.instance.playerData);
        }
        //敵生成
        await SetUpEnemies();

        //カットイン
        await HeroineCutIn();
        //プレイヤーコマンド選択
        await PlayerCommandSelect();
    }

    //妹カットイン処理
    private async UniTask HeroineCutIn()
    {
        //フェーズ変更
        currentPhase = BattlePhase.CutIn;

        if(heroineCutInManager != null)
        {
            await heroineCutInManager.PlayCutIn(0); ;
        }

        //ログ表示
        await LogManager.instance.DisplayLogText("やる気が出てきた！");

        Debug.Log("バフ付与");
    }

    //コマンド選択処理
    private async UniTask PlayerCommandSelect()
    {
        //フェーズ変更
        currentPhase = BattlePhase.PlayerSelect;
        await LogManager.instance.DisplayLogText("あなたのターン");

        //入力開始
        commandManager.SetPlayerInputActive(true);
    }

    //選択されたコマンドの処理
    public async UniTaskVoid OnPlayerActionSelected(string action)
    {
        if (currentPhase != BattlePhase.PlayerSelect) return;

        //フェーズ変更
        currentPhase = BattlePhase.PlayerAction;

        //たたかう選択時
        if (action == "たたかう")
        {
            await LogManager.instance.DisplayLogText($"誰を攻撃する？");
            //ターゲットを選択
            UnitStatus target = await TargetSelectManager.instance.SelectTarget(activeEnemyObject);

            if (target != null)
            {
                await LogManager.instance.DisplayLogText($"{playerUnitStatus.unitName}の攻撃！");
                await ExecutePlayerAttack(target);
            }
            else
            {
                //キャンセル時はコマンド選択に戻る
                await PlayerCommandSelect();
            }
        }
        else if (action == "アイテム")
        {
            //アイテムウィンドウを開く
            itemWindowManager.Open(
                (selectedItem) =>
                {
                    //アイテム決定時のコールバック
                    ExcuteItemAction(selectedItem).Forget();
                },
                async () =>
                {
                    //キャンセル時のコールバック
                    await PlayerCommandSelect();
                }
            );
        }
        else if (action == "にげる")
        {
            ExecutePlayerRun().Forget();
        }
    }

    //敵の行動処理
    public async UniTask EnemyAction()
    {
        currentPhase = BattlePhase.EnemyAction;

        List<UnitStatus> attackers = new List<UnitStatus>(activeEnemyObject);

        //順番に攻撃処理
        foreach(var enemy in attackers)
        {
            //倒されていないかチェック
            if (enemy == null || enemy.isDead)
            {
                continue;
            }

            await LogManager.instance.DisplayLogText($"{enemy.unitName}の攻撃!");

            //ダメージ処理
            await CalculateDamage(enemy, playerUnitStatus);

            //プレイヤーが倒れたら終了
            if (playerUnitStatus.isDead)
            {
                //ゲームオーバー処理
                await PlayerDefeat();
                return;
            }

            await UniTask.Delay(500);
        }

        await PlayerCommandSelect();
    }

    //敵の生成処理
    private async UniTask SetUpEnemies()
    {
        //前回の表示をクリア
        foreach(var obj in activeEnemyObject)
        {
            Destroy(obj.gameObject);
        }
        activeEnemyObject.Clear();

        //敵リストを取得
        List<EnemyData> enemiesSpawn = GameManager.instance.nextBattleEnemies;
        //演出待機用リスト
        List<UniTask> spawnTasks = new List<UniTask>();

        //データの数だけ生成して配置
        foreach(EnemyData data in enemiesSpawn)
        {
            GameObject enemyObj;
            //ボス用プレハブがあるかチェック
            bool isSpecalPrefab = data.battlePrefab != null;

            //生成処理分岐
            if (isSpecalPrefab)
            {
                enemyObj = Instantiate(data.battlePrefab, enemyContainer);

                enemyObj.transform.localPosition = data.battlePrefab.transform.localPosition;
                enemyObj.transform.localScale = data.battlePrefab.transform.localScale;
            }
            else
            {
                enemyObj = Instantiate(enemyPrefab, enemyContainer);
            }

            UnitStatus status = enemyObj.GetComponent<UnitStatus>();
            if (status != null)
            {
                status.EnemyInitialize(data);
                //出現演出の追加
                spawnTasks.Add(status.PlayEnemySpawnEffect());
                activeEnemyObject.Add(status);
            }

            if (!isSpecalPrefab)
            {
                Transform graphicTransform = enemyObj.transform.Find("Graphic");
                if (graphicTransform != null)
                {
                    //データの反映
                    Image sr = enemyObj.transform.Find("Graphic").GetComponent<Image>();
                    sr.sprite = data.enemySprite;
                    sr.preserveAspect = true;
                }
            }
        }
        //出現演出が終わるまで待機
        if (spawnTasks.Count > 0)
        {
            await UniTask.WhenAll(spawnTasks);
        }

        //ログ表示
        await LogManager.instance.DisplayLogText("モンスターがあらわれた！");
    }

    //プレイヤーの攻撃実行処理
    public async UniTask ExecutePlayerAttack(UnitStatus target)
    {
        //ダメージ処理
        await CalculateDamage(playerUnitStatus, target);

        //敵の死亡確認
        if (target.isDead)
        {
            await EnemyDeath(target, target.GetEnemyData());
        }

        //勝利判定
        await CheckBattleEnd();
    }

    //プレイヤーのアイテム使用処理
    private async UniTask ExcuteItemAction(ItemData item)
    {
        await LogManager.instance.DisplayLogText($"{playerUnitStatus.unitName}は{item.itemName}を使った!");
        //回復処理
        bool success = StatusManager.instance.RecoverHP(item.healAmount);
        if (success)
        {
            playerUnitStatus.currentHP = GameManager.instance.currentHp;
            playerUnitStatus.UpdateHPUI();

            //在庫を減らす
            InventoryManager.instance.RemoveItem(item, NOMAL_USE_ITEM_AMOUNT);
            await LogManager.instance.DisplayLogText($"{item.healAmount}回復した!");
        }
        else
        {
            await LogManager.instance.DisplayLogText($"なにも起きなかった...");
        }

            //敵のターンへ
            await EnemyAction();
    }

    //プレイヤーの逃走実行処理
    public async UniTask ExecutePlayerRun()
    {
        await LogManager.instance.DisplayLogText($"{playerUnitStatus.unitName}は逃げ出した！");

        if (GameManager.instance.isEscapeDisabled)
        {
            await LogManager.instance.DisplayLogText("この敵からはにげられない");
            await EnemyAction();
        }
        //逃走成功判定
        if (CaluculateRunSuccess())
        {
            await LogManager.instance.DisplayLogText("うまく逃げ切れた!");

            //バトル結果を保存
            GameManager.instance.SaveBattleResult(playerUnitStatus);

            //スライドイン
            await GameManager.instance.Fade.SlideIn(SLIDEIN_DURATION, effectImage);

            //シーン遷移
            if (GameManager.instance != null && SceneLoader.instance != null)
            {
                await SceneLoader.instance.ExcuteReturnScene(
                    GameManager.instance.lasstSceneName,
                    GameManager.instance.lastEncountPosition,
                    PlayerMove.instance);

                //シンボルエンカウント時のみ無敵時間発生
                if (SymbolEncounterManager.instance.isSymbolEncounter)
                {
                    GameManager.instance.StartInvincibleTimer(SAFE_TIME).Forget();
                }
            }
        }
        else
        {
            await LogManager.instance.DisplayLogText("しかし回り込まれてしまった!");
            await EnemyAction();
        }
    }

    //攻撃計算、ダメージ反映処理
    private async UniTask CalculateDamage(UnitStatus attacker,UnitStatus target)
    {
        //回避率計算
        int evadeChance = Mathf.Min(target.Speed, 50);

        bool isMiss = Random.Range(0, 100) < evadeChance;
        //回避判定
        if (isMiss)
        {
            await LogManager.instance.DisplayLogText($"ミス! {target.unitName}に攻撃が当たらない!");
            return;
        }

        //ダメージ計算
        int baseDamage = attacker.Attack - target.Defense;
        //ダメージにブレをつける(0.9～1.1倍)
        float randomVariation = Random.Range(0.9f, 1.1f);
        //最低1ダメージ保障
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * randomVariation));

        //ダメージ適用
        target.TakeDamage(finalDamage);

        //演出の分岐
        if (target.isPlayer)
        {
            //UI反映
            target.UpdateHPUI();
            //GameManagerと同期
            if (GameManager.instance != null)
            {
                GameManager.instance.currentHp = target.currentHP;
            }
            //画面を揺らす
            await battleContent.DOShakePosition(0.4f, 40f, 50, 90f);
        }
        else
        {
            await target.PlayEnmeyDamageEffect();
        }
        await LogManager.instance.DisplayLogText($"{target.unitName}に{finalDamage}のダメージ!");
    }

    //逃走成功確率計算処理
    private bool CaluculateRunSuccess()
    {
        //敵の平均速度を計算
        float enemyAvgSpeed = 0;
        foreach(var enemy in activeEnemyObject)
        {
            enemyAvgSpeed += enemy.Speed;
        }
        enemyAvgSpeed /= activeEnemyObject.Count;

        //プレイヤーの方が速ければ逃走しやすい
        float successRate = (playerUnitStatus.Speed / enemyAvgSpeed) * 50;

        //成功率を20%～80%の範囲に制限
        successRate = Mathf.Clamp(successRate, 20, 80);

        return Random.Range(0, 100) < successRate;
    }

    //敵の撃破処理
    private async UniTask EnemyDeath(UnitStatus target,EnemyData enemyData)
    {
        //QuestManagerに通知
        if (QuestManager.instance != null)
        {
            QuestManager.instance.OnEnemyKilled(enemyData);
        }

        //倒した敵リストに追加
        defatedEnemyDataList.Add(enemyData);

        //消滅演出
        await target.PlayEnemyDeathEffect();
        await LogManager.instance.DisplayLogText($"{target.unitName}をたおした!");

        //リストから消す
        activeEnemyObject.Remove(target);

        //オブジェクトの破壊
        Destroy(target.gameObject);
    }

    //勝利判定
    private async UniTask CheckBattleEnd()
    {
        //敵を倒しきったとき
        if (activeEnemyObject.Count <= 0)
        {
            await PlayerVictory();
            return;
        }
        else
        {
            //敵のターンへ
            await EnemyAction();
        }
    }

    //勝利時処理
    private async UniTask PlayerVictory()
    {
        currentPhase = BattlePhase.Result;
        await LogManager.instance.DisplayLogText("戦闘に勝利した");

        GameManager.instance.isEscapeDisabled = false;

        //経験値獲得
        int totalExp = 0;
        foreach(var enemy in defatedEnemyDataList)
        {
            totalExp += enemy.expValue;
        }
        await playerUnitStatus.GainExp(totalExp);

        //倒した敵リストをクリア
        defatedEnemyDataList.Clear();

        if (GameManager.instance != null)
        {
            //シンボルエネミー撃破時処理
            GameManager.instance.DefeatedSymbolEnemy();

            //バトル結果を保存
            GameManager.instance.SaveBattleResult(playerUnitStatus);
        }
        //シーン遷移
        if (GameManager.instance != null && SceneLoader.instance != null)
        {
            await SceneLoader.instance.ExcuteReturnScene(
                GameManager.instance.lasstSceneName,
                GameManager.instance.lastEncountPosition,
                PlayerMove.instance);
        }
    }

    //敗北時処理
    private async UniTask PlayerDefeat()
    {
        currentPhase = BattlePhase.Result;

        GameManager.instance.isEscapeDisabled = false;

        await LogManager.instance.DisplayLogText($"{playerUnitStatus.unitName}は力尽きた...");
        //フェードアウト
        await GameManager.instance.Fade.FadeOut(FADEOUT_DURATION, effectImage);

        //体力回復
        StatusManager.instance.RecoverHP(GameManager.instance.playerData.maxHP);

        //シーン遷移
        await SceneLoader.instance.ExcuteSceneTransition(homeSceneName, homeMarkerId, PlayerMove.instance);
    }
}
