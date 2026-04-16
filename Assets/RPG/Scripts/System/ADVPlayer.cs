using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Utage;
using UtageExtensions;

public class ADVPlayer : MonoBehaviour
{
    [Header("Utageの参照")]
    public AdvEngine advEngine;

    [Header("フェード処理用UI")]
    [SerializeField] private Image fadeTarget;

    //汎用ADV再生処理
    public async UniTask PlayScenario(string labelName)
    {
        if (advEngine == null) return;

        advEngine.JumpScenario(labelName);

        await UniTask.WaitUntil(() => advEngine.IsEndScenario);
    }

    //村シーンに戻る処理
    public async UniTask TransitionScene(string sceneName,string markerId)
    {
        //フェードアウト
        await GameManager.instance.Fade.FadeOut(2.0f, fadeTarget);
        //シーン遷移
        await SceneLoader.instance.ExcuteSceneTransition(sceneName, markerId, PlayerMove.instance);
    }
}
