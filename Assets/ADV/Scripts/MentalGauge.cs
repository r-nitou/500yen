using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MentalGauge : MonoBehaviour
{
    [SerializeField, Header("--- 以下、子オブジェ ---")]
    // 変更点: Slider から Image へ変更
    private Image gauge_ = null;
    [SerializeField]
    private TextMeshProUGUI text_ = null;

    // ゲージ補間用
    private float valueBegin_ = 0f;
    private float valueTarget_ = 0f;
    private float valueMoveTimer_ = -1.0f;
    private const float VALUE_DURATION = 1.5f;

    // ゲージの値とテキストを同時に更新するためのプロパティ

    public float Value { get => valueTarget_; set => SetValue(value); }
    public string Text { get => text_.text; set => text_.text = value; }


    private void Update()
    {
        UpdateValue();
    }

    private void SetValue(float value)
    {
        // 変更点: gauge_.value から gauge_.fillAmount へ変更
        valueBegin_ = gauge_.fillAmount;
        valueTarget_ = value;
        valueMoveTimer_ = 0.0f;
    }

    public void SetValueForce(float value)
    {
        gauge_.fillAmount = value;
        valueBegin_ = value;
        valueTarget_ = value;
        valueMoveTimer_ = -1.0f;
    }

    private void UpdateValue()
    {
        // ゲージの値を徐々に変化させる
        if (valueMoveTimer_ >= 0.0f)
        {
            valueMoveTimer_ += Time.deltaTime;
            if (valueMoveTimer_ >= VALUE_DURATION)
            {
                // 変更点: gauge_.value から gauge_.fillAmount へ変更
                gauge_.fillAmount = Mathf.Lerp(gauge_.fillAmount, valueTarget_, 1.0f);
                valueMoveTimer_ = -1.0f;
                return;
            }

            // 変更点: gauge_.value から gauge_.fillAmount へ変更
            gauge_.fillAmount = Mathf.Lerp(valueBegin_, valueTarget_, valueMoveTimer_ / VALUE_DURATION);
        }
    }
}