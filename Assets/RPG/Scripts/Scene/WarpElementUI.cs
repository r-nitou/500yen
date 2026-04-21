using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarpElementUI : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private TMP_Text floorText;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    public FloorData Data { get; set; }

    //階層データをセットする処理
    public void SetData(FloorData data)
    {
        Data = data;
        floorText.text = data.floorName;
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        floorText.color = selected ? selectedColor : defaultColor;
    }
}
