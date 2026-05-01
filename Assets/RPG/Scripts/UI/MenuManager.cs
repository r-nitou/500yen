using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private EquipmentWindowManager equipmentWindowManager;
    [SerializeField] private QuestWindowManager questWindowManager;

    [Header("演出設定")]
    [SerializeField] private Image fadeTarget;
    [SerializeField] private float fadeDuration;
    [SerializeField] private string targetScene;

    private bool isSubMenuOpen = false;

    private int currentIndex = 0;
    private bool ishandlinginput = false;

    public enum MenuState { Main,Item,Quest,Status,Equip }
    public MenuState currentState = MenuState.Main;

    //メニューを開く処理
    public void OpenMenu()
    {
        currentIndex = 0;
        menuUI.SetActive(true);
        UpdateMenuUI();
        ishandlinginput = true;
        isSubMenuOpen = false;
        currentState = MenuState.Main;
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

    private void OpenEquip()
    {
        currentState = MenuState.Equip;
        isSubMenuOpen = true;
        equipmentWindowManager.OpenEquipment();
    }

    //依頼ウィンドウを開く処理
    private void OpenQuest()
    {
        currentState = MenuState.Quest;
        Debug.Log("依頼表示");
        isSubMenuOpen = true;
        questWindowManager.OpenQuest();
    }

    //タイトルに戻る実行処理
    private async void ExcuteReturnTitleCommand()
    {
        //セーブ実行
        SaveManager.instance.Save();

        if (fadeTarget != null)
        {
            //フェードアウト
            await GameManager.instance.Fade.FadeOut(fadeDuration, fadeTarget);
        }

        //メニューを閉じる
        if (GlobalUIManager.instance != null)
        {
            GlobalUIManager.instance.ToggleMenu(false);
        }
        //タイトルシーンの読み込み
        await SceneLoader.instance.ExcuteSceneTransition(targetScene, "", PlayerMove.instance);

        //フェード対象を元に戻す
        if (fadeTarget != null)
        {
            await GameManager.instance.Fade.FadeIn(fadeDuration, fadeTarget);
            fadeTarget.gameObject.SetActive(false);
        }
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
            case MenuState.Equip:
                equipmentWindowManager.OnNavigate(context);
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
            case MenuState.Equip:
                equipmentWindowManager.OnSubmit(context);
                break;
        }
    }

    //キャンセルのイベント処理
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!ishandlinginput || !context.performed) return;
        
        if (isSubMenuOpen)
        {
            if (currentState == MenuState.Equip)
            {
                bool returnToMain = equipmentWindowManager.OnCancel();
                if (returnToMain)
                {
                    currentState = MenuState.Main;
                    isSubMenuOpen = false;
                    UpdateMenuUI();
                }
            }
            else
            {
                //どのサブウィンドウも閉じる
                statusWindowManager.CloseStatus();
                itemWindowManager.CloseItemMenu();
                equipmentWindowManager.CloseEquipment();
                questWindowManager.CloseQuest();

                currentState = MenuState.Main;
                isSubMenuOpen = false;
                UpdateMenuUI();
            }
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
                OpenEquip();
                break;
            case 2:
                OpenItem();
                break;
            case 3:
                OpenQuest();
                break;
            case 4:
                GlobalUIManager.instance.ExcuteSaveCommand();
                break;
            case 5:
                ExcuteReturnTitleCommand();
                break;
        }
    }
}
