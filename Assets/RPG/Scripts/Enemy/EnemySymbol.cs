using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySymbol : MonoBehaviour
{
    [Header("出現する敵のリスト")]
    [SerializeField] private List<EnemyData> enemyList = new List<EnemyData>();

    [Header("バトル設定")]
    [SerializeField] private string enemyGroupId = "";
    [SerializeField] private string battleSceneName = "BattleScene";

    private bool isEncount = false;
    private Vector3 initialPosition;
    private Vector3 returnPosition;

    private void Awake()
    {
        initialPosition = this.transform.position;

        //撃破済みリストの中にいたら削除
        if (SymbolEncounterManager.instance != null && SymbolEncounterManager.instance.defeatedSymbolId.Contains(enemyGroupId))
        {
            gameObject.SetActive(false);
            //宝箱を表示する
            SpawnReplacementChest();
        }
    }

    private void StartSymbolEncounter()
    {
        isEncount = true;
        Debug.Log($"<color=yellow>シンボルエンカウント</color>: {enemyGroupId}");
        if (GameManager.instance != null)
        {
            GameManager.instance.SetEncountData(
                SceneManager.GetActiveScene().name,
                returnPosition,
                enemyList,
                enemyGroupId,
                initialPosition
                );
        }

        //プレイヤーの操作を無効化
        PlayerMove.instance.InputAction.Disable();

        EncounterEffect.instance.PlayEncountEffect(battleSceneName).Forget();
    }

    //中ボスの代わりに宝箱の生成
    private void SpawnReplacementChest()
    {
        //宝箱の生成
        GameObject chestObj = Instantiate(SymbolEncounterManager.instance.treasureBoxPrefab, 
            initialPosition, Quaternion.identity);
        TreasureBox chest = chestObj.GetComponent<TreasureBox>();

        if (chest != null)
        {
            //開封済みかどうか確認
            bool opened = SymbolEncounterManager.instance.openedChestIds.Contains(enemyGroupId);
            ItemData drop = (enemyList.Count > 0) ? enemyList[0].dropItem : null;
            //宝箱を初期化
            chest.Initialize(enemyGroupId, drop, opened);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.instance != null && GameManager.instance.isInvincible) 
        {
            return;
        }

        if (collision.CompareTag("Player") && !isEncount)
        {
            if (enemyList.Count == 0)
            {
                Debug.LogWarning("敵データがセットされていない");
            }

            //プレイヤーの現在位置を復帰ポイントにする
            returnPosition = collision.transform.position;
            StartSymbolEncounter();
        }
    }
}
