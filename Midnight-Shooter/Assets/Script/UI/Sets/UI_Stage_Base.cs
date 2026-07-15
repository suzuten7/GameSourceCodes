using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Stage_Base : MonoBehaviour
{
    [SerializeField] bool games;
    [SerializeField] Image randomUI;
    [SerializeField] List<UI_Stage_Add> stageUIs;
    private void Update()
    {
        if (!games) randomUI.color = Net_Connect.NetCon.stageID == -1 ? Color.yellow : Color.white;
        else randomUI.color = Color.white;
        for (int i = 0; i < Mathf.Max(stageUIs.Count, Data_Base.DB.stages.Count); i++)
        {
            if (i >= Data_Base.DB.stages.Count)
            {
                stageUIs[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= stageUIs.Count)
            {
                stageUIs.Add(Instantiate(stageUIs[0], stageUIs[0].transform.parent));
            }
            var sui = stageUIs[i];
            sui.ID = i;
            if (!games) sui.backImage.color = Net_Connect.NetCon.stageID == i ? Color.yellow : Color.white;
            else sui.backImage.color = Color.white;

            var sd = Data_Base.DB.stages[i];
            sui.nameTx.text = LocalizSystem.LocailzString("StageName", sd.name);
            sui.detail.derailTitle = sui.nameTx.text;
            sui.detail.derailMain = LocalizSystem.LocailzString("StageInfo",sd.name,false, sd.info);
            sui.img.texture = sd.icon;
            sui.img.color = sd.iconCol;

        }
    }

    public void Selectd(int id)
    {
        if (!games)
        {
            Net_Connect.NetCon.stageID = id;
        }
        else
        {
            var playID = Net_Connect.NetCon.StageSceneIDGet(id);
            Net_Connect.InsRunner.LoadScene(SceneRef.FromIndex(playID));
        }
    }
}
