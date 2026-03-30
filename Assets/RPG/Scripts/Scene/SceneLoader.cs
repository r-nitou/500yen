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

        //シーン遷移
        await SceneManager.LoadSceneAsync(trigger);
        Debug.Log($"{trigger}に行きます");

        //指定されたIDの場所にキャラクターを配置する
        SetCharacterToMarker(markerId, player);
    }

    //キャラクターを指定の位置に飛ばす処理
    public void SetCharacterToMarker(string markerId, PlayerMove player)
    {
        Debug.Log("呼ばれたよ");

        //シーン内の全マーカーからIDが一致するものを検索
        SceneMarker[] markers = GameObject.FindObjectsByType<SceneMarker>(FindObjectsSortMode.None);
        SceneMarker targetMarker = markers.FirstOrDefault(m => m.MarkerId == markerId);

        //RPGありシーン
        if (targetMarker != null)
        {
            //キャラクターを表示して配置
            player.gameObject.SetActive(true);
            player.transform.position = targetMarker.transform.position;

            //向きをセット
            player.SetDirection(targetMarker.LookDirection);

            //仲間の配置
            FollowerMove follower = FollowerMove.instance;
            if (follower != null)
            {
                follower.gameObject.SetActive(true);

                //位置と向きを合わせる
                follower.transform.position = targetMarker.transform.position;
                follower.SetDirection(targetMarker.LookDirection);
            }

            //カメラのセット
            var vcam = GameObject.FindAnyObjectByType<CinemachineCamera>();
            if (vcam != null)
            {
                vcam.Follow = player.transform;

                vcam.ForceCameraPosition(player.transform.position, Quaternion.identity);
            }

            //プレイヤーの操作を再開
            player.InputAction.Player.Enable();

            Debug.Log("指定の位置に配置した");
        }
        //ADVシーン
        else
        {
            //プレイヤーを非表示
            player.gameObject.SetActive(false);

            //仲間も非表示
            FollowerMove follower = GameObject.FindAnyObjectByType<FollowerMove>();
            if(follower != null)
            {
                follower.gameObject.SetActive(false);
            }

            Debug.Log("非表示にした");
        }
    }
}
