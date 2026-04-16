using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public const int MAX_QUEST_COUNT = 4;

    public static QuestManager instance;

    [Header("すべての依頼")]
    [SerializeField] private List<QuestData> allQuestData = new List<QuestData>();
    [Header("現在受注中の依頼")]
    [SerializeField] private List<QuestStatus> activeQuest = new List<QuestStatus>();

    public List<QuestData> AllQuests => allQuestData;
    public List<QuestStatus> ActiveQuests => activeQuest;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //依頼の受注システム
    public bool AcceptQuest(QuestData data)
    {
        //最大数受けているかチェック
        if (activeQuest.Count >= MAX_QUEST_COUNT)
        {
            Debug.Log("依頼はこれ以上受けられません");
            return false;
        }

        //受注済みかチェック
        if (activeQuest.Any(q => q.data.questID == data.questID))
        {
            Debug.Log("すでに受注済みです");
            return false;
        }

        //新しく追加
        activeQuest.Add(new QuestStatus(data));
        Debug.Log($"依頼を受けました : {data.questName}");

        return true;
    }

    //依頼解除システム
    public void CancellQuest(QuestData data)
    {
        //現在受けている依頼か検索
        QuestStatus target = activeQuest.Find(q => q.data.questID == data.questID);
        if (target != null)
        {
            activeQuest.Remove(target);
        }
    }

    //討伐カウントの更新処理
    public void OnEnemyKilled(EnemyData enemy)
    {
        foreach(var quest in activeQuest)
        {
            if (quest.data.targetEnemy == enemy && !quest.IsCleared)
            {
                quest.currentCount++;
                //撃破数の表示
                Debug.Log($"{quest.data.questName}:{quest.currentCount}/{quest.data.requiredCount}");
                if (quest.IsCleared)
                {
                    Debug.Log($"{quest.data.questName}を達成しました!");
                }
            }
        }
    }

    //依頼の報告処理
    public int CompleteSelectedQuests(List<string> selectedQuestId)
    {
        int totalReward = 0;

        foreach(string id in selectedQuestId)
        {
            QuestStatus quest = activeQuest.Find(q => q.data.questID == id);
            if (quest != null)
            {
                totalReward += quest.data.rewardGold;
                //受注済みリストから解除
                activeQuest.Remove(quest);
            }
        }
        return totalReward;
    }

    public void ReportQuest(QuestStatus quest)
    {
        if (quest.IsCleared && !quest.isReported)
        {
            //報告済みにする
            quest.isReported = true;
            //お金の加算
            GameManager.instance.gold += quest.data.rewardGold;
            //リストから削除
            activeQuest.Add(quest);
        }
    }

    //まだ受けていない依頼リストを取得する処理
    public List<QuestData> GetAvailableQuests()
    {
        //全リストの中から現在受注中でないものを返す
        return allQuestData.Where(q => !activeQuest.Any(active => active.data.questID == q.questID)).ToList();
    }
}
