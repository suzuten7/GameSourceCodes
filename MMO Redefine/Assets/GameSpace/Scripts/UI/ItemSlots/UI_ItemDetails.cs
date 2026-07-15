namespace UIs
{
    using Datas;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using static Datas.Data_Equips;
    using static Datas.Data_Get;
    using static GmSystem.GS_SaveValues;
    using static UI_System;
    using static Player.Player_Controle;
    using static GmSystem.GS_ChangeSet;
    public class UI_ItemDetails : MonoBehaviour
    {
        [Header("◆メインオプション")]
        [SerializeField] Canvas canvas;
        [SerializeField] RectTransform PRect;
        [SerializeField] RectTransform Rect;
        [SerializeField] TextMeshProUGUI InfoText;

        UI_ItemSlotBase SItemSlot = null;
        UI_DebugItem_Single DItemSlot = null;
        UI_JobTree_Single JTreeUI = null;
        UI_Acive_Single AciveUI = null;
        void Start()
        {
            Rect.gameObject.SetActive(false);
        }
        private void LateUpdate()
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = PCont.PI.actions["Point"].ReadValue<Vector2>()
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            bool Check = false;
            SItemSlot = null;
            DItemSlot = null;
            foreach (var res in results)
            {
                if (res.gameObject.TryGetComponent<UI_ItemDragDrop>(out var dragitem) && dragitem.ItemSlot != null)
                {
                    SItemSlot = dragitem.ItemSlot;
                    Item_Set();
                    Check = true;
                    break;
                }
                if (res.gameObject.TryGetComponent<UI_ItemSlotBase>(out var selitem) && selitem != null)
                {
                    SItemSlot = selitem;
                    Item_Set();
                    Check = true;
                    break;
                }
                if (res.gameObject.TryGetComponent<UI_DebugItem_Single>(out var dsitem))
                {
                    DItemSlot = dsitem;
                    Item_Set();
                    Check = true;
                    break;
                }
                if (res.gameObject.TryGetComponent<UI_JobTree_Single>(out var jtreeui))
                {
                    JTreeUI = jtreeui;
                    JTree_Set();
                    Check = true;
                    break;
                }
                if (res.gameObject.TryGetComponent<UI_Acive_Single>(out var aciveui))
                {
                    AciveUI = aciveui;
                    Acive_Set();
                    Check = true;
                    break;
                }
            }
            ChangeActive(Rect.gameObject, Check);
        }

        void Item_Set()
        {

            Data_SlotGID itemData = null;
            if (SItemSlot != null) itemData = SItemSlot.SlotGIDD;
            if (DItemSlot != null) itemData = ItemGIDDataGet(DItemSlot.GID);
            if (itemData == null)
            {
                ChangeText(InfoText,"無");
                return;
            }
            var infostr = itemData.Name;


            if (itemData is Data_Item itemD)
            {
                infostr += "\n";
                for (int i = 0; i < 5; i++) infostr += itemD.Rarity >= i ? "★" : "☆";
                if (itemD.BuyGold > 0) infostr += "\n売却額:" + ValueStrings(itemD.BuyGold * 0.5f) + "ゴールド";
                if(itemD.UpgradeExp > 0) infostr += "\n強化Exp:" + ValueStrings(itemD.UpgradeExp);
            }
            if (itemData.Info != "") infostr += "\n" + itemData.Info;
            else infostr += "\n説明って何ですか???";
            if(itemData is Data_Consumables conData)
            {
                infostr += "\n効果";
                if (conData.Attack.Info != "") infostr += "\n" + conData.Attack.Info;
                infostr += "\n" + conData.Attack.Infos();
            }

            Class_State_EquipmentValues EquipVal = null;
            if (SItemSlot != null)
            {
                EquipVal = SItemSlot.ItemEquipVal;
                if (EquipVal != null)
                {
                    infostr += "\n取得時刻" + EquipVal.GetDateStr;
                    infostr += "\nLV" + EquipVal.LV;
                    infostr += "\nExp" + EquipVal.Exp + "/" + EquipVal.NextExp;
                    if (EquipVal.AddOps.Count > 0)
                    {
                        infostr += "\n追加効果<size=65%>";
                        for (int i = 0; i < EquipVal.AddOps.Count; i++)
                        {
                            var vals = DB.OpValues.GetAddValues(EquipVal.AddOps[i].State, EquipVal.AddOps[i].Option);
                            EquipStrs(new Class_EquipmentAdds
                            {
                                EquipIfs = new Class_EquipmentIf[0],
                                State = EquipVal.AddOps[i].State,
                                Option = EquipVal.AddOps[i].Option,
                                Values = vals != null ? vals.Value : Vector2.zero,
                            },
                                EquipVal.AddOps[i].LV, out var oName, out var oOp, out var oVal, out var oIf);
                            infostr += "\n[" + (i+1) + "]" + EquipVal.AddOps[i].LV + "LV:" + oName + oOp + oVal;
                        }
                        infostr += "</size>";
                    }
                }
            }
            if (itemData is Data_Wepon wepData)
            {
                if (wepData.EquipmentAdds.Length > 0)
                {
                    infostr += "\n装備効果<size=65%>";
                    infostr += EquipAdds(wepData.EquipmentAdds, EquipVal != null ? EquipVal.LV : 1);
                    infostr += "</size>";
                }
                infostr += "\n攻撃:" + wepData.Attack.Name;
                if (wepData.Attack.Info != "") infostr += "\n" + wepData.Attack.Info;
                infostr += "\n" + wepData.Attack.Infos();
            }
            if (itemData is Data_Equipment equipData)
            {
                if(equipData.EquipmentAdds.Length > 0)
                {
                    infostr += "\n装備効果<size=65%>";
                    infostr += EquipAdds(equipData.EquipmentAdds, EquipVal != null ? EquipVal.LV : 1);
                    infostr += "</size>";
                }
            }
            if (itemData is Data_Attack atkData)
            {
                infostr += "\n" + atkData.Infos();
            }
            ChangeText(InfoText, infostr);
        }
        void JTree_Set()
        {
            var JTM = JTreeUI.JTreeMake;
            var JTD = JTM.JTDGroupsGet[JTreeUI.GID].JTreeGroup.JobTrees[JTreeUI.SID];
            var JTS = JTM.JTSGroupGet[JTreeUI.GID];
            var JLv = JTS.LVs[JTreeUI.SID];
            var infostr = JTD.CTree.Name;

            infostr += "(G:" + JTreeUI.GID + ",S:" + JTreeUI.SID+")";
            infostr += "\n" + JTD.CTree.Info;
            if (JTD.CTree.StateAdds.Length > 0)
            {
                infostr += "\n補正";
                for (int i = 0; i < JTD.CTree.StateAdds.Length; i++)
                {
                    EquipStrs(JTD.CTree.StateAdds[i],JLv, out var oName, out var oOP, out var oVal, out var oIf);
                    if (oIf != "") infostr += "\n条件" + oIf;
                    infostr += "\n" + oName + ":" + oOP + ":" + oVal;
                }
            }
            if (JTD.CTree.SkillAdds.Length > 0)
            {
                infostr += "\nスキル開放";
                for (int i = 0; i < JTD.CTree.SkillAdds.Length; i++)
                {
                    infostr += "\n「" + JTD.CTree.SkillAdds[i].Name + "」";
                    infostr += "\n" + JTD.CTree.SkillAdds[i].Infos();
                }
            }
            if (JTD.CTree.TriggerAttacks.Length > 0)
            {
                infostr += "\nトリガー";
                for (int i = 0; i < JTD.CTree.TriggerAttacks.Length; i++)
                {
                    var TAD = JTD.CTree.TriggerAttacks[i];
                    infostr += "\n" + TAD.Trigger + "「" + TAD.Attack.Name + "」";
                }
            }
            infostr += JTS.LVs[JTreeUI.SID] >= JTD.CTree.LVMax ? "<color=#FFFF00>" : "<color=#FFFFFF>";
            infostr += "\nLV:" + JLv + "/" + JTD.CTree.LVMax + "</color>";
            infostr += JTM.UsePointGet < JTD.CTree.Point ? "<color=#FF0000>" : "<color=#FFFFFF>";
            infostr += "\n必要ポイント:" + JTM.UsePointGet + "/" + JTD.CTree.Point + "</color>";
            if (JTD.BTreeIDs.Length > 0)
            {
                var PLV = JTM.PreLVsGet(JTD, JTS);
                infostr += PLV < JTD.PrereLV ? "<color=#FF0000>" : "<color=#FFFFFF>";
                infostr += "\n前提LV:" + PLV + "/" + JTD.PrereLV + "</color>";
            }

            ChangeText(InfoText,infostr);

        }
        void Acive_Set()
        {
            var data = DB.Acives.KeyGetDataID(AciveUI.ID).Item1;
            var acv = AciveGet(AciveUI.ID,false);
            var str = data.Name;
            switch (acv != null ? acv.Get : 0)
            {
                default:str += "<未解除>";break;
                case 1:str += "<解除済み>";break;
                case 2:str += "<再解除中>";break;
            }
            if(data.Info!="") str += "\n" + data.Info;
            if (data.ProgressMax != "")
            {
                switch (data.ProgressType)
                {
                    case Data_Acive.Enum_ProgressType.Int:
                        var icv = 0;
                        if (acv != null) int.TryParse(acv.Progress, out icv);
                        var imv = int.TryParse(data.ProgressMax, out var oimv) ? oimv : 0;
                        str += "\n" + icv + "/" + imv;
                        break;
                    case Data_Acive.Enum_ProgressType.Float:
                        var fcv = 0f;
                        if(acv!=null)float.TryParse(acv.Progress, out fcv);
                        var fmv = float.TryParse(data.ProgressMax, out var ofmv) ? ofmv : 0;
                        str += "\n" + ValueStrings(fcv) + "/" + ValueStrings(fmv);
                        break;
                }
            }
            ChangeText(InfoText, str);
        }
        void Update()
        {
            var size = Rect.sizeDelta;

            Vector2 mpos = PCont.PI.actions["Point"].ReadValue<Vector2>();

            Vector2 lpos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                PRect, mpos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out lpos);
            //Debug.Log(lpos.x + "rct"+ PRect.rect.xMin + "_ "+ PRect.rect.xMax);
            lpos += new Vector2(20f, 20f);
            lpos.x = Mathf.Clamp(lpos.x, PRect.rect.xMin + size.x+0, PRect.rect.xMax -0);
            lpos.y = Mathf.Clamp(lpos.y, PRect.rect.yMin + size.y+0, PRect.rect.yMax -0);

            Rect.anchoredPosition = lpos;


        }

        string EquipAdds(Class_EquipmentAdds[] EquAdds,int LV)
        {
            var EquLists = EquAdds.ToList();
            EquipSort(EquLists);
            var str = "";
            for (int i = 0; i < EquLists.Count; i++)
            {
                EquipStrs(EquLists[i],LV, out var oName, out var oOp, out var oVal, out var oIf);
                str += "\n";
                if (oIf != "") str += oIf + "\n";
                str += oName + oOp + oVal;
            }
            return str;
        }

    }
}
