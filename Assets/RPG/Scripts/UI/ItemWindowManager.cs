using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemWindowManager : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private GameObject itemMenu;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private GameObject itemSlotPrefab;

    [Header("サブウィンドウ参照")]
    [SerializeField] private ItemConfirmUI confirmUI;
    [SerializeField] private ItemResultUI resultUI;

    private List<ItemSlotUI> uiSlots = new List<ItemSlotUI>();
    private int currentIndex = 0;

    //サブウィンドウが開いているかどうか
    private bool IsAnySubWindow => (confirmUI != null && confirmUI.gameObject.activeSelf ||
                                  resultUI != null && resultUI.gameObject.activeSelf);

    //アイテムメニューの移動入力
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (IsAnySubWindow)
        {
            if (confirmUI.gameObject.activeSelf) confirmUI.OnNavigate(context);
            return;
        }

        //アイテムリストの移動処理
        if (uiSlots.Count <= 1) return;
        Vector2 input = context.ReadValue<Vector2>();
        if (Mathf.Abs(input.y) > 0.5f)
        {
            int direction = input.y > 0 ? -1 : 1;
            currentIndex = (currentIndex + direction + uiSlots.Count) % uiSlots.Count;
            UpdateSelection();
        }
    }

    //アイテムメニューの決定処理
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (IsAnySubWindow)
        {
            if (confirmUI.gameObject.activeSelf) confirmUI.OnSubmit(context);
            else if (resultUI.gameObject.activeSelf) resultUI.OnSubmit(context);
            return;
        }

        if (uiSlots.Count == 0) return;
        ExcuteItemSelect();
    }

    //アイテムメニューを開く処理
    public void OpenItemMenu()
    {
        itemMenu.SetActive(true);
        currentIndex = 0;
        RefreshItemList();
    }

    //アイテムメニューを閉じる処理
    public void CloseItemMenu()
    {
        itemMenu.SetActive(false);
    }

    //アイテムリストを更新する処理
    public void RefreshItemList()
    {
        //既存のUIスロットを削除
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }
        uiSlots.Clear();

        var inventory = GameManager.instance.inventoryManager.PlayerInventory;

        if (inventory.Count == 0)
        {
            descriptionText.text = "";
            return;
        }

        //インデックスが範囲外にならないように調整
        if (currentIndex >= inventory.Count)
        {
            currentIndex = Mathf.Max(0, inventory.Count - 1);
        }

        //アイテムリストからUIスロットを生成
        foreach (var slot in inventory)
        {
            GameObject obj = Instantiate(itemSlotPrefab, contentRoot);
            ItemSlotUI slotUI=obj.GetComponent<ItemSlotUI>();
            slotUI.SetItem(slot);
            uiSlots.Add(slotUI);
        }

        //表示を更新
        UpdateSelection();
    }

    //アイテム選択時の処理
    private void ExcuteItemSelect()
    {
        var inventory = InventoryManager.instance.PlayerInventory;
        if (currentIndex >= inventory.Count) return;

        var selectSlot = inventory[currentIndex];
        //消耗品以外は何もしない
        if (selectSlot.item.itemtype != ItemType.Consimable) return;

        //確認ウィンドウを開く
        confirmUI.Open(selectSlot, (isYes) =>
        {
            if (isYes)
            {
                //アイテム使用
                RequestUseItem(selectSlot);
            }
        });
    }

    //アイテムの使用をリクエストする処理
    private void RequestUseItem(ItemSlot slot)
    {
        bool succes = InventoryManager.instance.UseItem(slot.item);
        if (succes)
        {
            string resultMessage = $"{slot.item.healAmount}回復した!";
            resultUI.ShowResult(resultMessage, () =>
            {
                RefreshItemList();
            });
        }
        else
        {
            string failMessage = "今は使えない...";
            resultUI.ShowResult(failMessage, null);
        }
    }

    private void UpdateSelection()
    {
        if (uiSlots.Count == 0)
        {
            return;
        }

        for(int i = 0; i < uiSlots.Count; i++)
        {
            uiSlots[i].SetSelection(i == currentIndex);
        }

        //選択中のアイテムの説明を表示
        var selectedItem = GameManager.instance.inventoryManager.PlayerInventory[currentIndex].item;
        descriptionText.text = selectedItem.description;
    }
}
