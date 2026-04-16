using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ReportSelectionUI : MonoBehaviour
{
    [Header("スクリプト参照")]
    [SerializeField] private VillageChiefADVManager chiefADVManager;
    [SerializeField] private VillageChiefManager chiefManager;

    [Header("UI参照")]
    [SerializeField] GameObject ReportWindow;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform container;

    private List<ReportQuestSlot> activeSlot = new List<ReportQuestSlot>();

    //「報告する」ボタンをクリック
    public void OnClickReportConfirm()
    {
        //チェックのついたIDを検索
        List<string> selectedID = activeSlot
            .Where(s => s.IsChecked)
            .Select(s => s.QuestID)
            .ToList();

        //１つ以上チェックがあれば報告実行
        if (selectedID.Count > 0)
        {
            ReportWindow.SetActive(false);
            //報酬支払
            chiefADVManager.ExcuteReport(selectedID).Forget();
        }
        else
        {
            return;
        }
    }

    //閉じるボタン
    public void OnClickReportCancel()
    {
        ReportWindow.SetActive(false);

        chiefManager.OpenCommand();
    }

    //報告ウィンドウを開く
    public void OnOpenReport(List<QuestStatus> clearQuest)
    {
        ReportWindow.SetActive(true);

        //既存のスロットの破棄
        foreach(Transform child in container)
        {
            Destroy(child.gameObject);
        }
        activeSlot.Clear();

        //達成済みの依頼を並べる
        foreach(var quest in clearQuest)
        {
            GameObject slotObject = Instantiate(slotPrefab, container);
            ReportQuestSlot slot = slotObject.GetComponent<ReportQuestSlot>();

            //スロットにデータをセット
            slot.SetUp(quest);
            activeSlot.Add(slot);
        }
    }
}
