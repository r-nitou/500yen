using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneBGMData
{
    public string sceneName;
    public AudioClip bgmClip;
}
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("BGM設定")]
    [SerializeField] private List<SceneBGMData> sceneBGMList = new List<SceneBGMData>();
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float defaultVolume = 0.5f;

    private AudioSource audioSource;
    private AudioClip currentClip;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            //初期設定
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0;
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

    //シーンが読み込まれたときに呼ばれる処理
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //リストの中から現在のシーンと一致するシーン名を検索する
        SceneBGMData data = sceneBGMList.Find(d => d.sceneName == scene.name);

        if (data != null && data.bgmClip != null)
        {
            PlayBGM(data.bgmClip);
        }
    }

    //指定したBGMをフェード付きで再生する処理
    public void PlayBGM(AudioClip clip)
    {
        //同じ曲が流れている場合は何もしない
        if (audioSource.clip == clip && audioSource.isPlaying) return;

        //別の曲の場合はフェードしてから切り替える
        Sequence bgmSequence = DOTween.Sequence();
        //現在流れているBGMをフェードアウト
        bgmSequence.Append(audioSource.DOFade(0, fadeDuration));

        //曲を入れ替える
        bgmSequence.AppendCallback(() =>
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        });
        //新しい曲をフェードイン
        bgmSequence.Append(audioSource.DOFade(defaultVolume, fadeDuration));
    }

    //BGMを観戦に停止する
    public void StopBGM()
    {
        audioSource.DOFade(0f, fadeDuration).OnComplete(() => audioSource.Stop());
    }
}
