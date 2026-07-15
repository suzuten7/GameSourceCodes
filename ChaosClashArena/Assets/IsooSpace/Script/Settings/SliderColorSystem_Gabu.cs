using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderColorSystem_Gabu : MonoBehaviour
{
    #region 変数  
    [SerializeField] private Slider slider;

    [SerializeField] private Image image;

    [SerializeField, Header("何色に変わるか")]
    private Color color = Color.red;

    [SerializeField, Header("何割り異常になったら変わるか")]
    private float ratio = 0.6f;
    #endregion

    void Update()
    {
        if ((slider.value / slider.maxValue) > ratio)
        {
            image.color = color;
        }
        else
        {

            image.color = Color.white;
        }
    }
}
