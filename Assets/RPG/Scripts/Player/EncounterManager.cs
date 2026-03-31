using Cysharp.Threading.Tasks;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [Header("エンカウンター設定")]
    [SerializeField] private LayerMask encountLayer;
    [SerializeField] private int minSafeSteps = 5;
    [SerializeField] private int maxSafeSteps = 20;
    [SerializeField] private float encounterChance = 0.1f;

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

        if (EncounterEffect.instance != null)
        {
            EncounterEffect.instance.PlayEncountEffect("BattleScene").Forget();
        }
    }
}
