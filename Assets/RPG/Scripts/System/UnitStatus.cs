using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitStatus : MonoBehaviour
{
    [Header("HP表示設定")]
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Image hpImage;

    [Header("現在の状態")]
    public string unitName;
    public int level;
    public int currentHP;

    //基礎値
    [HideInInspector] public int maxHP;
    [HideInInspector] public int baseAttack;
    [HideInInspector] public int baseDefense;
    [HideInInspector] public int baseSpeed;

    [Header("装備補正")]
    private int equipmentAttack;
    private int equipmentDefense;
    private int equipmentSpeed;

    [Header("戦闘補正(バフ・デバフ)")]
    public float attackBuff = 1.0f;
    public float defenseBuff = 1.0f;
    public float speedBuff = 1.0f;

    [Header("判定フラグ")]
    public bool isPlayer;
    public bool isDead => currentHP <= 0;

    public Action OnStatusChanged;

    private Image cachedGraphic;
    private EnemyData originData;

    //計算用プロパティ
    public int Attack => Mathf.RoundToInt((baseAttack + equipmentAttack) * attackBuff);
    public int Defense => Mathf.RoundToInt((baseDefense + equipmentDefense) * defenseBuff);
    public int Speed => Mathf.RoundToInt((baseSpeed + equipmentSpeed) * speedBuff);

    //PlayerDataからステータスを初期化する処理
    public void PlayerInitialize(PlayerData playerData)
    {
        unitName = playerData.playerName;
        level = playerData.level;
        maxHP = playerData.maxHP;
        baseAttack = playerData.attack;
        baseDefense = playerData.defense;
        baseSpeed = playerData.speed;

        equipmentAttack = playerData.TotalAttack - playerData.attack;
        equipmentDefense = playerData.TotalDefense - playerData.defense;
        equipmentSpeed = playerData.TotalSpeed - playerData.speed;

        isPlayer = true;
        OnStatusChanged?.Invoke();

        UpdateHPUI(false);
    }

    //EnemyDataからステータスを初期化する処理
    public void EnemyInitialize(EnemyData enemyData)
    {
        var graphicObj = transform.Find("Graphic");
        if (graphicObj != null)
        {
            cachedGraphic = graphicObj.GetComponent<Image>();
        }

        originData = enemyData;

        unitName = enemyData.enemyName;
        level = enemyData.level;
        maxHP = enemyData.maxHp;
        currentHP = maxHP;
        baseAttack = enemyData.attack;
        baseDefense = enemyData.defense;
        baseSpeed = enemyData.speed;
        isPlayer = false;
        OnStatusChanged?.Invoke();
    }

    //EnemyDataのゲッター
    public EnemyData GetEnemyData()
    {
        return originData;
    }

    //ダメージを受ける処理
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        OnStatusChanged?.Invoke();
    }

    //プレイヤーのHPUIを更新する処理
    public void UpdateHPUI(bool animate = true)
    {
        if (hpImage == null && hpText == null) return;

        float targetValue = (float)currentHP / maxHP;

        //テキスト反映
        hpText.text = $"{currentHP}/{maxHP}";

        //スライダー反映
        if (animate)
        {
            //減少演出
            hpImage.DOFillAmount(targetValue, 0.3f).SetEase(Ease.OutQuad);
        }
        else
        {
            hpImage.fillAmount = targetValue;
        }
    }

    //経験値を獲得する処理
    public async UniTask GainExp(int amount)
    {
        if (!isPlayer)
        {
            return;
        }

        PlayerData data = GameManager.instance.playerData;
        data.currentExp += amount;

        await LogManager.instance.DisplayLogText($"{unitName}は{amount}の経験値を獲得した");

        //連続レベルアップも考慮する
        while (data.currentExp >= data.nextLevelExp)
        {
            await LevelUp(data);
        }
    }

    //レベルアップ処理
    public async UniTask LevelUp(PlayerData data)
    {
        data.currentExp -= data.nextLevelExp;
        //レベルアップ
        data.level++;
        level = data.level;

        //ステータス上昇
        maxHP += data.hpGain;
        baseAttack += data.attackGain;
        baseDefense += data.defenseGain;
        baseSpeed += data.sppedGain;

        //次のレベルアップに必要な経験値量を更新
        data.CalculateNextLevelExp();

        await LogManager.instance.DisplayLogText($"{unitName}は{level}レベルに上がった");
        await LogManager.instance.DisplayLogText("各ステータスが上がった");

        //UI更新
        UpdateHPUI(true);
    }

    //敵用ダメージ演出
    public async UniTask PlayEnmeyDamageEffect()
    {
        if (cachedGraphic == null)
        {
            return;
        }

        //点滅処理
        await cachedGraphic.DOColor(Color.red, 0.08f)
            .SetLoops(4, LoopType.Yoyo)
            .SetEase(Ease.Linear)
            .ToUniTask();

        cachedGraphic.color = Color.white;
    }

    //敵死亡時演出
    public async UniTask PlayEnemyDeathEffect()
    {
        if (cachedGraphic == null)
        {
            return;
        }

        //再生中のアニメーションを終了する
        cachedGraphic.DOKill();

        //演出の流れを作成
        Sequence deathSeq = DOTween.Sequence();

        //敵キャラクターを黒くする
        deathSeq.Append(cachedGraphic.DOColor(Color.black, 0.3f));

        //フェード処理と震える演出処理を同時に行う
        //震える演出
        deathSeq.Join(cachedGraphic.rectTransform.DOShakePosition(0.5f, 15f, 20));
        //フェードアウト演出
        deathSeq.Join(cachedGraphic.DOFade(0, 0.5f).SetEase(Ease.InQuad));
        //少し沈む演出
        deathSeq.Join(cachedGraphic.rectTransform.DOAnchorPosY(-30f, 0.5f).SetRelative());

        await deathSeq.ToUniTask();

    }
}
