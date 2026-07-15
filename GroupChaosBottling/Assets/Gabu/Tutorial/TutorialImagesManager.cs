using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialImagesManager : MonoBehaviour
{
    #region 変数

    private Texture[] texture = new Texture[20];
    [SerializeField]
    private RawImage rawImage = null;
    [SerializeField]
    private RadioButtonManager radioButtonManager = null;

    #endregion


    #region 関数

    public void SetImages(Texture[] textures)
    {
        //if (textures.Length > this.texture.Length)
        //{
        //    Debug.LogError("範囲外です。Image array length mismatch");
        //    return;
        //}
        ResetImages();
        radioButtonManager.ResetButtons();
        this.texture = textures;
        radioButtonManager.SetEableRange(texture.Length);
        rawImage.texture = texture[radioButtonManager.ButtonCount];
    }

    private void ResetImages()
    {
        radioButtonManager.ResetButtons();
        texture = new Texture[0];
        rawImage.texture = null;
        return;
    }

    private void OnSelectIndex(CurrentIndex callback)
    {
        SelectImage(callback.index);
    }

    private void SelectImage(int index)
    {
        if (index < 0 || index >= texture.Length)
        {
            Debug.LogError("範囲外です。Index out of range");
            return;
        }

        if (texture[index] == null)
        {
            Debug.LogError("選択したインデックスに画像が設定されていません。Image is not assigned");
            return;
        }
        rawImage.texture = null; // 一旦クリア
        rawImage.texture = texture[index];
    }

    #endregion

    public void Start()
    {
        if (rawImage == null)
        {
            Debug.LogError("画像の貼り付け先が設定されていません。RawImage is not assigned");
        }

        if (radioButtonManager == null)
        {
            Debug.LogError("ラジオボタンの設定がされていません。RadioButtonManager is not assigned");
        }
    }

    #region 有効 / 無効化
    public void OnEnable()
    {
        radioButtonManager.OnChangeIndex += OnSelectIndex;
    }
    public void OnDisable()
    {
        radioButtonManager.OnChangeIndex -= OnSelectIndex;
    }

    #endregion
}
