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

    private List<ItemSlotUI> uiSlots = new List<ItemSlotUI>();
    private int currentIndex = 0;

    //アイテムメニューの移動入力
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed || uiSlots.Count <= 1) return;

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
        if (!context.performed || uiSlots.Count == 0) return;

        Debug.Log($"{uiSlots[currentIndex].nameText}を使用します");
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
