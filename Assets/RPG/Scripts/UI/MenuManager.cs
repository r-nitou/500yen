using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [Header("MenuUIの参照")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private List<TMP_Text> menuTexts = new List<TMP_Text>();
    [SerializeField] private Color selectedColor=Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    [Header("各コマンド参照")]
    [SerializeField] private StatusWindowManager statusWindowManager;
    [SerializeField] private ItemWindowManager itemWindowManager;

    private bool isSubMenuOpen = false;

    private int currentIndex = 0;
    private bool ishandlinginput = false;

    //メニューを開く処理
    public void OpenMenu()
    {
        currentIndex = 0;
        menuUI.SetActive(true);
        UpdateMenuUI();
        ishandlinginput = true;
        isSubMenuOpen = false;
    }

    //メニューを閉じる処理
    public void CloseMenu()
    {
        menuUI.SetActive(false);
        ishandlinginput = false;
    }

    //ステータスウィンドウを開く処理
    private void OpenStatus()
    {
        Debug.Log("ステータス表示");
        isSubMenuOpen = true;
        statusWindowManager.OpenStatus(GameManager.instance.playerData);
    }

    //アイテムウィンドウを開く処理
    private void OpenItem()
    {
        Debug.Log("アイテム表示");
        isSubMenuOpen = true;
        itemWindowManager.OpenItemMenu();
    }

    //項目移動のイベント処理
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!ishandlinginput || isSubMenuOpen) return;
        
        if (context.performed)
        {
            Vector2 navigationInput = context.ReadValue<Vector2>();
            if (Mathf.Abs(navigationInput.y) > 0.5)
            {
                int direction = navigationInput.y > 0 ? -1 : 1;
                MoveSelection(direction);
            }
        }
    }

    //決定のイベント処理
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!ishandlinginput || isSubMenuOpen || !context.performed) return;
        ExcuteSelection();
    }

    //キャンセルのイベント処理
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!ishandlinginput || !context.performed) return;

        if (isSubMenuOpen)
        {
            //どのサブウィンドウも閉じる
            statusWindowManager.CloseStatus();
            itemWindowManager.CloseItemMenu();

            isSubMenuOpen = false;
            UpdateMenuUI();
        }
        else
        {
            UImanager.instance.CloseMenu();
        }
    }

    //カーソル移動処理
    private void MoveSelection(int direction)
    {
        currentIndex = (currentIndex + direction + menuTexts.Count) % menuTexts.Count;
        UpdateMenuUI();
    }

    //選択中のメニューをわかりやすくする処理
    private void UpdateMenuUI()
    {
        for(int i = 0; i < menuTexts.Count; i++)
        {
            menuTexts[i].color = (i == currentIndex) ? selectedColor : defaultColor;
        }
    }

    //選択したメニューに応じた処理を実行する処理
    private void ExcuteSelection()
    {
        switch (currentIndex)
        {
            case 0:
                OpenStatus();
                break;
            case 1:
                Debug.Log("装備表示");
                break;
            case 2:
                OpenItem();
                break;
            case 3:
                Debug.Log("タイトルに戻る");
                break;
        }
    }
}
