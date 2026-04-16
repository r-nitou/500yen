using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletinBoardUI : MonoBehaviour
{
    [Header("リスト表示UI")]
    [SerializeField] private GameObject questSlotPrefab;
    [SerializeField] private Transform questListContainer;
    [SerializeField] private TMP_Text questCountText;

    [Header("詳細表示用UI")]
    [SerializeField] private TMP_Text detailQuestNameText;
    [SerializeField] private TMP_Text targetCountText;
    [SerializeField] private GameObject detailObject;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image targetEnemyImage;
    [SerializeField] private TMP_Text rewaredText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text actionButtonText;

    private QuestData selectedQuest;

    //受注ボタンが押されたときの処理
    public void OnClickAccept()
    {
        if (selectedQuest == null) return;

        //現在この依頼を受けているか
        bool isAlreadyAccepted = QuestManager.instance.ActiveQuests.Any(q => q.data.questID == selectedQuest.questID);
        if (isAlreadyAccepted)
        {
            //解除処理
            QuestManager.instance.CancellQuest(selectedQuest);
        }
        else
        {
            //受注処理
            QuestManager.instance.AcceptQuest(selectedQuest);
        }

        //UI更新
        UpdateButtonState();
        UpdateQuestCountUI();
    }

    //依頼掲示板を閉じるボタン
    public void OnCloseBoard()
    {
        gameObject.SetActive(false);
    }

    //掲示板の初期化処理
    public void InitializeBoard()
    {
        RefreshQuestList();

        //１番上の依頼を初期選択する
        var availableQuests = QuestManager.instance.AllQuests;
        if (availableQuests.Count > 0)
        {
            ShowDetail(availableQuests[0]);
        }
    }

    //リストの生成・更新
    public void RefreshQuestList()
    {
        //既存のスロットの削除
        foreach(Transform child in questListContainer)
        {
            Destroy(child.gameObject);
        }

        //受注可能な依頼を取得
        List<QuestData> allQuests = QuestManager.instance.AllQuests;

        foreach(QuestData data in allQuests)
        {
            GameObject slotObj = Instantiate(questSlotPrefab, questListContainer);
            QuestSlot slot = slotObj.GetComponent<QuestSlot>();
            slot.SetUp(data, this);
        }

        //受注数の表示更新
        UpdateQuestCountUI();
    }

    //詳細表示処理
    public void ShowDetail(QuestData data)
    {
        selectedQuest = data;
        detailObject.SetActive(true);

        detailQuestNameText.text = data.questName;
        targetCountText.text = $"{data.targetEnemy.enemyName}{data.requiredCount}体の撃破";
        descriptionText.text = data.description;
        targetEnemyImage.sprite = data.targetEnemy.enemySprite;
        rewaredText.text = $"報酬金 : {data.rewardGold}";

        //ボタン表示更新
        UpdateButtonState();
    }

    //受注数を更新する処理
    private void UpdateQuestCountUI()
    {
        int current = QuestManager.instance.ActiveQuests.Count;
        questCountText.text = $"受注数 : {current}/4";
    }

    //受注ボタンの表示を更新
    private void UpdateButtonState()
    {
        //現在この依頼を受けているか確認
        bool isAlreadyAccepted = QuestManager.instance.ActiveQuests.Any(q => q.data.questID == selectedQuest.questID);

        if (isAlreadyAccepted)
        {
            actionButtonText.text = "依頼を取り消す";
            actionButton.interactable = true;
        }
        else
        {
            actionButtonText.text = "依頼を受注する";
            actionButton.interactable = QuestManager.instance.ActiveQuests.Count < QuestManager.MAX_QUEST_COUNT;
        }
    }
}
