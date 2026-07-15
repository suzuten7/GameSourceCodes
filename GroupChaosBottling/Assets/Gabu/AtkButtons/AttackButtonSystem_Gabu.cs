using System;
using UnityEngine;
using UnityEngine.UI;

public class AttackButtonSystem_Gabu : MonoBehaviour
{
    #region 変数

    private float currentCT = 0f;
    public float ct = 0f;
    public float currentCharge = 0f;
    public float charge = 0f;
    public AnimatorStatu statu = AnimatorStatu.Normal;

    [SerializeField] private Slider _CTSider;
    [SerializeField] private Slider _ChargeSlider;
    [SerializeField] private UISystem_Gabu[] _uiSystem;

    public enum AnimatorStatu : int
    {
        NONE = -1, Normal = 0, Highlighted, Pressed, Selected, Disabled
    }

    #endregion

    #region 関数


    #endregion

    private void Update()
    {
        if (currentCT == ct)
        {
            currentCT = ct;
            _CTSider.value = currentCT;
        }

        if(currentCharge == charge)
        {
            currentCharge = charge;
            _ChargeSlider.value = currentCharge;
        }

        for (int i = 0; i < _uiSystem.Length; i++)
        {
            if (statu == AnimatorStatu.NONE)
            {
                _uiSystem[i].IsChangeable = true;
                continue;
            }
            _uiSystem[i].setStatu = (int)statu;
        }
    }
}