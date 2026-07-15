namespace UIs
{
    using System.Collections.Generic;
    using TMPro;
    using Unity.VisualScripting;
    using UnityEngine;
    using static Datas.Data_Attack;
    using static Datas.Data_Get;
    using static GmSystem.GS_GlobalState;
    using static UI_System;
    using static GmSystem.GS_EnumToJpString;
    using static GmSystem.GS_ChangeSet;
    public class UI_ValueGraph_Main : MonoBehaviour
    {
        int time = 0;
        int sintime = 0;
        [SerializeField] UI_CharaBase CharaUI;
        [SerializeField] TMP_Dropdown SetDr;
        [SerializeField] List<Class_GraphSet> GraphSets;
        [System.Serializable]
        public class Class_GraphSet
        {
            public string Key;
            public string DispName;
            public bool On;
            public bool AddUse;
            public UI_ValueGraph_BrokenLine LineUI;
            public UI_ValueGraph_Single SinUI;
        }
        public void Adds(string Key,float val)
        {
            foreach (var gset in GraphSets)
            {
                if (gset.Key != Key) continue;
                if (!gset.On) return;
                gset.LineUI.lingVal.ValueAdd(val);
                return;
            }
        }
        public void OnSet(UI_ValueGraph_Single sinui)
        {
            foreach (var gset in GraphSets)
            {
                if (gset.SinUI !=  sinui) continue;
                gset.On = !gset.On;
                if(gset.On) for (int i = 0; i < gset.LineUI.lingVal.values.Length; i++) gset.LineUI.lingVal.values[i] = 0;
                return;
            }
        }

        private void Start()
        {
            SetUp("ValPer_HPPer","HP割合", true, false, Color.green, new Color(0.5f,0,0));
            SetUp("ValPer_MPPer", "MP割合", true, false, Color.blue);
            SetUp("ValPer_STPer", "スタミナ割合", true, false, Color.yellow, new Color(0, 0, 0.5f));
            SetUp("ValPer_EXPer", "EX割合", true, false, Color.white);

            SetUp("AddDamage_All", "全体", true, true, new Color(1, 0.5f, 0));
            SetUp("AddDamage_Hit", "ヒット数", true, true, new Color(0.5f, 1, 0));
            SetUp("AddDamage_Crit", "会心数", true, true, new Color(1, 1, 0.3f));
            SetUpSub("AddDamage");
            SetUp("TakeDamage_All", "全体", true, true, new Color(0.7f, 0, 0));
            SetUp("TakeDamage_Hit", "ヒット数", true, true, new Color(0.2f, 0.7f, 0));
            SetUp("TakeDamage_Crit", "会心数", false, true, new Color(0.5f, 0.5f, 0));
            SetUpSub("TakeDamage");
            SetUp("AddHeal_All", "全体", true, true, new Color(0.3f, 1, 0.3f));
            SetUp("AddHeal_Hit", "ヒット数", false, true, new Color(0.3f, 1, 0.8f));
            SetUp("AddHeal_Crit", "会心数", false, true, new Color(0.8f, 1, 0.3f));
            SetUpSub("AddHeal");
            SetUp("TakeHeal_All", "全体", true, true, new Color(0.3f, 0.3f, 1));
            SetUp("TakeHeal_Hit", "ヒット数", false, true, new Color(0.3f, 0.8f, 1));
            SetUp("TakeHeal_Crit", "会心数", false, true, new Color(0.8f, 0.3f, 1));
            SetUpSub("TakeHeal");

            HideChange();
        }
        void SetUpSub(string types)
        {


            SetUp(types + "_SWR", "武器右", false, true, new Color(1, 0.5f, 0.5f));
            SetUp(types + "_SWL", "武器左", false, true, new Color(0.5f, 0.5f, 1));
            SetUp(types + "_SOther", "武器以外", false, true, new Color(1, 0.5f, 1));

            SetUp(types + "_A" + Enum_AtkType.Normal,EnumToJp(Enum_AtkType.Normal), false, true, new Color(0.8f, 0.8f, 0.8f));
            SetUp(types+ "_A" + Enum_AtkType.Hev, EnumToJp(Enum_AtkType.Hev), false, true, new Color(1.0f, 0.8f, 0.8f));
            SetUp(types+ "_A" + Enum_AtkType.Skill, EnumToJp(Enum_AtkType.Skill), false, true, new Color(0.8f, 0.8f, 1.0f));
            SetUp(types+ "_A" + Enum_AtkType.EX, EnumToJp(Enum_AtkType.EX), false, true, new Color(1.0f, 0.8f, 1.0f));
            SetUp(types + "_A" + Enum_AtkType.Other, EnumToJp(Enum_AtkType.Other), false, true, new Color(0.6f, 0.2f, 0.6f));

            for (int i = 0; i <= (int)Enum_Element.Dark; i++)
            {
                SetUp(types+ "_" + (Enum_Element)i, EnumToJp((Enum_Element)i) + "属性", false, true, DB.ElementColors[i]);
            }

            SetUp(types+ "_R" + Enum_RangeType.Short, EnumToJp(Enum_RangeType.Short), false, true, new Color(0.5f, 0, 0));
            SetUp(types+ "_R" + Enum_RangeType.Midle, EnumToJp(Enum_RangeType.Midle), false, true, new Color(0, 0.5f, 0));
            SetUp(types+ "_R" + Enum_RangeType.Long, EnumToJp(Enum_RangeType.Long), false, true, new Color(0.5f, 0, 0.5f));
            SetUp(types+ "_R" + Enum_RangeType.Other, EnumToJp(Enum_RangeType.Other), false, true, new Color(0.5f, 0.5f, 0.5f));
        }
        void SetUp(string key,string dispName,bool on, bool addUse, Color col,Color? negcol = null)
        {
            var lui = Instantiate(GraphSets[0].LineUI, GraphSets[0].LineUI.transform.parent);
            for (int i = 0; i < lui.lingVal.values.Length; i++) lui.lingVal.values[i] = 0;
            lui.color = col;
            lui.negColor = negcol != null ? negcol.Value : col;
            lui.MaxPers = addUse;
            var sui = Instantiate(GraphSets[0].SinUI, GraphSets[0].SinUI.transform.parent);

            GraphSets.Add(new Class_GraphSet
            {
                Key = key,
                DispName = dispName,
                On = on,
                AddUse = addUse,
                LineUI =lui,
                SinUI = sui,
            });
        }
        public void HideChange()
        {
            foreach (var gset in GraphSets)
            {
                var keytype = gset.Key.Split("_")[0];
                var act = false;
                if (SetDr.value == 0 && gset.On) act = true;
                switch (keytype)
                {
                    case "ValPer": if (SetDr.value == 1) act = true; break;
                    case "AddDamage": if (SetDr.value == 2) act = true; break;
                    case "TakeDamage": if (SetDr.value == 3) act = true; break;
                    case "AddHeal": if (SetDr.value == 4) act = true; break;
                    case "TakeHeal": if (SetDr.value == 5) act = true; break;
                    default: if (SetDr.value == 6) act = true; break;
                }
                ChangeActive(gset.SinUI.gameObject, act);
            }
        }
        public void AllOff()
        {
            foreach (var gset in GraphSets)
            {
                gset.On = false;
            }
        }
        private void FixedUpdate()
        {
            if (CharaUI.Sta != null) CharaUI.Sta.ValGraph = this;
            time++;
            if (GraphSets[0].On) sintime++;
            else sintime = 0;
            foreach (var gset in GraphSets)
            {
                ChangeColor(gset.SinUI.SelUI,OnColors(gset.On));
                var str = gset.DispName;

                var keys = gset.Key.Split("_");
                if (SetDr.value == 0 && keys.Length > 1)
                {
                    switch (keys[0])
                    {
                        case "AddDamage": str = "与ダメ" + str; break;
                        case "TakeDamage": str = "受ダメ" + str; break;
                        case "AddHeal": str = "与回復" + str; break;
                        case "TakeHeal": str = "受回復" + str; break;
                    }
                }
                ChangeText(gset.SinUI.KeyTx, str);
                if (!gset.On)
                {
                    ChangeText(gset.SinUI.ValTx,"Off");
                    ChangeActive(gset.LineUI.gameObject, false);
                    continue;
                }
                ChangeActive(gset.LineUI.gameObject, true);

                if (gset.AddUse) gset.LineUI.SetVerticesDirty();
                if (time % 60 != 0) continue;
                float val;
                var valtx = "";
                switch (gset.Key)
                {
                    default:
                        val = gset.LineUI.lingVal.GetTotal();
                        valtx = "平均" + ValueStrings(val / gset.LineUI.lingVal.values.Length) + "/s";
                        valtx += "\n最大" + ValueStrings(gset.LineUI.lingVal.GetMax());
                        valtx += "|合計" + ValueStrings(val);
                        gset.LineUI.lingVal.StepAdd(0);
                        break;
                    case "SinTime":
                        var tm = (Mathf.Sin(sintime * 0.1f) + 1) * 0.5f;
                        valtx = (sintime / 60f).ToString("F0") + "秒";
                        gset.LineUI.lingVal.StepAdd(tm);
                        break;
                    case "ValPer_HPPer":
                        val = CharaUI.Sta != null ? (CharaUI.Sta.HP / CharaUI.Sta.F_MHP) : 0;
                        valtx = (val * 100).ToString("F1") + "%";
                        gset.LineUI.lingVal.StepAdd(val);
                        break;
                    case "ValPer_MPPer":
                        val = CharaUI.Sta != null ? (CharaUI.Sta.MP / CharaUI.Sta.F_MMP) : 0;
                        valtx = (val * 100).ToString("F1") + "%";
                        gset.LineUI.lingVal.StepAdd(val);
                        break;
                    case "ValPer_STPer":
                        val = CharaUI.Sta != null ? (CharaUI.Sta.ST / CharaUI.Sta.F_MST) : 0;
                        valtx = (val * 100).ToString("F1") + "%";
                        gset.LineUI.lingVal.StepAdd(val);
                        break;
                    case "ValPer_EXPer":
                        val = CharaUI.Sta != null ? (CharaUI.Sta.EX / 100) : 0;
                        valtx = (val * 100).ToString("F1") + "%";
                        gset.LineUI.lingVal.StepAdd(val);
                        break;
                    case "":
                        break;
                }
                if (!gset.AddUse) gset.LineUI.SetVerticesDirty();
                 ChangeText(gset.SinUI.ValTx,valtx);
            }

        }

    }
}
