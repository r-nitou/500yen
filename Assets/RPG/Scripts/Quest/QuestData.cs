using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class QuestStatus
{
    public QuestData data;
    public int currentCount;
    public bool isReported;

    //コンストラクタ
    public QuestStatus(QuestData questData)
    {
        data = questData;
        currentCount = 0;
        isReported = false;
    }

    //達成しているかどうか確認するプロパティ
    public bool IsCleared => currentCount >= data.requiredCount;
}

[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/QuestData")]
public class QuestData : ScriptableObject
{
    [Header("基本情報")]
    public string questID;
    public string questName;
    [TextArea]
    public string description;

    [Header("討伐対象")]
    public EnemyData targetEnemy;
    public int requiredCount;

    [Header("報酬設定")]
    public int rewardGold;
}
