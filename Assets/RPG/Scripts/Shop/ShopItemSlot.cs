using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text priceText;

    private ItemData itemData;

    //商品データをスロットにセットする処理
    public void SetUp(ItemData data, bool isSellMode = false)
    {
        itemData = data;
        icon.sprite = data.icon;
        nameText.text = data.itemName;

        //モードに応じて表示価格を変える
        int displayPrice = isSellMode ? data.sellPrice : data.buyPrice;
        priceText.text = $"{displayPrice} G";
    }

    //ボタンが押されたときの処理
    public void OnClick()
    {
        Object.FindAnyObjectByType<ShopManager>().OnClickItemSlot(itemData);
    }
}
