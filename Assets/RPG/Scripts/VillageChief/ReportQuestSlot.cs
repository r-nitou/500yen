using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportQuestSlot : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private Toggle reportToggle;

    private string questID;
    public string QuestID => questID;
    public bool IsChecked => reportToggle.isOn;
   
    //プレハブ初期設定
    public void SetUp(QuestStatus status)
    {
        questID = status.data.questID;
        questNameText.text = status.data.questName;
        reportToggle.isOn = false;
    }
}
