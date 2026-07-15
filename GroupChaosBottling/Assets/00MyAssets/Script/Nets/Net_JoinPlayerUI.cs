using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DataBase;
using static Manifesto;
using static PlayerValue;
public class Net_JoinPlayerUI : MonoBehaviour,UI_Sin_Set.SetReturn
{
    Photon.Realtime.Player PlayerD;
    [Tooltip("番号テキスト"), SerializeField] TextMeshProUGUI IndexTx;
    [Tooltip("プレイヤー名テキスト"), SerializeField] TextMeshProUGUI NameTx;
    [Tooltip("プレイヤー名テキスト"), SerializeField] TextMeshProUGUI OKsTx;
    [Tooltip("マスター用管理UI"), SerializeField] GameObject MasterOnlyUI;
    [Tooltip("マスター表示"), SerializeField] GameObject IsMasterUI;

    [SerializeField] TextMeshProUGUI Memo;
    [SerializeField] UI_Sin_Set Chara_UI;
    public List<UI_Sin_Set> FAtk_UIs;
    public List<UI_Sin_Set> BAtk_UIs;
    [SerializeField] List<UI_Sin_Set> Passive_UIs;
    [SerializeField] Toggle ChangeSubTo;
    [SerializeField] TextMeshProUGUI ChangeSubTx;
    [SerializeField] GameObject ValResetBObj;
    [SerializeField] bool ValResetDisp;

    public int AtkID = -1;
    public Image[] Bars;
    public TextMeshProUGUI[] ValTxs;
    State_Base PSta;
    public void UISet(int Index,Photon.Realtime.Player Player,bool Plays,State_Base Sta=null)
    {
        PSta = Sta;
        PlayerD = Player;
        var IndexStr = "[" + Index + "]";
        if(IndexTx.text != IndexStr) IndexTx.text = IndexStr;
        var SubNum = Sta == null ? Index : (Mathf.Max(0, Sta.PLValues.SubID) + 1);
        if (Player != null)
        {
            if (Sta == null && NameTx.text != Player.NickName) NameTx.text = Player.NickName;
            if (Sta != null && NameTx.text != Sta.Name) NameTx.text = Sta.Name;
        }
        else
        {
            var DName = SubNum <= 1 ? "プレイヤー" : ("サブ" + (SubNum - 1));
            if (NameTx.text != DName) NameTx.text = DName;
        }


        if (Player != null && !Plays)
        {
            var MasterDisp = PhotonNetwork.IsMasterClient && PhotonNetwork.LocalPlayer != Player;
            if(MasterOnlyUI.activeSelf!=MasterDisp)MasterOnlyUI.SetActive(MasterDisp);
            if(IsMasterUI.activeSelf != Player.IsMasterClient) IsMasterUI.SetActive(Player.IsMasterClient);
        }

        var CPro = Player != null ? Player.CustomProperties : null;
        var SStr = "";
        if (!PhotonNetwork.OfflineMode)
        {
            var OKs = CPro.TryGetValue("OK", out var oOK) ? (bool)oOK : false;
            var OKStr = OKs ? "OK" : "準備中";
            if (OKsTx.text != OKStr) OKsTx.text = OKStr;
            var OKCol = OKs ? new Color(1, 0.5f, 0) : Color.white;
            if (OKsTx.color != OKCol) OKsTx.color = OKCol;

            if (Sta == null) SStr = !ChangeSubTo.isOn ? "PL_" : "SB_";
            else SStr = Sta.PLValues.SubID <= 0 ? "PL_" : "SB_";

            var SubUse = CPro.TryGetValue("SubUse", out var oSubUse) ? (bool)oSubUse : false;
            if (!SubUse) ChangeSubTx.text = "<color=#FFFF00>";
            else ChangeSubTx.text = !ChangeSubTo.isOn ? "<color=#FFFF00>" : "<color=#FFFFFF>";
            ChangeSubTx.text += "プレイヤー</color>\n";
            if (!SubUse) ChangeSubTx.text += "<color=#FF8888>";
            else ChangeSubTx.text += ChangeSubTo.isOn ? "<color=#FFFF00>" : "<color=#FFFFFF>";
            ChangeSubTx.text += "サブ</color>";
            var MemoStr = CPro.TryGetValue(SStr + "Memo", out var oMemo) ? (string)oMemo : "";
            if (Memo.text != MemoStr) Memo.text = MemoStr;

            var CID = CPro.TryGetValue(SStr + "Chara", out var oCID) ? (int)oCID : 0;
            var CharaData = DB.Charas[CID];
            if (Chara_UI.Icon.texture != CharaData.Icon) Chara_UI.Icon.texture = CharaData.Icon;
        }
        else
        {
            var PriID = 0;
            switch (SubNum)
            {
                case 1: PriID = PSaves.PriSetID; break;
                default: PriID = PSaves.AddSet1; break;
                case 3: PriID = PSaves.AddSet2; break;
                case 4: PriID = PSaves.AddSet3; break;
            }
            if(PriID < 0)
            {
                if(gameObject.activeSelf)gameObject.SetActive(false);
                return;
            }
            else if (!gameObject.activeSelf) gameObject.SetActive(true);
            var MemoStr = "(" + (PriID + 1) + ")" + PriSets[PriID].Disp;
            if (Memo.text != MemoStr) Memo.text = MemoStr;
            var CharaData = DB.Charas[PriSets[PriID].CharaID];
            if (Chara_UI.Icon.texture != CharaData.Icon) Chara_UI.Icon.texture = CharaData.Icon;
        }

        for (int i = 0; i < 4; i++)
        {
            Class_Save_PriSet PriSet = null;
            if (PhotonNetwork.OfflineMode)
            {
                switch (SubNum)
                {
                    case 1: PriSet = PriSetGet; break;
                    default: PriSet = PriSets[PSaves.AddSet1]; break;
                    case 3: PriSet = PriSets[PSaves.AddSet2]; break;
                    case 4: PriSet = PriSets[PSaves.AddSet3]; break;
                }
            }
            #region 表攻撃
            if (FAtk_UIs.Count <= i) FAtk_UIs.Add(Instantiate(FAtk_UIs[0], FAtk_UIs[0].transform.parent));
            var FSinUI = FAtk_UIs[i];
            var FAtkID = 0;
            if (!PhotonNetwork.OfflineMode)FAtkID = CPro.TryGetValue(SStr + "FAtk_" + i, out var oFAtk) ? (int)oFAtk : 0;
            else
            {
                switch (i)
                {
                    case 0: FAtkID = PriSet.AtkF.N_AtkID; break;
                    case 1: FAtkID = PriSet.AtkF.S1_AtkID; break;
                    case 2: FAtkID = PriSet.AtkF.S2_AtkID; break;
                    case 3: FAtkID = PriSet.AtkF.E_AtkID; break;
                }
            }
            Data_Atk FAtkData = null;
            switch (i)
            {
                case 0:
                    FAtkData = DB.N_Atks[FAtkID];
                    break;
                case 1:
                case 2:
                    FAtkData = DB.S_Atks[FAtkID];
                    break;
                case 3:
                    FAtkData = DB.E_Atks[FAtkID];
                    break;
            }
            if (FSinUI.Buttons.interactable != (Sta != null)) FSinUI.Buttons.interactable = Sta != null;
            FSinUI.Returns = this;
            FSinUI.ID = i;
            if(FSinUI.Icon.texture != FAtkData.Icon) FSinUI.Icon.texture = FAtkData.Icon;
            #endregion
            #region 裏攻撃
            if (BAtk_UIs.Count <= i) BAtk_UIs.Add(Instantiate(BAtk_UIs[0], BAtk_UIs[0].transform.parent));
            var BSinUI = BAtk_UIs[i];
            var BAtkID = 0;
            if (!PhotonNetwork.OfflineMode) BAtkID = CPro.TryGetValue(SStr + "BAtk_" + i, out var oBAtk) ? (int)oBAtk : 0;
            else
            {
                switch (i)
                {
                    case 0: BAtkID = PriSet.AtkB.N_AtkID; break;
                    case 1: BAtkID = PriSet.AtkB.S1_AtkID; break;
                    case 2: BAtkID = PriSet.AtkB.S2_AtkID; break;
                    case 3: BAtkID = PriSet.AtkB.E_AtkID; break;
                }
            }
            Data_Atk BAtkData = null;
            switch (i)
            {
                case 0:
                    BAtkData = DB.N_Atks[BAtkID];
                    break;
                case 1:
                case 2:
                    BAtkData = DB.S_Atks[BAtkID];
                    break;
                case 3:
                    BAtkData = DB.E_Atks[BAtkID];
                    break;
            }
            if (BSinUI.Buttons.interactable != (Sta != null)) BSinUI.Buttons.interactable = Sta != null;
            BSinUI.Returns = this;
            BSinUI.ID = i+4;
            if(BSinUI.Icon.texture != BAtkData.Icon) BSinUI.Icon.texture = BAtkData.Icon;
            #endregion
            #region パッシブ
            if (Passive_UIs.Count <= i) Passive_UIs.Add(Instantiate(Passive_UIs[0], Passive_UIs[0].transform.parent));
            var PSinUI = Passive_UIs[i];
            var PID = 0;
            if (!PhotonNetwork.OfflineMode) PID = CPro.TryGetValue(SStr + "Passive_" + i, out var oPassive) ? (int)oPassive : 0;
            else
            {
                switch (i)
                {
                    case 0: PID = PriSet.Passive.P1_ID; break;
                    case 1: PID = PriSet.Passive.P2_ID; break;
                    case 2: PID = PriSet.Passive.P3_ID; break;
                    case 3: PID = PriSet.Passive.P4_ID; break;
                }
            }
            var PassiveData = DB.Passives[PID];
            if(PSinUI.Icon.texture != PassiveData.Icon) PSinUI.Icon.texture = PassiveData.Icon;
            #endregion
        }
        bool ValRBDisp = ValResetDisp && Sta != null && Sta.photonView.IsMine;
        if (ValResetBObj.activeSelf != ValRBDisp) ValResetBObj.SetActive(ValRBDisp);
    }
    public void MasterChange()
    {
        PhotonNetwork.SetMasterClient(PlayerD);
    }
    public void KickPlayer()
    {
        PhotonNetwork.CloseConnection(PlayerD);
    }

    void UI_Sin_Set.SetReturn.ReturnID(string Type, int ID)
    {
        if (AtkID != ID) AtkID = ID;
        else AtkID = -1;
    }
    public void AtkIDReset()
    {
        AtkID = -1;
    }
    public void PLValReset()
    {
        if (PSta == null) return;
        if (!PSta.photonView.IsMine) return;
        PSta.AddInfoReset();
        PSta.PLValues.AddHeal = 0;
        PSta.PLValues.AddBuf = 0;
        PSta.PLValues.AddDBuf = 0;
        PSta.PLValues.E_AtkCount = 0;
        PSta.PLValues.ReceiveDam = 0;
        PSta.PLValues.DeathCount = 0;
        for(int i = 0; i < PSta.PLValues.AtkDams.Length; i++)
        {
            PSta.PLValues.AtkDams[i] = 0;
            PSta.PLValues.AtkHits[i] = 0;
            PSta.PLValues.AtkHeals[i] = 0;
            PSta.PLValues.AtkBufs[i] = 0;
            PSta.PLValues.AtkDBufs[i] = 0;
        }
    }
}
