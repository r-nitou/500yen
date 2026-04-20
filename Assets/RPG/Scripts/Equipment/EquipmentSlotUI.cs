using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    [Header("UIの参照")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject equipTag;

    [Header("色の設定")]
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    //このスロットが持つアイテムデータ
    public ItemData CurrentItem { get; set; }

    //アイテム情報をセットする
    public void SetSlot(ItemData item,bool isEquipment)
    {
        CurrentItem = item;
        if (item != null)
        {
            nameText.text = item.itemName;
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
            //装備中なら[E]を表示
            equipTag.SetActive(isEquipment);
        }
        else
        {
            nameText.text = "はずす";
            iconImage.enabled = false;
            equipTag.SetActive(false);
        }
    }

    //装備中タグの切り替えを行う処理
    public void UpdateEquipTag(bool isEquipment)
    {
        if (equipTag != null)
        {
            equipTag.SetActive(isEquipment);
        }
    }

    //色の切り替えを行う処理
    public void SetSelection(bool isSelected)
    {
        nameText.color = isSelected ? selectedColor : defaultColor;
    }

}
