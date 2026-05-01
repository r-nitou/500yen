using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager instance { get; set; }

    [Header("各スクリプト参照")]
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private CommonMessageWindow messageWindow;
    [SerializeField] private SelectionWindow selectionWindow;
    [SerializeField] private WarpUIManager warpUIManager;
    [SerializeField] private TutorialManager tutorialManager;

    [Header("表示対象シーン")]
    [SerializeField] private string[] menuCanOpenScene = { "VillageScene", "CaveScene", "CaveScene_B1", "CaveScene_B2" };

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
        FindPlayer();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlayer == null) return;

        //メニューを開いていいシーンか
        string activeScene = SceneManager.GetActiveScene().name;
        if (!menuCanOpenScene.Contains(activeScene)) return;

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
        SwitchToUIInput();

        //ウィンドウを表示してキー入力を待つ
        await messageWindow.OpenMessage(message, currentPlayer.InputAction);

        SwitchToPlayerInput();
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
        if (currentPlayer == null) FindPlayer();
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

    //チュートリアルを表示する処理
    public async UniTask ShowTutorial(string tutorialId)
    {
        if (currentPlayer == null) FindPlayer();

        //InputSystemの切り替え
        SwitchToUIInput();

        await tutorialManager.OpenTutorial(tutorialId, currentPlayer.InputAction);

        SwitchToPlayerInput();
    }

    //セーブコマンドを選択したとき
    public async void ExcuteSaveCommand()
    {
        //メニューの操作を切る
        currentPlayer.InputAction.UI.Navigate.performed -= menuManager.OnNavigate;
        currentPlayer.InputAction.UI.Submit.performed -= menuManager.OnSubmit;
        currentPlayer.InputAction.UI.Cancel.performed -= menuManager.OnCancel;

        //セーブを実行
        SaveManager.instance.Save();

        await UniTask.Yield(PlayerLoopTiming.Update);

        //ログ表示
        if (messageWindow != null)
        {
            await messageWindow.OpenMessage("セーブ完了", currentPlayer.InputAction);
        }

        //メニューの操作を復活させる
        currentPlayer.InputAction.UI.Navigate.performed += menuManager.OnNavigate;
        currentPlayer.InputAction.UI.Submit.performed += menuManager.OnSubmit;
        currentPlayer.InputAction.UI.Cancel.performed += menuManager.OnCancel;
    }

    //Playerを検索する処理
    public void FindPlayer()
    {
        currentPlayer = Object.FindFirstObjectByType<PlayerMove>();
    }

    //InputSystemをUI用に切り替える処理
    public void SwitchToUIInput()
    {
        if (currentPlayer == null) FindPlayer();
        if (currentPlayer == null) return;
        currentPlayer.InputAction.Player.Disable();
        currentPlayer.InputAction.UI.Enable();
    }

    //InputSystemをPlayer用に切り替える処理
    public void SwitchToPlayerInput()
    {
        if (currentPlayer == null) FindPlayer();
        if (currentPlayer == null) return;
        currentPlayer.InputAction.Player.Enable();
        currentPlayer.InputAction.UI.Disable();
    }
}
