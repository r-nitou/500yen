using Cysharp.Threading.Tasks;
using UnityEngine;

public class VillageChiefManager : MonoBehaviour
{
    [Header("スクリプト参照")]
    [SerializeField] private VillageChiefADVManager chiefADVManager;
    [SerializeField] private ReportSelectionUI reportSelectionUI;
    [Header("UI参照")]
    [SerializeField] private GameObject homeCommandRoot;

    //報告するボタンをクリックした時
    public void OnClickReport()
    {
        //コマンド非表示
        CloseCommand();

        //達成済みの依頼のみ
        var clearedQuests = QuestManager.instance.ActiveQuests.FindAll(q => q.IsCleared);

        //達成済みの依頼があればウィンドウを開く
        if (clearedQuests.Count > 0) 
        {
            reportSelectionUI.OnOpenReport(clearedQuests);
        }
        else
        {
            chiefADVManager.NoReportQuest().Forget();
        }
    }
    
    //コマンドを表示する処理
    public void OpenCommand()
    {
        homeCommandRoot.SetActive(true);
    }

    public void CloseCommand()
    {
        homeCommandRoot.SetActive(false);
    }
}
