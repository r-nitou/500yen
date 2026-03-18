using System.Collections;
using UnityEngine;
using Utage;

public class AdvController : MonoBehaviour
{
    [Header("UtageのAdvEngine")]
    public AdvEngine advEngine;

    [Header("ADV表示用のルートオブジェクト（Canvasなど）")]
    public Canvas advUIRoot;

    private void Start()
    {
        // 最初はADV画面を非表示にしておく
        if (advUIRoot != null)
        {
            advUIRoot.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ボタンから呼び出すメソッド
    /// </summary>
    /// <param name="scenarioLabel">再生したいシナリオのラベル名（Excelで設定したもの）</param>
    public void PlayADV(string scenarioLabel)
    {
        // 既にシナリオ再生中、またはエンジンが初期化されていない場合は処理しない
        if (advEngine.IsWaitBootLoading || advUIRoot.gameObject.activeSelf)
        {
            Debug.LogWarning("ADVエンジンの準備ができていないか、既に再生中です。");
            return;
        }

        StartCoroutine(PlayScenario(scenarioLabel));
    }

    private IEnumerator PlayScenario(string scenarioLabel)
    {
        // 1. ADVのUIを表示する
        advUIRoot.gameObject.SetActive(true);

        // 2. 指定したラベルからシナリオを再生開始
        advEngine.JumpScenario(scenarioLabel);

        // ※ JumpScenarioを呼んだ直後はフラグが切り替わっていないことがあるため、1フレーム待つ
        yield return null;

        // 3. シナリオが終了するまで待機
        while (!advEngine.IsEndScenario)
        {
            yield return null;
        }

        // 4. シナリオが終了したら、ADVのUIを非表示にして元に戻す
        advUIRoot.gameObject.SetActive(false);

        Debug.Log("シナリオ再生が終了し、元の画面に戻りました。");
    }
}