using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    private const float INTERACT_DISTANCE = 1.0f;

    [Header("判定用レイヤー")]
    [SerializeField] private LayerMask transitionLayer;

    [Header("スクリプト参照")]
    [SerializeField] private PlayerMove playerMove;

    //[Header("UI設定")]
    //[SerializeField] private GameObject destinationWindow;
    //[SerializeField] private TMP_Text questionText;
    //[SerializeField] private Button yewButton;
    //[SerializeField] private Button noButton;

    private CancellationToken token;

    private void Awake()
    {
        //キャンセラレーショントークンの取得
        token = this.GetCancellationTokenOnDestroy();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("入力検知：Interactボタンが押されました");
        CheckInteractable().Forget();
    }

    private void Start()
    {
        UImanager.instance.InputAction.Player.Interact.performed += OnInteractPerformed;
    }

    //プレイヤーの「調べる」を行う処理
    private async UniTaskVoid CheckInteractable()
    {
        Debug.Log("調べるよ！");
        //プレイヤーの向きから正面にレイを飛ばす
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            playerMove.LookDirection,
            INTERACT_DISTANCE,
            transitionLayer
            );

        //何か当たったか
        if (hit.collider != null)
        {
            //当たったオブジェクトにドアの設定がついているか
            SceneTransitionTrigger trigger = hit.collider.GetComponent<SceneTransitionTrigger>();

            if (trigger != null)
            {
                //UIManagerに行先ウィンドウを表示させる
                await UImanager.instance.OpenDestinationWindow(trigger);
                Debug.Log("表示するよ");
            }
        }
        else
        {
            Debug.Log("何もないよ");
        }
    }

    // デバッグ用：エディタ上でレイの範囲を可視化
    private void OnDrawGizmosSelected()
    {
        if (playerMove == null) return;

        Gizmos.color = Color.yellow;
        Vector3 direction = new Vector3(playerMove.LookDirection.x, playerMove.LookDirection.y, 0);
        Gizmos.DrawRay(transform.position, direction * INTERACT_DISTANCE);
    }

}
