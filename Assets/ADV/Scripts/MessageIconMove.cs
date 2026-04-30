using UnityEngine;
using DG.Tweening;

public class MessageIconMove : MonoBehaviour
{
    [Header("設定")]
    [SerializeField, Tooltip("動かす対象のRectTransform（未指定なら自身のコンポーネントを取得）")]
    private RectTransform iconRectTransform;

    [SerializeField, Tooltip("上下に動く距離")]
    private float moveDistance = 10f;

    [SerializeField, Tooltip("片道（上または下）にかかる時間")]
    private float duration = 0.5f;

    private Tween floatTween;
    private float defaultYPos;

    private void Start()
    {
        if (iconRectTransform == null)
        {
            iconRectTransform = GetComponent<RectTransform>();
        }

        // 初期表示位置のY座標を記憶しておく（位置ズレ防止のため）
        defaultYPos = iconRectTransform.anchoredPosition.y;

        ShowIcon();
    }

    /// <summary>
    /// メッセージ表示が完了した際に呼び出すメソッド
    /// </summary>
    public void ShowIcon()
    {
        // 重複してTweenが再生されるのを防ぐために一度キルする
        floatTween?.Kill();

        // 初期位置を確実にリセット（非表示時に位置がズレたまま終わっていた場合の対策）
        Vector2 pos = iconRectTransform.anchoredPosition;
        pos.y = defaultYPos;
        iconRectTransform.anchoredPosition = pos;

        // 上下のループアニメーションを開始
        floatTween = iconRectTransform
            .DOAnchorPosY(defaultYPos + moveDistance, duration)
            .SetLoops(-1, LoopType.Yoyo) // -1で無限ループ、Yoyoで「行って戻る」を繰り返す
            .SetEase(Ease.InOutSine);    // InOutSineを使うとフワッとした滑らかな動きになります
    }

    /// <summary>
    /// 次のメッセージへ進む（クリック/タップされた）際に呼び出すメソッド
    /// </summary>
    public void HideIcon()
    {
        // Tweenを止める
        floatTween?.Kill();
    }

    private void OnDestroy()
    {
        // オブジェクトが破棄される際にTweenも確実に破棄する（メモリリーク防止）
        floatTween?.Kill();
    }
}
