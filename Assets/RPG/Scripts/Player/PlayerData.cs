using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("基本プロフィール")]
    public string playerName;
    public int level = 1;

    [Header("ベースステータス")]
    public int maxHP = 100;
    public int attack = 20;
    public int defense = 10;
    public int speed = 5;

    [Header("成長データ")]
    public int currentExp = 0;
    public int nextLevelExp = 0;

    public int hpGain = 10;
    public int attackGain = 2;
    public int defenseGain = 1;
    public int sppedGain = 1;

    //レベルアップに必要な経験値量を計算する処理
    public void CalculateNextLevelExp()
    {
        nextLevelExp = Mathf.RoundToInt(level * 100 * Mathf.Pow(1.2f, level - 1));
    }
}
