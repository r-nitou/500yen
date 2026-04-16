using TMPro;
using UnityEngine;

public class BattleItemSlotUI : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text countText;

    [Header("色設定")]
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    public ItemSlot CurrentSlot { get; set; }

    //プレハブの情報の初期化
    public void SetItem(ItemSlot slot)
    {
        CurrentSlot = slot;

        if (slot != null && slot.item != null)
        {
            nameText.text = slot.item.itemName;
            countText.text = $"×{slot.count}";
        }
        else
        {
            nameText.text = "---";
            countText.text = "";
        }
    }

    //見た目を変える
    public void SetSelection(bool isSelected)
    {
        nameText.color = isSelected ? selectedColor : defaultColor;
    }
}
