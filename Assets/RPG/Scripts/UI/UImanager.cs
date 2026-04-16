using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class UImanager : MonoBehaviour
{
    //UIManagerのシングルトン化
    public static UImanager instance { get; set; }

    [Header("各UIスクリプトの参照")]
    [SerializeField] private SelectionWindow selectionWindow;
    [SerializeField] private CommonMessageWindow messageWindow;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private BulletinBoardUI boardUi;

    private PlayerMove currentPlayer;
    private bool isMenuOpen = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (currentPlayer == null) return;

        //メニューの開閉の監視
        if (currentPlayer.InputAction.Player.Menu.triggered && !isMenuOpen)
        {
            ToggleMenu(true);
        }
    }

    //行先ウィンドウを表示する処理
    public async UniTask<bool> ShowSelectionWindow(string message)
    {
        //念のためPlayerを検索する
        if (currentPlayer == null)
        {
            FindPlayer();
        }
        if(currentPlayer == null)
        {
            Debug.LogError("Playerが見つからないため、選択ウィンドウを表示できません");
            return false;
        }

        //InputSystemの切り替え
        SwitchToUIInput();

        bool result = await selectionWindow.OpenWindow(message,currentPlayer.InputAction);

        SwitchToPlayerInput();

        return result;
    }

    //メッセージウィンドウを表示する処理
    public async UniTask ShowMessage(string message)
    {
        //念のためPlayerを検索する
        if (currentPlayer == null)
        {
            FindPlayer();
        }
        if (currentPlayer == null)
        {
            Debug.LogError("Playerが見つからないため、選択ウィンドウを表示できません");
            return;
        }

        //InputSystemの切り替え
        SwitchToUIInput();

        //ウィンドウを表示してキー入力を待つ
        await messageWindow.OpenMessage(message, currentPlayer.InputAction);

        SwitchToPlayerInput();
    }

    //メニューUIを表示する処理
    private void ToggleMenu(bool open)
    {
        isMenuOpen = open;

        //入力切替
        if (open)
        {
            SwitchToUIInput();

            currentPlayer.InputAction.UI.Navigate.performed += menuManager.OnNavigate;
            currentPlayer.InputAction.UI.Submit.performed += menuManager.OnSubmit;
            currentPlayer.InputAction.UI.Cancel.performed += menuManager.OnCancel;

            menuManager.OpenMenu();
        }
        else
        {
            menuManager.CloseMenu();

            currentPlayer.InputAction.UI.Navigate.performed -= menuManager.OnNavigate;
            currentPlayer.InputAction.UI.Submit.performed -= menuManager.OnSubmit;
            currentPlayer.InputAction.UI.Cancel.performed -= menuManager.OnCancel;

            SwitchToPlayerInput();
        }
    }

    //掲示板を開く処理
    public async UniTask ShowBuletinBoard()
    {
        SwitchToUIInput();
        boardUi.gameObject.SetActive(true);
        boardUi.InitializeBoard();

        await UniTask.WaitWhile(() => boardUi.gameObject.activeSelf);
        SwitchToPlayerInput();
    }

    public void CloseMenu()
    {
        if (isMenuOpen) 
        {
            ToggleMenu(false);
        }
    }

    //Playerを検索する処理
    public void FindPlayer()
    {
        currentPlayer = Object.FindFirstObjectByType<PlayerMove>();

        if (currentPlayer != null)
        {
            Debug.Log("Playerを見つけた");
        }
        else
        {
            Debug.LogWarning("Playerが見つからない");
        }
    }

    //InputSystemをUI用に切り替える処理
    public void SwitchToUIInput()
    {
        if (currentPlayer == null) return;
        currentPlayer.InputAction.Player.Disable();
        currentPlayer.InputAction.UI.Enable();
    }

    //InputSystemをPlayer用に切り替える処理
    public void SwitchToPlayerInput()
    {
        if (currentPlayer == null) return;
        currentPlayer.InputAction.Player.Enable();
        currentPlayer.InputAction.UI.Disable();
    }
}
