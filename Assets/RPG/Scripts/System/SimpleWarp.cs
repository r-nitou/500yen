using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SimpleWarp : MonoBehaviour
{
    [Header("ワープ先設定")]
    [SerializeField] private Transform targetPos;
    [SerializeField] private Vector2 lookDirection = Vector2.down;

    [Header("演出設定")]
    [SerializeField] private Image fadeTarget;
    [SerializeField] private float fadeDuration;

    //プレイヤーの「調べる」入力で呼び出される処理
    public async UniTask Intaract()
    {
        //入力制限
        PlayerMove.instance.InputAction.Player.Disable();
        //フェードアウト
        await GameManager.instance.Fade.FadeOut(fadeDuration, fadeTarget, Color.white);
        //ワープ実行
        ExcuteWarp();
        //フェードイン
        await GameManager.instance.Fade.FadeIn(fadeDuration, fadeTarget, Color.white);
        //入力制限解除
        PlayerMove.instance.InputAction.Player.Enable();
    }

    //ワープを実行する処理
    private void ExcuteWarp()
    {

        //プレイヤーの位置と向きを更新
        var player = PlayerMove.instance;
        if (player != null)
        {
            player.transform.position = targetPos.position;
            player.SetDirection(lookDirection);
        }

        //名間の位置も更新
        if (FollowerMove.instance != null)
        {
            FollowerMove.instance.transform.position = targetPos.position;
            FollowerMove.instance.SetDirection(lookDirection);
        }
    }
}
