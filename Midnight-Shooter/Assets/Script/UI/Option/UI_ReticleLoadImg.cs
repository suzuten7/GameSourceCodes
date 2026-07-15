using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ReticleLoadImg : MonoBehaviour
{
    [SerializeField] UI_OptionManager optionManager;
    [SerializeField] TextMeshProUGUI optionTx;
    [SerializeField] TextMeshProUGUI cSetTx;
    [SerializeField] RawImage cSetIm;
    [SerializeField] List<UI_ImageLoader_Sets> setUIs;
    public int cselID = 0;
    public int bselID = 0;
    string baseStr;
    private void Start()
    {
        baseStr = optionTx.text;
        Load();
    }
    public void Update()
    {
        var imgSets = UI_ImageLoader_Base.ImgSets;
        for (int i = 0; i < Mathf.Max(setUIs.Count, imgSets.Count+1); i++)
        {
            if(i >= imgSets.Count+1)
            {
                setUIs[i].gameObject.SetActive(false);
                continue;
            }
            if(i >= setUIs.Count)
            {
                setUIs.Add(Instantiate(setUIs[0], setUIs[0].transform.parent));
            }
            var sui = setUIs[i];
            setUIs[i].ID = i;
            setUIs[i].backImage.color = cselID != i ? Color.white : Color.yellow;
            if (i <= 0)
            {
                setUIs[i].nameTx.text = LocalizSystem.LocailzSCInfo("---:未使用");
                setUIs[i].img.color = Color.clear;
            }
            else
            {
                var set = imgSets[i-1];
                setUIs[i].nameTx.text = (i-1).ToString("D3") + ":"+  set.name;
                setUIs[i].img.texture = set.IconGet;
                setUIs[i].img.color = Color.white;
            }
        }

        optionTx.text = UI_OptionManager.ChangeStr(optionTx.text,bselID != cselID);
        if (cselID > 0) cSetTx.text = (cselID - 1).ToString("D3") + ":" + imgSets[cselID - 1].name;
        else cSetTx.text = LocalizSystem.LocailzSCInfo("---:未使用");
        cSetIm.color = cselID > 0 ? Color.white : Color.clear;
        if(cselID > 0)cSetIm.texture = imgSets[cselID - 1].IconGet;
    }
    public void Change(int ID)
    {
        cselID = ID;
        optionManager.CheckChanged();
    }
    public void Load()
    {
        //保存値読み込み
        int loadValue = Library_SaveFiles.LoadFileInt("Option",gameObject.name, 0);

        cselID = loadValue;
        bselID = loadValue;
    }
}
