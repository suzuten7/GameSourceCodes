using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using static Suzuten_PlayerSets;
using static Suzuten_PlayerState;
using static Suzuten_DataBase;
using System.Linq;
using static CameraSettings_Gabu;
using Photon.Pun;
public class Suzuten_BattleUI : MonoBehaviourPunCallbacks
{
    #region 変数
    [SerializeField] Suzuten_DataBase DB;
    [SerializeField] TextMeshProUGUI StartEndTx;

    [SerializeField] TextMeshProUGUI[] PLTxs;
    [SerializeField] GameObject StartIms;

    [SerializeField] GameObject CentorLine;
    [SerializeField] TextMeshProUGUI TimeTx;
    [SerializeField] TextMeshProUGUI RoundTx;
    [SerializeField] TextMeshProUGUI DistanceTx;
    [SerializeField] GameObject NetStayUI;
    [SerializeField] TextMeshProUGUI NetCosTx;
    [SerializeField] TextMeshProUGUI NetLeaves;
    [SerializeField] PlayerUI[] PlayerUIs;

    public Suzuten_InfoTxs InfoTxs;
    int LeaveTime;
    #endregion
    #region クラス
    [System.Serializable]
    class PlayerUI
    {
        public Image HPbar;
        public Image Slipbar;
        public Image HPRembar;
        public TextMeshProUGUI HPtx;
        public Image MPbar;
        public List<Slider> JDMPCosts;
        public Image SPbar;
        public Image SPbarFill;
        public RawImage CharaImage;
        public Image CHSTregbar;
        public Image CHSTregRembar;
        public Image CHSTregFill;
        public Image ACSTregbar;
        public Image ACSTregRembar;
        public TextMeshProUGUI CharaName;
        public TextMeshProUGUI WinTx;
        public TextMeshProUGUI[] ACNameTxs;
        public TextMeshProUGUI[] ACPerTxs;
        public Image[] ACBacks;
        public Image[] ACCTs;
        public TextMeshProUGUI DebugTx;
        public List<Suzuten_BufSinUI> BufsSinUI;

    }
    #endregion
    private void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            PlayerUI PUI = PlayerUIs[i];
            PUI.ACSTregRembar.fillAmount = 0;
        }
    }
    private void FixedUpdate()
    {
        if (!PSsCheck()) return;
        for (int i = 0; i < 2; i++)
        {
            #region プレイヤー設定
            PlayerUI PUI = PlayerUIs[i];
            Suzuten_PlayerState PS;
            if (PhotonNetwork.OfflineMode) PS = PSs[i];
            else PS = PSs[i == (PhotonNetwork.LocalPlayer.ActorNumber - 1) ? 0 : 1];
            #endregion
            #region バー変動
            if (PS.LastDamTime >= 30||PS.HP<=0||!BattleFlag)
            {
                PUI.HPRembar.fillAmount = Mathf.Max(PUI.HPRembar.fillAmount - 0.002f, PUI.HPbar.fillAmount);
                PUI.CHSTregRembar.fillAmount = Mathf.Max(PUI.CHSTregRembar.fillAmount - 0.002f, PUI.CHSTregbar.fillAmount);
            }
            #endregion
        }
    }
    void LateUpdate()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
                {
                    NetLeaves.gameObject.SetActive(true);
                    LeaveTime += 1;
                    NetLeaves.text = "対戦相手が\n退出しました\n";
                    NetLeaves.text += Mathf.Clamp((120 - LeaveTime) / 60f, 0, 3f).ToString("F0") + "秒後に自動退出";
                    if (LeaveTime >= 120) PhotonNetwork.LeaveRoom();
                }
                else
                {
                    NetLeaves.gameObject.SetActive(false);
                    LeaveTime = 0;
                    if (PSet.NetStay)
                    {
                        NetStayUI.SetActive(true);
                        NetCosTx.text = "ラウンド:" + PSet.StartRound + "\nプレイヤー状態\n";
                        NetCosTx.text += (PSet.StartStay / 60f).ToString("F1") + "s\n";
                        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
                        {
                            NetCosTx.text += PhotonNetwork.CurrentRoom.Players[i + 1].NickName + ":";
                            NetCosTx.text += (PSet.PlayerCos[i] ? "OK" : "Wait") + ":";
                            NetCosTx.text += (PSet.PlayerOKs[i] ? "OK" : "Wait") + "\n";
                        }
                    }
                    else NetStayUI.SetActive(false);
                }
            }
        }
        else
        {
            NetLeaves.gameObject.SetActive(false);
            NetStayUI.SetActive(false);
        }
        if (!PSsCheck()) return;
        #region 開始前表示
        if (StartTime > 0)
        {
            if (StartTime > 120)
            {
                StartIms.gameObject.SetActive(true);
                StartEndTx.text = "";
                for (int i = 0; i < 2; i++)
                {
                    Suzuten_PlayerState PS;
                    if (PhotonNetwork.OfflineMode) PS = PSs[i];
                    else PS = PSs[i == (PhotonNetwork.LocalPlayer.ActorNumber - 1) ? 0 : 1];
                    if (PhotonNetwork.OfflineMode) PLTxs[i].text = "Player" + (i + 1) + "\n" + PS.CD.CharaName;
                    else
                    {
                        PLTxs[i].text = PS.photonView.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber ? "(You)" : "";
                        PLTxs[i].text += PS.photonView.Owner.NickName + "\n" + PS.CD.CharaName;
                    }
                }
            }
            else
            {
                StartEndTx.text = "<color=#00FFFF>Round:" + RoundCount + "</color>\n<size=150%>" + ((StartTime + 60) / 60f).ToString("F0")+"</size>";
                StartEndTx.color = Color.white;
                StartIms.gameObject.SetActive(false);
            }
        }
        #endregion
        #region 勝敗表示
        else if (WinNums >= -1)
        {
            if (WinNums == -1)
            {
                StartEndTx.text = "<color=#00FFFF>Round:" + RoundCount+ "</color>\nDraw";
                StartEndTx.color = Color.green;
            }
            else
            {
                bool WinCheck = false;
                for(int i = 0; i < WinCounts.Length; i++)
                {
                    if (WinCounts[i] >= BOP_Battle[2]) WinCheck = true;
                }
                if (!WinCheck)
                {
                    StartEndTx.color = Color.white;
                    StartEndTx.text = "<color=#00FFFF>Round:" + RoundCount + "</color>\n";
                    if (PhotonNetwork.OfflineMode)StartEndTx.text += "Player" + (WinNums + 1);
                    else StartEndTx.text += PhotonNetwork.CurrentRoom.Players[WinNums+1].NickName;
                    StartEndTx.text += "WIN";
                }
                else
                {
                    StartEndTx.color = Color.red;
                    StartEndTx.text = "WINNER!!!\n～";
                    if (PhotonNetwork.OfflineMode) StartEndTx.text += "Player" + (WinNums + 1);
                    else StartEndTx.text += PhotonNetwork.CurrentRoom.Players[WinNums+1].NickName;
                    StartEndTx.text += "～\n" + PSs[WinNums].CD.CharaName;
                }
            }
        }
        #endregion
        #region バトル開始表示
        else if (BattleTime>0)
        {

            StartIms.gameObject.SetActive(false);
            StartEndTx.text = "GO!!!";
            StartEndTx.color -= new Color(0, 0, 0, 0.005f);

        }
        #endregion
        #region タイムアップ表示
        else
        {
            StartIms.gameObject.SetActive(false);
            StartEndTx.text = "TimeUP!!!";
            if (BattleTime == 0) StartEndTx.color = new Color(1, 0, 0, 0);
            else StartEndTx.color += new Color(0, 0, 0, 0.01f);
        }
        #endregion

        #region 各情報表示
        CentorLine.SetActive(!MultiDisplayMode);
        TimeTx.text = (Mathf.Max(0, BattleTime)/60f).ToString("F0");
        if (BattleTime > 600) TimeTx.color = Color.white;
        else if(BattleTime > 0)
        {
            Color HSV = new Color(0,0.5f + Mathf.Sin(BattleTime/15f)/2f, 1);
            TimeTx.color = Color.HSVToRGB(HSV.r,HSV.g,HSV.b);
        }
        else TimeTx.color = Color.red;
        RoundTx.text = "Round:" + RoundCount;
        DistanceTx.text = (Vector3.Distance(PSs[0].PosGet(), PSs[1].PosGet())*100f).ToString("F0") + "TGMKm";
        #endregion
        for (int i = 0; i < 2; i++)
        {
            #region プレイヤー設定
            PlayerUI PUI = PlayerUIs[i];
            Suzuten_PlayerState PS;
            int PID;
            if (PhotonNetwork.OfflineMode) PID = i;
            else PID = i == (PhotonNetwork.LocalPlayer.ActorNumber - 1) ? 0 : 1;
            PS = PSs[PID];
            #endregion
            #region 各ステータス表示
            #region プレイヤー名
            if (PhotonNetwork.OfflineMode) PUI.CharaName.text = "Player" + (PS.PI.playerIndex + 1) + ":" + PS.CD.CharaName;
            else
            {
                PUI.CharaName.text = PS.photonView.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber ? "(You)" : "";
                PUI.CharaName.text += PS.photonView.Owner.NickName + ":" + PS.CD.CharaName;
            }
            #endregion
            PUI.CharaImage.texture = PS.CD.CharaImage;
            #region 勝ち数
            if (BOP_Battle[2] > 5) PUI.WinTx.text = "★" + WinCounts[PID] + "/" + BOP_Battle[2];
            else if (BOP_Battle[2] > 0)
            {
                PUI.WinTx.text = "";
                for (int j = 0; j < BOP_Battle[2]; j++) PUI.WinTx.text += WinCounts[PID] > j ? "★" : "☆";
            }
            else PUI.WinTx.text = "★" + WinCounts[PID];
            #endregion
            #region スリップダメージ計算
            float SlipHP = PS.HP;
            if (PS.Bufs.TryGetValue((int)BufsE.炎上, out BufsC Buf_Fire)) SlipHP -= Buf_Fire.BufPower * 0.1f* Buf_Fire.BufTime * (BOP_AtkStan[6] * 0.01f);
            if (PS.Bufs.TryGetValue((int)BufsE.毒, out BufsC Buf_Poison)) SlipHP -= Buf_Poison.BufPower * 0.1f * Buf_Poison.BufTime * (BOP_AtkStan[6] * 0.01f);
            #endregion
            #region 各バー設定
            PUI.HPbar.fillAmount = SlipHP / (PS.CD.MHP * (BOP_HMSP[0] * 0.01f));
            PUI.Slipbar.fillAmount = PS.HP / (PS.CD.MHP * (BOP_HMSP[0] * 0.01f));
            if (Settings_Bool[PS.PI.playerIndex, 2])
            {
                PUI.HPtx.text = "<size=50%>"+PS.HP.ToString("F0") + "/"+ (PS.CD.MHP * (BOP_HMSP[0] * 0.01f)).ToString("F0");
                PUI.HPtx.text += "</size><size=30%>(" + (PS.HP / (PS.CD.MHP * (BOP_HMSP[0] * 0.01f)) * 100f).ToString("F1") + "%)</size>";
            }
            else PUI.HPtx.text = (PS.HP / (PS.CD.MHP * (BOP_HMSP[0] * 0.01f)) * 100f).ToString("F1") + "%";
            PUI.MPbar.fillAmount = PS.MP / (PS.CD.MMP * (BOP_HMSP[2] * 0.01f));
            PUI.SPbar.fillAmount = PS.SP / (PS.CD.MSP * (BOP_HMSP[4] * 0.01f));
            PUI.CHSTregbar.fillAmount = PS.CHST / Mathf.Max(1, PS.CD.StanRegist * (BOP_AtkStan[2] * 0.01f));
            if (PS.ActionTime > 0) PUI.ACSTregRembar.fillAmount = 1;
            else if (PS.ActionTime < 0) PUI.ACSTregRembar.fillAmount = (float)-PS.ActionTime / Mathf.Max(1, PS.StanTime);
            else PUI.ACSTregRembar.fillAmount = 0;
            PUI.CHSTregFill.color = PS.ActionTime >= 0 ? Color.cyan : Color.red;
            if (PS.ActionTime > 0 && PS.ActionID >= 0) PUI.ACSTregbar.fillAmount = PS.ACST / Mathf.Max(1, PS.CD.Actions[PS.ActionID].StanRegist * (BOP_AtkStan[3] * 0.01f));
            else PUI.ACSTregbar.fillAmount = 0;
            #endregion
            #region ジャンプダッシュコスト表示
            int JDCounts = Mathf.FloorToInt(PS.CD.MMP * (BOP_HMSP[2] * 0.01f) / DB.JDSPCost);
            for (int j = 0; j < Mathf.Max(JDCounts, PUI.JDMPCosts.Count); j++)
            {
                if (JDCounts <= 30)
                {
                    if (j < JDCounts)
                    {
                        if (j >= PUI.JDMPCosts.Count) PUI.JDMPCosts.Add(Instantiate(PUI.JDMPCosts[0], PUI.JDMPCosts[0].transform.parent));
                        PUI.JDMPCosts[j].gameObject.SetActive(true);
                        PUI.JDMPCosts[j].value = (j + 1f) * (DB.JDSPCost / (PS.CD.MMP * (BOP_HMSP[2] * 0.01f)));
                    }
                    else PUI.JDMPCosts[j].gameObject.SetActive(false);
                }
                else
                {
                    if (j < PUI.JDMPCosts.Count) PUI.JDMPCosts[j].gameObject.SetActive(false);
                }

            }
            #endregion
            #endregion
            #region 各アクション表示
            #region SP表示色設定
            int SPUseds = -1;
            for(int k = 0; k < 5; k++)
            {
                int IDss = ACSetID[PID, k];
                if (IDss >=0&& PS.CD.Actions[IDss] !=null && PS.CD.Actions[IDss].SPCost > 0 && PS.SP >= PS.CD.Actions[IDss].SPCost) SPUseds = PS.CD.Actions[IDss].SPAC ? 1 : 0;
            }
            if (SPUseds == -1) PUI.SPbarFill.color = Color.yellow;
            else if (SPUseds == 0) PUI.SPbarFill.color = Color.cyan;
            else
            {
                Color SP_BaseCol = PUI.SPbarFill.color;
                Color SP_HSVCol;
                Color.RGBToHSV(SP_BaseCol, out SP_HSVCol.r, out SP_HSVCol.g, out SP_HSVCol.b);
                SP_HSVCol.r = Mathf.Repeat(SP_HSVCol.r + 0.01f, 1f);
                PUI.SPbarFill.color = Color.HSVToRGB(SP_HSVCol.r, 1, 1);
            }
            #endregion
            #region アクション名,CT表示
            for (int j = 0; j < 5; j++)
            {
                int IDss = ACSetID[PID, j];
                Suzuten_ActionData AD = IDss >= 0 ? PS.CD.Actions[IDss] : null;
                if (AD)
                {
                    PUI.ACNameTxs[j].text = AD.ACName;
                    if (AD.SPCost > 0)
                    {
                        PUI.ACPerTxs[j].text = (100f * AD.SPCost / (PS.CD.MSP * BOP_HMSP[4] * 0.01f)).ToString("F0") + "%";
                        PUI.ACPerTxs[j].color = PS.SP >= AD.SPCost ? Color.green : Color.red;
                    }
                    else PUI.ACPerTxs[j].text = "";
                    if (PS.ActionTime < 0) PUI.ACBacks[j].color = PS.ActionID == j ? Color.red : new Color(1f, 0.75f, 0.5f);
                    else if (PS.ActionTime > 0 && PS.ActionID == j) PUI.ACBacks[j].color = Color.yellow;
                    else if (AD.SPAC)
                    {
                        if (PS.SP >= AD.SPCost)
                        {
                            Color Col = PUI.ACBacks[j].color;
                            Color.RGBToHSV(Col, out Col.r, out Col.g, out Col.b);
                            Col = Color.HSVToRGB(Mathf.Repeat(Col.r + 0.01f, 1f), 1, 1);
                            PUI.ACBacks[j].color = Col;
                        }
                        else
                        {
                            PUI.ACBacks[j].color = new Color(1f, 0.75f, 0.5f);
                        }

                    }
                    else if (AD.SPCost <= 0) PUI.ACBacks[j].color = Color.white;
                    else PUI.ACBacks[j].color = PS.SP >= AD.SPCost ? Color.cyan : new Color(1f, 0.75f, 0.5f);

                    PUI.ACCTs[j].fillAmount = (float)PS.ACCTs[j] / Mathf.Max(1, Mathf.RoundToInt(AD.CT * 60 * (BOP_AtkStan[7] * 0.01f)));
                }
                else
                {
                    PUI.ACNameTxs[j].text = "無し";
                    PUI.ACPerTxs[j].text = "";
                    PUI.ACBacks[j].color = Color.gray;
                    PUI.ACCTs[j].fillAmount = 0.0f;
                }

            }
            #endregion
            #endregion
            #region バフ表示
            List<int> Bufs = PS.Bufs.Keys.ToList();
            for(int j = 0; j < Mathf.Max(PUI.BufsSinUI.Count,Bufs.Count); j++)
            {
                if (PUI.BufsSinUI.Count <= j) PUI.BufsSinUI.Add(Instantiate(PUI.BufsSinUI[0], PUI.BufsSinUI[0].transform.parent));
                if (j < Bufs.Count)
                {
                    PS.Bufs.TryGetValue(Bufs[j], out BufsC Buf);
                    if (Buf != null)
                    {
                        PUI.BufsSinUI[j].gameObject.SetActive(true);
                        PUI.BufsSinUI[j].BufName.text = ((BufsE)Bufs[j]).ToString();
                        PUI.BufsSinUI[j].BufPower.text = Buf.BufPower>0 ? Buf.BufPower.ToString("F0") : "";
                        PUI.BufsSinUI[j].BufTimesIm.fillAmount = (float)Buf.BufTime / Mathf.Max(1, Buf.BufSTime);
                        PUI.BufsSinUI[j].BufBacks.color = DB.BufColors[(int)Bufs[j]];
                    }
                    else
                    {
                        PUI.BufsSinUI[j].gameObject.SetActive(false);
                    }
                }
                else
                {
                    PUI.BufsSinUI[j].gameObject.SetActive(false);
                }
            }
            #endregion
            #region デバッグ表示
            PUI.DebugTx.text = "[デバッグ]";
            PUI.DebugTx.text += "\n物理力:"+PS.RigPow.ToString("F1");
            PUI.DebugTx.text += "\n衝突ダメージ:" + PS.PhisPow.ToString("F0");
            PUI.DebugTx.text += "\n速度:" + PS.RigObj.velocity;
            #endregion
        }

    }
    public void SceneB(int SceneID)
    {
        SceneChangeUIs.SCUIDisp();
        SceneManager.LoadSceneAsync(SceneID);
    }

    public override void OnLeftRoom()
    {
        if (LeaveTime < 120) return;
        SceneChangeUIs.SCUIDisp();
        SceneManager.LoadSceneAsync(2);

    }
}
