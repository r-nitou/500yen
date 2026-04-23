using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class FollowerMove : MonoBehaviour
{
    private const float MOVE_THRESHOLD = 0.001f;

    public static FollowerMove instance { get; set; }

    [Header("移動速度")]
    [SerializeField] private float moveSpeed = 5f;

    private PlayerMove player;

    private Vector2 lookDirection = Vector2.zero;
    private CancellationToken token;

    private Animator animator;

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

        animator = GetComponent<Animator>();
        token = this.GetCancellationTokenOnDestroy();
    }

    //前の仲間から呼ばれる移動命令
    public async UniTask FollowMove(Vector3 targetPosition, Vector2 direction)
    {
        lookDirection = direction;

        //アニメーションの更新
        UpdateAnimation(lookDirection, true);

        while (Vector3.Distance(transform.position, targetPosition) > MOVE_THRESHOLD)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        transform.position = targetPosition;

        //アニメーションをIdle状態に戻す
        UpdateAnimation(lookDirection, false);
    }


    //アニメーションを更新する処理
    private void UpdateAnimation(Vector2 direction,bool moving)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);

        animator.SetBool("IsMoving", moving);
    }

    //シーン遷移時に向きを指定する処理
    public void SetDirection(Vector2 direction)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);

        animator.SetBool("IsMoving", false);
    }
}
