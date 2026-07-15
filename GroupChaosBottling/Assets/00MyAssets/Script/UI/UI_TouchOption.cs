using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerValue;
public class UI_TouchOption : MonoBehaviour
{
    [SerializeField] Slider AtkUISizeSlider;
    [SerializeField] TextMeshProUGUI AtkUISizeValTx;
    [SerializeField] Slider MoveStickSizeSlider;
    [SerializeField] TextMeshProUGUI MoveStickSizeValTx;
    [SerializeField] Slider JumpDashUISizeSlider;
    [SerializeField] TextMeshProUGUI JumpDashUISizeValTx;

    private void Start()
    {
        AtkUISizeSlider.value = PSaves.AtkUISize / 100f;
        MoveStickSizeSlider.value = PSaves.MoveStickSize / 100f;
        JumpDashUISizeSlider.value = PSaves.JumpDashUISize / 100f;
        UISet();
    }
    private void OnEnable()
    {
        Start();
    }
    public void AtkUISizeSets()
    {
        PSaves.AtkUISize = Mathf.RoundToInt(AtkUISizeSlider.value * 100);
        UISet();
    }
    public void MoveStickSizeSets()
    {
        PSaves.MoveStickSize = Mathf.RoundToInt(MoveStickSizeSlider.value * 100);
        UISet();
    }
    public void JumpDashUISizeSets()
    {
        PSaves.JumpDashUISize = Mathf.RoundToInt(JumpDashUISizeSlider.value * 100);
        UISet();
    }
    void UISet()
    {
        AtkUISizeValTx.text = PSaves.AtkUISize.ToString("F0") + "%";
        MoveStickSizeValTx.text = PSaves.MoveStickSize.ToString("F0") + "%";
        JumpDashUISizeValTx.text = PSaves.JumpDashUISize.ToString("F0") + "%";
    }
}
