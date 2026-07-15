using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static PlayerValue;
public class VolumeChangerSystem_Gabu : MonoBehaviour
{
    #region 変数

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource AudioPlay;
    [SerializeField] string MasterValue;
    [SerializeField] string BGMValue;
    [SerializeField] string SEValue;
    [SerializeField] string SystemValue;

    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;
    [SerializeField] Slider SystemSlider;

    [SerializeField] TextMeshProUGUI MasterValTx;
    [SerializeField] TextMeshProUGUI BGMValTx;
    [SerializeField] TextMeshProUGUI SEValTx;
    [SerializeField] TextMeshProUGUI SystemValTx;

    #endregion
    #region 関数
    public void PDSave()
    {
        Save();
    }
    void ValTxSet()
    {
        MasterValTx.text = (MasterSlider.value * 100f).ToString("F0")+"%";
        BGMValTx.text = (BGMSlider.value * 100f).ToString("F0") + "%";
        SEValTx.text = (SESlider.value * 100f).ToString("F0") + "%";
        SystemValTx.text = (SystemSlider.value * 100f).ToString("F0") + "%";
    }
    #endregion
    void VolSet()
    {
        audioMixer.SetFloat(MasterValue, Mathf.Clamp(Mathf.Log(PSaves.MasterVol/100f, 10) * 60f, -80, 20));
        audioMixer.SetFloat(BGMValue, Mathf.Clamp(Mathf.Log(PSaves.BGMVol / 100f, 10) * 60f, -80, 20));
        audioMixer.SetFloat(SEValue, Mathf.Clamp(Mathf.Log(PSaves.SEVol / 100f, 10) * 60f, -80, 20));
        audioMixer.SetFloat(SystemValue, Mathf.Clamp(Mathf.Log(PSaves.SystemVol / 100f, 10) * 60f, -80, 20));
    }
    private void Start()
    {
        MasterSlider.value = PSaves.MasterVol / 100f;
        BGMSlider.value = PSaves.BGMVol / 100f;
        SESlider.value = PSaves.SEVol / 100f;
        SystemSlider.value = PSaves.SystemVol / 100f;
        VolSet();
        ValTxSet();

    }

    private void Update()
    {
        bool Change = false;
        var ChengeV = Mathf.RoundToInt(MasterSlider.value * 100);
        if (PSaves.MasterVol != ChengeV)
        {
            PSaves.MasterVol = ChengeV;
            Change = true;
        }
        ChengeV = Mathf.RoundToInt(BGMSlider.value * 100);
        if (PSaves.BGMVol != ChengeV)
        {
            PSaves.BGMVol = ChengeV;
            Change = true;
        }
        ChengeV = Mathf.RoundToInt(SESlider.value * 100);
        if (PSaves.SEVol != ChengeV)
        {
            PSaves.SEVol = ChengeV;
            Change = true;
        }
        ChengeV = Mathf.RoundToInt(SystemSlider.value * 100);
        if (PSaves.SystemVol != ChengeV)
        {
            PSaves.SystemVol = ChengeV;
            Change = true;
        }
        if (Change)
        {
            VolSet();
            ValTxSet();
            AudioPlay.Play();
        }
    }


}
