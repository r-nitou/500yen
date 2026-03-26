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

    [Header("行先表示UIの設定")]
    [SerializeField] private GameObject destinationWindow;
    [SerializeField] private TMP_Text destinationText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    //共通で使うInputActionの参照
    public PlayerInputAction InputAction { get; set; }

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

        //InputActionのインスタンス化
        InputAction = new PlayerInputAction();
    }

    private void OnEnable() => InputAction.Enable();
    //InputSystemを無効化
    private void OnDisable() => InputAction.Disable();

    //行先ウィンドウを表示する処理
    public async UniTask OpenDestinationWindow(SceneTransitionTrigger trigger)
    {
        Debug.Log("ウィンドウを表示");
        //ウィンドウを表示
        destinationWindow.SetActive(true);
        //テキストの更新
        destinationText.text = $"{trigger.DestinationText}にいきますか？";

        //InputSystemの切り替え
        InputAction.Player.Disable();
        InputAction.UI.Enable();

        //すぐに操作できるように最初のボタンを選択状態にする
        EventSystem.current.SetSelectedGameObject(yesButton.gameObject);

        //ボタンが押されるまで待機する処理
        bool? isYes = null;

        yesButton.onClick.AddListener(() => isYes = true);
        noButton.onClick.AddListener(() => isYes = false);

        //isYesに値が入るまで待機
        await UniTask.WaitUntil(() => isYes != null);

        //ボタンが押されたのでリスナーを解除
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        //結果に応じた処理
        if (isYes == true)
        {
            //シーン遷移
            await SceneManager.LoadSceneAsync(trigger.TargetSceneName);
            //今はデバッグログ
             Debug.Log($"{trigger.TargetSceneName}にいきます");
        }

        //ウィンドウを閉じる
        CloseDestinationWindow();
    }

    //行先ウィンドウを非表示する処理
    public void CloseDestinationWindow()
    {
        //ウィンドウを非表示
        destinationWindow.SetActive(false);

        //InputSystemの切り替え
        InputAction.Player.Enable();
        InputAction.UI.Disable();

        Debug.Log("ウィンドウを閉じる");
    }

    //選択肢ウィンドウを挟まず、直接移動する処理
    public async UniTaskVoid ExcuteDirectionTransition(string sceneName)
    {
        //プレイヤーの操作停止
        InputAction.Player.Disable();

        //シーン遷移
        await SceneManager.LoadSceneAsync(sceneName);
        Debug.Log($"{sceneName}に行きます");
    }
}
