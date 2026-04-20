using Cysharp.Threading.Tasks;
using UnityEngine;

public class AlterPortal : MonoBehaviour
{
    [Header("移動先設定")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetMarkerId;

    [Header("演出設定")]
    [SerializeField] private string alterMessage = "女神像がある\n触れますか?";

    private bool isTransitioning = false;

    //プレイヤーの「調べる」操作のときに呼ばれる処理
    public async UniTask Intaract()
    {
        if (isTransitioning) return;
        //確認ウィンドウの表示
        bool isYes = await GlobalUIManager.instance.ShowSelectionWindow(alterMessage);

        if (isYes)
        {
            isTransitioning = true;
            await MoveToNextFloor();
        }
    }

    //次の層に行く処理
    public async UniTask MoveToNextFloor()
    {
        if (SceneLoader.instance == null) return;

        //シーン遷移処理
        await SceneLoader.instance.ExcuteSceneTransition(
            targetSceneName,
            targetMarkerId,
            PlayerMove.instance
            );

        isTransitioning = false;
    }
}
