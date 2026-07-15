using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChengeTextColor_Gabu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private Vector4 speed = Vector4.zero;
    [SerializeField, Header("増加倍率")] private float f = 0.01f;

    void Update()
    {
        Color col = tmp.color;
        Color col2;
        Color.RGBToHSV(col, out col2.r, out col2.g, out col2.b);

        col2.r = Mathf.Repeat(col2.r + speed.x * f, 1f);
        col2.g = Mathf.Repeat(col2.g + speed.y * f, 1f);
        col2.b = Mathf.Repeat(col2.b + speed.z * f, 1f);
        if(speed.w!=0) col.a =  0.5f+Mathf.Sin(Time.time*speed.w)/2f;

        Color col3 = Color.HSVToRGB(col2.r, col2.g, col2.b);
        col3.a = col.a;
        tmp.color = col3;
    }
}
