using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Unity.Cinemachine;
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
    public async UniTask ExcuteSceneTransition(string trigger, string markerId, PlayerMove player)
    {
        //プレイヤーの操作停止
        player.InputAction.Player.Disable();

        //時間の変更
        UpdatePhase(trigger);

        //シーン遷移
        await SceneManager.LoadSceneAsync(trigger);
        Debug.Log($"{trigger}に行きます");

        //指定されたIDの場所にキャラクターを配置する
        SetCharacterToMarker(markerId, player);

        //プレイヤーの操作を再開
        player.InputAction.Player.Enable();
    }

    //バトルから復帰するための処理
    public async UniTask ExcuteReturnScene(string sceneName, Vector3 targetPosition, PlayerMove player)
    {
        //プレイヤーの操作停止
        player.InputAction.Player.Disable();

        //シーン遷移
        await SceneManager.LoadSceneAsync(sceneName);

        SetUpSceneEntities(player, targetPosition, Vector2.down);

        //プレイヤー操作再開
        player.InputAction.Player.Enable();
    }

    //キャラクターを指定の位置に飛ばす処理
    public void SetCharacterToMarker(string markerId, PlayerMove player)
    {
        //シーン内の全マーカーからIDが一致するものを検索
        SceneMarker[] markers = GameObject.FindObjectsByType<SceneMarker>(FindObjectsSortMode.None);
        SceneMarker targetMarker = markers.FirstOrDefault(m => m.MarkerId == markerId);

        //RPGありシーン
        if (targetMarker != null)
        {
            SetUpSceneEntities(player, targetMarker.transform.position, targetMarker.LookDirection);
        }
        //ADVシーン
        else
        {
            HideEntities(player);
        }
    }

    //キャラクター、カメラの再設定を行う処理
    private void SetUpSceneEntities(PlayerMove player,Vector3 position,Vector2 direction)
    {
        //プレイヤー設定
        player.gameObject.SetActive(true);
        player.transform.position = position;
        player.SetDirection(direction);

        //仲間の設定
        FollowerMove follower = FollowerMove.instance;
        if (follower != null)
        {
            follower.gameObject.SetActive(true);
            follower.transform.position = position;
            follower.SetDirection(direction);
        }

        //カメラの再設定
        var vcam = GameObject.FindAnyObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            vcam.Follow = player.transform;
            vcam.ForceCameraPosition(position, Quaternion.identity);
        }
    }

    //キャラクターを非表示にする
    private void HideEntities(PlayerMove player)
    {
        player.gameObject.SetActive(false);
        FollowerMove follower = FollowerMove.instance;
        if (follower != null)
        {
            follower.gameObject.SetActive(false);
        }
    }

    //行先に応じて時間を変える処理
    private void UpdatePhase(string nextSceneName)
    {
        if (GameManager.instance == null && nextSceneName == null) return;

        //シーン名に基づいて時間を切り替える
        if (nextSceneName == "CaveScene") 
        {
            //ダンジョンのときは昼
            GameManager.instance.currentPhase = DayPhase.DayTime;
        }
        else if (nextSceneName == "VillageScene")
        {
            //今が昼なら夜にする
            if(GameManager.instance.currentPhase== DayPhase.DayTime)
            {
                GameManager.instance.currentPhase = DayPhase.Night;
            }
            else
            {
                return;
            }
        }
    }
}

