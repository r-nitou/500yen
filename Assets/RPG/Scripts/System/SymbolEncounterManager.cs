using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropResultData
{
    public Vector3 spawnTreasureBoxPosition;
    public ItemData dropItem;
    public string symbolId;
}
public class SymbolEncounterManager : MonoBehaviour
{
    public static SymbolEncounterManager instance;

    [Header("シンボル状態管理")]
    public List<string> defeatedSymbolId = new List<string>();
    public string currentSymbolId;
    public bool isSymbolEncounter;

    [Header("宝箱の設定")]
    public GameObject treasureBoxPrefab;
    public List<string> openedChestIds = new List<string>();

    [Header("ドロップ報酬データ")]
    public DropResultData dropResultData;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //エンカウントしたときの情報を保存する処理
    public void SetEncountData(string symbolId, List<EnemyData> enemies, Vector3 position)
    {
        currentSymbolId = symbolId;
        isSymbolEncounter = true;

        //ドロップアイテムの検索
        ItemData item = null;
        foreach(var enemy in enemies)
        {
            if (enemy.dropItem != null)
            {
                item = enemy.dropItem;
                break;
            }
        }

        //ドロップアイテムがあれば
        if (item != null)
        {
            dropResultData = new DropResultData
            {
                spawnTreasureBoxPosition = position,
                dropItem = item,
                symbolId = symbolId
            };
        }
        else
        {
            dropResultData = null;
        }
    }

    //シンボルエネミー撃破時処理
    public void DefeatedSymbolEnemy()
    {
        if (isSymbolEncounter && !string.IsNullOrEmpty(currentSymbolId))
        {
            //撃破済みリストに追加
            if (!defeatedSymbolId.Contains(currentSymbolId))
            {
                defeatedSymbolId.Add(currentSymbolId);
            }
        }

        //フラグとIDをリセット
        isSymbolEncounter = false;
    }

    //開封した宝箱を記録する
    public void RegisterOpenedChest(string symbolId)
    {
        //宝箱をすでに開けていたらリストに追加
        if (!openedChestIds.Contains(symbolId))
        {
            openedChestIds.Add(symbolId);
        }
    }
}
