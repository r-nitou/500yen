using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Utage;

public class AdvController : MonoBehaviour
{
    [Header("UtageのAdvEngine"), SerializeField]
    private AdvEngine advEngine = null;

    private UnityEvent onPlayFinish_ = new UnityEvent();//PlayADVの前に登録しておく
    public UnityEvent OnPlayFinish_ => onPlayFinish_; 

    private bool isPlaying = false;// 再生中フラグ

    /// <summary>
    /// ボタンから呼び出すメソッド
    /// </summary>
    /// <param name="scenarioLabel">再生したいシナリオのラベル名（Excelで設定したもの）</param>
    public void PlayADV(string scenarioLabel)
    {
        // 既にシナリオ再生中、またはエンジンが初期化されていない場合は処理しない
        if (isPlaying)
        {
            Debug.LogWarning("ADVエンジンは既に再生中です。");
            return;
        }
        if (advEngine.IsWaitBootLoading)
        {
            Debug.LogWarning("ADVエンジンの準備ができていないです。");
            return;
        }

        StartCoroutine(PlayScenario(scenarioLabel));
    }

    public void StopADV()
    {
        // 再生中でなければ何もしない
        if (!isPlaying) return;

        // Utage側のシナリオを強制終了する
        advEngine.EndScenario();

        OnPlayFinished();

        Debug.Log("シナリオの強制終了メソッドが呼ばれました。");
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public bool CanPlay()
    {
        return !(advEngine.IsWaitBootLoading || isPlaying);
    }

    private IEnumerator PlayScenario(string scenarioLabel)
    {
        // 1. 指定したラベルからシナリオを再生開始
        isPlaying = true;
        advEngine.JumpScenario(scenarioLabel);
        yield return null;

        // 2. シナリオが終了するまで待機
        while (!advEngine.IsEndScenario)
        {
            yield return null;
        }

        // 3. シナリオ終了
        OnPlayFinished();
    }

    private void OnPlayFinished()
    {
        Debug.Log("シナリオ再生が終了し、元の画面に戻りました。");
        isPlaying = false;
        OnPlayFinish_?.Invoke();
        OnPlayFinish_.RemoveAllListeners();
    }
}