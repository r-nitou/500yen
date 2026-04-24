using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClearStele : MonoBehaviour
{
    [Header("移動先設定")]
    [SerializeField] private string clearSceneName = "ClearScene";

    [Header("演出用設定")]
    [SerializeField] private Image fadeaTarget;
    [SerializeField] private float fadeDuration = 2.0f;

    [Header("イベント会話")]
    [SerializeField, TextArea(3, 10)]
    private string[] clearDialogues;

    //プレイヤーから「調べる」から呼ばれる
    public async UniTask Intaract()
    {
        //クリアフラグの確認
        if (GameManager.instance.IslastBossDefated)
        {
            //イベントメッセージの表示
            await GlobalUIManager.instance.ShowClearMessage(clearDialogues);

            //フェードアウト
            await GameManager.instance.Fade.FadeOut(fadeDuration, fadeaTarget, Color.white);

            //シーン遷移
            await SceneLoader.instance.ExcuteSceneTransition(clearSceneName, "", PlayerMove.instance);
        }
    }
}
