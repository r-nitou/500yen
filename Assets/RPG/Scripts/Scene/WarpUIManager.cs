using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarpUIManager : MonoBehaviour
{
    [Header("プレハブ設定")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject warpPrefab;

    private List<WarpElementUI> elements = new List<WarpElementUI>();
    private int selectedIndex;

    private bool isDecided = false;
    private bool isCancelled = false;
    private FloorData selectedData = null;

    //入力イベント用関数
    //選択用処理
    private void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input.y > 0.5f)
        {
            ChangeSelection(-1);
        }
        else if (input.y < 0.5)
        {
            ChangeSelection(1);
        }
    }
    //決定用処理
    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (elements.Count > 0)
        {
            selectedData = elements[selectedIndex].Data;
            isDecided = true;
        }
    }
    //キャンセル用処理
    private void OnCancel(InputAction.CallbackContext context)
    {
        selectedData = null;
        isCancelled = true;
    }
    //ウィンドウを開いて選択結果を返す処理
    public async UniTask<FloorData> OpenAndSelect(PlayerInputAction input)
    {
        gameObject.SetActive(true);
        InitializeList();

        selectedIndex = 0;
        UpdateSelection();
        isDecided = false;
        isCancelled = false;
        selectedData = null;

        //イベント登録
        input.UI.Submit.performed += OnSubmit;
        input.UI.Cancel.performed += OnCancel;
        input.UI.Navigate.performed += OnNavigate;

        await UniTask.WaitUntil(() => isDecided || isCancelled);

        //イベント解除
        input.UI.Submit.performed -= OnSubmit;
        input.UI.Cancel.performed -= OnCancel;
        input.UI.Navigate.performed -= OnNavigate;

        gameObject.SetActive(false);
        return selectedData;
    }

    //移動可能なところの表示処理
    private void InitializeList()
    {
        //既存のリストの破棄
        foreach(var ele in elements)
        {
            Destroy(ele.gameObject);
        }
        elements.Clear();

        foreach(var floor in GameManager.instance.visitedFloorData)
        {
            GameObject obj = Instantiate(warpPrefab, contentRoot);
            WarpElementUI element = obj.GetComponent<WarpElementUI>();
            element.SetData(floor);
            elements.Add(element);
        }
    }

    //選択している移動先を更新する処理
    private void ChangeSelection(int direction)
    {
        if (elements.Count == 0) return;
        selectedIndex = (selectedIndex + direction + elements.Count) % elements.Count;
        UpdateSelection();
    }

    //見た目をわかりやすくする処理
    private void UpdateSelection()
    {
        for(int i = 0; i < elements.Count; i++)
        {
            elements[i].SetSelected(i == selectedIndex);
        }
    }
}
