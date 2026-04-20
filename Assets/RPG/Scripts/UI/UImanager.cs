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
