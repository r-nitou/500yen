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
    [SerializeField, Header("ADV")]
    private AdvController advController_ = null;

    [SerializeField, Header("パラメータのスクリプタブobj")]
    private YoungerSisterParameter parameter_ = null;

    [SerializeField, Header("表示を切り替えるボタンのリスト")]
    private Button[] buttonList_ = null;

    private int eventPlayPower_ = 30;

    private bool needStartEnter_ = true;

    private bool isFirstEvent_ = false;

    // 開始時セリフのパターン数
    private const int ENTER_TEXT_MAX = 4;


    private void Awake()
    {
        foreach (Button button in buttonList_)
        {
            button.gameObject.SetActive(false);
        }

        // TODO: セーブデータ参照して
        isFirstEvent_ = false;
    }

    private void Update()
    {
        // Startでは再生できなかったのでここで呼ぶ
        if (advController_.CanPlay() && needStartEnter_)
        {
            needStartEnter_ = false;

            // 初回イベント
            if (isFirstEvent_)
            {
                isFirstEvent_ = false;

                advController_.OnPlayFinish_.AddListener(OnEnterADVFinish);
                advController_.PlayADV("TODO: 初回限定の特殊セリフID");
                
                return;
            }

            // 通常イベント
            parameter_.PrintLog();
            advController_.OnPlayFinish_.AddListener(OnEnterADVFinish);
            PlayEnterADV();
        }
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
        foreach (Button button in buttonList_)
        {
            button.gameObject.SetActive(false);
        }
        parameter_.PrintLog();
        advController_.OnPlayFinish_.AddListener(OnEventFinish);
        advController_.PlayADV("C1001_HOME_ACT_SUCCESS_" + index.ToString("000"));
    }

    private void OnEnterADVFinish()
    {
        foreach (Button button in buttonList_)
        {
            button.gameObject.SetActive(true);
        }
    }

    private void PlayEnterADV()
    {
        // ランダムでテキストを選ぶ
        string randID = UnityEngine.Random.Range(1, ENTER_TEXT_MAX + 1).ToString("000");
        string likeID = "H_";
        
        // (妹_家_開始時_ + 好感度 + id)
        advController_.PlayADV("C1001_HOME_ENTER_" + likeID + randID);

        //セリフやモーションがあればここで再生
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
}
