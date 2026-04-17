using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [Header("MenuUIの参照")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private List<TMP_Text> menuTexts = new List<TMP_Text>();
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    [Header("各コマンド参照")]
    [SerializeField] private StatusWindowManager statusWindowManager;
    [SerializeField] private ItemWindowManager itemWindowManager;
    [SerializeField] private QuestWindowManager questWindowManager;

    private bool isSubMenuOpen = false;

    private int currentIndex = 0;
    private bool ishandlinginput = false;

    private enum MenuState { Main,Item,Quest,Status }
    private MenuState currentState = MenuState.Main;

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
        currentState = MenuState.Status;
        Debug.Log("ステータス表示");
        isSubMenuOpen = true;
        statusWindowManager.OpenStatus(GameManager.instance.playerData);
    }

    //アイテムウィンドウを開く処理
    private void OpenItem()
    {
        currentState = MenuState.Item;
        Debug.Log("アイテム表示");
        isSubMenuOpen = true;
        itemWindowManager.OpenItemMenu();
    }

    //依頼ウィンドウを開く処理
    private void OpenQuest()
    {
        currentState = MenuState.Quest;
        Debug.Log("依頼表示");
        isSubMenuOpen = true;
        questWindowManager.OpenQuest();
    }
    //項目移動のイベント処理
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!ishandlinginput  || !context.performed) return;

        //状況に応じて入力を振り分ける
        switch (currentState)
        {
            case MenuState.Main:
                if (isSubMenuOpen) return;
                Vector2 navigationInput = context.ReadValue<Vector2>();
                if (Mathf.Abs(navigationInput.y) > 0.5)
                {
                    int direction = navigationInput.y > 0 ? -1 : 1;
                    MoveSelection(direction);
                }
                break;
            case MenuState.Item:
                itemWindowManager.OnNavigate(context);
                break;
        }
    }

    //決定のイベント処理
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!ishandlinginput || !context.performed) return;

        //状況に応じて入力を振り分ける
        switch (currentState)
        {
            case MenuState.Main:
                if (isSubMenuOpen) return;
                ExcuteSelection();
                break;
            case MenuState.Item:
                itemWindowManager.OnSubmit(context);
                break;
        }
    }

    //キャンセルのイベント処理
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!ishandlinginput || !context.performed) return;
        
        if (isSubMenuOpen)
        {
            currentState = MenuState.Main;
            //どのサブウィンドウも閉じる
            statusWindowManager.CloseStatus();
            itemWindowManager.CloseItemMenu();
            questWindowManager.CloseQuest();

            isSubMenuOpen = false;
            UpdateMenuUI();
        }
        else
        {
            GlobalUIManager.instance.ToggleMenu(false);
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
                OpenQuest();
                break;
            case 4:
                Debug.Log("タイトルに戻る");
                break;
        }
    }
}
