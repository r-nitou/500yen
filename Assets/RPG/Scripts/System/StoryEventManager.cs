using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueData
{
    [Header("話す人の名前")]
    public string speakerName;
    [Header("セリフ(1要素2行まで)")]
    [TextArea(1,2)]
    public string[] message;
}
public class StoryEventManager : MonoBehaviour
{
    public static StoryEventManager instance;

    [Header("オープニングイベント設定")]
    [SerializeField] private List<DialogueData> openingDialogues = new List<DialogueData>();
    [SerializeField] private Transform[] openingTargets;

    [Header("カメラ演出設定")]
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private float panDuration = 1.5f;
    [SerializeField] private float waitTime = 1.5f;

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
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    //シーン読み込みのたびに呼ばれる
    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        CheckAndPlayEvent();
    }

    private void Start()
    {
        CheckAndPlayEvent();
    }

    //イベント判定処理
    private void CheckAndPlayEvent()
    {
        if (GameManager.instance != null && GameManager.instance.isNewGame)
        {
            GameManager.instance.isNewGame = false;

            FetchSceneReferences();                                  
            PlayOpeningEvent().Forget();
        }
    }

    //オープニングイベントを再生する処理
    public async UniTask PlayOpeningEvent()
    {
        await UniTask.Yield(PlayerLoopTiming.Update);
        if (openingDialogues == null || openingDialogues.Count == 0) return;

        //入力切替
        if (GlobalUIManager.instance != null)
        {
            GlobalUIManager.instance.SwitchToUIInput();
        }

        //設定したセリフを流す
        foreach (var dialogue in openingDialogues) 
        {
            await GlobalUIManager.instance.ShowEventMessage(dialogue.message, dialogue.speakerName);
        }

        //カメラを目的地に動かす
        if (virtualCamera != null && openingTargets != null && openingTargets.Length > 0) 
        {
            await PlayCameraPanEffect(openingTargets);
        }

        await ObjectiveUIManager.instance.SetObjective("大きな家に行く");

        //入力切替
        if (GlobalUIManager.instance != null)
        {
            GlobalUIManager.instance.SwitchToPlayerInput();
        }
    }

    private async UniTask PlayCameraPanEffect(Transform[] targets)
    {
        if (virtualCamera == null || targets == null || targets.Length == 0) return;
        //元の追従対象を取得
        Transform originalFollow = virtualCamera.Follow;
        if (originalFollow == null) return;

        //ダミーオブジェクトの作成
        GameObject dummyTarget = new GameObject("CameraDummyTarget");
        dummyTarget.transform.position = originalFollow.position;
        //追従対象の変更
        virtualCamera.Follow = dummyTarget.transform;

        foreach(Transform target in targets)
        {
            if (target == null) continue;

            //ダミーを目的地まで移動させる
            await dummyTarget.transform.DOMove(target.position, panDuration)
            .SetEase(Ease.InOutQuad)
            .ToUniTask();
            //目的地で待機
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        }

        //カメラ位置を元に戻す
        await dummyTarget.transform.DOMove(originalFollow.position, panDuration)
            .SetEase(Ease.InOutQuad)
            .ToUniTask();

        //追従をカメラに戻してダミーを破棄
        virtualCamera.Follow = originalFollow;
        Destroy(dummyTarget);
    }

    //シーン内のオブジェクトを再取得する処理
    private void FetchSceneReferences()
    {
        //カメラの再取得
        if (virtualCamera == null)
        {
            virtualCamera = UnityEngine.Object.FindFirstObjectByType<CinemachineCamera>();
        }

        //ターゲットの再取得
        GameObject targetsParet = GameObject.Find("Opening");
        if (targetsParet != null)
        {
            List<Transform> targetList = new List<Transform>();
            foreach (Transform child in targetsParet.transform) 
            {
                targetList.Add(child);
            }
            //リストを変換
            openingTargets = targetList.ToArray();
        }
    }
}
