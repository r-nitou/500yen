using TMPro;
using UnityEngine;

public class YoungerEventButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI costText_ = null;
    [SerializeField]
    private int eventCost_ = 10;
    public int EventCost => eventCost_;

    private void Start()
    {
        if (costText_ != null)
        {
            SetCostText();
        }
    }

    public void SetCostText() 
    {
        costText_.text = EventCost.ToString();
    }
}
