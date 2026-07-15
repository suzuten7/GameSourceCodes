using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Entities.UniversalDelegates;

public class SceneChangeUIs : MonoBehaviour
{
    [SerializeField] Suzuten_DataBase DB;
    static SceneChangeUIs Ins;
    [SerializeField] UIsC[] UIs;
    [System.Serializable]
    class UIsC
    {
        public GameObject Canvass;
        public Image[] Images;
        public RawImage[] RawImages;
        public TextMeshProUGUI[] Texts;
    }
    bool Mode = false;
    float con = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (Ins == null)
        {
            DontDestroyOnLoad(gameObject);
            Ins = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            ImagesSets();
        }
        else Destroy(gameObject);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        con += Random.Range(0.1f,0.4f);
        if (con >= 4) con = 0;
        for (int a = 0; a < UIs.Length; a++)
        {
            var UI = UIs[a];
            List<Color> Col = new List<Color>();
            for (int i = 0; i < UI.Images.Length; i++)Col.Add(UI.Images[i].color);
            for (int i = 0; i < Col.Count; i++)
            {
                Color Cols = Col[i];
                if (!Mode) Cols.a = Mathf.Clamp01(Cols.a - 0.05f);
                else Cols.a = Mathf.Clamp01(Cols.a + 0.1f);
                UI.Images[i].color = Cols;
            }
            Col.Clear();
            for (int i = 0; i < UI.RawImages.Length; i++) Col.Add(UI.RawImages[i].color);
            for (int i = 0; i < Col.Count; i++)
            {
                Color Cols = Col[i];
                if (!Mode) Cols.a = Mathf.Clamp01(Cols.a - 0.05f);
                else Cols.a = Mathf.Clamp01(Cols.a + 0.1f);
                UI.RawImages[i].color = Cols;
            }
            Col.Clear();
            for (int i = 0; i < UI.Texts.Length; i++) Col.Add(UI.Texts[i].color);
            for (int i = 0; i < Col.Count; i++)
            {
                Color Cols = Col[i];
                if (!Mode) Cols.a = Mathf.Clamp01(Cols.a - 0.05f);
                else Cols.a = Mathf.Clamp01(Cols.a + 0.1f);
                UI.Texts[i].color = Cols;
            }
            UI.Texts[0].text = "ロード中<size=35%>(小嘘)</size>";
            for (int j = 0; j < Mathf.FloorToInt(con); j++)
            {
                UI.Texts[0].text += ".";
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Mode = false;
        Debug.Log(scene.name + " scene loaded");
    }
    static public void SCUIDisp()
    {
        if (Ins == null) return;

        Ins.Mode = true;
        Ins.ImagesSets();
    }

    void ImagesSets()
    {
        for(int i = 0; i < UIs.Length; i++)
        {
            UIs[i].Canvass.SetActive(true);
            var CData = DB.Charas[Random.Range(0, DB.Charas.Length)];
            UIs[i].RawImages[0].texture = CData.CharaImage;
            UIs[i].Texts[1].text = CData.CharaName;
            if (Random.value <= 0.65f)
            {
                int InfoID = Random.Range(0, CData.Actions.Length);
                UIs[i].Texts[2].text = CData.Actions[InfoID].ACName;
                UIs[i].Texts[3].text = CData.Actions[InfoID].ACInfo;
            }
            else
            {
                UIs[i].Texts[2].text = "キャラ説明";
                UIs[i].Texts[3].text = CData.CharaInfo;
            }

        }
    }

}
