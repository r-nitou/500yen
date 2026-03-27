using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance { get; set; }

    private void Awake()
    {
        if (instance == null)
        {
            //シーンをまたいで存在する
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //RPG用シーン遷移処理
    public async UniTaskVoid ExcuteDirectionTransition(SceneTransitionTrigger trigger,PlayerMove player)
    {
        //プレイヤーの操作停止
        player.InputAction.Player.Disable();

        //シーン遷移
        await SceneManager.LoadSceneAsync(trigger.TargetSceneName);
        Debug.Log($"{trigger.TargetSceneName}に行きます");

        //プレイヤーの操作再開
        player.InputAction.Player.Enable();
    }
}
