using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    //定数の定義
    private const float MOVE_THRESHOLD = 0.001f;        //移動完了とする距離
    private const float COLLISION_RADIUS = 0.2f;       //障害物判定の円の半径
    private const float GRID_UNIT = 1.0f;               //1マスの単位

    public PlayerInputAction InputAction { get; set; }
    public static PlayerMove instance { get; set; }

    [Header("プレイヤーの移動速度")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("仲間の参照")]
    [SerializeField] private FollowerMove follower;

    [Header("障害物レイヤー")]
    [SerializeField] private LayerMask obstacleLayer;

    private Animator animator;

    private Vector2 moveInput;
    //PlayerInputでプレイヤーの向きを保持
    private Vector2 lookDirection = Vector2.down;
    //移動前の座標
    private Vector3 previousPosition;

    public bool isMoveing;
    private CancellationToken token;

    //外部からプレイヤーの向きを参照できるようにする
    public Vector2 LookDirection => lookDirection;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // シーン切り替え時にこのオブジェクトを破壊しない
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //オブジェクト破棄時にタスクを止める
        token = this.GetCancellationTokenOnDestroy();

        //InputActionのインスタンス化
        InputAction = new PlayerInputAction();

        animator = GetComponent<Animator>();
    }
    //InputSystemを有効化
    private void OnEnable()
    {
        if (InputAction != null)
        {
            InputAction.Enable();
        }
    }
    //InputSystemを無効化
    private void OnDisable()
    {
        if (InputAction != null)
        {
            InputAction.Disable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //キーボードからの入力を受ける
        moveInput = InputAction.Player.Move.ReadValue<Vector2>();

        //移動中でなければ入力を受け付ける
        if (isMoveing) return;

        //移動量が0でないなら
        if (moveInput != Vector2.zero)
        {
            UpdateDirection(moveInput);

            UpdateAnimation(false);

            TryMove(moveInput);
        }
    }

    //移動先が移動可能か判定する処理
    private bool CanMoveCheck(Vector3 position)
    {
        //障害物がないかチェック
        bool isMoveCheck = !Physics2D.OverlapCircle(position, COLLISION_RADIUS, obstacleLayer);
        return isMoveCheck;
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
        //移動前の座標を保存
        previousPosition = transform.position;
        Vector2 currentDirection = lookDirection;

        //移動開始時、仲間を動かす
        if (follower != null)
        {
            follower.FollowMove(previousPosition, currentDirection).Forget();
        }

        isMoveing = true;
        //アニメーションを更新する
        UpdateAnimation(true);

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

        //アニメーションを止める
        UpdateAnimation(false);

        //エンカウントチェック
        var encounter = GetComponent<EncounterManager>();
        if(encounter != null)
        {
            encounter.CheckEncounter();
        }

        //移動完了後、足元に自動遷移トリガーがあるかチェック
        CheckAutoTransition().Forget();
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

    //アニメーションを更新する処理
    private void UpdateAnimation(bool moving)
    {
        if (animator == null) return;

        animator.SetFloat("MoveX", lookDirection.x);
        animator.SetFloat("MoveY", lookDirection.y);

        animator.SetBool("IsMoving", moving);
    }

    //足元に自動遷移トリガーがあるかチェックする処理
    private async UniTaskVoid CheckAutoTransition()
    {
        //足元にTransitiontレイヤーがあるかチェック
        Collider2D hit = Physics2D.OverlapCircle(transform.position, COLLISION_RADIUS, LayerMask.GetMask("Transition"));

        if (hit != null)
        {
            SceneTransitionTrigger trigger = hit.GetComponent<SceneTransitionTrigger>();
            if (trigger != null && trigger.Type == SceneTransitionTrigger.EntranceType.Auto ) 
            {
                if (trigger.CanEnter())
                {
                    SceneLoader.instance.ExcuteSceneTransition
                        (trigger.TargetSceneName, trigger.TargetMarkerId, this).Forget();
                }
                else
                {
                    //移動フラグを立てて入力と移動を停止する
                    isMoveing = true;
                    UpdateAnimation(false);
                    if (trigger.TargetSceneName == "CaveScene") 
                    {
                        await GlobalUIManager.instance.ShowMessage("暗くて先が見えない...");
                    }
                    //メッセージを閉じたら移動可能にする
                    isMoveing = false;
                }
            }
        }
    }

    //シーン遷移時に向きを指定する処理
    public void SetDirection(Vector2 direction)
    {
        lookDirection = direction;

        if (animator == null) return;

        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);

        animator.SetBool("IsMoving", false);
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
