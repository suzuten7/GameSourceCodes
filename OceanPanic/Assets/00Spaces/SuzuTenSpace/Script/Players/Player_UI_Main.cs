using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using static GameInfos;
using System.Linq;
using UnityEngine.InputSystem;
using static DataBase;
using System.Collections.Generic;

public class Player_UI_Main : MonoBehaviour
{
    #region エディタ変数
    [SerializeField] Player_States PSta;
    [SerializeField] Player_ActionBase ACs;
    PlayerInput PI;

    [SerializeField] TextMeshProUGUI PosTx;
    [SerializeField] Transform PlayerTrans;
    [SerializeField] Transform CamTrans;

    [SerializeField] bool SPNoUse;
    [SerializeField] GameObject SPBars;
    [SerializeField] Slider SPBar;
    [SerializeField] GameObject LowSPs;

    [SerializeField] GameObject ACUIs;
    [SerializeField] Slider[] ACCTBars;
    [SerializeField] TextMeshProUGUI[] ACKeyTxs;
    [SerializeField] string[] ACKeyStrs1;
    [SerializeField] string[] ACKeyStrs2;
    [SerializeField] string[] ACKeyStrs3;
    [SerializeField] string[] ACKeyStrs4;

    [SerializeField] Image SpakePanel;
    [SerializeField] TextMeshProUGUI SpakeTxs;

    [SerializeField] TextMeshProUGUI GTimeTx;

    [SerializeField] Player_UI_Sin_Message MessageBase;

    [SerializeField] TextMeshProUGUI FeCoTx;
    [SerializeField] TextMeshProUGUI ScoreTx;
    [SerializeField] GameObject EndUI;
    [SerializeField] TextMeshProUGUI EndWinTxs;
    [SerializeField] GameObject EndResultPanel;
    [SerializeField] TextMeshProUGUI ResultBattleTimeTxs;
    [SerializeField] TextMeshProUGUI ResultWinTxs;
    [SerializeField] TextMeshProUGUI ResultPilotRoleTxs;
    [SerializeField] TextMeshProUGUI ResultPilotScoreTxs;
    [SerializeField] TextMeshProUGUI ResultPilotPlayerTxs;

    [SerializeField] TextMeshProUGUI ResultVisitRoleTxs;
    [SerializeField] TextMeshProUGUI ResultVisitScoreTxs;
    [SerializeField] TextMeshProUGUI ResultVisitPlayerTxs;

    [SerializeField] List<Player_UI_Sin_EndFeResult> FeResultUIs;


    [SerializeField] TextMeshProUGUI EndSCTxs;

    [SerializeField] TextMeshProUGUI InputTxs;
    [SerializeField, TextArea] string[] InputStrs;
    [SerializeField, TextArea] string RoleStrs;
    #endregion
    #region 内部変数
    int Rampsine = 0;
    #endregion
    void Start()
    {
        PI = FindFirstObjectByType<PlayerInput>();
        #region 表示初期化
        if (SpakePanel != null)
        {
            SpakePanel.gameObject.SetActive(true);
            Color SpakesCol = SpakePanel.color;
            SpakesCol.a = 0;
            SpakePanel.color = SpakesCol;
        }
        EndPanelOC(false);
        #endregion
    }
    void LateUpdate()
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        int CScID = -1;
        switch (PI.currentControlScheme)
        {
            default: CScID = 0; break;
            case "KeyboardMouse": CScID = 0; break;
            case "Gamepad": CScID = 1; break;
            case "VR": CScID = -1; break;
            case "Touch": CScID = -1; break;
        }
        #region 入力テキスト設定
        if (InputTxs != null)
        {
            if (CScID >= 0 && CScID < InputStrs.Length) InputTxs.text = $"<size=25><color=#E6E6E633>{InputStrs[CScID]}</color></size>";
            else InputTxs.text = "";
            InputTxs.text += "\n" + RoleStrs;
        }
        #endregion
        #region 座標テキスト設定
        PosTx.text = "(" + PlayerTrans.position.x.ToString("F0");
        PosTx.text += "," + PlayerTrans.position.y.ToString("F0");
        PosTx.text += "," + PlayerTrans.position.z.ToString("F0") + ")";
        #endregion
        #region スタミナバー設定
        if (SPBars != null) SPBars.SetActive(!PSta.GostModes && !SPNoUse);
        if (SPBar != null)
        {
            SPBar.value = (float)PSta.SP / PSta.MSP;
            LowSPs.SetActive(PSta.LowSP);
        }
        #endregion
        #region CTバー設定
        if (ACUIs != null) ACUIs.SetActive(!PSta.GostModes);
        for (int i = 0; i < ACCTBars.Length; i++)
        {
            ACCTBars[i].value = (float)ACs.CTs[i] / ACs.ACCTs[i];
        }
        if (ACKeyTxs.Length > 0)
        {
            ACKeyTxs[0].text = ACKeyStrs1[CScID];
            ACKeyTxs[1].text = ACKeyStrs2[CScID];
            ACKeyTxs[2].text = ACKeyStrs3[CScID];
            ACKeyTxs[3].text = ACKeyStrs4[CScID];
        }
        #endregion
        #region ショック設定
        if (SpakePanel != null)
        {
            Color SpakesCol = SpakePanel.color;
            if (PSta.Spakes > 0) SpakesCol.a += 0.02f;
            else SpakesCol.a -= 0.01f;
            SpakesCol.a = Mathf.Clamp(SpakesCol.a, 0, 0.6f);
            SpakePanel.color = SpakesCol;
            SpakeTxs.alpha = SpakesCol.a;
        }
        #endregion
        #region 時間テキスト設定
        if (GInfo.STTime <= OniWaitTime) GTimeTx.text = "<size=50>鬼待機時間</size>\n" + ((OniWaitTime + 60 - GInfo.STTime) / 60).ToString("D0") + "秒";
        else
        {
            int LTime = (GInfo.TLimits - GInfo.GameTime);
            GTimeTx.text = (LTime / 3600).ToString("D2") + ":" + (LTime / 60 % 60).ToString("D2");
        }
        GTimeTx.color = GInfo.Ramps ? new Color(1f, Mathf.Sin(Rampsine * 0.1f) / 2f + 0.5f, Mathf.Sin(Rampsine * 0.1f) / 2f + 0.5f) : Color.white;
        #endregion
        #region メッセージ設定
        var Mess = GInfo.Messages.Keys.ToArray();
        for (int i = 0; i < Mess.Length; i++)
        {
            if ((PSta.Types == TypesE.鬼操縦者 || PSta.Types == TypesE.鬼監視者) && GInfo.Messages[Mess[i]] == 1) continue;
            if ((PSta.Types == TypesE.逃げ) && GInfo.Messages[Mess[i]] == 2) continue;
            var MesIns = Instantiate(MessageBase, MessageBase.transform.parent);
            MesIns.gameObject.SetActive(true);
            MesIns.Texts.text = Mess[i];
        }
        GInfo.Messages.Clear();
        #endregion
        ScoreTx.text = "<size=30>スコア</size>\n" + PSta.Score;
        bool PlancLifeMode = CRoom.CustomProperties.TryGetValue("GameOption2", out var Op2Val) && (bool)Op2Val;
        #region 逃走者テキスト設定
        if (PlancLifeMode)
        {
            FeCoTx.text = "逃走者死亡数\\n" + GInfo.Deaths + "/" + Mathf.RoundToInt(GInfo.FePlanTotal * 1.5f);
        }
        else
        {
            FeCoTx.text = "残逃走者数\\n" + GInfo.FePlanCount + "/" + GInfo.FePlanTotal;
        }
        #endregion
        #region ゲーム終了UI設定
        if (GInfo.End)
        {
            EndUI.SetActive(true);
            #region 勝利処理
            bool PCWin = true;
            if (GInfo.FePlanCount <= 0) PCWin = false;
            if (PlancLifeMode && GInfo.Deaths >= Mathf.RoundToInt(GInfo.FePlanTotal * 1.5f)) PCWin = false;
            bool Win = false;
            if (PSta.Types != TypesE.逃げ)
            {
                Win = !PCWin;
            }
            else
            {
                Win = PCWin;
            }
            if (!PCWin) EndWinTxs.text = "プランクトン\n(逃走者)が絶滅した\n";
            else EndWinTxs.text = "プランクトン\n(逃走者)は逃げ切った\n";
            if (PSta.Types != TypesE.他)
            {
                if (Win)
                {
                    EndWinTxs.text += "勝利";
                    EndWinTxs.color = Color.red;
                }
                else
                {
                    EndWinTxs.text += "敗北";
                    EndWinTxs.color = Color.blue;
                }
            }
            else
            {
                EndWinTxs.color = Color.magenta;
            }
            #endregion
            #region リザルト
            int PilotID = (int)CRoom.CustomProperties["Oni_Pilot"];
            int VisitID = (int)CRoom.CustomProperties["Oni_Visit"];
            bool WMode = (CRoom.CustomProperties.TryGetValue("GameOption1", out var Op1Val) && (bool)Op1Val);
            ResultBattleTimeTxs.text = "試合時間";
            ResultBattleTimeTxs.text += (GInfo.ToGameTime / 3600).ToString("D2") + ":" + (GInfo.ToGameTime / 60 % 60).ToString("D2");
            ResultWinTxs.text = (PCWin ? "逃走者" : "鬼") + "の勝利";
            if (PSta.Types != TypesE.他) ResultWinTxs.color = Win ? Color.red : Color.blue;
            else ResultWinTxs.color = Color.magenta;
            #region 操縦者
            ResultPilotRoleTxs.text = WMode ? "操縦者1" : "操縦者";
                Player_Pilot_WhalesAction RPilot = null;
            for (int i = 0; i < PilotAcs.Length; i++)
            {
                if (PilotAcs[i] == null) continue;
                if (PilotAcs[i].photonView.OwnerActorNr == PilotID)
                {
                    RPilot = PilotAcs[i];
                    break;
                }
            }
            if (RPilot != null)
            {
                ResultPilotPlayerTxs.text = RPilot.photonView.Owner.NickName;
                ResultPilotScoreTxs.text = "スコア:" + RPilot.PSta.Score;
            }
            else
            {
                ResultPilotPlayerTxs.text = "<color=#FF0000><<不在>></color>";
                ResultPilotScoreTxs.text = "スコア:-";
            }
            #endregion
            #region 監視者
            ResultVisitRoleTxs.text = WMode ? "操縦者2" : "監視者";
            Player_Pilot_WhalesAction RPilot2 = null;
            Player_Visit_WhalesAction RVisit = null;
            if (WMode)
            {
                for (int i = 0; i < PilotAcs.Length; i++)
                {
                    if (PilotAcs[i] == null) continue;
                    if (PilotAcs[i].photonView.OwnerActorNr == VisitID)
                    {
                        RPilot2 = PilotAcs[i];
                        break;
                    }
                }
                if (RPilot2 != null)
                {
                    ResultVisitPlayerTxs.text = RPilot2.photonView.Owner.NickName;
                    ResultVisitScoreTxs.text = "スコア:" + RPilot2.PSta.Score;
                }
                else
                {
                    ResultVisitPlayerTxs.text = "<color=#FF0000><<不在>></color>";
                    ResultVisitScoreTxs.text = "スコア:-";
                }
            }
            else
            {
                for (int i = 0; i < VisitAcs.Length; i++)
                {
                    if (VisitAcs[i] == null) continue;
                    if (VisitAcs[i].photonView.OwnerActorNr == VisitID)
                    {
                        RVisit = VisitAcs[i];
                        break;
                    }
                }
                if (RVisit != null)
                {
                    ResultVisitPlayerTxs.text = RVisit.photonView.Owner.NickName;
                    ResultVisitScoreTxs.text = "スコア:" + RVisit.PSta.Score;
                }
                else
                {
                    ResultVisitPlayerTxs.text = "<color=#FF0000><<不在>></color>";
                    ResultVisitScoreTxs.text = "スコア:-";
                }
            }
            #endregion
            #region 逃走者
            for(int i = 0; i < Mathf.Max(FugiAcs.Length, FeResultUIs.Count); i++)
            {
                if (i < FugiAcs.Length)
                {
                    if (FeResultUIs.Count <= i)
                    {
                        var InsRUI = Instantiate(FeResultUIs[0], FeResultUIs[0].transform.parent);
                        FeResultUIs.Add(InsRUI);
                    }
                    var SinRUI = FeResultUIs[i];
                    if (FugiAcs[i] == null)
                    {
                        SinRUI.gameObject.SetActive(false);
                        continue;
                    }
                    SinRUI.gameObject.SetActive(true);
                    SinRUI.PlayerTx.text = FugiAcs[i].photonView.Owner.NickName;
                    SinRUI.ScoreTx.text = FugiAcs[i].PSta.Score.ToString();
                    if (PlancLifeMode)
                    {
                        SinRUI.DeathTx.color = Color.white;
                        SinRUI.DeathTx.text = FugiAcs[i].PSta.DeathCount.ToString();
                    }
                    else
                    {
                        if (FugiAcs[i].PSta.DeathTime >= 0)
                        {
                            var DTime = FugiAcs[i].PSta.DeathTime;
                            SinRUI.DeathTx.color = new Color(1, 0.6f, 0.6f);
                            SinRUI.DeathTx.text = (DTime / 3600).ToString("D2") + ":" + (DTime / 60 % 60).ToString("D2");
                        }
                        else
                        {
                            SinRUI.DeathTx.color = new Color(1, 1, 0);
                            SinRUI.DeathTx.text = "生存";
                        }
                    }


                }
                else FeResultUIs[i].gameObject.SetActive(false);
            }
            #endregion
            #endregion

            EndSCTxs.text = ((600 - GInfo.EndTime + 1) / 60).ToString("D0") + "秒後にルームに戻ります";
        }
        else EndUI.SetActive(false);
        #endregion
    }
    private void FixedUpdate()
    {
        #region カウント
        if (GInfo.Ramps) Rampsine++;
        #endregion
    }

    public void EndPanelOC(bool OC)
    {
        EndResultPanel.SetActive(OC);
    }
}
