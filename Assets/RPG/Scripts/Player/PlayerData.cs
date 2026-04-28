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

    [Header("現在の装備品")]
    public ItemData equippedWeapon;
    public ItemData equippedArmor;
    public ItemData equippedAccessory;

    [Header("ヒロインステータスボーナス")]
    public YoungerSisterParameter sisterParameter;

    [Header("成長データ")]
    public int currentExp = 0;
    public int nextLevelExp = 0;

    public int hpGain = 10;
    public int attackGain = 2;
    public int defenseGain = 1;
    public int speedGain = 1;

    //装備を含めた最終的なステータスを計算する処理
    //攻撃力
    public int TotalAttack
    {
        get
        {
            int boost = 0;
            if (equippedWeapon != null) boost += equippedWeapon.attackBoost;
            if (equippedArmor != null) boost += equippedArmor.attackBoost;
            if (equippedAccessory != null) boost += equippedAccessory.attackBoost;

            int sisterBonus = sisterParameter != null ? sisterParameter.GetBonusAttack() : 0;

            return attack + boost + sisterBonus;
        }
    }
    //防御力
    public int TotalDefense
    {
        get
        {
            int boost = 0;
            if (equippedWeapon != null) boost += equippedWeapon.defenseBoost;
            if (equippedArmor != null) boost += equippedArmor.defenseBoost;
            if (equippedAccessory != null) boost += equippedAccessory.defenseBoost;

            int sisterBonus = sisterParameter != null ? sisterParameter.GetBonusDefense() : 0;

            return defense + boost + sisterBonus;
        }
    }
    //すばやさ
    public int TotalSpeed
    {
        get
        {
            int boost = 0;
            if (equippedWeapon != null) boost += equippedWeapon.speedBoost;
            if (equippedArmor != null) boost += equippedArmor.speedBoost;
            if (equippedAccessory != null) boost += equippedAccessory.speedBoost;

            int sisterBonus = sisterParameter != null ? sisterParameter.GetBonusSpeed() : 0;

            return speed + boost + sisterBonus;
        }
    }

    //レベルアップに必要な経験値量を計算する処理
    public void CalculateNextLevelExp()
    {
        nextLevelExp = Mathf.RoundToInt(level * 100 * Mathf.Pow(1.2f, level - 1));
    }
}
