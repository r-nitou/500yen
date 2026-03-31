using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemySymbol : MonoBehaviour
{
    [Header("バトル設定")]
    [SerializeField] private string enemyGroupId = "";
    [SerializeField] private string battleSceneName = "BattleScene";

    [Header("撃破設定")]
    [SerializeField] private bool destroyOnWin = true;

    private bool isEncount = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartSymbolEncounter()
    {
        isEncount = true;
        
        Debug.Log($"<color=yellow>シンボルエンカウント</color>: {enemyGroupId}");

        //プレイヤーの操作を無効化
        PlayerMove.instance.InputAction.Disable();

        EncounterEffect.instance.PlayEncountEffect(battleSceneName).Forget();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isEncount)
        {
            StartSymbolEncounter();
        }
    }
}
