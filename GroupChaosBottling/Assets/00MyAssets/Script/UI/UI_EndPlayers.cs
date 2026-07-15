using System.Collections.Generic;
using UnityEngine;
using static BattleManager;
using static Statics;
using static DataColors;
public class UI_EndPlayers : MonoBehaviour
{
    [SerializeField] List<Net_JoinPlayerUI> PlayerUIs;

    void Update()
    {
        float AddDamMax = 0;
        int AddHitMax = 0;
        float HealMax = 0;
        int BufMax = 0;
        int DBufMax = 0;
        int E_AtkMax = 0;
        float RecDamMax = 0;
        int DeathMax = 0;
        for (int i = 0; i < BTManager.PlayerList.Count; i++)
        {
            var Sta = BTManager.PlayerList[i];
            if (Sta == null || !Sta.gameObject.activeInHierarchy) continue;
            AddDamMax = Mathf.Max(AddDamMax, Sta.PLValues.AddDamTotal);
            AddHitMax = Mathf.Max(AddHitMax, Sta.PLValues.AddHitTotal);
            HealMax = Mathf.Max(HealMax, Sta.PLValues.AddHeal);
            BufMax = Mathf.Max(BufMax, Sta.PLValues.AddBuf);
            DBufMax = Mathf.Max(DBufMax, Sta.PLValues.AddDBuf);
            E_AtkMax = Mathf.Max(E_AtkMax, Sta.PLValues.E_AtkCount);
            RecDamMax = Mathf.Max(RecDamMax, Sta.PLValues.ReceiveDam);
            DeathMax = Mathf.Max(DeathMax, Sta.PLValues.DeathCount);
        }
        for (int i = 0; i < Mathf.Max(BTManager.PlayerList.Count, PlayerUIs.Count); i++)
        {

            if (PlayerUIs.Count <= i) PlayerUIs.Add(Instantiate(PlayerUIs[0], PlayerUIs[0].transform.parent));
            var SinUI = PlayerUIs[i];
            bool Disp = false;
            if (i < BTManager.PlayerList.Count)
            {
                var Sta = BTManager.PlayerList[i];
                if (Sta != null && Sta.gameObject.activeInHierarchy)
                {
                    Disp = true;
                    SinUI.UISet(i+1, Sta.photonView.Owner, true, Sta);
                    float[] BarPer = new float[8];
                    string[] ValStr = new string[8];


                    if (SinUI.AtkID < 0)
                    {
                        BarPer[0] = Float_Cuts(Sta.PLValues.AddDamTotal / Mathf.Max(1f, AddDamMax), 50);
                        ValStr[0] = Sta.PLValues.AddDamTotal.ToString("F0");
                        BarPer[1] = Float_Cuts(Sta.PLValues.AddHitTotal / Mathf.Max(1f, AddHitMax), 50);
                        ValStr[1] = Sta.PLValues.AddHitTotal.ToString("F0");
                        BarPer[2] = Float_Cuts(Sta.PLValues.AddHeal / Mathf.Max(1f, HealMax), 50);
                        ValStr[2] = Sta.PLValues.AddHeal.ToString("F0");
                        BarPer[3] = Float_Cuts(Sta.PLValues.AddBuf / Mathf.Max(1f, BufMax), 50);
                        ValStr[3] = Sta.PLValues.AddBuf.ToString("F0");
                        BarPer[4] = Float_Cuts(Sta.PLValues.AddDBuf / Mathf.Max(1f, DBufMax), 50);
                        ValStr[4] = Sta.PLValues.AddDBuf.ToString("F0");
                    }
                    else
                    {
                        BarPer[0] = Float_Cuts(Sta.PLValues.AtkDams[SinUI.AtkID] / Mathf.Max(1f, Sta.PLValues.AddDamTotal), 50);
                        ValStr[0] = Sta.PLValues.AtkDams[SinUI.AtkID].ToString("F0");
                        BarPer[1] = Float_Cuts(Sta.PLValues.AtkHits[SinUI.AtkID] / Mathf.Max(1f, Sta.PLValues.AddHitTotal), 50);
                        ValStr[1] = Sta.PLValues.AtkHits[SinUI.AtkID].ToString("F0");
                        BarPer[2] = Float_Cuts(Sta.PLValues.AtkHeals[SinUI.AtkID] / Mathf.Max(1f, Sta.PLValues.AddHeal), 50);
                        ValStr[2] = Sta.PLValues.AtkHeals[SinUI.AtkID].ToString("F0");
                        BarPer[3] = Float_Cuts(Sta.PLValues.AtkBufs[SinUI.AtkID] / Mathf.Max(1f, Sta.PLValues.AddBuf), 50);
                        ValStr[3] = Sta.PLValues.AtkBufs[SinUI.AtkID].ToString("F0");
                        BarPer[4] = Float_Cuts(Sta.PLValues.AtkDBufs[SinUI.AtkID] / Mathf.Max(1f, Sta.PLValues.AddDBuf), 50);
                        ValStr[4] = Sta.PLValues.AtkDBufs[SinUI.AtkID].ToString("F0");
                    }
                    for (int j = 0; j < 4; j++)
                    {
                        var Col = DCol.ColSelects(SinUI.AtkID == j);
                        if (SinUI.FAtk_UIs[j].BackImage.color != Col) SinUI.FAtk_UIs[j].BackImage.color = Col;
                        Col = DCol.ColSelects(SinUI.AtkID == (j + 4));
                        if (SinUI.BAtk_UIs[j].BackImage.color != Col) SinUI.BAtk_UIs[j].BackImage.color = Col;
                    }

                    BarPer[5] = Float_Cuts(Sta.PLValues.E_AtkCount / Mathf.Max(1f, E_AtkMax), 50);
                    ValStr[5] = Sta.PLValues.E_AtkCount.ToString("F0");
                    BarPer[6] = Float_Cuts(Sta.PLValues.ReceiveDam / Mathf.Max(1f, RecDamMax), 50);
                    ValStr[6] = Sta.PLValues.ReceiveDam.ToString("F0");
                    BarPer[7] = Float_Cuts(Sta.PLValues.DeathCount / Mathf.Max(1f, DeathMax), 50);
                    ValStr[7] = Sta.PLValues.DeathCount.ToString("F0");
                    for (int j = 0; j < 8; j++)
                    {
                        if (SinUI.Bars[j].fillAmount != BarPer[j]) SinUI.Bars[j].fillAmount = BarPer[j];
                        if (SinUI.ValTxs[j].text != ValStr[j]) SinUI.ValTxs[j].text = ValStr[j];
                    }
                }
                else
                {
                    Disp = false;
                }
            }

            if (SinUI.gameObject.activeSelf != Disp) SinUI.gameObject.SetActive(Disp);

        }
    }
}
