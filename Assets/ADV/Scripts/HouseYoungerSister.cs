using System;
using UnityEngine;
using UnityEngine.UI;

public enum YoungerSisterParameterType
{
    NONE = 0,
    PRACTICE,       // 応援練習
    MAKE_BENTO,     // お弁当作り
    MAINTENANCE,    // 装備メンテナンス
    PRESENT,        // プレゼント
    PAMPER,         // 甘やかす
}

public class HouseYoungerSister: MonoBehaviour
{
    enum State
    {
        IDLE = 0,
        TALK,           // 会話
        STILL,          // スチル
    }

    [SerializeField, Header("ADV")]
    private AdvController advController_ = null;

    [SerializeField, Header("パラメータのスクリプタブobj")]
    private YoungerSisterParameter parameter_ = null;

    [SerializeField, Header("表示を切り替えるボタンのリスト")]
    private Button[] buttonList_ = null;

    private State state_ = State.IDLE;

    private int eventPlayPower_ = 30;

    private bool needStartEnter_ = true;

    private bool isFirstEvent_ = false;

    // 開始時セリフのパターン数
    private const int ENTER_TEXT_MAX = 4;


    private void Awake()
    {
        InitializeADV();
        state_ = State.IDLE;

        // TODO: セーブデータ参照して
        isFirstEvent_ = false;
    }

    private void Update()
    {
        switch (state_)
        {
            case State.IDLE:
                // Startでは再生できなかったのでここで呼ぶ
                if (advController_.CanPlay() && needStartEnter_)
                {
                    needStartEnter_ = false;

                    // 初回イベント
                    if (isFirstEvent_)
                    {
                        isFirstEvent_ = false;

                        InitializeADV();
                        advController_.OnPlayFinish_.AddListener(OnEnterADVFinish);
                        advController_.PlayADV("TODO: 初回限定の特殊セリフID");

                        return;
                    }

                    // 通常イベント
                    PlayEnterADV();
                }
                break;

            case State.TALK:
                break;

            case State.STILL:
                break;

            default:
                Debug.Log("HouseYoungerSister::Update -> case文がdefaultです");
                break;
        }

        
    }

    private void InitializeADV()
    {
        state_ = State.TALK;

        // ボタンの非表示
        foreach (Button button in buttonList_)
        {
            button.gameObject.SetActive(false);
        }

        // パラメータログ確認
        parameter_.PrintLog();
    }

    public void OnPressButtonEvent(int index)
    {
        YoungerSisterParameterType eventType = (YoungerSisterParameterType)index;
        switch (eventType)
        {
            case YoungerSisterParameterType.PRACTICE:
                parameter_.attack += 10;
                break;

            case YoungerSisterParameterType.MAKE_BENTO:
                parameter_.defense += 10;
                break;

            case YoungerSisterParameterType.MAINTENANCE:
                parameter_.speed += 10;
                break;

            case YoungerSisterParameterType.PRESENT:
                parameter_.affection += 10;// TODO: プレゼントの内容によって変わるように
                break;

            case YoungerSisterParameterType.PAMPER:
                parameter_.affection += 10;
                parameter_.attack -= 5;
                parameter_.defense -= 5;
                parameter_.speed -= 5;
                break;

            default:
                Debug.Log("YoungerSyster::OnPressButtonEvent -> case文がdefaultです");
                return;
        }

        eventPlayPower_ -= 10;

        // ADV再生
        InitializeADV();
        advController_.OnPlayFinish_.AddListener(OnEventFinish);
        advController_.PlayADV("C1001_HOME_ACT_SUCCESS_" + index.ToString("000"));
    }

    public void PlayEnterADV()
    {
        InitializeADV();
        advController_.OnPlayFinish_.AddListener(OnEnterADVFinish);

        // ランダムでテキストを選ぶ
        string randID = UnityEngine.Random.Range(1, ENTER_TEXT_MAX + 1).ToString("000");
        string likeID = "H_";
        
        // (妹_家_開始時_ + 好感度 + id)
        advController_.PlayADV("C1001_HOME_ENTER_" + likeID + randID);

        //セリフやモーションがあればここで再生
    }
    private void OnEnterADVFinish()
    {
        foreach (Button button in buttonList_)
        {
            button.gameObject.SetActive(true);
        }
    }

    private void OnEventFinish()
    {
        if (eventPlayPower_ <= 0)
        {
            // イベント終了
            Debug.Log("HouseYoungerSister::今回の育成は終わりです。");

            // セリフやモーションがあればここで再生

            return;
        }

        // まだ育成できる
        Debug.Log($"妹強化用パワー: {eventPlayPower_}");
        OnEnterADVFinish();
    }

    public void PlayTouchBodyADV()
    {
        InitializeADV();
        advController_.OnPlayFinish_.AddListener(OnEnterADVFinish);

        // タップされたらセリフやモーションがあればここで再生
        int id = 1;
        advController_.PlayADV("C1001_HOME_TOUCH_" + id.ToString("000"));
    }

    public void StartStill()
    {
        if (state_ == State.STILL)
        {
            Debug.Log("すでにスチル中です");
            return;
        }

        // TODO: スチル再生の処理
        Debug.Log("HouseYoungerSister::StartStill -> スチル開始");
        state_ = State.STILL;

    }
}
