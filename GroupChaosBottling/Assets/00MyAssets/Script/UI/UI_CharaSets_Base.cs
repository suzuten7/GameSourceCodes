using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static DataBase;
using static Manifesto;
using static PlayerValue;
using static BattleManager;
using static Statics;
using static DataColors;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;

public class UI_CharaSets_Base : MonoBehaviour, UI_Sin_Set.SetReturn
{
    #region 変数
    [SerializeField] List<UI_Sin_Set> PriSet_Sin_UIs;
    [SerializeField] List<UI_Sin_Set> AddSet_Sin_UIs;
    //[SerializeField] Toggle FBToggle;
    [SerializeField] TextMeshProUGUI InfoTx;
    [EnumIndex(typeof(Enum_SetSlot))]
    [SerializeField] UI_Sin_Set[] StateUIs;
    [EnumIndex(typeof(Enum_SetSlot))]
    [SerializeField] UI_Sin_Set[] SetSelectUIs;
    [SerializeField] UI_Sin_Set NonUI;
    [SerializeField] List<UI_Sin_Set> Set_Sin_UIs;
    [SerializeField] Scrollbar SelectsBar;
    [SerializeField] TMP_Dropdown FilterDr;
    [SerializeField] TMP_Dropdown GeneStaDr;

    [SerializeField] UIChange UChange;
    [SerializeField] GameObject NSelUI;
    [SerializeField] TMP_InputField DispIn;
    [SerializeField] TMP_InputField MemoIn;

    [SerializeField] ChangeButtonSettings buttonSetting;


    [SerializeField] Transform ModelTrans;

    [SerializeField] Toggle StateAddsT;
    [SerializeField] TextMeshProUGUI StateTx;

    [SerializeField] UI_CharaSets_GeneAdd SetGeneAdd;
    [SerializeField] UI_CharaSets_GeneList SetGeneList;
    [SerializeField] UI_CharaSets_Others SetOthers;

    int SelectID = 0;
    int FilterID = -1;
    GameObject ModelIns;
    int ModelID = -1;
    #endregion
    private void Start()
    {
        // フィルターを更新
        UIUpdate();
        FilterUpdate();
    }
    public void UIUpdate()
    {
        //メモ更新
        DispIn.text = PriSetGet.Disp;
        MemoIn.text = PriSetGet.Memo;
        //
        bool NSelDisp = false;
        switch (UChange.UIID)
        {
            case 0: NSelDisp = true;break;
            case 4: NSelDisp = true; break;
            case 5: NSelDisp = true; break;
        }
        NSelUI.SetActive(NSelDisp);
        // PriSet_Sin_UIsの更新
        for (int i = 0; i < PriSets.Length; i++)
        {
            if (PriSet_Sin_UIs.Count <= i) PriSet_Sin_UIs.Add(Instantiate(PriSet_Sin_UIs[0], PriSet_Sin_UIs[0].transform.parent));
            var SinUI = PriSet_Sin_UIs[i];
            SinUI.Returns = this;
            SinUI.Type = "PriSet";
            SinUI.BackImage.color = DCol.ColSelects(PSaves.PriSetID == i);
            SinUI.Icon.texture = DB.Charas[PriSets[i].CharaID].Icon;
            SinUI.ID = i;
            SinUI.Name.text = (i + 1).ToString("");
        }
        //サブ
        for (int i = 0; i < 3; i++)
        {
            if (AddSet_Sin_UIs.Count <= i) AddSet_Sin_UIs.Add(Instantiate(AddSet_Sin_UIs[0], AddSet_Sin_UIs[0].transform.parent));
            var SinUI = AddSet_Sin_UIs[i];
            SinUI.Returns = this;
            SinUI.ID = i;
            SinUI.Type = "AddSet";
            SinUI.Name.text = "[" + (i + 1).ToString("") + "]";
            int AddID = PSaves.AddSet1;
            switch (i)
            {
                case 1: AddID = PSaves.AddSet2; break;
                case 2: AddID = PSaves.AddSet3; break;
            }
            SinUI.Info.text = AddID >= 0 ? (AddID + 1).ToString("") : "-";
            SinUI.BackImage.color = AddID >= 0 ? Color.white : Color.gray;
            if (i >= 1 && !PhotonNetwork.OfflineMode) SinUI.BackImage.color = new Color(1, 0.5f, 0.5f);
            if (AddID >= 0)
            {
                SinUI.Icon.texture = DB.Charas[PriSets[AddID].CharaID].Icon;
                SinUI.Icon.color = Color.white;
            }
            else SinUI.Icon.color = Color.clear;

        }
        string Info = "";
        int DataCount = 0;
        // キャラデータの更新
        for (int i = 0; i < SetSelectUIs.Length; i++)
        {
            SetSelectUIs[i].Returns = this;
            SetSelectUIs[i].Type = "SelectChange";
            SetSelectUIs[i].ID = i;
            SetSelectUIs[i].BackImage.color = DCol.ColSelects(SetSelectUIs[i].ID == SelectID);

            string Name = "";
            Texture Icon = null;
            string Infos = "";
            int DCount = 0;
            Data_Chara CharaD = null;
            Data_Atk AtkD = null;
            Data_Passive PassD = null;
            Class_Save_GeneData GeneD = null;

            switch ((Enum_SetSlot)i)
            {
                case Enum_SetSlot.キャラ:
                    CharaD = DB.Charas[PriSetGet.CharaID];
                    DCount = DB.Charas.Length;
                    break;
                case Enum_SetSlot.表通常:
                case Enum_SetSlot.裏通常:
                    AtkD = DB.N_Atks[PriSetGet.AtkGet(i == (int)Enum_SetSlot.裏通常).N_AtkID];
                    DCount = DB.N_Atks.Length;
                    break;
                case Enum_SetSlot.表スキル1:
                case Enum_SetSlot.裏スキル1:
                    AtkD = DB.S_Atks[PriSetGet.AtkGet(i == (int)Enum_SetSlot.裏スキル1).S1_AtkID];
                    DCount = DB.S_Atks.Length;
                    break;
                case Enum_SetSlot.表スキル2:
                case Enum_SetSlot.裏スキル2:
                    AtkD = DB.S_Atks[PriSetGet.AtkGet(i == (int)Enum_SetSlot.裏スキル2).S2_AtkID];
                    DCount = DB.S_Atks.Length;
                    break;
                case Enum_SetSlot.表必殺:
                case Enum_SetSlot.裏必殺:
                    AtkD = DB.E_Atks[PriSetGet.AtkGet(i == (int)Enum_SetSlot.裏必殺).E_AtkID];
                    DCount = DB.E_Atks.Length;
                    break;
                case Enum_SetSlot.パッシブ1:
                    PassD = DB.Passives[PriSetGet.Passive.P1_ID];
                    DCount = DB.Passives.Length;
                    break;
                case Enum_SetSlot.パッシブ2:
                    PassD = DB.Passives[PriSetGet.Passive.P2_ID];
                    DCount = DB.Passives.Length;
                    break;
                case Enum_SetSlot.パッシブ3:
                    PassD = DB.Passives[PriSetGet.Passive.P3_ID];
                    DCount = DB.Passives.Length;
                    break;
                case Enum_SetSlot.パッシブ4:
                    PassD = DB.Passives[PriSetGet.Passive.P4_ID];
                    DCount = DB.Passives.Length;
                    break;
                case Enum_SetSlot.因子1:
                    GeneD = GeneGet.G1_ID >= 0 ? Genes.Datas[GeneGet.G1_ID] : null;
                    DCount = Genes.Datas.Count;
                    break;
                case Enum_SetSlot.因子2:
                    GeneD = GeneGet.G2_ID >= 0 ? Genes.Datas[GeneGet.G2_ID] : null;
                    DCount = Genes.Datas.Count;
                    break;
                case Enum_SetSlot.因子3:
                    GeneD = GeneGet.G3_ID >= 0 ? Genes.Datas[GeneGet.G3_ID] : null;
                    DCount = Genes.Datas.Count;
                    break;
                case Enum_SetSlot.因子4:
                    GeneD = GeneGet.G4_ID >= 0 ? Genes.Datas[GeneGet.G4_ID] : null;
                    DCount = Genes.Datas.Count;
                    break;
                case Enum_SetSlot.因子5:
                    GeneD = GeneGet.G5_ID >= 0 ? Genes.Datas[GeneGet.G5_ID] : null;
                    DCount = Genes.Datas.Count;
                    break;
            }
            if (CharaD != null)
            {
                Name = CharaD.Name;
                Icon = CharaD.Icon;
                if (i == SelectID) Info = CharaD.Name;
                if (ModelTrans != null && ModelID != PriSetGet.CharaID)
                {
                    ModelID = PriSetGet.CharaID;
                    if (ModelIns != null) Destroy(ModelIns);
                    ModelIns = Instantiate(CharaD.ModelObj.gameObject, ModelTrans.position, Quaternion.identity);
                    ModelIns.transform.parent = ModelTrans;
                }
            }
            if (AtkD != null)
            {
                Name = AtkD.Name;
                Icon = AtkD.Icon;
                if (i == SelectID) Info = AtkD.Name + AtkD.InfoGets();
            }
            if (PassD != null)
            {
                Name = PassD.Name;
                Icon = PassD.Icon;
                if (i == SelectID) Info = PassD.Name + "\n" + PassD.Info;
            }
            if (i >= (int)Enum_SetSlot.因子1 && i <= (int)Enum_SetSlot.因子5)
            {
                if (GeneD != null)
                {
                    Name = "「" + GeneD.Name + "」";
                    Name += "\n<size=75%>" + (Enum_GeneTypes)GeneD.Type + "</size>";
                    Infos = GeneD.Lock ? "★" : "";
                    Icon = DB.Genes[GeneD.Type].Images[GeneD.Format];
                    SetSelectUIs[i].Icon.color = Color.white;
                    StateUIs[i].Icon.color = Color.white;
                    if (i == SelectID)
                    {
                        Info = GeneInfo(GeneD, true);
                    }
                }
                else
                {
                    Name = "虚無";
                    SetSelectUIs[i].Icon.color = Color.clear;
                    StateUIs[i].Icon.color = Color.clear;
                }
            }

            if (i == SelectID) DataCount = DCount;
            SetSelectUIs[i].Name.text = Name;
            SetSelectUIs[i].Icon.texture = Icon;
            if (SetSelectUIs[i].Info != null) SetSelectUIs[i].Info.text = Infos;
            StateUIs[i].Name.text = Name;
            if (StateUIs[i].Info != null) StateUIs[i].Info.text = Infos;
            StateUIs[i].Icon.texture = Icon;
        }

        InfoTx.text = Info;

        // Set_Sin_UIsの更新
        int SelNum = -1;
        int DispCount = 0;
        for (int i = 0; i < Mathf.Max(DataCount, Set_Sin_UIs.Count); i++)
        {
            bool Disp = true;
            if (i < DataCount)
            {
                if (Set_Sin_UIs.Count <= i)
                {
                    Set_Sin_UIs.Add(Instantiate(Set_Sin_UIs[0], Set_Sin_UIs[0].transform.parent));
                }
                var SinUI = Set_Sin_UIs[i];
                SinUI.Returns = this;
                SinUI.ID = i;

                buttonSetting.targets.Add(SinUI.GetComponent<ImageAnimationManager>());

                bool Select = false;
                string Name = "";
                Texture Icon = null;
                string Infos = "";
                Data_Chara CharaD = null;
                Data_Atk AtkD = null;
                Data_Passive PassD = null;
                Class_Save_GeneData GeneD = null;

                switch ((Enum_SetSlot)SelectID)
                {
                    case Enum_SetSlot.キャラ:
                        CharaD = DB.Charas[i];
                        SinUI.Type = "Chara";
                        Select = i == PriSetGet.CharaID;
                        break;
                    case Enum_SetSlot.表通常:
                    case Enum_SetSlot.裏通常:
                        AtkD = DB.N_Atks[i];
                        SinUI.Type = SelectID == (int)Enum_SetSlot.裏通常 ? "B_N_Atk" : "F_N_Atk";
                        Select = i == PriSetGet.AtkGet(SelectID == (int)Enum_SetSlot.裏通常).N_AtkID;
                        break;
                    case Enum_SetSlot.表スキル1:
                    case Enum_SetSlot.裏スキル1:
                        AtkD = DB.S_Atks[i];
                        SinUI.Type = SelectID == (int)Enum_SetSlot.裏スキル1 ? "B_S1_Atk" : "F_S1_Atk";
                        Select = i == PriSetGet.AtkGet(SelectID == (int)Enum_SetSlot.裏スキル1).S1_AtkID;
                        break;
                    case Enum_SetSlot.表スキル2:
                    case Enum_SetSlot.裏スキル2:
                        AtkD = DB.S_Atks[i];
                        SinUI.Type = SelectID == (int)Enum_SetSlot.裏スキル2 ? "B_S2_Atk" : "F_S2_Atk";
                        Select = i == PriSetGet.AtkGet(SelectID == (int)Enum_SetSlot.裏スキル2).S2_AtkID;
                        break;
                    case Enum_SetSlot.表必殺:
                    case Enum_SetSlot.裏必殺:
                        AtkD = DB.E_Atks[i];
                        SinUI.Type = SelectID == (int)Enum_SetSlot.裏必殺 ? "B_E_Atk" : "F_E_Atk";
                        Select = i == PriSetGet.AtkGet(SelectID == (int)Enum_SetSlot.裏必殺).E_AtkID;
                        break;
                    case Enum_SetSlot.パッシブ1:
                        PassD = DB.Passives[i];
                        SinUI.Type = "P1";
                        Select = i == PriSetGet.Passive.P1_ID;
                        break;
                    case Enum_SetSlot.パッシブ2:
                        PassD = DB.Passives[i];
                        SinUI.Type = "P2";
                        Select = i == PriSetGet.Passive.P2_ID;
                        break;
                    case Enum_SetSlot.パッシブ3:
                        PassD = DB.Passives[i];
                        SinUI.Type = "P3";
                        Select = i == PriSetGet.Passive.P3_ID;
                        break;
                    case Enum_SetSlot.パッシブ4:
                        PassD = DB.Passives[i];
                        SinUI.Type = "P4";
                        Select = i == PriSetGet.Passive.P4_ID;
                        break;
                    case Enum_SetSlot.因子1:
                        GeneD = Genes.Datas[i];
                        SinUI.Type = "G1";
                        Select = i == GeneGet.G1_ID;
                        break;
                    case Enum_SetSlot.因子2:
                        GeneD = Genes.Datas[i];
                        SinUI.Type = "G2";
                        Select = i == GeneGet.G2_ID;
                        break;
                    case Enum_SetSlot.因子3:
                        GeneD = Genes.Datas[i];
                        SinUI.Type = "G3";
                        Select = i == GeneGet.G3_ID;
                        break;
                    case Enum_SetSlot.因子4:
                        GeneD = Genes.Datas[i];
                        SinUI.Type = "G4";
                        Select = i == GeneGet.G4_ID;
                        break;
                    case Enum_SetSlot.因子5:
                        GeneD = Genes.Datas[i];
                        SinUI.Type = "G5";
                        Select = i == GeneGet.G5_ID;
                        break;
                }
                if (CharaD != null)
                {
                    Name = CharaD.Name;
                    Icon = CharaD.Icon;
                }
                if (AtkD != null)
                {
                    Name = AtkD.Name;
                    Icon = AtkD.Icon;
                    if (FilterDr.value > 0)
                    {
                        var FilterKeys = Enum.GetValues(typeof(Enum_AtkFilter));
                        if (!AtkD.Filters.Contains((Enum_AtkFilter)FilterKeys.GetValue(FilterDr.value - 1))) Disp = false;
                    }
                }
                if (PassD != null)
                {
                    Name = PassD.Name;
                    Icon = PassD.Icon;
                    if (FilterDr.value > 0)
                    {
                        var FilterKeys = Enum.GetValues(typeof(Enum_PassiveFilter));
                        if (!PassD.Filters.Contains((Enum_PassiveFilter)FilterKeys.GetValue(FilterDr.value - 1))) Disp = false;
                    }
                }
                if (GeneD != null)
                {
                    if (GeneD.Format != (SelectID - (int)Enum_SetSlot.因子1)) Disp = false;
                    else
                    {
                        if (FilterDr.value > 0)
                        {
                            var FilterKeys = Enum.GetValues(typeof(Enum_GeneTypes));
                            if (GeneD.Type != (int)FilterKeys.GetValue(FilterDr.value - 1)) Disp = false;
                        }
                        if (GeneStaDr.value > 0)
                        {
                            var MStaKeys = Enum.GetValues(typeof(Enum_GeneOptions));
                            if (GeneD.Main != (int)MStaKeys.GetValue(GeneStaDr.value - 1)) Disp = false;
                        }
                        Name = "「" + GeneD.Name + "」";
                        Name += "\n<size=75%>" + Manifesto.ET((Enum_GeneTypes)GeneD.Type) + ":" + Manifesto.ET((Enum_GeneFormat)GeneD.Format) + "</size>";
                        Icon = DB.Genes[GeneD.Type].Images[GeneD.Format];
                        Infos = GeneD.Lock ? "★" : "";
                    }
                }
                SinUI.BackImage.color = DCol.ColSelects(Select);
                if (Select && Disp) SelNum = DispCount;
                SinUI.Name.text = Name;
                SinUI.Icon.texture = Icon;
                SinUI.Info.text = Infos;
            }
            Set_Sin_UIs[i].gameObject.SetActive(i < DataCount && Disp);
            if (i < DataCount && Disp) DispCount++;
        }
        if (SelNum >= 0)
        {
            var NumD = 1f / MathF.Max(1f, DispCount - 1f);
            var NumP = 1f - SelNum / MathF.Max(1f, DispCount - 1f);
            if (SelectsBar.value < (NumP - NumD * 3) || SelectsBar.value > (NumP + NumD * 3)) SelectsBar.value = NumP;
        }
        else SelectsBar.value = 1;

        NonUI.gameObject.SetActive(SelectID >= (int)Enum_SetSlot.因子1);
        NonUI.Returns = this;
        switch ((Enum_SetSlot)SelectID)
        {
            case Enum_SetSlot.因子1: NonUI.Type = "G1"; break;
            case Enum_SetSlot.因子2: NonUI.Type = "G2"; break;
            case Enum_SetSlot.因子3: NonUI.Type = "G3"; break;
            case Enum_SetSlot.因子4: NonUI.Type = "G4"; break;
            case Enum_SetSlot.因子5: NonUI.Type = "G5"; break;
        }
        int GeneSelID = GeneIDGet(SelectID);
        NonUI.BackImage.color = DCol.ColSelects(GeneSelID == -1);
        NonUI.ID = -1;

        SetGeneList.UIUpdates();
        SetGeneAdd.UIUpdates();
        SetOthers.UIUpdates();
        //
        var HP2Set = GeneSetCount(GeneGet, Enum_GeneTypes.体力) >= 2;
        StateTx.text = StatesStr("最大HP", DB.Player.MHP, Enum_GeneOptions.最大HP, PriSetGet.PassiveLVGet(Enum_Passive.HP増加) * 20 + (HP2Set ? 20 : 0),false);
        StateTx.text += "\n" + StatesStr("HP回復速度", DB.Player.HPRegene, Enum_GeneOptions.HP回復速度, PriSetGet.PassiveLVGet(Enum_Passive.自然再生) * 50, false);
        StateTx.text += "\n" + StatesStr("最大MP", DB.Player.MMP, Enum_GeneOptions.最大MP, PriSetGet.PassiveLVGet(Enum_Passive.MP増加) * 20, false);
        StateTx.text += "\n" + StatesStr("MP回復速度", DB.Player.MPRegene, Enum_GeneOptions.MP回復速度, PriSetGet.PassiveLVGet(Enum_Passive.気力増幅) * 10, false);
        StateTx.text += "\n" + StatesStr("SP回復速度", DB.Player.SPRegene, Enum_GeneOptions.SP回復速度, 0, false);
        var SP2Set = GeneSetCount(GeneGet, Enum_GeneTypes.必殺) >= 2;
        StateTx.text += "\n" + StatesStr("SP増加量", 100, Enum_GeneOptions.SP回復量, PriSetGet.PassiveLVGet(Enum_Passive.SPブースト) * 25 + (SP2Set ? 20 : 0), true);
        var Atk2Set = GeneSetCount(GeneGet, Enum_GeneTypes.攻撃) >= 2;
        StateTx.text += "\n" + StatesStr("攻撃力", DB.Player.Atk, Enum_GeneOptions.攻撃力, PriSetGet.PassiveLVGet(Enum_Passive.攻撃力増加) * 10 + (Atk2Set ? 10 : 0), false);
        var Def2Set = GeneSetCount(GeneGet, Enum_GeneTypes.防御) >= 2;
        StateTx.text += "\n" + StatesStr("防御力", DB.Player.Def, Enum_GeneOptions.防御力, PriSetGet.PassiveLVGet(Enum_Passive.防御力増加) * 25 + (Def2Set ? 25 : 0), false);
        StateTx.text += "\n" + StatesStr("近距離ダメージ", 100, Enum_GeneOptions.近ダメージ, PriSetGet.PassiveLVGet(Enum_Passive.近距離強化) * 25, true);
        StateTx.text += "\n" + StatesStr("遠距離ダメージ", 100, Enum_GeneOptions.遠ダメージ, PriSetGet.PassiveLVGet(Enum_Passive.遠距離強化) * 15, true);
        var Hv2Set = GeneSetCount(GeneGet, Enum_GeneTypes.一撃) >= 2;
        var SP4Set = GeneSetCount(GeneGet, Enum_GeneTypes.必殺) >= 4;
        StateTx.text += "\n" + StatesStr("通常ダメージ", 100, Enum_GeneOptions.通常ダメージ,
            PriSetGet.PassiveLVGet(Enum_Passive.メイン強化) * 15 + PriSetGet.PassiveLVGet(Enum_Passive.通常強化) * 20 + (Hv2Set ? -30 : 0), true);
        StateTx.text += "\n" + StatesStr("重撃ダメージ", 100, Enum_GeneOptions.重撃ダメージ,
            PriSetGet.PassiveLVGet(Enum_Passive.メイン強化) * 15 + PriSetGet.PassiveLVGet(Enum_Passive.重落強化) * 25 + (Hv2Set ? 50 : 0), true);
        StateTx.text += "\n" + StatesStr("落下ダメージ", 100, Enum_GeneOptions.落下ダメージ,
            PriSetGet.PassiveLVGet(Enum_Passive.メイン強化) * 15 + PriSetGet.PassiveLVGet(Enum_Passive.重落強化) * 25, true);
        StateTx.text += "\n" + StatesStr("スキルダメージ", 100, Enum_GeneOptions.スキルダメージ, PriSetGet.PassiveLVGet(Enum_Passive.スキル強化) * 25, true);
        StateTx.text += "\n" + StatesStr("必殺ダメージ", 100, Enum_GeneOptions.必殺ダメージ, PriSetGet.PassiveLVGet(Enum_Passive.必殺強化) * 30 + (SP4Set ? 20 : 0), true);

        //プレイヤー反映
        if (BTManager != null)
        {
            for (int i = 0; i < BTManager.PlayerList.Count; i++)
            {
                var PSta = BTManager.PlayerList[i];
                if (PSta != null && PSta.photonView.IsMine)
                {
                    switch (PSta.PLValues.SubID)
                    {
                        case 0:
                            PSta.PLValues.Sets = PriSetGet;
                            break;
                        case 1:
                            if (PSaves.AddSet1 < 0) PhotonNetwork.Destroy(PSta.gameObject);
                            else PSta.PLValues.Sets = PriSets[PSaves.AddSet1];
                            break;
                        case 2:
                            if (PSaves.AddSet2 < 0) PhotonNetwork.Destroy(PSta.gameObject);
                            else PSta.PLValues.Sets = PriSets[PSaves.AddSet2];
                            break;
                        case 3:
                            if (PSaves.AddSet3 < 0) PhotonNetwork.Destroy(PSta.gameObject);
                            else PSta.PLValues.Sets = PriSets[PSaves.AddSet3];
                            break;
                    }
                }
            }
            Net_BaseUI.CharaSet();
        }


    }
    string StatesStr(string TypeName, float BaseVal, Enum_GeneOptions GOP, float Per, bool PerAdd)
    {
        var FVal = BaseVal + GenePowT(GeneGet, GOP);
        if (!PerAdd) FVal *= 1f + (Per * 0.01f);
        else FVal += Per;
        var Str = TypeName;
        Str += "\n<size=80%>" + FVal.ToString("F1");
        if (PerAdd) Str += "%";
        Str += "</size>";
        if (StateAddsT.isOn)
        {
            Str += "<size=60%>\n";
            Str += Manifesto.T("LABEL_BASE") + BaseVal.ToString("F1");
            if (PerAdd) Str += "%";
            Str += "\n" + Manifesto.T("LABEL_GENE") + GenePowT(GeneGet, GOP).ToString("F1");
            if (PerAdd) Str += "%";
            Str += "\n" + Manifesto.T("LABEL_PASSIVE") + Per.ToString("F1") + "%";
            Str += "</size>";
        }
        return Str;
    }
    #region　関数
    void FilterUpdate()
    {
        // フィルターのオプションをクリア
        FilterDr.options.Clear();
        FilterDr.options.Add(new TMP_Dropdown.OptionData { text = "無" });
        int FID = -1;
        // 選択IDに応じたフィルターの更新
        switch ((Enum_SetSlot)SelectID)
        {
            default:
                GeneStaDr.gameObject.SetActive(false);
                break;
            case Enum_SetSlot.表通常:
            case Enum_SetSlot.裏通常:
            case Enum_SetSlot.表スキル1:
            case Enum_SetSlot.裏スキル1:
            case Enum_SetSlot.表スキル2:
            case Enum_SetSlot.裏スキル2:
            case Enum_SetSlot.表必殺:
            case Enum_SetSlot.裏必殺:
                var AtkKeys = Enum.GetValues(typeof(Enum_AtkFilter));
                for (int i = 0; i < AtkKeys.Length; i++)
                {
                    FilterDr.options.Add(new TMP_Dropdown.OptionData { text = Manifesto.ET((Enum_AtkFilter)AtkKeys.GetValue(i)) });
                }
                FID = 0;
                GeneStaDr.gameObject.SetActive(false);
                break;
            case Enum_SetSlot.パッシブ1:
            case Enum_SetSlot.パッシブ2:
            case Enum_SetSlot.パッシブ3:
            case Enum_SetSlot.パッシブ4:
                var PassKeys = Enum.GetValues(typeof(Enum_PassiveFilter));
                for (int i = 0; i < PassKeys.Length; i++)
                {
                    FilterDr.options.Add(new TMP_Dropdown.OptionData { text = PassKeys.GetValue(i).ToString() });
                }
                FID = 1;
                GeneStaDr.gameObject.SetActive(false);
                break;
            case Enum_SetSlot.因子1:
            case Enum_SetSlot.因子2:
            case Enum_SetSlot.因子3:
            case Enum_SetSlot.因子4:
            case Enum_SetSlot.因子5:
                FilterDr.options.Clear();
                FilterDr.options.Add(new TMP_Dropdown.OptionData { text = "タイプ" });
                var TypeKeys = Enum.GetValues(typeof(Enum_GeneTypes));
                for (int i = 0; i < TypeKeys.Length; i++)
                {
                    if ((int)TypeKeys.GetValue(i) == (int)Enum_GeneTypes.終) continue;
                    FilterDr.options.Add(new TMP_Dropdown.OptionData { text = TypeKeys.GetValue(i).ToString() });
                }
                GeneStaDr.options.Clear();
                GeneStaDr.options.Add(new TMP_Dropdown.OptionData { text = "メイン\nステータス" });
                var MStaKeys = Enum.GetValues(typeof(Enum_GeneOptions));
                for (int i = 0; i < MStaKeys.Length; i++)
                {
                    if ((int)MStaKeys.GetValue(i) == (int)Enum_GeneOptions.終) continue;
                    GeneStaDr.options.Add(new TMP_Dropdown.OptionData { text = MStaKeys.GetValue(i).ToString() });
                }
                FID = 2;
                GeneStaDr.gameObject.SetActive(true);
                break;
        }
        if (FilterID != FID) FilterDr.value = 0;
        FilterID = FID;
    }
    void UI_Sin_Set.SetReturn.ReturnID(string Type, int ID)
    {

        // タイプに応じたIDの更新
        switch (Type)
        {
            case "PriSet":
                PSaves.PriSetID = ID;
                break;
            case "AddSet":
                switch (ID)
                {
                    case 0: PSaves.AddSet1 = PSaves.AddSet1 != PSaves.PriSetID ? PSaves.PriSetID : -1; break;
                    case 1: PSaves.AddSet2 = PSaves.AddSet2 != PSaves.PriSetID ? PSaves.PriSetID : -1; break;
                    case 2: PSaves.AddSet3 = PSaves.AddSet3 != PSaves.PriSetID ? PSaves.PriSetID : -1; break;
                }
                break;
            case "SelectChange":
                SelectID = ID;
                FilterUpdate();
                break;
            case "Chara":
                PriSetGet.CharaID = ID;
                break;
            case "F_N_Atk":
                PriSetGet.AtkF.N_AtkID = ID;
                break;
            case "F_S1_Atk":
                PriSetGet.AtkF.S1_AtkID = ID;
                break;
            case "F_S2_Atk":
                PriSetGet.AtkF.S2_AtkID = ID;
                break;
            case "F_E_Atk":
                PriSetGet.AtkF.E_AtkID = ID;
                break;
            case "B_N_Atk":
                PriSetGet.AtkB.N_AtkID = ID;
                break;
            case "B_S1_Atk":
                PriSetGet.AtkB.S1_AtkID = ID;
                break;
            case "B_S2_Atk":
                PriSetGet.AtkB.S2_AtkID = ID;
                break;
            case "B_E_Atk":
                PriSetGet.AtkB.E_AtkID = ID;
                break;
            case "P1":
                PriSetGet.Passive.P1_ID = ID;
                break;
            case "P2":
                PriSetGet.Passive.P2_ID = ID;
                break;
            case "P3":
                PriSetGet.Passive.P3_ID = ID;
                break;
            case "P4":
                PriSetGet.Passive.P4_ID = ID;
                break;
            case "G1":
                GeneGet.G1_ID = ID;
                break;
            case "G2":
                GeneGet.G2_ID = ID;
                break;
            case "G3":
                GeneGet.G3_ID = ID;
                break;
            case "G4":
                GeneGet.G4_ID = ID;
                break;
            case "G5":
                GeneGet.G5_ID = ID;
                break;

        }
        UIUpdate();
        return;
        if (buttonSetting.currentIndex != ID)
        {
            Debug.Log("SetSettings");
            buttonSetting.targets.Clear();
            foreach (var item in Set_Sin_UIs)
            {
                buttonSetting.targets.Add(item.GetComponent<ImageAnimationManager>());
            }
            buttonSetting.SetSettings(ID);
        }
    }

    public void Saves()
    {
        // データを保存
        Save();
    }


    public void MemoSet()
    {
        PriSetGet.Disp = DispIn.text;
        PriSetGet.Memo = MemoIn.text;
    }




    #endregion
}
