using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonSystem_Gabu : MonoBehaviour
{
    #region 変数
    [SerializeField, Header("ゲームを閉じる")]
    private bool isClose = false;
    [SerializeField, Header("on off するオブジェクト")]
    private GameObject obj;

    [SerializeField, Header("色変わるか")]
    private bool isColorful = true;
    [SerializeField]
    private float changeSpeed;
    [SerializeField]
    private Button button;
    [SerializeField]
    private ColorBlock colorBlock;
    #endregion

    private void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    private void Update()
    {
        if (!isColorful)
        {
            return;
        }

        ChangeButtonColor(changeSpeed);
    }

    public void OnClickButton(int num)
    {
        if (isClose)
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        }
        else
        {
            SceneChangeUIs.SCUIDisp();
            SceneManager.LoadSceneAsync(num);
        }
    }

    public void ChangeButtonColor(float speed)
    {
        Color col = colorBlock.selectedColor;
        //Color col2 = colorBlock.highlightedColor;

        Color.RGBToHSV(col, out col.r, out col.g, out col.b);
        //Color.RGBToHSV(col2, out col2.r, out col2.g, out col2.b);

        col.r += 0.01f * speed;
        //col2.r += 0.01f * speed;

        col.r = Mathf.Repeat(col.r, 1f);
        //col2.r = Mathf.Repeat(col2.r, 1f);

        col = Color.HSVToRGB(col.r, col.g, col.b);
        //col2 = Color.HSVToRGB(col2.r, col2.g, col2.b);

        //colorBlock.colorMultiplier = 5f;
        colorBlock.selectedColor = col;
        colorBlock.highlightedColor = col;
        colorBlock.pressedColor = col;
        colorBlock.disabledColor = col;
        button.colors = colorBlock;

    }
}