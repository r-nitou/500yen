using Cysharp.Threading.Tasks;
using UnityEngine;

public class ADVSceneLoadManager : MonoBehaviour
{
    [Header("遷移設定")]
    [SerializeField] private string nextSceneName = "VillageScene";
    [SerializeField] private string homeMarkerId = "HomeMarker";

    //寝るボタンから呼び出される関数
    public void OnSleepSelected()
    {
        ExcuteSleep().Forget();
    }

    //時間の変更、プレイヤーの配置をする処理
    private async UniTaskVoid ExcuteSleep()
    {
        //時間を「朝」に変更
        if (GameManager.instance != null)
        {
            GameManager.instance.currentPhase = DayPhase.Morning;
        }

        //シーン遷移
        if (SceneLoader.instance != null)
        {
            await SceneLoader.instance.ExcuteSceneTransition(nextSceneName, homeMarkerId, PlayerMove.instance);
        }
    }
}
