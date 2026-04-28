using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class TutorialData
{
    [Header("チュートリアルID")]
    public string tutorialID;

    [Header("表示する画像")]
    public Sprite tutorialImage;

    [Header("説明文")]
    [TextArea(3, 6)]
    public string description;
}

public class TutorialManager : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private GameObject tutorialUIObject;
    [SerializeField] private Image tutorialImageUI;
    [SerializeField] private TMP_Text descriptionText;

    [Header("チュートリアルデータベース")]
    [SerializeField] private List<TutorialData> tutorialDataBase = new List<TutorialData>();

    private bool isDecided = false;

    //決定ボタン処理
    private void OnSubmit(InputAction.CallbackContext context)
    {
        isDecided = true;
    }

    public async UniTask OpenTutorial(string id,PlayerInputAction input)
    {
        //指定したチュートリアルデータの検索
        TutorialData data = tutorialDataBase.Find(t => t.tutorialID == id);
        if (data == null) return;

        //データをセット
        if (data.tutorialImage != null)
        {
            tutorialImageUI.gameObject.SetActive(true);
            tutorialImageUI.sprite = data.tutorialImage;
        }

        descriptionText.text = data.description;

        //UIを表示
        tutorialUIObject.SetActive(true);

        await UniTask.Yield(PlayerLoopTiming.Update);

        //イベント登録
        input.UI.Submit.performed += OnSubmit;
        isDecided = false;

        try
        {
            await UniTask.WaitUntil(() => isDecided);
        }
        finally
        {
            //イベント解除してUIを閉じる
            input.UI.Submit.performed -= OnSubmit;
            tutorialUIObject.SetActive(false);
        }
    }
}
