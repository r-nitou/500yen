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
    private Vector3 returnPosition;

    private void Awake()
    {
        //撃破済みリストの中にいたら削除
        if (GameManager.instance != null && GameManager.instance.defeatedSymbolId.Contains(enemyGroupId))
        {
            gameObject.SetActive(false);
        }
    }

    private void StartSymbolEncounter()
    {
        isEncount = true;
        Debug.Log($"<color=yellow>シンボルエンカウント</color>: {enemyGroupId}");
        if (GameManager.instance != null)
        {
            GameManager.instance.SetEncountData(SceneManager.GetActiveScene().name, returnPosition, enemyList, enemyGroupId);

            GameManager.instance.isSymbolEncounter = true;
        }

        //プレイヤーの操作を無効化
        PlayerMove.instance.InputAction.Disable();

        EncounterEffect.instance.PlayEncountEffect(battleSceneName).Forget();
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
