using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [Header("メニュー項目")]
    [SerializeField] private List<TMP_Text> menuTexts;
    [SerializeField] private TMP_Text continueText;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.black;

    [Header("シーン遷移設定")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private string newGameMarkerId;

    [Header("入力設定")]
    [SerializeField] private PlayerInputAction inputAction;

    [Header("演出設定")]
    [SerializeField] private Image fadeTarget;

    private int selectedIndex = 0;
    private bool isHandlingInput = true;

    private void Awake()
    {
        //入力の有効化
        inputAction = new PlayerInputAction();
        inputAction.Player.Disable();
        inputAction.UI.Enable();
        inputAction.UI.Navigate.performed += OnNavigate;
        inputAction.UI.Submit.performed += OnSubmit;
    }


    //セーブデータがあるか判定する処理
    private bool HasSaveData
    {
        get
        {
            return false;
        }
    }

    //入力の解除
    private void OnDestroy()
    {
        //イベント解除
        inputAction.UI.Navigate.performed -= OnNavigate;
        inputAction.UI.Submit.performed -= OnSubmit;
        inputAction.Disable();
    }

    //上下入力処理
    private void OnNavigate(InputAction.CallbackContext context)
    {
        if (!isHandlingInput) return;

        Vector2 input = context.ReadValue<Vector2>();
        if (input.y > 0.5f)
        {
            ChangeSelection(-1);
        }
        else if (input.y < -0.5)
        {
            ChangeSelection(1);
        }
    }
    //決定入力処理
    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (!isHandlingInput) return;
        ExcuteSelection().Forget();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerMove.instance.gameObject.SetActive(false);

        //UIの表示・非表示
        SetUpMenuVisibility();
        UpdateSelection();
    }

    //決定したコマンドを実行する処理
    private async UniTask ExcuteSelection()
    {
        isHandlingInput = false;

        switch (selectedIndex)
        {
            case 0:
                await StartNewGame();
                break;
            case 1:
                Debug.Log("Continue:未実装");
                break;
            case 2:
                Debug.Log("設定:未実装");
                break;
            case 3:
                Debug.Log("ゲーム終了:未実装");
                break;
        }
    }

    //新しくゲームを始める処理
    private async UniTask StartNewGame()
    {
        await GameManager.instance.Fade.FadeOut(1.0f, fadeTarget, Color.white);

        //データの初期化

        //ニューゲーム開始フラグを立てる
        if (GameManager.instance != null)
        {
            GameManager.instance.isNewGame = true;
        }
        //シーン遷移開始
        inputAction.UI.Disable();
        await SceneLoader.instance.ExcuteSceneTransition(nextSceneName, newGameMarkerId, PlayerMove.instance);
    }

    //メニューの構成を変える処理
    private void SetUpMenuVisibility()
    {
        if (!HasSaveData)
        {
            continueText.gameObject.SetActive(false);
        }
        else
        {
            continueText.gameObject.SetActive(true);
        }
    }

    //選択しているコマンドを変更する処理
    private void ChangeSelection(int direction)
    {
        selectedIndex = (selectedIndex + direction + menuTexts.Count) % menuTexts.Count;
        if (!menuTexts[selectedIndex].gameObject.activeSelf)
        {
            selectedIndex = (selectedIndex + direction + menuTexts.Count) % menuTexts.Count;
        }
        UpdateSelection();
    }

    //選択しているコマンドをわかりやすく更新する処理
    private void UpdateSelection()
    {
        for (int i = 0; i < menuTexts.Count; i++)
        {
            menuTexts[i].color = (i == selectedIndex) ? selectedColor : defaultColor;
        }
    }
}
