using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ReticleSelect_Base : MonoBehaviour
{
    [SerializeField] UI_OptionManager optionManager;
    [SerializeField] TextMeshProUGUI optionTx;
    [SerializeField] TextMeshProUGUI cSetTx;
    [SerializeField] RawImage cSetIm;
    [SerializeField] List<UI_ReticleSelect_Add> setUIs;
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
        var tps = Data_Base.DB.reticles;
        for (int i = 0; i < Mathf.Max(setUIs.Count, tps.Count); i++)
        {
            if (i >= tps.Count)
            {
                setUIs[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= setUIs.Count)
            {
                setUIs.Add(Instantiate(setUIs[0], setUIs[0].transform.parent));
            }
            var sui = setUIs[i];
            var tp = tps[i];
            setUIs[i].ID = i;
            setUIs[i].backImage.color = cselID != i ? Color.white : Color.yellow;

            setUIs[i].nameTx.text = i.ToString("D3")+ ":"+ LocalizSystem.LocailzString("ReticleImgName", tp.name);
            setUIs[i].iconIm.texture = tp.icon;
            setUIs[i].iconIm.color = tp.iconCol;
        }

        optionTx.text = UI_OptionManager.ChangeStr(optionTx.text, bselID != cselID);
        cSetTx.text = cselID.ToString("D3")+ ":"+ LocalizSystem.LocailzString("ReticleImgName", tps[cselID].name);
        cSetIm.texture = tps[cselID].icon;
        cSetIm.color = tps[cselID].iconCol;
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
