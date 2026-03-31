using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HouseYoungerSister: MonoBehaviour
{
    [SerializeField, Header("ADV")]
    private AdvController advController_ = null;

    [SerializeField, Header("表示を切り替えるボタンのリスト")]
    private Button[] buttonList_ = null;

    private bool needInvoke_ = true;


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
            advController_.PlayADV("H_001_01");
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

        string id = index.ToString("00");
        advController_.PlayADV("H_101_" + id);
    }

    private void OnEnterADVFinish()
    {
        foreach (Button button in buttonList_)
        {
            button.gameObject.SetActive(true);
        }
    }
}
