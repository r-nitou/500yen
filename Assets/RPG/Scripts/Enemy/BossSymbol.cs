using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossSymbol : MonoBehaviour
{
    [Header("ボス設定")]
    [SerializeField] private string symbolId;
    [SerializeField] private string battleSceneName = "BattleScene";
    [SerializeField] private List<EnemyData> enemyList;
    [SerializeField, TextArea(3, 10)]
    private string[] openingDialogues;

    private bool isInteracting = false;
    private Vector3 initializePosition;

    private void Awake()
    {
        initializePosition = transform.position;
        if (SymbolEncounterManager.instance.defeatedSymbolId.Contains(symbolId))
        {
            gameObject.SetActive(false);
        }
    }

    //ボス戦開始時のイベント処理
    private async UniTask StartBossEvent()
    {
        //入力切替
        GlobalUIManager.instance.SwitchToUIInput();

        //会話開始
        var input = PlayerMove.instance.InputAction;
        await GlobalUIManager.instance.ShowBossMessage(openingDialogues);

        //バトル開始
        //にげられなくする
        GameManager.instance.isEscapeDisabled = true;

        Vector3 returnPosition = PlayerMove.instance.transform.position;

        GameManager.instance.SetEncountData(
            SceneManager.GetActiveScene().name,
            returnPosition,
            enemyList,
            symbolId,
            initializePosition
        );

        PlayerMove.instance.InputAction.Disable();
        await EncounterEffect.instance.PlayEncountEffect(battleSceneName);

        isInteracting = false;
    }

    private async UniTask OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isInteracting)
        {
            isInteracting = true;
            await StartBossEvent();
        }
    }
}
