using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroineCutInManager : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private RectTransform heroineImageRect;
    [SerializeField] private GameObject supportTextObject;
    [SerializeField] private TMP_Text supportText;

    [Header("位置指定用オブジェクト")]
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;

    [Header("演出設定")]
    [SerializeField] private float slideDuration = 1f;
    [SerializeField] private float fadeDuration = 0.7f;
    [SerializeField] private int displayDelayTime = 1000;

    private void Awake()
    {
        heroineImageRect.gameObject.SetActive(false);
        supportTextObject.SetActive(false);
    }

    //ヒロインのカットイン演出を開始する処理
    public async UniTask PlayCutIn(int affectionLevel)
    {
        //セットアップ
        heroineImageRect.position = startPosition.position;

        Image heroineImage = heroineImageRect.GetComponent<Image>();
        heroineImage.color = new(1, 1, 1, 0);

        heroineImageRect.gameObject.SetActive(true);
        supportTextObject.SetActive(false);

        //テキスト内容設定
        supportText.text = "お兄ちゃん\nがんばって!";

        //スライドイン演出
        var slideInTask = heroineImageRect.DOMove(endPosition.position, slideDuration).ToUniTask();
        var fadeInTask = heroineImage.DOFade(1f, fadeDuration).ToUniTask();
        await UniTask.WhenAll(slideInTask, fadeInTask);

        //テキスト表示
        supportTextObject.SetActive(true);
        await UniTask.Delay(displayDelayTime);
        supportTextObject.SetActive(false);

        //スライドアウト演出
        var slideOutTask = heroineImageRect.DOMove(startPosition.position, slideDuration).ToUniTask();
        var fadeOutTask = heroineImage.DOFade(0f, fadeDuration).ToUniTask();
        await UniTask.WhenAll(slideOutTask, fadeOutTask);

        heroineImageRect.gameObject.SetActive(false);
    }
}
