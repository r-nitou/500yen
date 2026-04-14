using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utage;

public class ShopManager : MonoBehaviour
{
    private const int MAX_BUY_ITEMS = 99;

    [Header("スクリプトの参照")]
    [SerializeField] private ADVPlayer advPlayer;
    [Header("販売アイテムリスト")]
    [SerializeField] private ShopInventory shopInventory;
    [Header("UI設定")]
    [SerializeField] private GameObject shopCommandRoot;
    [SerializeField] private GameObject buyItemCommandRoot;
    [SerializeField] private GameObject amountMenuRoot;
 
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject shopItemSlotPrefab;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text ownedCountText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text totalPriceText;
    [SerializeField] private TMP_Text yesButton;

    private int maxAmount = MAX_BUY_ITEMS;
    private ItemData selectedItemData;
    private int currentAmount = 1;
    private bool isSellMode = false;

    public bool IsShopUIActive => shopCommandRoot.activeSelf || buyItemCommandRoot.activeSelf;

    //道具屋の各種コマンド表示
    public void OpenShopCommand()
    {
        //ボタン表示・所持金更新
        shopCommandRoot.SetActive(true);
        buyItemCommandRoot.SetActive(false);
        amountMenuRoot.SetActive(false);
        UpdateGoldUI();
    }

    //「購入」ボタンクリック時の処理
    public void OnClickBuy()
    {
        //各種UI表示・非表示
        shopCommandRoot.SetActive(false);
        buyItemCommandRoot.SetActive(true);
        //商品リストを生成
        RefreshShopList();
    }
    //「売却」ボタンクリック時の処理
    public void OnclickSell()
    {
        isSellMode = true;
        shopCommandRoot.SetActive(false);
        buyItemCommandRoot.SetActive(true);
        RefreshSellList();
    }
    //「店を出る」ボタンクリック時の処理
    public void OnClickExit()
    {
        //ボタン非表示
        shopCommandRoot.SetActive(false);

        advPlayer.advEngine.JumpScenario("C2002_SHOP_EXIT_001");
        Debug.Log("道具屋を出ます");
    }

    //「購入」メニューを閉じる処理
    public void OnClickCloseBuyMenu()
    {
        shopCommandRoot.SetActive(true);
        buyItemCommandRoot.SetActive(false);
        amountMenuRoot.SetActive(false);
        isSellMode = false;
    }

    //アイテムスロット選択時処理
    public void OnClickItemSlot(ItemData item)
    {
        SelectItem(item);
        OpenAmountMenu();
    }

    //商品リストを生成する処理
    public void RefreshShopList()
    {
        //既存のスロットの破棄
        foreach(Transform item in itemContainer)
        {
            Destroy(item.gameObject);
        }
        //リストの生成
        foreach(ItemData item in shopInventory.itemList)
        {
            GameObject slotObj = Instantiate(shopItemSlotPrefab, itemContainer);
            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();
            slot.SetUp(item);
        }

        //リストが空でなければ、1番上を選択
        if (shopInventory.itemList.Count > 0)
        {
            SelectItem(shopInventory.itemList[0]);
        }
    }
    //売却リストを生成する処理
    public void RefreshSellList()
    {
        foreach(Transform item in itemContainer)
        {
            Destroy(item.gameObject);
        }

        ItemData firstItem = null;
        int sellableCount = 0;
        //自分のインベントリからリストを生成
        foreach(ItemSlot slot in InventoryManager.instance.PlayerInventory)
        {
            //「大事なもの」以外を表示する
            if (slot.item.itemtype != ItemType.Impotant)
            {
                GameObject slotObj = Instantiate(shopItemSlotPrefab, itemContainer);
                ShopItemSlot slotUI = slotObj.GetComponent<ShopItemSlot>();
                slotUI.SetUp(slot.item,true);

                if (firstItem == null)
                {
                    firstItem = slot.item;
                }

                sellableCount++;
            }
        }

        //売却可能アイテムの一番上を表示
        if (sellableCount > 0) 
        {
            SelectItem(firstItem);
        }
        else
        {
            ClearItemText();
        }
    }

    //アイテムを選択したときに呼ばれる処理
    public void SelectItem(ItemData item)
    {
        selectedItemData = item;

        descriptionText.text = item.description;
        if (InventoryManager.instance != null)
        {
            int count = InventoryManager.instance.GetItemCount(item);
            ownedCountText.text = $"所持数 : {count}";
        }
    }

    //購入数選択メニュー系処理
    public void OpenAmountMenu()
    {
        currentAmount = 1;
        amountMenuRoot.SetActive(true);
        maxAmount = isSellMode ?
            InventoryManager.instance.GetItemCount(selectedItemData)
            : MAX_BUY_ITEMS;

        UpdateAmountUI();
    }
    public void OnClickChangeAmount(int amount)
    {
        currentAmount = Mathf.Clamp(currentAmount + amount, 1, maxAmount);
        UpdateAmountUI();
    }
    public void UpdateAmountUI()
    {
        amountText.text = currentAmount.ToString();

        //モードに応じて単価を切り替える
        int unitPrice = isSellMode ? selectedItemData.sellPrice : selectedItemData.buyPrice;
        int totalPrice = unitPrice * currentAmount;
        //モードに応じて表示を切り替える
        string priceLabel = isSellMode ? "売却額" : "購入額";
        string buttonText = isSellMode ? "売却する" : "購入する";
        totalPriceText.text = $"{priceLabel} : {totalPrice} G";
        yesButton.text = $"{buttonText}";
    }
    public void OnClickCancelAmount()
    {
        amountMenuRoot.SetActive(false);
    }
    public void ConfirmPurchase()
    {
        if (selectedItemData == null) return;
        if (isSellMode)
        {
            //金額計算
            int totalGain = selectedItemData.sellPrice * currentAmount;

            //インベントリーから削除
            if (InventoryManager.instance.RemoveItem(selectedItemData, currentAmount))
            {
                GameManager.instance.gold += totalGain;
            }
        }
        else
        {
            //金額計算
            int totalCost = selectedItemData.buyPrice * currentAmount;

            if (GameManager.instance.gold >= totalCost)
            {
                GameManager.instance.gold -= totalCost;
                InventoryManager.instance.AddItem(selectedItemData, currentAmount);
            }
        }
        //表示更新
        UpdateGoldUI();
        SelectItem(selectedItemData);
        amountMenuRoot.SetActive(false);
        RefreshSellList();
    }

    //所持金を更新する処理
    public void UpdateGoldUI()
    {
        goldText.text = $"{GameManager.instance.gold} G";
    }

    //テキスト類を空にする処理
    private void ClearItemText()
    {
        selectedItemData = null;
        descriptionText.text = "売却できるアイテムはありません";
        ownedCountText.text = "";
    }
}
