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
    [SerializeField] private BulletinBoardUI boardUi;

    private PlayerMove currentPlayer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentPlayer = GlobalUIManager.instance.CurrentPlayer;
    }

    //行先ウィンドウを表示する処理
    public async UniTask<bool> ShowSelectionWindow(string message)
    {
        //InputSystemの切り替え
        GlobalUIManager.instance.SwitchToUIInput();

        bool result = await selectionWindow.OpenWindow(message,currentPlayer.InputAction);

        GlobalUIManager.instance.SwitchToPlayerInput();

        return result;
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

    //掲示板を開く処理
    public async UniTask ShowBuletinBoard()
    {
        GlobalUIManager.instance.SwitchToUIInput();
        boardUi.gameObject.SetActive(true);
        boardUi.InitializeBoard();

        await UniTask.WaitWhile(() => boardUi.gameObject.activeSelf);
        GlobalUIManager.instance.SwitchToPlayerInput();
    }
}
