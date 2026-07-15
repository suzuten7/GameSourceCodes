using TMPro;
using UnityEngine;
using static PlayerValue;
public class UI_QualitySet : MonoBehaviour
{
    [SerializeField] TMP_Dropdown QualityDr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        QualityDr.value = PSaves.QualityLV;
    }
    void Update()
    {
        var QLV = QualitySettings.GetQualityLevel();
        if (QualityDr.value != QLV)
        {
            QualitySettings.SetQualityLevel(QualityDr.value);
            PSaves.QualityLV = QualityDr.value;
        }
    }
}
