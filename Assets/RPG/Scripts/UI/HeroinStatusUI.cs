using UnityEngine;

public class HeroinStatusUI : MonoBehaviour
{
    [Header("データ参照")]
    [SerializeField] private YoungerSisterParameter sisterParameter;

    [Header("UI参照")]
    [SerializeField] private MentalGauge affectionGauge;
    [SerializeField] private MentalGauge moodGauge;
    [SerializeField] private MentalGauge healthGauge;
    [SerializeField] private MentalGauge mentalGauge;

    private void OnEnable()
    {
        //ステータス反映
        InitializeStatus();
    }

    //ステータス反映処理
    private void InitializeStatus()
    {
        if (sisterParameter == null) return;

        //好感度
        if (affectionGauge != null)
        {
            //割合を計算
            float affectionRate = (float)sisterParameter.affection / YoungerSisterParameter.PARAM_MAX;
            affectionGauge.SetValueForce(affectionRate);
            affectionGauge.Text = "好感度";
        }

        //機嫌
        if (moodGauge != null)
        {
            //割合を計算
            float moodRate = (float)sisterParameter.attack / YoungerSisterParameter.PARAM_MAX;
            moodGauge.SetValueForce(moodRate);
            moodGauge.Text = "機嫌";
        }

        //健康
        if (healthGauge != null)
        {
            //割合を計算
            float healthRate = (float)sisterParameter.defense / YoungerSisterParameter.PARAM_MAX;
            healthGauge.SetValueForce(healthRate);
            healthGauge.Text = "健康";
        }

        //メンタル
        if (mentalGauge != null)
        {
            //割合を計算
            float mentalRate = (float)sisterParameter.speed / YoungerSisterParameter.PARAM_MAX;
            mentalGauge.SetValueForce(mentalRate);
            mentalGauge.Text = "メンタル";
        }
    }
}
