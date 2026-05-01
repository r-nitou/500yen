using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;

public class EquipmentWindowManager : MonoBehaviour
{
    [Header("各UIの親")]
    [SerializeField] private GameObject equipmentWindowRoot;
    [SerializeField] private GameObject itemSelectionRoot;
    [SerializeField] private GameObject detailItemWindowRoot;
    [Header("現在の装備枠")]
    [SerializeField] private TMP_Text[] partTexts;
    [SerializeField] private TMP_Text[] currentEquipmentName;
    [Header("装備詳細")]
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statusComparisonText;
    [Header("プレハブ設定")]
    [SerializeField] private GameObject equipmentSlotPrefab;
    [SerializeField] private GameObject unEquipmentSlotPrefab;
    [SerializeField] private Transform contentRoot;
    [Header("色の設定")]
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    private enum EquipState
    {
        SelectPart,
        SelectItem
    }
    private EquipState currentState = EquipState.SelectPart;

    private int partIndex = 0;
    private int itemIndex = 0;
    public List<EquipmentSlotUI> uiSlots = new List<EquipmentSlotUI>();
    private List<ItemSlot> filteredItems = new List<ItemSlot>();

    private ItemType currentTargetItemType;

    //移動入力処理
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Vector2 input = context.ReadValue<Vector2>();

        if (currentState == EquipState.SelectPart) 
        {
            if (Mathf.Abs(input.y) > 0.5f)
            {
                partIndex = (partIndex + (input.y > 0 ? -1 : 1) + partTexts.Length) % partTexts.Length;
                UpdatePartSelection();
            }
        }
        else if (currentState == EquipState.SelectItem)
        {
            if (Mathf.Abs(input.y) > 0.5f)
            {
                if (uiSlots.Count < 0) return;
                itemIndex = (itemIndex + (input.y > 0 ? 1 : -1) + uiSlots.Count) % uiSlots.Count;
                UpdateItemSelection();
            }
        }
    }

    //決定入力処理
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentState == EquipState.SelectPart)
        {
            OpenItemSelection();
        }
        else if (currentState == EquipState.SelectItem)
        {
            EquipmentSelectItem();
        }
    }

    //キャンセル入力処理
    public bool OnCancel()
    {
        if (currentState == EquipState.SelectItem) 
        {
            //アイテム選択をキャンセルして部位選択に戻る
            itemSelectionRoot.SetActive(false);
            detailItemWindowRoot.SetActive(false);
            currentState = EquipState.SelectPart;
            UpdatePartSelection();
            return false;
        }
        else
        {
            CloseEquipment();
            return true;
        }
    }

    //装備メニューを開く処理
    public void OpenEquipment()
    {
        //設定の初期化
        equipmentWindowRoot.SetActive(true);
        itemSelectionRoot.SetActive(false);
        detailItemWindowRoot.SetActive(false);
        currentState = EquipState.SelectPart;
        partIndex = 0;

        //現在の装備を表示
        RefreshCurrntEquipment();
        UpdatePartSelection();
    }

    //装備メニューを閉じる処理
    public void CloseEquipment()
    {
        equipmentWindowRoot.SetActive(false);
    }

    //現在の装備を表示する処理
    private void RefreshCurrntEquipment()
    {
        //現在の武器、防具、アクセサリーをプレイヤーデータから引っ張ってくる
        PlayerData playerData = GameManager.instance.playerData;
        currentEquipmentName[0].text = playerData.equippedWeapon != null ? playerData.equippedWeapon.itemName : "なし";
        currentEquipmentName[1].text = playerData.equippedArmor != null ? playerData.equippedArmor.itemName : "なし";
        currentEquipmentName[2].text = playerData.equippedAccessory != null ? playerData.equippedAccessory.itemName : "なし";
    }

    //装備リスト生成処理
    private void OpenItemSelection()
    {
        currentState = EquipState.SelectItem;
        itemSelectionRoot.SetActive(true);
        detailItemWindowRoot.SetActive(true);
        itemIndex = 0;

        //対象の装備タイプを決定
        currentTargetItemType = partIndex == 0 ? ItemType.Weapon :
                              (partIndex == 1 ? ItemType.Armor : ItemType.Accessory);

        //インベントリーから対象のタイプを検索
        filteredItems = InventoryManager.instance.PlayerInventory
                          .Where(slot => slot.item.itemtype == currentTargetItemType).ToList();

        //UIリスト生成
        //既存のリスト削除
        foreach (Transform child in contentRoot) 
        {
            Destroy(child.gameObject);
        }
        uiSlots.Clear();

        //装備を外すプレハブを生成
        GameObject unEquipObj = Instantiate(unEquipmentSlotPrefab, contentRoot);
        EquipmentSlotUI unequipUI = unEquipObj.GetComponent<EquipmentSlotUI>();
        //ItemDataはnull、装備中フラグはfalse
        unequipUI.SetSlot(null, false);
        uiSlots.Add(unequipUI);

        //アイテムリスト生成
        PlayerData pd = GameManager.instance.playerData;

        foreach (var slot in filteredItems) 
        {
            GameObject gameObject = Instantiate(equipmentSlotPrefab, contentRoot);
            EquipmentSlotUI slotObj = gameObject.GetComponent<EquipmentSlotUI>();

            //装備中か判定
            bool isEquipped = (slot.item == pd.equippedWeapon
                           || slot.item == pd.equippedArmor
                           || slot.item == pd.equippedAccessory);
            slotObj.SetSlot(slot.item, isEquipped);
            uiSlots.Add(slotObj);
        }
        UpdateItemSelection();
    }

    //装備実行処理
    private void EquipmentSelectItem()
    {
        if (uiSlots.Count == 0) return;
        ItemData newItem = uiSlots[itemIndex].CurrentItem;
        PlayerData pd = GameManager.instance.playerData;

        //現在装備している装備を取得
        ItemData currentEquip = (partIndex == 0) ? pd.equippedWeapon :
                                (partIndex == 1 ? pd.equippedArmor : pd.equippedAccessory);
        //「はずす」または現在装備している装備を選択した
        bool isUnEquipment = (newItem == null) || (newItem == currentEquip);

        //装備をはずす
        if (isUnEquipment)
        {
            SetEquipmentData(null);
        }
        else
        {
            SetEquipmentData(newItem);
        }

        RefreshCurrntEquipment();
        RefreshSlotEquipmentTag();
    }

    //部位に応じてデータをセットする処理
    private void SetEquipmentData(ItemData item)
    {
        PlayerData pd = GameManager.instance.playerData;
        if (partIndex == 0) pd.equippedWeapon = item;
        else if (partIndex == 1) pd.equippedArmor = item;
        else if (partIndex == 2) pd.equippedAccessory = item;
    }

    //部位選択更新処理
    public void UpdatePartSelection()
    {
        for (int i = 0; i < partTexts.Length; i++)
        {
            partTexts[i].color = (i == partIndex) ? selectedColor : defaultColor;
        }
    }
    //装備アイテム選択更新処理
    public void UpdateItemSelection()
    {
        for(int i = 0; i < uiSlots.Count; i++)
        {
            uiSlots[i].SetSelection(i == itemIndex);
        }

        if (uiSlots.Count > 0)
        {
            UpdateDetailPanel(uiSlots[itemIndex].CurrentItem);
        }
    }

    //アイテム詳細パネル更新処理
    public void UpdateDetailPanel(ItemData item)
    {
        //現在の装備を取得
        PlayerData pd = GameManager.instance.playerData;

        ItemData currentEquip = null;
        //ステータス比較表示
        string statName = "";
        int currentTotal = 0;

        switch (partIndex)
        {
            case 0:
                currentEquip = pd.equippedWeapon;
                statName = "攻撃力";
                currentTotal = pd.TotalAttack;
                break;
            case 1:
                currentEquip = pd.equippedArmor;
                statName = "防御力";
                currentTotal = pd.TotalDefense;
                break;
            case 2:
                currentEquip = pd.equippedAccessory;
                statName = "すばやさ";
                currentTotal = pd.TotalSpeed;
                break;

        }

        //今の装備による上昇値
        int currentBoost = 0;
        if (currentEquip != null)
        {
            currentBoost = (partIndex == 0) ? currentEquip.attackBoost :
                       (partIndex == 1) ? currentEquip.defenseBoost : currentEquip.speedBoost;
        }

        int afterTotal = 0;
        if (item == null || item == currentEquip) 
        {
            //はずした後のステータス計算
            afterTotal = currentTotal - currentBoost;
        }
        else
        {
            //変化量の計算と表示
            int newItemBoost = (partIndex == 0) ? item.attackBoost :
                              (partIndex == 1 ? item.defenseBoost : item.speedBoost);
            afterTotal = currentTotal - currentBoost + newItemBoost;
        }
        //装備の説明表示
        descriptionText.text = (item != null) ? item.description : "装備をはずします";
        statusComparisonText.text = $"変化量\n{statName} : {currentTotal}→{afterTotal}";
    }

    //装備中タグを更新する処理
    private void RefreshSlotEquipmentTag()
    {
        PlayerData pd = GameManager.instance.playerData;

        foreach (var slotUI in uiSlots) 
        {
            if (slotUI == null || slotUI.CurrentItem == null) continue;

            //装備中か判定
            bool isEquipped = (slotUI.CurrentItem == pd.equippedWeapon ||
                               slotUI.CurrentItem == pd.equippedArmor ||
                               slotUI.CurrentItem == pd.equippedAccessory);

            //タグの更新
            slotUI.UpdateEquipTag(isEquipped);
        }
    }
}
