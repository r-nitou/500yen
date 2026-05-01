using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public enum YoungerSisterButtonType
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

    [SerializeField, Header("パラメータ表示のリスト")]
    private YoungerSisterGaugeList gaugeList_ = null;

    [SerializeField, Header("表示を切り替えるボタンのリスト")]
    private YoungerEventButton[] buttonList_ = null;

    [SerializeField]
    private TextMeshProUGUI eventPowerText = null;

    [Header("遷移設定")]
    [SerializeField] 
    private string nextSceneName_ = "VillageScene";
    [SerializeField] 
    private string homeMarkerId_ = "HomeEntrance";

    [SerializeField]
    private GameObject tutorial_ = null;

    [SerializeField]
    private Image eventPower_ = null;

    [SerializeField]
    private Image fadeImage;

    private State state_ = State.IDLE;

    private int eventPlayPower_ = 30;

    private bool needStartEnter_ = true;
    private bool needStartTutorial_ = false;
    private bool isTutorial_ = false;
    
    // 永続化用のキー（定数）
    private const string FIRST_EVENT_KEY = "HouseEnterIsFirst";

    // 開始時セリフのパターン数
    private const int ENTER_TEXT_MAX = 5;


    private void Awake()
    {
        InitializeADV();
        gaugeList_.SetParameterForce(parameter_);
        gaugeList_.SetActive(false);
        eventPower_.gameObject.SetActive(false);
        state_ = State.IDLE;
    }

    private void Update()
    {
        switch (state_)
        {
            case State.IDLE:
                // Startでは再生できなかったのでここで呼ぶ
                if (needStartEnter_ && advController_.CanPlay())
                {
                    needStartEnter_ = false;

                    // 初回か判定 //TODO: JSONに変わるっぽい
                    if (PlayerPrefs.GetInt(FIRST_EVENT_KEY, 1) != 0)
                    {
                        PlayerPrefs.SetInt(FIRST_EVENT_KEY, 0);// false >> 再生済み
                        PlayerPrefs.Save();

                        // 初回用のイベント
                        InitializeADV();
                        advController_.OnPlayFinish_.AddListener(SetTutorialWait);
                        advController_.PlayADV("C1001_HOME_FRIST");

                        return;
                    }

                    // 通常イベント
                    PlayEnterADV();
                }
                else if (needStartTutorial_ && advController_.CanPlay())
                {
                    OpenTutorial();
                    return;
                }
                break;

            case State.TALK:
                // ボタン待機中モココ
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
        foreach (YoungerEventButton button in buttonList_)
        {
            button.gameObject.SetActive(false);
        }

        // パラメータログ確認
        parameter_.PrintLog();
    }

    public void OnPressButtonEvent(int index)
    {
        YoungerSisterButtonType eventType = (YoungerSisterButtonType)index;
        switch (eventType)
        {
            case YoungerSisterButtonType.PRACTICE:
                if (CheckEventPoint(index))
                {
                    parameter_.attack += 10;
                }
                break;

            case YoungerSisterButtonType.MAKE_BENTO:
                if (CheckEventPoint(index))
                {
                    parameter_.defense += 10;
                }
                break;

            case YoungerSisterButtonType.MAINTENANCE:
                if (CheckEventPoint(index))
                {
                    parameter_.speed += 10;
                }
                break;

            case YoungerSisterButtonType.PRESENT:
                if (CheckEventPoint(index))
                {
                    parameter_.affection += 10;// TODO: プレゼントの内容によって変わるように
                }
                break;

            case YoungerSisterButtonType.PAMPER:
                if (CheckEventPoint(index))
                {
                    parameter_.affection += 10;
                    parameter_.attack -= 5;
                    parameter_.defense -= 5;
                    parameter_.speed -= 5;
                }
                break;

            default:
                Debug.Log("YoungerSyster::OnPressButtonEvent -> case文がdefaultです");
                return;
        }

        eventPowerText.text = eventPlayPower_.ToString() + " Pt";

        // ADV再生
        InitializeADV();
        advController_.OnPlayFinish_.AddListener(OnEventFinish);
        advController_.PlayADV("C1001_HOME_ACT_SUCCESS_" + index.ToString("000"));
    }

    private bool CheckEventPoint(int index)
    {
        if (eventPlayPower_ - buttonList_[index].EventCost >= 0)
        {
            eventPlayPower_ -= buttonList_[index].EventCost;
            return true;
        }
        return false;
    }

    public void PlayEnterADV()
    {
        InitializeADV();
        advController_.OnPlayFinish_.AddListener(OnEnterADVFinish);

        // ランダムでテキストを選ぶ
        string randID = UnityEngine.Random.Range(1, ENTER_TEXT_MAX + 1).ToString("000");
        string likeID = parameter_.GetAffectionForm().ToString() + "_";
        
        // (妹_家_開始時_ + 好感度 + id)
        advController_.PlayADV("C1001_HOME_ENTER_" + likeID + randID);

        //セリフやモーションがあればここで再生
    }

    private void OnEnterADVFinish()
    {
        foreach (YoungerEventButton button in buttonList_)
        {
            button.gameObject.SetActive(true);
        }

        gaugeList_.SetActive(true);
        eventPower_.gameObject.SetActive(true);
    }

    private void OnEventFinish()
    {
        Debug.Log($"OnEventFinish:(eventPlayPower_) {eventPlayPower_}");

        // パラメータの反映
        gaugeList_.SetParameter(parameter_);

        // イベント終了
        if (eventPlayPower_ <= 0)
        {
            Debug.LogWarning("HouseYoungerSister::今回の育成は終わりです。");
            // 次フレームで OnSleepEvent を呼ぶ（再入を避ける）
            StartCoroutine(CallOnSleepEventNextFrame());

            return;
        }

        // まだ育成できる
        OnEnterADVFinish();
    }

    public void PlayTouchBodyADV()
    {
        bool isDead = (parameter_.GetAffectionRate() <= 0.01f);

        InitializeADV();
        advController_.OnPlayFinish_.AddListener(OnEnterADVFinish);

        // タップされたらセリフやモーションがあればここで再生
        advController_.PlayADV("C1001_HOME_TOUCH_001");
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

    private IEnumerator CallOnSleepEventNextFrame()
    {
        // 再生できるようになるまで待つ
        while (advController_.CanPlay() == false)
        {
            yield return null;
        }

        OnSleepEvent();
        yield return null;
    }

    public void OnSleepEvent()
    {
        Debug.Log("OnSleepEvent");
        advController_.StopADV();
        
        // adv再生後にシーン遷移
        InitializeADV();
        advController_.OnPlayFinish_.AddListener(OnFinishedSleepADV);
        advController_.PlayADV("C1001_HOME_SLEEP_01");
    }

    private void OnFinishedSleepADV()
    {
        Debug.Log("OnFinishedSleepADV");
        StartFadeTask();
    }

    // ADVSceneLoadManagerから移動済
    private async void StartFadeTask()
    {
        await GameManager.instance.Fade.FadeOut(1.0f, fadeImage);
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
        
        Debug.Log("FadeTask Excute");

        // シーン移動
        ExcuteSleep().Forget();
    }

    //時間の変更、プレイヤーの配置をする処理
    private async UniTaskVoid ExcuteSleep()
    {
        //時間を「朝」に変更
        if (GameManager.instance != null)
        {
            GameManager.instance.currentPhase = DayPhase.Morning;
        }

        //シーン遷移
        if (SceneLoader.instance != null)
        {
            await SceneLoader.instance.ExcuteSceneTransition(nextSceneName_, homeMarkerId_, PlayerMove.instance);
        }
    }

    private void SetTutorialWait()
    {
        needStartTutorial_ = true;
        state_ = State.IDLE;
    }

    public void OpenTutorial()
    {
        isTutorial_ = true;
        tutorial_.SetActive(true);

        eventPower_.gameObject.SetActive(false);
        gaugeList_.SetActive(false);

        InitializeADV();
        Debug.Log("HYS::OpenTutorial->AddListener");
        advController_.OnPlayFinish_.AddListener(CloseTutorial);
        advController_.PlayADVFast("TUTORIAL_001");
    }

    private void CloseTutorial()
    {
        Debug.Log("HYS::CloseTutorial");

        isTutorial_ = false;
        tutorial_.SetActive(false);
        advController_.StopADV();
        OnEnterADVFinish();
    }

    public void OnInputTutorialFinish(InputAction.CallbackContext context)
    {
        if (isTutorial_ && context.started)
        {
            //CloseTutorial();
        }
    }
}
