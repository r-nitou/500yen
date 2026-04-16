using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemConfirmUI : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private GameObject confirmWindow;
    [SerializeField] private TMP_Text itemNametext;
    [SerializeField] private TMP_Text yesSelect;
    [SerializeField] private TMP_Text noSelect;

    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    private Action<bool> onSelect;
    private int currentIndex = 0;
    private bool isActive = false;

    //アイテム使用確認メニューの表示処理
    public void Open(ItemSlot slot, Action<bool> callback)
    {
        confirmWindow.SetActive(true);
        itemNametext.text = $"{slot.item.itemName}\nを使用しますか?";
        onSelect = callback;
        isActive = true;

        UpdateSelectionVisual();
    }

    //移動入力を受けとる処理
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!isActive && !context.performed) return;

        Vector2 input = context.ReadValue<Vector2>();

        //左右入力でインデックスを切り替え
        if (Mathf.Abs(input.x) > 0.5f)
        {
            currentIndex = currentIndex == 0 ? 1 : 0;
            UpdateSelectionVisual();
        }
    }

    //決定入力を受け取る処理
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if(!isActive && !context.performed) return;

        isActive = false;
        confirmWindow.SetActive(false);

        //currentIncexが0ならyes、1ならnoを返す
        onSelect?.Invoke(currentIndex == 0);
    }

    //見た目を変える処理
    private void UpdateSelectionVisual()
    {
        yesSelect.color = (currentIndex == 0) ? selectedColor : defaultColor;
        noSelect.color = (currentIndex == 1) ? selectedColor : defaultColor;
    }
}
