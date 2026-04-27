using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CommonMessageWindow : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private GameObject messageWindowObjct;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject nameTagObject;
    [SerializeField] private TMP_Text nameTagText;

    private bool isDecided = false;

    //決定ボタンを押したときの処理
    private void OnSubmit(InputAction.CallbackContext context)
    {
        isDecided = true;
    }

    public async UniTask OpenMessage(string message, PlayerInputAction input, string speaker = "")
    {
        await OpenDialogue(new string[] { message }, input, speaker);
    }

    public async UniTask OpenDialogue(string[] messages, PlayerInputAction input, string speaker = "")
    {
        //ネームタグ表示・非表示切り替え
        UpdateNameTag(speaker);
        //メッセージウィンドウを表示
        messageWindowObjct.SetActive(true);
        //イベント登録
        input.UI.Submit.performed += OnSubmit;

        foreach(string msg in messages)
        {
            //メッセージの表示
            messageText.text = msg;
            isDecided = false;

            await UniTask.WaitUntil(() => isDecided);
        }

        //イベント解除
        input.UI.Submit.performed -= OnSubmit;
        messageWindowObjct.SetActive(false);
    }

    //ネームタグの表示、非表示を切り替える処理
    public void UpdateNameTag(string tag)
    {
        if (string.IsNullOrEmpty(tag))
        {
            if (nameTagObject != null) 
            {
                nameTagObject.SetActive(false);
            }
        }
        else
        {
            if(nameTagObject != null)
            {
                nameTagObject.SetActive(true);
            }
            if (nameTagText != null)
            {
                nameTagText.text = tag;
            }
        }
    }
}
