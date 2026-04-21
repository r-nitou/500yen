using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WarpStatue : MonoBehaviour
{
    [Header("この場所の設定")]
    [SerializeField] private string floorDisplayName;
    [SerializeField] private string markerId;

    [Header("演出設定")]
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private Image fadeTarget;

    private bool isProcessing = false;

    //プレイヤーに調べられた時の処理
    public async UniTask Intaract()
    {
        if (isProcessing) return;
        isProcessing = true;

        string currentScene = SceneManager.GetActiveScene().name;
        var input = PlayerMove.instance.InputAction;

        //既に登録されているか確認
        if (!GameManager.instance.IsFloorRegistered(markerId))
        {
            //初回登録
            GameManager.instance.RegisterVisitedFloor(floorDisplayName, currentScene, markerId);
            await GlobalUIManager.instance.ShowMessage($"{floorDisplayName}の英雄像を解放した");
        }
        else
        {
            await ProcessOpenWarpMenu(input);
        }
        isProcessing = false;
    }

    //ワープメニューを開いて選択した場所に転送する処理
    private async UniTask ProcessOpenWarpMenu(PlayerInputAction input)
    {
        FloorData selectedFloor = await GlobalUIManager.instance.ShowWarpWindow();

        if (selectedFloor != null) 
        {
            //フェードアウト
            await GameManager.instance.Fade.FadeOut(fadeDuration, fadeTarget, Color.white);
            //ワープ
            await SceneLoader.instance.ExcuteSceneTransition(
                selectedFloor.sceneName,
                selectedFloor.markerId,
                PlayerMove.instance
                );
        }
    }
}
