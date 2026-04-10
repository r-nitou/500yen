using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] public TMP_Text nameText;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    //アイテムスロットのデータを設定する処理
    public void SetItem(ItemSlot slot)
    {
        iconImage.sprite = slot.item.icon;
        nameText.text = slot.item.itemName;
        countText.text = $"×{slot.count}";
    }

    public void SetSelection(bool isSelected)
    {
        nameText.color = isSelected ? selectedColor : defaultColor;
    }
}
