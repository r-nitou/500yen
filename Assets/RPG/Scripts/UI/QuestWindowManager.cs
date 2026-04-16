using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class QuestWindowManager : MonoBehaviour
{
    [Header("オブジェクト参照")]
    [SerializeField] private GameObject questMenuObject;
    [SerializeField] private GameObject questMenuSlotPrefab;
    [SerializeField] private Transform slotContainer;

    //依頼ウィンドウを開く
    public void OpenQuest()
    {
        questMenuObject.SetActive(true);
        RefreshList();
    }

    //依頼ウィンドウを閉じる
    public void CloseQuest()
    {
        questMenuObject.SetActive(false);
    }
    
    //依頼スロットを生成
    public void RefreshList()
    {
        //既存のスロットを削除
        foreach(Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        //現在受けているクエストを取得
        List<QuestStatus> activeQuest = QuestManager.instance.ActiveQuests;
        foreach(var status in activeQuest)
        {
            GameObject slotObj = Instantiate(questMenuSlotPrefab, slotContainer);
            QuestMenuSlot slot = slotObj.GetComponent<QuestMenuSlot>();
            slot.SetUp(status);
        }
    }
}
