using UnityEngine;
using UnityEngine.UI;
using System;

public class RadioButtonSystem : MonoBehaviour
{
    #region 変数

    [SerializeField]
    private ImageAnimation_Gabu[] imageAnimationManagers;
    [SerializeField]
    public Button buttons;

    #endregion

    public void ChangeStatu(UISystem_Gabu.AnimatorStatu statu)
    {
        if (imageAnimationManagers == null)
        {
            return;
        }
        foreach (var imageAnimationManager in imageAnimationManagers)
        {
            imageAnimationManager.ChangeStatu(statu);
        }
    }

    public delegate void OnClickButton(RadioButtonSystem button);
}

