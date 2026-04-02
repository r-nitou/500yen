using UnityEngine;

[CreateAssetMenu(fileName = "YoungerSisterParameter", menuName = "Scriptable Objects/YoungerSisterParameter")]
public class YoungerSisterParameter : ScriptableObject
{
    // パラメータの上下限
    public const int PARAM_MAX = 100;
    public const int PARAM_MIN = -100;

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
        affection = 0;
        attack = 0;
        defense = 0;
        speed = 0;
    }

    public float GetAffectionRate() { return (float)(affection) / (PARAM_MAX); }
    public float GetAttackeRate() { return (float)(attack) / (PARAM_MAX); }
    public float GetDefenceRate() { return (float)(defense) / (PARAM_MAX); }
    public float GetSpeedRate() { return (float)(speed) / (PARAM_MAX); }

    public void PrintLog() { Debug.Log($"--- 妹パラメーター ---\n" + $"好感度: {affection}, 攻撃力: {attack}, 防御力: {defense}, 素早さ: {speed}"); }
}
