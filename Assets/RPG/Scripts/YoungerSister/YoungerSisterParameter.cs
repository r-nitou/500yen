using UnityEngine;

public enum YoungerSisterForm
{
	Normal = 0,	//通常
	High_3,		//最高
	High_2,		//とても高い
	High_1,		//少し高い
	Low_1,		//低い
	Low_2,		//最低
}

[CreateAssetMenu(fileName = "YoungerSisterParameter", menuName = "Scriptable Objects/YoungerSisterParameter")]
public class YoungerSisterParameter : ScriptableObject
{
    // パラメータの上下限
    public const int PARAM_MAX = 100;
    public const int PARAM_MIN = 0;

    [Header("基本ステータス ---")]
    [Range(PARAM_MIN, PARAM_MAX)]
    public int affection; // 好感度
    [Range(PARAM_MIN, PARAM_MAX)]
    public int attack;    // 攻撃力
    [Range(PARAM_MIN, PARAM_MAX)]
    public int defense;   // 防御力
    [Range(PARAM_MIN, PARAM_MAX)]
    public int speed;     // 素早さ

    public void ResetParam()
    {
        affection = 50;
        attack = 30;
        defense = 30;
        speed = 30;
    }

    public float GetAffectionRate() { return (float)(affection) / (PARAM_MAX); }
    public float GetAttackRate() { return ((float)(attack) / PARAM_MAX) * GetAffectionRate(); }
    public float GetDefenseRate() { return ((float)(defense) / PARAM_MAX) * GetAffectionRate(); }
    public float GetSpeedRate() { return ((float)(speed) / PARAM_MAX) * GetAffectionRate(); }

    public void PrintLog() { Debug.Log($"--- 妹パラメーター ---\n" + $"好感度: {affection}, 攻撃力: {attack}, 防御力: {defense}, 素早さ: {speed}"); }

    //好感度による加算倍率の計算
    public float GetAffectionMultiplier()
    {
        if (affection <= 10) return 0.5f;
        if (affection <= 30) return 0.75f;
        if (affection <= 50) return 1.0f;
        if (affection <= 70) return 1.25f;
        return 1.5f;
    }

    //パラメータの最終ボーナス値
    public int GetBonusAttack() => Mathf.FloorToInt(attack * GetAffectionMultiplier());
    public int GetBonusDefense() => Mathf.FloorToInt(defense * GetAffectionMultiplier());
    public int GetBonusSpeed() => Mathf.FloorToInt(speed * GetAffectionMultiplier());

	// 好感度enumの獲得
	public YoungerSisterForm GetAffectionForm()
	{
		float rate = GetAffectionRate();
		if (rate > 0.94f) return YoungerSisterForm.High_3;
		if (rate > 0.74f) return YoungerSisterForm.High_2;
		if (rate > 0.60f) return YoungerSisterForm.High_1;
		if (rate > 0.36f) return YoungerSisterForm.Normal;
		if (rate > 0.16f) return YoungerSisterForm.Low_1;
		
		return YoungerSisterForm.Low_2;
	}
}
