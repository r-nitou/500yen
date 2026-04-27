using Cysharp.Threading.Tasks;
using UnityEngine;

public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager instance { get; set; }

    [Header("各スクリプト参照")]
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private CommonMessageWindow messageWindow;
    [SerializeField] private SelectionWindow selectionWindow;
    [SerializeField] private WarpUIManager warpUIManager;

    private PlayerMove currentPlayer;
    private bool isMenuOpen = false;

    public PlayerMove CurrentPlayer => currentPlayer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlayer == null) return;

        //メニューの開閉の監視
        if (currentPlayer.InputAction.Player.Menu.triggered && !isMenuOpen)
        {
            ToggleMenu(true);
        }
    }

    //メニューUIを表示する処理
    public void ToggleMenu(bool open)
    {
        isMenuOpen = open;

        //入力切替
        if (open)
        {
            SwitchToUIInput();

            //イベント登録
            currentPlayer.InputAction.UI.Navigate.performed += menuManager.OnNavigate;
            currentPlayer.InputAction.UI.Submit.performed += menuManager.OnSubmit;
            currentPlayer.InputAction.UI.Cancel.performed += menuManager.OnCancel;

            menuManager.OpenMenu();
        }
        else
        {
            menuManager.CloseMenu();
            //イベント解除
            currentPlayer.InputAction.UI.Navigate.performed -= menuManager.OnNavigate;
            currentPlayer.InputAction.UI.Submit.performed -= menuManager.OnSubmit;
            currentPlayer.InputAction.UI.Cancel.performed -= menuManager.OnCancel;

            SwitchToPlayerInput();
        }
    }

    //メッセージウィンドウを表示する処理
    public async UniTask ShowMessage(string message)
    {

        //InputSystemの切り替え
        GlobalUIManager.instance.SwitchToUIInput();

        //ウィンドウを表示してキー入力を待つ
        await messageWindow.OpenMessage(message, currentPlayer.InputAction);

        GlobalUIManager.instance.SwitchToPlayerInput();
    }

    //ボス戦前イベント用処理
    public async UniTask ShowBossMessage(string[] messages)
    {
        await messageWindow.OpenDialogue(messages, currentPlayer.InputAction);
    }

    //クリアイベント用メッセージ処理
    public async UniTask ShowClearMessage(string[] messages)
    {
        await messageWindow.OpenDialogue(messages, currentPlayer.InputAction);
    }

    //ストリーイベント用メッセージ処理
    public async UniTask ShowEventMessage(string[] messages,string speakerName = "")
    {
        await messageWindow.OpenDialogue(messages, currentPlayer.InputAction, speakerName);
    }

    //行先ウィンドウを表示する処理
    public async UniTask<bool> ShowSelectionWindow(string message)
    {
        //InputSystemの切り替え
        SwitchToUIInput();

        bool result = await selectionWindow.OpenWindow(message, currentPlayer.InputAction);

        SwitchToPlayerInput();

        return result;
    }

    //ワープリストを表示する処理
    public async UniTask<FloorData> ShowWarpWindow()
    {
        if (warpUIManager == null) return null;

        //InputSystemの切り替え
        SwitchToUIInput();

        FloorData result = await warpUIManager.OpenAndSelect(currentPlayer.InputAction);

        SwitchToPlayerInput();
        return result;
    }

    //Playerを検索する処理
    public void FindPlayer()
    {
        currentPlayer = Object.FindFirstObjectByType<PlayerMove>();
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
