
namespace UIs
{
    using System.Collections.Generic;
    using UnityEngine;
    using static UI_System;
    using TMPro;
    using UnityEngine.UI;
    using static GmSystem.GS_SaveValues;
    using Player;
    using static GmSystem.GS_GlobalValues;
    using static Datas.Data_Get;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_ChangeSet;
    public class UI_CharaSel_Make : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI MyCharaNameTx;
        [SerializeField] RawImage MyCharaIcon;
        [SerializeField] Image[] BotSelIms;
        [SerializeField] TextMeshProUGUI[] BotNameTxs;
        [SerializeField] RawImage[] BotIcons;
        [SerializeField] List<UI_CharaSel_Single> SingleUIs;
        [SerializeField] RectTransform AddCharaUI;
        private void LateUpdate()
        {
            var MChara = LPlayerCharas[LPlayerVal.UseChara];
            ChangeText(MyCharaNameTx,"操作キャラ「" + MChara.Name + "」");
            ChangeTexture(MyCharaIcon,MChara.PlayerIconGet(out _), true);
            for(int i = 0; i < BotNameTxs.Length; i++)
            {
                Player_State PSta = null;
                foreach(var pl in MyPList)
                {
                    if(CanControl(pl.Object) && pl.BotID == i + 1)
                    {
                        PSta = pl;
                        break;
                    }
                }
                var botstr = "ボット" + (i + 1);
                if (PSta != null)
                {
                    var BotChara = LPlayerCharas[PSta.CharaID];
                    botstr += "\n「" + BotChara.Name + "」";
                    ChangeTexture(BotIcons[i],BotChara.PlayerIconGet(out _),true);
                    ChangeActive(BotIcons[i].gameObject, true);
                    ChangeColor(BotSelIms[i],OnColors(ui_system.CharaUIs[PSta.CharaID].CharaSelUI.isActive));
                }
                else
                {
                    botstr += "\n未出現";
                    ChangeActive(BotIcons[i].gameObject, false);
                    ChangeColor(BotSelIms[i],ui_system.OffColor);
                }
                ChangeText(BotNameTxs[i], botstr);
            }
            for (int i = 0; i < LPlayerCharas.Count; i++)
            {
                if (i >= SingleUIs.Count)
                {
                    SingleUIs.Add(Instantiate(SingleUIs[0], SingleUIs[0].transform.parent));
                    AddCharaUI.transform.SetAsLastSibling();
                }
                var sui = SingleUIs[i];
                sui.ID = i;
                ChangeColor(sui.SelImg,OnColors(ui_system.CharaUIs[i].CharaSelUI.isActive));
                ChangeText(sui.NameTx,LPlayerCharas[i].Name);
                ChangeTexture(sui.ModelImg, LPlayerCharas[i].PlayerIconGet(out _),true);
            }
        }
        public void UIOpen(int ID)
        {
            ui_system.CharaUIs[ID].CharaSelUI.OpenClose();
        }
        public void AddChara()
        {
            LPlayerCharas.Add(new Class_Local_CharaSet(LPlayerCharas.Count));
        }
        public void BotClear()
        {
            MyPlayer.RPC_BotClear();
        }
        public void BotSlotOpen(int i)
        {
            Player_State PSta = null;
            foreach (var pl in MyPList)
            {
                if (CanControl(pl.Object) && pl.BotID == i + 1)
                {
                    PSta = pl;
                    break;
                }
            }
            if (PSta != null)
            {
                UIOpen(PSta.CharaID);
            }
        }
        public void Change(int ID,bool back)
        {
            var cid = Mathf.Clamp(ID + (!back ? 1 : -1), 0, GetSave_Charas.Count - 1);
            var chara = LPlayerCharas[cid];
            LPlayerCharas[cid] = LPlayerCharas[ID];
            LPlayerCharas[ID] = chara;
        }
    }
}

