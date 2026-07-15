using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatText_Gabu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] bool Per;
    public void DescriptionText(float value)
    {
        tmp.text = Per ? ((value*100f).ToString("F0")+"%") : value.ToString();
    }
}
