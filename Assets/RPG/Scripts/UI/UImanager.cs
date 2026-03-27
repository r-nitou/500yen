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

    [Header("プレイヤーの参照")]
    [SerializeField] private GameObject playerObject;
    [Header("各UIスクリプトの参照")]
    [SerializeField] private SelectionWindow selectionWindow;

    //共通で使うInputActionの参照
    private PlayerInputAction playerInput;

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
        if (playerObject != null)
        {
            playerInput = playerObject.GetComponent<PlayerMove>().InputAction;
        }
    }

    //行先ウィンドウを表示する処理
    public async UniTask<bool> ShowSelectionWindow(string message)
    {
        //InputSystemの切り替え
        playerInput.Player.Disable();
        playerInput.UI.Enable();

        bool result = await selectionWindow.OpenWindow(message,playerInput);

        //InputSystemの切り替え
        playerInput.Player.Enable();
        playerInput.UI.Disable();

        return result;
    }

    //InputActionを無効化する処理
    public void CloseDestinationWindow()
    {
        playerInput?.Disable();
    }

    //選択肢ウィンドウを挟まず、直接移動する処理
    public async UniTaskVoid ExcuteDirectionTransition(string sceneName)
    {
        //プレイヤーの操作停止
        playerInput.Player.Disable();

        //シーン遷移
        await SceneManager.LoadSceneAsync(sceneName);
        Debug.Log($"{sceneName}に行きます");
    }
}
