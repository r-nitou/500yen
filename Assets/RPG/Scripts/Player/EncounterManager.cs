using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RangeAttribute = UnityEngine.RangeAttribute;

[System.Serializable]
public class SpawnCountWeight
{
    [Range(0, 100)] public int count1 = 60;
    [Range(0, 100)] public int count2 = 25;
    [Range(0, 100)] public int count3 = 10;
    [Range(0, 100)] public int count4 = 4;
    [Range(0, 100)] public int count5 = 1;
}

public class EncounterManager : MonoBehaviour
{
    [Header("エンカウンター設定")]
    [SerializeField] private LayerMask encountLayer;
    [SerializeField] private int minSafeSteps = 5;
    [SerializeField] private int maxSafeSteps = 20;
    [SerializeField] private float encounterChance = 0.1f;
    [SerializeField] private EncounterTable encounterTable;

    private int currentStepCount = 0;

    //エンカウントするか判定する処理
    public void CheckEncounter()
    {
        //プレイヤーがエンカウントレイヤーにいるか判定
        if (!IsEncounterArea())
        {
            return;
        }

        currentStepCount++;

        if (currentStepCount < minSafeSteps)
        {
            return;
        }

        //遭遇確立を1歩ごとに増やす
        float chance = encounterChance + (float)(currentStepCount - minSafeSteps) * 0.05f;

        //安全歩数を超えて、ランダム値がエンカウント確立以下ならエンカウント
        if (Random.value < chance || currentStepCount >= maxSafeSteps)
        {
            OccurEncounter();
        }
    }

    //エンカウントエリアにいるか判定するレイヤー
    private bool IsEncounterArea()
    {
        Collider2D hit = Physics2D.OverlapPoint(transform.position, encountLayer);

        //エンカウントレイヤーにいる場合はtrue、いない場合はfalseを返す
        return hit != null;
    }

    //エンカウント発生処理
    private void OccurEncounter()
    {
        Debug.Log("<color=red>エンカウント発生！</color>");
        //歩数をリセット
        currentStepCount = 0;

        //抽選開始
        List<EnemyData> result = DecideEnemies();

        //エンカウントした場所を記録
        string currentScene = SceneManager.GetActiveScene().name;
        Vector3 currentPos = transform.position;

        //GameManagerに情報を記録
        if (GameManager.instance != null)
        {
            GameManager.instance.SetEncountData(
                currentScene,
                currentPos,
                result,
                "",
                Vector3.zero
                );
        }

        if (EncounterEffect.instance != null)
        {
            EncounterEffect.instance.PlayEncountEffect("BattleScene").Forget();
        }
    }

    //出現する敵を抽選する処理
    public List<EnemyData> DecideEnemies()
    {
        //EnemyDataリストのインスタンス作成
        List<EnemyData> selectedEnemies = new List<EnemyData>();

        //出現数を抽選
        int count = GetRandomSpawnCount(encounterTable.spawnWeights);

        //出現する敵の数を抽選
        for (int i = 0; i < count; i++) 
        {
            int randomindex = Random.Range(0, encounterTable.enemyPool.Count);
            selectedEnemies.Add(encounterTable.enemyPool[randomindex]);
        }

        return selectedEnemies;
    }

    //敵との遭遇数を抽選する処理
    private int GetRandomSpawnCount(SpawnCountWeight weight)
    {
        int roll = Random.Range(0, 100);

        if (roll < weight.count1) return 1;
        roll -= weight.count1;

        if (roll < weight.count2) return 2;
        roll -= weight.count2;

        if (roll < weight.count3) return 3;
        roll -= weight.count3;

        if (roll < weight.count4) return 4;
        roll -= weight.count4;

        return 5;
    }
}
