using UnityEngine;

public enum  ItemType
{
    Consimable,
    Weapon,
    Armor,
    Accessory,
    Impotant
}

//アイテムスロットのデータクラス
[System.Serializable]
public class ItemSlot
{
    public ItemData item;
    public int count;

    //コンストラクタ
    public ItemSlot(ItemData item, int count)
    {
        this.item = item;
        this.count = count;
    }
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("基本情報")]
    public string itemName;
    [TextArea]
    public string description;
    public ItemType itemtype;
    public Sprite icon;

    [Header("ショップ設定")]
    public int buyPrice;
    public int sellPrice;

    [Header("装備品設定")]
    public int attackBoost;
    public int defenseBoost;
    public int speedBoost;

    [Header("効果量")]
    public int healAmount;
}
