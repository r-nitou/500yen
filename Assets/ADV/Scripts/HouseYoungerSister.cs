using UnityEngine;
using UnityEngine.UI;

public class HouseYoungerSister: MonoBehaviour
{
    [SerializeField, Header("ADV")]
    private AdvController advController_ = null;

    [SerializeField, Header("表示を切り替えるボタンのリスト")]
    private Button[] buttonList_ = null;

    private bool needInvoke_ = true;

    // 開始時セリフのパターン数
    private const int ENTER_TEXT_MAX = 4;


    private void Awake()
    {
        foreach (Button button in buttonList_)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Startでは再生できなかったのでここで呼ぶ
        if (advController_.CanPlay() && needInvoke_)
        {
            needInvoke_ = false;
            advController_.OnPlayFinish_.AddListener(OnEnterADVFinish);
            PlayEnterADV();
        }
    }

    public void OnPressButtonEvent(int index)
    {
        switch (index)
        {
            // 機嫌
            case 1:
                break;

            // 健康
            case 2:
                break;

            // メンタル
            case 3:
                break;

            default:
                Debug.Log("YoungerSyster::OnPressButtonEvent -> case文がdefaultです");
                return;
        }

        advController_.PlayADV("C1001_HOME_ENTER_H_001");
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
        string randID = Random.Range(1, ENTER_TEXT_MAX + 1).ToString("000");
        string likeID = "H_";
        
        // (妹_家_開始時_ + 好感度 + id)
        advController_.PlayADV("C1001_HOME_ENTER_" + likeID + randID);

        //セリフやモーションがあればここで再生
    }
}
