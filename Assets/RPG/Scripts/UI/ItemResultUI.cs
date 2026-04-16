using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemResultUI : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private GameObject resultMenu;
    [SerializeField] private TMP_Text resultText;

    private Action onClosed;
    private bool isActive = false;

    //アイテム使用の結果を表示する処理
    public void ShowResult(string message,Action callback)
    {
        resultMenu.SetActive(true);
        resultText.text = message;
        onClosed = callback;
        isActive = true;
    }

    //決定入力を受け取る処理
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!isActive && !context.performed) return;

        isActive = false;
        resultMenu.SetActive(false);
        onClosed?.Invoke();
    }
}
