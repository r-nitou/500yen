using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public static LogManager instance { get; set; }

    [Header("ログ設定")]
    [SerializeField] private GameObject battleLog;
    [SerializeField] private TMP_Text logText;
    [SerializeField] private float textSpeed = 0.05f;

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

        //ログ初期設定
        if (logText != null)
        {
            battleLog.SetActive(false);
            logText.text = "";
        }
    }

    //文字を1文字ずつ表示する処理
    public async UniTask DisplayLogText(string message)
    {
        //バトルログ表示
        battleLog.SetActive(true);
        logText.text = "";

        //1文字ずつ表示
        foreach(char c in message)
        {
            logText.text += c;
            await UniTask.Delay((int)(textSpeed * 1000));
        }

        //表示完了後少し待たせる
        await UniTask.Delay(500);

        //バトルログ非表示
        battleLog.SetActive(false);
    }
}
