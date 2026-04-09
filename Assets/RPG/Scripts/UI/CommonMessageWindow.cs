using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CommonMessageWindow : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private GameObject messageWindowObjct;
    [SerializeField] private TMP_Text messageText;

    private bool isDecided = false;

    //決定ボタンを押したときの処理
    private void OnSubmit(InputAction.CallbackContext context)
    {
        isDecided = true;
    }

    public async UniTask OpenMessage(string message,PlayerInputAction input)
    {
        //メッセージウィンドウを表示
        messageText.text = message;
        messageWindowObjct.SetActive(true);
        isDecided = false;

        //イベント登録
        input.UI.Submit.performed += OnSubmit;

        await UniTask.WaitUntil(() => isDecided);

        //イベント解除
        input.UI.Submit.performed -= OnSubmit;
        messageWindowObjct.SetActive(false);
    }
}
