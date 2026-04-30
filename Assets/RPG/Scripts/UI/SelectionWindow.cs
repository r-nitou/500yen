using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionWindow : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text[] menuTexts;

    [Header("カラー設定")]
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    private int currentIndex = 0;

    private bool? result = null;

    private void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 navigate = context.ReadValue<Vector2>();
        if (navigate.x != 0 || navigate.y != 0)
        {
            int direction = (navigate.x > 0 || navigate.y < 0) ? 1 : -1;
            currentIndex = (currentIndex + direction + menuTexts.Length) % menuTexts.Length;
            UpdateVisuals();
        }
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        result = (currentIndex == 0);
    }

    //UIManagerから呼ばれる表示処理
    public async UniTask<bool> OpenWindow(string message, PlayerInputAction inputActions)
    {
        gameObject.SetActive(true);
        messageText.text = message;
        currentIndex = 0;
        result = null;
        UpdateVisuals();

        await UniTask.Yield(PlayerLoopTiming.Update);

        inputActions.UI.Navigate.performed += OnNavigate;
        inputActions.UI.Submit.performed += OnSubmit;

        await UniTask.WaitUntil(() => result != null);

        inputActions.UI.Navigate.performed -= OnNavigate;
        inputActions.UI.Submit.performed -= OnSubmit;

        gameObject.SetActive(false);
        return result.Value;
    }

    //選択状態に合わせて色を更新する
    private void UpdateVisuals()
    {
        for(int i = 0; i < menuTexts.Length; i++)
        {
            menuTexts[i].color = (i == currentIndex) ? selectedColor : defaultColor;
        }
    }
}
