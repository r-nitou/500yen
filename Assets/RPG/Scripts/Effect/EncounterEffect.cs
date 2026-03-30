using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EncounterEffect : MonoBehaviour
{
    private const int FLASH_COUNT = 3;

    public static EncounterEffect instance { get; set; }

    [Header("エフェクト設定")]
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashDuration = 0.08f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //初期状態は透明
        flashImage.color = new Color(1, 1, 1, 0);
    }

    //エンカウントエフェクトを再生する処理
    public async UniTask PlayEncountEffect(string battleSceneName)
    {
        //プレイヤーの入力を無効化
        PlayerMove.instance.InputAction.Disable();

        //フラッシュエフェクトを再生
        for (int i = 0; i < FLASH_COUNT; i++) 
        {
            await flashImage.DOFade(0.8f, flashDuration).SetEase(Ease.OutExpo);
            await flashImage.DOFade(0f, flashDuration).SetEase(Ease.InExpo);
        }

        //画面を真っ白にする
        await flashImage.DOFade(1.0f, 0.2f);

        //シーン遷移
        await SceneLoader.instance.ExcuteSceneTransition(battleSceneName, "", PlayerMove.instance);

        // 4. 遷移後に白を消す
        await flashImage.DOFade(0f, 0.6f).SetEase(Ease.Linear);
    }
}
