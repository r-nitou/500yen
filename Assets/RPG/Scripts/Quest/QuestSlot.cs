using TMPro;
using UnityEngine;

public class QuestSlot : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private TMP_Text questNameText;
    private QuestData myQuestData;
    private BulletinBoardUI uiManager;

    //初期設定
    public void SetUp(QuestData data, BulletinBoardUI manager)
    {
        myQuestData = data;
        uiManager = manager;
        questNameText.text = data.questName;
    }

    //クリック時に詳細を表示するボタン
    public void OnClickSlot()
    {
        uiManager.ShowDetail(myQuestData);
    }
}
