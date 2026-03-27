using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class FollowerMove : MonoBehaviour
{
    private const float MOVE_THRESHOLD = 0.001f;

    [Header("移動速度")]
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 lookDirection = Vector2.zero;
    private bool isMoving;
    private CancellationToken token;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        token = this.GetCancellationTokenOnDestroy();
    }

    //前の仲間から呼ばれる移動命令
    public async UniTask FollowMove(Vector3 targetPosition, Vector2 direction)
    {
        lookDirection = direction;

        //アニメーションの更新
        UpdateAnimation(lookDirection, true);

        isMoving = true;
        while (Vector3.Distance(transform.position, targetPosition) > MOVE_THRESHOLD)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        transform.position = targetPosition;
        isMoving = false;

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
}
