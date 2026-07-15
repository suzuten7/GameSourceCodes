using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Menu : MonoBehaviour
{
    [SerializeField] GameObject menuObj;
    [SerializeField] GameObject hostSets;
    [SerializeField] GameObject noHosts;
    [SerializeField] TextMeshProUGUI lerveTx;
    [SerializeField] Button setButton;
    [SerializeField] TMP_Dropdown teamdr;


    private void Start()
    {
        menuObj.SetActive(false);
    }
    void Update()
    {
        if (Player_Input.PI.pi.actions["Menu"].triggered)
        {
            menuObj.SetActive(!menuObj.activeSelf);
        }
        hostSets.SetActive(Net_Connect.InsRunner.IsServer);
        noHosts.SetActive(!Net_Connect.InsRunner.IsServer);
        lerveTx.text = LocalizSystem.LocailzSCInfo(Net_Connect.InsRunner.GameMode == Fusion.GameMode.Single ? "タイトル" : "切断");
        var pm = Obj_LocalObjects.MyPlayer;
        var setOn = Net_Value.NetCheck && Net_Value.NetValue.options[1];
        if (Obj_LocalObjects.MyPlayer == null) setOn = true;
        if(Obj_LocalObjects.MyPlayer != null && Net_Value.NetCheck && !Net_Value.NetValue.options[2])
        {
            int cost = 0;
            for (int i = 0; i < Obj_LocalObjects.MyPlayer.states.passives.Count; i++)
            {
                var pss = Obj_LocalObjects.MyPlayer.states.passives[i];
                var psd = Data_Base.PassiveDGet((Passive)pss.x);
                if (psd != null) cost += psd.CostGet(pss.y);
                if (pss.x >= (int)Passive.Cheat)
                {
                    setOn = true;
                    break;
                }
            }
            if(cost > Data_Base.DB.CostMax) setOn=true;
        }
        setButton.interactable = setOn;
        teamdr.interactable = Net_Value.NetCheck && Net_Value.NetValue.options[0] && Obj_LocalObjects.MyPlayer != null;
        if(pm != null)teamdr.value = pm.states.teamID;


    }
    public void Teamset()
    {
        var pm = Obj_LocalObjects.MyPlayer;
        pm.states.teamID = teamdr.value;
    }
    public void PlayViewChange()
    {
        if (Obj_LocalObjects.MyPlayer != null) Obj_LocalObjects.MyPlayer.RPC_ViewChange();
        else Net_PlayerSet.PlayerSet();
    }
}
