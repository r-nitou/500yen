using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EventPlayScene : MonoBehaviour
{
    public AdvController advController_;
    public EventPlayData eventData_;
    public SpriteRenderer bgImage_;
    public Image fadeImage_;

    private void Start()
    {
        if (eventData_.Still_)
        {
            bgImage_.sprite = eventData_.Still_;
        }

        StartCoroutine(StartADV());
    }

    private IEnumerator StartADV()
    {
        // 再生できるようになるまで待つ
        while (advController_.CanPlay() == false)
        {
            yield return null;
        }

        advController_.OnPlayFinish_.AddListener(OnFinishedSleepADV);
        advController_.PlayADV(eventData_.GetScenarioLabel());
        yield return null;
    }

    private void OnFinishedSleepADV()
    {
        Debug.Log("OnFinishedSleepADV");
        StartFadeTask();
    }

    // ADVSceneLoadManagerから移動済
    private async void StartFadeTask()
    {
        await GameManager.instance.Fade.FadeOut(1.0f, fadeImage_);
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));

        Debug.Log("FadeTask Excute");

        // シーン移動
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
            await SceneLoader.instance.ExcuteSceneTransition(eventData_.NextSceneName_, eventData_.HomeMarkerId_, PlayerMove.instance);
        }
    }
}
