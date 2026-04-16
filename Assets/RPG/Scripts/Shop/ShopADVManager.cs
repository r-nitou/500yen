using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShopADVManager : MonoBehaviour
{
    [Header("スクリプトの参照")]
    [SerializeField] private ADVPlayer aDVPlayer;
    [SerializeField] private ShopManager shopManager;

    [Header("シーン遷移設定")]
    [SerializeField] private string sceneName;
    [SerializeField] private string sceneMarkerId;
    //「店を出る」コマンドクリック時
    public void OnExitShop()
    {
        ExitShop().Forget();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartShopTalk().Forget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //道具屋に入ったときに流れるADV
    public async UniTask StartShopTalk()
    {
        await aDVPlayer.PlayScenario("C2001_SHOP_ENTER_001"); 
        //コマンド表示
        shopManager.OpenShopCommand();
    }

    //道具屋から出るときに流れるADV
    public async UniTask ExitShop()
    {
        shopManager.CloseShopCommand();
        //終了セリフ再生
        await aDVPlayer.PlayScenario("C2002_SHOP_EXIT_001");
        //村に戻る
        await aDVPlayer.TransitionScene(sceneName, sceneMarkerId); 
    }
}
