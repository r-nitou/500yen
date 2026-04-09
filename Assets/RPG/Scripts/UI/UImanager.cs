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
            return;
        }
    }

    private void Start()
    {
        FindPlayer();
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
        currentPlayer.InputAction.Player.Disable();
        currentPlayer.InputAction.UI.Enable();

        bool result = await selectionWindow.OpenWindow(message,currentPlayer.InputAction);

        //InputSystemの切り替え
        currentPlayer.InputAction.Player.Enable();
        currentPlayer.InputAction.UI.Disable();

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
        currentPlayer.InputAction.Player.Disable();
        currentPlayer.InputAction.UI.Enable();

        //ウィンドウを表示してキー入力を待つ
        await messageWindow.OpenMessage(message, currentPlayer.InputAction);

        //InputSystemの切り替え
        currentPlayer.InputAction.Player.Enable();
        currentPlayer.InputAction.UI.Disable();
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

    ////InputActionを無効化する処理
    //public void CloseDestinationWindow()
    //{
    //    currentPlayer.InputAction?.Disable();
    //}
}
