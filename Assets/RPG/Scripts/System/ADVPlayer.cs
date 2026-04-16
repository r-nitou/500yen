using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Utage;
using UtageExtensions;

public class ADVPlayer : MonoBehaviour
{
    [Header("Utageの参照")]
    public AdvEngine advEngine;

    [Header("スクリプトの参照")]
    [SerializeField] private ShopManager shopManager;

    [Header("フェード処理用UI")]
    [SerializeField] private Image fadeTarget;
    [Header("シーン遷移設定")]
    [SerializeField] private string sceneName;
    [SerializeField] private string sceneMarkerId;

    private bool isTransitioning = false;
    private string currentLabel = "";

    //指定した位置から再生する処理
    public void JumpToLabel(string labelName)
    {
        if (advEngine == null) return;
        //指定したラベルから再生
        advEngine.JumpScenario(labelName);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        JumpToLabel("C2001_SHOP_ENTER_001");
    }

    // Update is called once per frame
    void Update()
    {
        //currentLabelが空ではないときラベルを更新する
        if (!string.IsNullOrEmpty(advEngine.Page.ScenarioLabel)) 
        {
            currentLabel = advEngine.Page.ScenarioLabel;
        }

        //シナリオが終了したかチェック
        if (advEngine.IsEndScenario)
        {
            if (currentLabel == "C2002_SHOP_EXIT_001")
            {
                if (isTransitioning) return;
                currentLabel = "";
                ReturnVillage().Forget();
            }
            else if (currentLabel == "C2001_SHOP_ENTER_001")
            {
                if (!shopManager.IsShopUIActive)
                {
                    shopManager.OpenShopCommand();
                    currentLabel = "";
                }
            }
        }
    }

    //村シーンに戻る処理
    private async UniTask ReturnVillage()
    {
        isTransitioning = true;
        //フェードアウト
        await GameManager.instance.Fade.FadeOut(2.0f, fadeTarget);
        //シーン遷移
        await SceneLoader.instance.ExcuteSceneTransition(sceneName, sceneMarkerId, PlayerMove.instance);
        isTransitioning = false;
    }
}
