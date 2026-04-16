using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class VillageChiefADVManager : MonoBehaviour
{
    [Header("スクリプトの参照")]
    [SerializeField] private ADVPlayer aDVPlayer;
    [SerializeField] private VillageChiefManager chiefManager;

    [Header("シーン遷移設定")]
    [SerializeField] private string sceneName;
    [SerializeField] private string sceneMarkerId;
    //「村長の家を出る」選択時
    public void OnExitVillageChiefHome()
    {
        ExitHomeTalk().Forget();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartMayorTalk().Forget();
    }

    //村長の家に入ったときの処理
    private async UniTask StartMayorTalk()
    {
        await aDVPlayer.PlayScenario("C3001_CHIEF_ENTER_001");
        chiefManager.OpenCommand();
    }

    //村長の家から出るときの処理
    private async UniTask ExitHomeTalk()
    {
        chiefManager.CloseCommand();
        await aDVPlayer.PlayScenario("C3001_CHIEF_EXIT_001");
        await aDVPlayer.TransitionScene(sceneName, sceneMarkerId);
    }

    //報告できるものがないときのセリフ
    public async UniTask NoReportQuest()
    {
        await aDVPlayer.PlayScenario("C3001_CHIEF_NOREPORT_001");

        //ボタン表示
        chiefManager.OpenCommand();
    }

    //報告後の報酬支払い
    public async UniTaskVoid ExcuteReport(List<string> selectedID)
    {
        //報酬取得
        int totalReward = QuestManager.instance.CompleteSelectedQuests(selectedID);
        if (GameManager.instance != null) 
        {
            GameManager.instance.gold += totalReward;
        }

        //報告完了後セリフ
        await aDVPlayer.PlayScenario("C3001_CHIEF_REPORTSUCCESS_001");
        chiefManager.OpenCommand();
    }
}
