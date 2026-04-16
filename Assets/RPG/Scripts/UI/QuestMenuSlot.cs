using TMPro;
using UnityEngine;

public class QuestMenuSlot : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private GameObject clearObject;

    //スロット
    public void SetUp(QuestStatus status)
    {
        questNameText.text = status.data.questName;
        enemyNameText.text = status.data.targetEnemy.enemyName;
        //進捗表示
        countText.text = $"{status.currentCount}/{status.data.requiredCount}";

        //クリア済み表示
        if (status.IsCleared)
        {
            clearObject.SetActive(true);
            countText.color = Color.yellow;
        }
        else
        {
            clearObject.SetActive(false);
            countText.color = Color.white;
        }
    }
}
