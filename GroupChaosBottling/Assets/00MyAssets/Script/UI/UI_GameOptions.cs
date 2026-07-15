using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerValue;
public class UI_GameOptions : MonoBehaviour
{
    [SerializeField] Slider CDisSlider;
    [SerializeField] TextMeshProUGUI CDisValTx;
    [SerializeField] Slider CamSlider;
    [SerializeField] TextMeshProUGUI CamValTx;
    [SerializeField] Slider TargetSlider;
    [SerializeField] TextMeshProUGUI TargetValTx;
    [SerializeField] Slider DamTimeSlider;
    [SerializeField] TextMeshProUGUI DamTimeValTx;
    [SerializeField] Slider DamStackSlider;
    [SerializeField] TextMeshProUGUI DamStackValTx;
    [SerializeField] Slider AtkUISizeSlider;
    [SerializeField] TextMeshProUGUI AtkUISizeValTx;
    const float DStackP = 2.4772f;
    private void Start()
    {
        if (PSaves == null) return;
        CDisSlider.value = PSaves.CamDistance / 10f;
        CamSlider.value = PSaves.CamSpeed / 100f;
        TargetSlider.value = PSaves.TargetSpeed / 100f;
        DamTimeSlider.value = Mathf.Pow(PSaves.DamTime, 1f / DStackP);
        DamStackSlider.value = Mathf.Pow(PSaves.DamStack,1f/ DStackP);
        AtkUISizeSlider.value = PSaves.AtkUISize / 100f;
        UISet();
    }
    private void OnEnable()
    {
        Start();
    }
    public void CDisSets()
    {
        PSaves.CamDistance = Mathf.RoundToInt(CDisSlider.value*10);
        UISet();
    }
    public void CamSets()
    {
        PSaves.CamSpeed = Mathf.RoundToInt(CamSlider.value * 100);
        UISet();
    }
    public void TargetSets()
    {
        PSaves.TargetSpeed = Mathf.RoundToInt(TargetSlider.value * 100);
        UISet();
    }
    public void DamTimeSets()
    {
        PSaves.DamTime = Mathf.RoundToInt(Mathf.Pow(DamTimeSlider.value, DStackP));
        UISet();
    }
    public void DamStackSets()
    {
        PSaves.DamStack = Mathf.RoundToInt(Mathf.Pow(DamStackSlider.value, DStackP));
        UISet();
    }
    public void AtkUISizeSets()
    {
        PSaves.AtkUISize = Mathf.RoundToInt(AtkUISizeSlider.value * 100);
        UISet();
    }
    void UISet()
    {
        CDisValTx.text = (PSaves.CamDistance/10f).ToString("F1") + "μm";
        CamValTx.text = PSaves.CamSpeed.ToString("F0") + "%";
        TargetValTx.text = PSaves.TargetSpeed.ToString("F0") + "%";
        if (PSaves.DamTime <= 0) DamTimeValTx.text = "無効";
        else DamTimeValTx.text = PSaves.DamTime + "f";
        if (PSaves.DamStack <= 0) DamStackValTx.text = "無効";
        else DamStackValTx.text = PSaves.DamStack + "f";
        AtkUISizeValTx.text = PSaves.AtkUISize.ToString("F0") + "%";
    }
}
