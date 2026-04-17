using UnityEngine;

public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager instance { get; set; }

    [Header("各スクリプト参照")]
    [SerializeField] private MenuManager menuManager;

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
