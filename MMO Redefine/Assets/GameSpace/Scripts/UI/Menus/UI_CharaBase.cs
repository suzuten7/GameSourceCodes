namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Player;
    using static Datas.Data_Get;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static UIs.UI_System;
    using static FNet.Fusion_Manager;
    using System.Collections.Generic;
    using static GmSystem.GS_ChangeSet;

    public class UI_CharaBase : MonoBehaviour
    {
        public int CharaID;
        public UI_FadeActive CharaSelUI;
        public TMP_InputField NameIn;
        public RawImage ModelImg;
        public Image PlayerSelUI;
        public Image[] BotSelUIs;
        public UI_FadeActive[] SetUIs;
        public Image[] SelUIs;
        public UI_FadeActive[] NHideUIs;
        private void LateUpdate()
        {
            ChangeText(NameIn,LChara.Name,true);
            ChangeTexture(ModelImg,LChara.PlayerIconGet(out _),true);
            var wintra = ui_system.WindowsFadeUI.transform;
            if (CharaSelUI.transform.parent != wintra) CharaSelUI.transform.parent = wintra;
            for(int i = 0; i < SelUIs.Length; i++)
            {
                ChangeColor(SelUIs[i], OnColors(SetUIs[i].isActive));
            }
            foreach(var ui in SetUIs)
            {
                if (ui.transform.parent != wintra) ui.transform.parent = wintra;
                if (CharaID >= 0)
                {
                    if (!ui.isMoving && ui.isActive && !CharaSelUI.isActive) ui.OpenClose();
                }
            }
            wintra = ui_system.WindowsNoHide;
            foreach (var ui in NHideUIs)
            {
                if (ui.transform.parent != wintra) ui.transform.parent = wintra;
            }
            ChangeColor(PlayerSelUI,OnColors(LPlayerVal.UseChara == CharaID));
            if (CharaID >= 0)
            {
                var actbots = new bool[4];
                foreach (var sta in PStaList)
                {
                    if (sta == null) continue;
                    if (!CanControl(sta.Object)) continue;
                    if (sta.CharaID != CharaID) continue;
                    if(sta.BotID < 0) continue;
                    actbots[sta.BotID] = true;
                }
                for (int i = 0; i < 4; i++)
                {
                    ChangeColor(BotSelUIs[i], OnColors(actbots[i]));
                }
            }


        }
        public void NameChange()
        {
            LChara.Name = NameIn.text;
        }
        public void UseChenge()
        {
            LPlayerVal.UseChara = CharaID;
        }
        public void BotSpawne(int BotID)
        {
            MyPlayer.RPC_BotSpawne(CharaID, BotID);
        }
        public Player_State Sta
        {
            get
            {
                if (CharaID < 0) return MyPlayer;
                Player_State psta = null;
                foreach (var sta in PStaList)
                {
                    if (sta == null) continue;
                    if (sta.Object == null) continue;
                    if (sta.Object.InputAuthority != MyPlayer.Object.InputAuthority) continue;
                    if (sta.CharaID != (CharaID >= 0 ? CharaID : LPlayerVal.UseChara)) continue;
                    psta = sta;
                }
                return psta;
            }
        }
        public Class_Local_CharaSet LChara
        {
            get {
                var index = CharaID >= 0 ? CharaID : LPlayerVal.UseChara;
                if (index < LPlayerCharas.Count)return LPlayerCharas[index];
                else return null;
            }
        }
    }
}

