using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveUIManager : MonoBehaviour
{
    public static ObjectiveUIManager instance { get; set; }

    [Header("UI参照")]
    [SerializeField] private RectTransform objectiveRect;
    [SerializeField] private TMP_Text objectiveText;

    [Header("演出用設定")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float hiddenPosX = -250f;
    [SerializeField] private float showPosX = 20f;

    [Header("表示対象シーン")]
    [SerializeField] private string[] targetScene = { "VillageScene", "CaveScene", "CaveScene_B1", "CaveScene_B2" };
    [SerializeField] private float showDelay = 1.0f;

    private string currentObjectiveString = "";
    private bool isShown = false;
    private float idleTimer = 0f;

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
    }

    //初期化関数
    private void Initialize()
    {
        //画面外に配置
        Vector2 startPos = objectiveRect.anchoredPosition;
        startPos.x = hiddenPosX;
        objectiveRect.anchoredPosition = startPos;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //初期化
        Initialize();
    }

    private void Update()
    {
        UpdateVisibility();
    }

    //スライドアニメーション再生処理
    public void PlaySlideAnim(float targetX,Ease easeType)
    {
        //スライドイン
        objectiveRect.DOKill();
        objectiveRect.DOAnchorPosX(targetX, slideDuration)
            .SetEase(easeType);
    }

    //表示非表示の自動判定
    private void UpdateVisibility()
    {
        //シーンの確認
        string activeScene = SceneManager.GetActiveScene().name;
        bool isTargetScene = targetScene.Contains(activeScene);
        bool hasObjective = !string.IsNullOrEmpty(currentObjectiveString);

        //プレイヤーの移動状態の確認
        bool isMoveing = false;
        if (PlayerMove.instance != null)
        {
            isMoveing = PlayerMove.instance.isMoveing;
        }
        //タイマーの更新
        if (isMoveing || !isTargetScene || !hasObjective)
        {
            idleTimer = 0f;
        }
        else
        {
            idleTimer += Time.deltaTime;
        }

        bool shouldShoun = idleTimer >= showDelay;

        //状態が変化したときだけアニメーション再生
        if (shouldShoun && !isShown)
        {
            PlaySlideAnim(showPosX, Ease.OutCubic);
            isShown = true;
        }
        else if (!shouldShoun && isShown)
        {
            PlaySlideAnim(hiddenPosX, Ease.InCubic);
            isShown = false;
        }
    }

    //目的を外からセットする処理
    public async UniTask SetObjective(string newObjective)
    {
        currentObjectiveString = newObjective;
        objectiveText.text = currentObjectiveString;

        isShown = true;
        idleTimer = showDelay;

        objectiveRect.DOKill();
        await objectiveRect.DOAnchorPosX(showPosX, slideDuration)
            .SetEase(Ease.OutCubic).ToUniTask();
    }
}
