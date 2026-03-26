using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    //定数の定義
    private const float MOVE_THRESHOLD = 0.001f;        //移動完了とする距離
    private const float COLLISION_RADIUS = 0.2f;       //障害物判定の円の半径
    private const float GRID_UNIT = 1.0f;               //1マスの単位

    [Header("プレイヤーの移動速度")]
    [SerializeField] private float moveSpeed = 5f;
    [Header("障害物レイヤー")]
    [SerializeField] private LayerMask obstacleLayer;

    private PlayerInputAction input;
    private Vector2 moveInput;
    //PlayerInputでプレイヤーの向きを保持
    private Vector2 lookDirection = Vector2.down;

    private bool isMoveing;
    private CancellationToken token;

    //外部からプレイヤーの向きを参照できるようにする
    public Vector2 LookDirection => lookDirection;

    private void Awake()
    {
        //オブジェクト破棄時にタスクを止める
        token = this.GetCancellationTokenOnDestroy();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = UImanager.instance.InputAction;
    }

    // Update is called once per frame
    void Update()
    {
        //移動中でなければ入力を受け付ける
        if (isMoveing)  
        {
            return;
        }

        //キーボードからの入力を受ける
        moveInput = input.Player.Move.ReadValue<Vector2>();
        //移動量が0でないなら
        if (moveInput != Vector2.zero)
        {
            UpdateDirection(moveInput);
            TryMove(moveInput);
        }
    }

    //移動先が移動可能か判定する処理
    private bool CanMoveCheck(Vector3 position)
    {
        //障害物がないかチェック
        return !Physics2D.OverlapCircle(position, COLLISION_RADIUS, obstacleLayer);
    }

    //プレイヤーを動かす処理
    private void TryMove(Vector2 direction)
    {
        Vector3 targetPosition = transform.position;
        //斜め移動の防止
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            targetPosition.x += (direction.x > 0) ? GRID_UNIT : -GRID_UNIT;
        }
        else
        {
            targetPosition.y += (direction.y > 0) ? GRID_UNIT : -GRID_UNIT;
        }

        //移動先が移動可能か判定
        if (CanMoveCheck(targetPosition))
        {
            MoveGrid(targetPosition).Forget();
        }
    }

    //1マス動かす処理
    private async UniTaskVoid MoveGrid(Vector3 position)
    {
        isMoveing = true;

        while (Vector3.Distance(transform.position, position) > MOVE_THRESHOLD)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                position,
                moveSpeed * Time.deltaTime);

            //次のフレームまで待つ
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        transform.position = position;
        isMoveing = false;
    }

    //プレイヤーの向きを更新する処理
    private void UpdateDirection(Vector2 input)
    {
        //上下左右の向きをとる
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) 
        {
            //上下方向の向きをとる
            lookDirection = new Vector2(input.x > 0 ? 1 : -1, 0);
        }
        else
        {
            //左右方向の向きをとる
            lookDirection = new Vector2(0, input.y > 0 ? 1 : -1);
        }
    }
    private void OnDrawGizmosSelected()
    {
        // 1. 現在地の判定（足元が何かに埋まっていないか確認用）
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, COLLISION_RADIUS);

        // 2. 移動入力がある場合、その「移動先」を予測表示
        if (moveInput != Vector2.zero)
        {
            Gizmos.color = Color.red;
            // 次のフレームで移動しようとするターゲット位置を計算
            Vector3 targetPos = transform.position + new Vector3(moveInput.x, moveInput.y, 0);
            // 移動先の判定円を赤色で描画
            Gizmos.DrawWireSphere(targetPos, COLLISION_RADIUS);

            // 現在地から移動先への線
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }
}
