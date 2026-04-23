using UnityEngine;

public enum YoungerSisterGaugeType
{
    AFFECTION = 0,  // 好感度
    ATTACK,         // 攻撃力
    DEFENSE,        // 防御力
    SPEED,          // 素早さ
}

public class YoungerSisterGaugeList : MonoBehaviour
{
    [SerializeField]
    private MentalGauge[] gaugeList_ = null;
    public MentalGauge[] GaugeList => gaugeList_;


    public MentalGauge GetGauge(YoungerSisterGaugeType type) 
    { 
        return gaugeList_[(int)type]; 
    }

    public void SetParameter(YoungerSisterParameter scrObject)
    {
        GetGauge(YoungerSisterGaugeType.AFFECTION).Value    = scrObject.GetAffectionRate();
        GetGauge(YoungerSisterGaugeType.ATTACK).Value       = scrObject.GetAttackRate();
        GetGauge(YoungerSisterGaugeType.DEFENSE).Value      = scrObject.GetDefenseRate();
        GetGauge(YoungerSisterGaugeType.SPEED).Value        = scrObject.GetSpeedRate();
    }

    // ゲージの値を即座に変更する（補間なし）
    public void SetParameterForce(YoungerSisterParameter scrObject)
    {
        GetGauge(YoungerSisterGaugeType.AFFECTION).SetValueForce(   scrObject.GetAffectionRate());
        GetGauge(YoungerSisterGaugeType.ATTACK).SetValueForce(      scrObject.GetAttackRate());
        GetGauge(YoungerSisterGaugeType.DEFENSE).SetValueForce(     scrObject.GetDefenseRate());
        GetGauge(YoungerSisterGaugeType.SPEED).SetValueForce(       scrObject.GetSpeedRate());
    }

    public void SetActive(bool active)
    {
        foreach (MentalGauge item in gaugeList_)
        {
            item.gameObject.SetActive(active);
        }
    }
}
