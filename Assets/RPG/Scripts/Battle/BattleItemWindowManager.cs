using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleItemWindowManager : MonoBehaviour
{
    private const int COLUMNS = 3;

    [Header("UI参照")]
    [SerializeField] private GameObject itemWndowRoot;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject itemSlotPrefab;

    private List<BattleItemSlotUI> uiSlot = new List<BattleItemSlotUI>();
    private int currentIndex;

    private Action<ItemData> onItemSelected;
    private Action onCancel;

    private PlayerInputAction inputAction;
    private bool isSelecting = false;

    public bool IsActive => itemWndowRoot.activeSelf;

    private void Awake()
    {
        inputAction = new PlayerInputAction();
        itemWndowRoot.SetActive(false);
    }

    //アイテムカーソル移動処理
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed || uiSlot.Count <= 1) return;

        Vector2 input = context.ReadValue<Vector2>();
        int direction = 0;

        //横移動
        if (Mathf.Abs(input.x) > 0.5f)
        {
            direction = input.x > 0 ? 1 : -1;
        }
        //縦移動
        else if (Mathf.Abs(input.y) > 0.5f)
        {
            direction = input.y > 0 ? -COLUMNS : COLUMNS;
        }

        //アイテムリストの範囲内に制限
        if (direction != 0)
        {
            //移動先の数を計算
            int nextIndex = currentIndex + direction;
            if (nextIndex >= 0 && nextIndex < uiSlot.Count)
            {
                currentIndex = nextIndex;
                UpdateSelection();
            }
        }
    }

    //決定入力
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed || uiSlot.Count == 0) return;

        //現在選択しているアイテムを渡す
        var selectedItem = uiSlot[currentIndex].CurrentSlot.item;
        ItemData data = selectedItem;
        Close();
        onItemSelected?.Invoke(data);
    }

    //キャンセル入力
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed || !isSelecting) return;
        Close();
        onCancel?.Invoke();
    }

    //ウィンドウを開く
    public void Open(Action<ItemData> onSelect,Action onCancel)
    {
        itemWndowRoot.SetActive(true);
        this.onItemSelected = onSelect;
        this.onCancel = onCancel;

        currentIndex = 0;
        isSelecting = true;
        //アイテムリストの生成
        RefreshItemList();

        //入力イベント登録
        inputAction.Battle.Enable();
        inputAction.Battle.Navigate.performed += OnNavigate;
        inputAction.Battle.Decision.performed += OnSubmit;
        inputAction.Battle.Cancel.performed += OnCancel;
    }

    //ウィンドウを閉じる
    public void Close()
    {
        //入力イベント解除
        inputAction.Battle.Enable();
        inputAction.Battle.Navigate.performed -= OnNavigate;
        inputAction.Battle.Decision.performed -= OnSubmit;
        inputAction.Battle.Cancel.performed -= OnCancel;

        isSelecting = false;
        itemWndowRoot.SetActive(false);
    }

    private void RefreshItemList()
    {
        //既存のリストの破棄
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }
        uiSlot.Clear();

        //InventoryManagerから消耗品だけ取り出す
        var inventoryManager = InventoryManager.instance.PlayerInventory;
        foreach (var slot in inventoryManager)
        {
            if (slot.item.itemtype != ItemType.Consimable) continue;

            GameObject obj = Instantiate(itemSlotPrefab, contentRoot);
            BattleItemSlotUI slotUI = obj.GetComponent<BattleItemSlotUI>();
            slotUI.SetItem(slot);
            uiSlot.Add(slotUI);
        }
        //表示更新
        UpdateSelection();
    }
    //表示を更新する処理
    private void UpdateSelection()
    {
        if (uiSlot.Count == 0) return;

        for(int i = 0; i < uiSlot.Count; i++)
        {
            uiSlot[i].SetSelection(i == currentIndex);
        }
    }
}
