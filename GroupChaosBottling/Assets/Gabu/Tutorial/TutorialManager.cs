using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DataColors;
public class TutorialManager : MonoBehaviour, UI_Sin_Set.SetReturn
{
    #region 変数

    [SerializeField, Header("チュートリアルのスクリプタブルオブジェクト")]
    private TutorialBD DB;
    [SerializeField, Header("リストの要素のプレハブ")]
    List<UI_Sin_Set> UIs;
    [SerializeField, Header("選択中の要素の画像")]
    private RawImage imagesSpace;
    [SerializeField, Header("選択中の要素の名前")]
    private TextMeshProUGUI nameSpace;
    [SerializeField, Header("選択中のの要素の詳細")]
    private TextMeshProUGUI descriptionSpace;
    [SerializeField]
    private TutorialImagesManager tutorialImagesManager;

    int SelectID = 0;

    #endregion

    #region 関数

    /// <summary>
    /// チュートリアルの選択処理
    /// </summary>
    /// <param name="settings"></param>
    public void PickTutorial(TutorialSettings settings)
    {
        tutorialImagesManager.SetImages(settings.images); // 画像を表示する
        nameSpace.text = settings.tutorialName; // 名前を表示する
        descriptionSpace.text = settings.tutorialDescription; // 説明を表示する
    }

    private void GenerateListElementObject()
    {
        for (int i = 0; i < DB.tutorials.Length; i++)
        {
            if (UIs.Count <= i)
            {
                UIs.Add(Instantiate(UIs[0], UIs[0].transform.parent));
            }
            TutorialSettings settings = DB.tutorials[i];
            UIs[i].Returns = this;
            UIs[i].ID = i;
            UIs[i].Name.text = settings.tutorialName; // 名前を表示する
            UIs[i].Icon.texture = settings.icon; // 画像を表示する
            UIs[i].BackImage.color = DCol.ColSelects(i == SelectID); // 選択中の要素の色を変更する
        }
    }

    #endregion

    private void Start()
    {
        GenerateListElementObject();
        PickTutorial(DB.tutorials[SelectID]);
    }
    public void IDChange(bool Back)
    {
        if (Back)
        {
            SelectID--;
        }
        else
        {
            SelectID++;
        }
        SelectID = (int)Mathf.Repeat(SelectID, DB.tutorials.Length);
        Start();
    }
    void UI_Sin_Set.SetReturn.ReturnID(string Type, int ID)
    {
        SelectID = ID;
        Start();
    }
}
