using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    //共通のフェード処理
    //フェードアウト
    public async UniTask FadeOut(float duration, Image target, Color fadeColor = default)
    {
        if (target == null) return;
        if (fadeColor == default) fadeColor = Color.black;

        target.gameObject.SetActive(true);
        target.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);

        await target.DOFade(1f, duration).SetEase(Ease.Linear).ToUniTask();
    }

    //フェードイン
    public async UniTask FadeIn(float duration, Image target)
    {
        if (target == null) return;

        await target.DOFade(0f, duration).SetEase(Ease.Linear).ToUniTask();
        target.gameObject.SetActive(false);
    }

    //スライドイン
    public async UniTask SlideIn(float duration, Image target)
    {
        if (target == null) return;

        RectTransform rectTransform = target.rectTransform;

        //初期位置を画面の右外にする
        float screenWidth = Screen.width;
        rectTransform.anchoredPosition = new Vector2(screenWidth, 0);
        target.color = new Color(0, 0, 0, 1);
        target.gameObject.SetActive(true);

        //中央にスライドインさせる
        await rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutQuart).ToUniTask();
    }
}
