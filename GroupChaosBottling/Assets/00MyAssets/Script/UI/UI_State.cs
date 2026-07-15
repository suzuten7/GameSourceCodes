using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Manifesto;
using static DataBase;
using static Statics;
using static DataColors;
using static BattleManager;
public class UI_State : MonoBehaviour
{
    public State_Base Sta;
    [SerializeField] TextMeshProUGUI NameTx;
    [SerializeField] Image HPBackBar;
    [SerializeField] Image HPBackFill;
    [SerializeField] Image HPMiddleBar;
    [SerializeField] Image HPMiddleFill;
    [SerializeField] Image HPFrontBar;
    [SerializeField] Image HPFrontFill;

    [SerializeField] Image ShildBackBar;
    [SerializeField] Image ShildBackFill;
    [SerializeField] Image ShildFrontBar;
    [SerializeField] Image ShildFrontFill;

    [SerializeField] GameObject BreakBase;
    [SerializeField] Image BreakBackBar;
    [SerializeField] Image BreakBackFill;
    [SerializeField] Image BreakFrontBar;
    [SerializeField] Image BreakFrontFill;

    [SerializeField] TextMeshProUGUI BreakText;
    [SerializeField] List<UI_Sin_Buf> BufUIs;

    float CHPPer = 1;
    float CShildPer = 0;
    float ShildMax = 0;
    float CBreakPer = 0;
    virtual protected void LateUpdate()
    {
        if(Camera.main!=null) transform.LookAt(Camera.main.transform);
        BaseSet(100);
    }
    public void BaseSet(int HPCut)
    {
        var ChangeSpeed = 0.01f;

        if (NameTx!=null && NameTx.text != Sta.Name) NameTx.text = Sta.Name;
        float HPPer = Float_Cuts( Sta.HP / Mathf.Max(1f, Sta.FMHP), HPCut);
        var HPCol = (Sta.Team == BTManager.LocalCharas[0].Team) ? DCol.F_HP_Base : DCol.E_HP_Base;
        var BufChanges = -Sta.BufTPMultGet(Enum_Bufs.毒) / 60f;
        var BufPer = Float_Cuts(BufChanges / Mathf.Max(1f, Sta.FMHP), HPCut);
        var CHP_FC = Float_Cuts(CHPPer, HPCut);
        if (HPPer < CHPPer)
        {
            if (HPBackBar.fillAmount != CHP_FC) HPBackBar.fillAmount = CHP_FC;
            if(HPBackFill.color != DCol.HP_Rem) HPBackFill.color = DCol.HP_Rem;
            if(HPMiddleBar.fillAmount != HPPer) HPMiddleBar.fillAmount = HPPer;
            if(HPMiddleFill.color != DCol.HP_DBuf) HPMiddleFill.color = DCol.HP_DBuf;
            if(HPFrontBar.fillAmount != HPPer + BufPer) HPFrontBar.fillAmount = HPPer + BufPer;
            if(HPFrontFill.color != HPCol) HPFrontFill.color = HPCol;
            CHPPer = Mathf.Max(CHPPer - ChangeSpeed, HPPer);
        }
        else
        {

            if(HPBackBar.fillAmount != HPPer) HPBackBar.fillAmount = HPPer;
            if(HPBackFill.color != DCol.HP_Add) HPBackFill.color = DCol.HP_Add;
            if(HPMiddleBar.fillAmount != CHP_FC) HPMiddleBar.fillAmount = CHP_FC;
            if(HPMiddleFill.color != DCol.HP_DBuf) HPMiddleFill.color = DCol.HP_DBuf;
            var Buf_FC = Float_Cuts(CHPPer + BufPer, HPCut);
            if (HPFrontBar.fillAmount != Buf_FC) HPFrontBar.fillAmount = Buf_FC;
            if(HPFrontFill.color != HPCol) HPFrontFill.color = HPCol;
            CHPPer = Mathf.Min(CHPPer + ChangeSpeed, HPPer);
        }
        CHPPer = Mathf.Clamp01(CHPPer);
        if (ShildBackBar != null)
        {
            ShildMax = Mathf.Max(ShildMax, Sta.BufPowGet(Enum_Bufs.シールド));
            if(CShildPer <= 0)ShildMax = 0;
            float ShildPer = Float_Cuts(Sta.BufPowGet(Enum_Bufs.シールド) / Mathf.Max(1f, ShildMax), HPCut);
            var CSh_FC = Float_Cuts(CShildPer, HPCut);
            if (ShildPer < CShildPer)
            {
                if (ShildBackBar.fillAmount != CSh_FC) ShildBackBar.fillAmount = CSh_FC;
                if (ShildBackFill.color != DCol.Shild_Rem) ShildBackFill.color = DCol.Shild_Rem;
                if (ShildFrontBar.fillAmount != ShildPer) ShildFrontBar.fillAmount = ShildPer;
                if (ShildFrontFill.color != DCol.Shild_Base) ShildFrontFill.color = DCol.Shild_Base;
                CShildPer = Mathf.Max(CShildPer - ChangeSpeed, ShildPer);
            }
            else
            {
                if (ShildBackBar.fillAmount != ShildPer) ShildBackBar.fillAmount = ShildPer;
                if (ShildBackFill.color != DCol.Shild_Add) ShildBackFill.color = DCol.Shild_Add;
                if (ShildFrontBar.fillAmount != CSh_FC) ShildFrontBar.fillAmount = CSh_FC;
                if (ShildFrontFill.color != DCol.Shild_Base) ShildFrontFill.color = DCol.Shild_Base;
                CShildPer = Mathf.Min(CShildPer + ChangeSpeed, ShildPer);
            }
            CShildPer = Mathf.Clamp01(CShildPer);
        }
        if (BreakBase != null)
        {
            if (BreakBase.activeSelf != Sta.FMBreak > 0) BreakBase.SetActive(Sta.FMBreak > 0);
            if (Sta.BreakT <= 0)
            {
                var BreakVPer = Float_Cuts(Sta.BreakV / Mathf.Max(1f, Sta.FMBreak),100);
                var BreakCPer = Float_Cuts(CBreakPer, 100);
                if (BreakBackBar.fillAmount != BreakVPer) BreakBackBar.fillAmount = BreakVPer;
                if (BreakBackFill.color != DCol.Break_Add) BreakBackFill.color = DCol.Break_Add;
                if (BreakFrontBar.fillAmount != BreakVPer) BreakFrontBar.fillAmount = BreakCPer;
                if (BreakFrontFill.color != DCol.Break_Base) BreakFrontFill.color = DCol.Break_Base;
                if (BreakText != null && BreakText.text != "") BreakText.text = "";
                CBreakPer = Mathf.Min(CBreakPer + ChangeSpeed, BreakVPer);
            }
            else
            {
                var BreakTPer = Float_Cuts(Sta.BreakT / Mathf.Max(1f, Sta.BreakTime),100);
                if(BreakBackBar.fillAmount != BreakTPer) BreakBackBar.fillAmount = BreakTPer;
                if (BreakFrontBar.fillAmount != 0) BreakFrontBar.fillAmount = 0;
                var BreakCol = Color.HSVToRGB(Mathf.Repeat(Time.time * 1f, 1f), 1, 1);
                if(BreakBackFill.color != BreakCol) BreakBackFill.color = BreakCol;
                if(BreakText != null && BreakText.text != "Break!!!") BreakText.text = "Break!!!";
                CBreakPer = 0;
            }
        }

        for (int i = 0; i < Mathf.Max(BufUIs.Count, Sta.Bufs.Count); i++)
        {
            if (i < Sta.Bufs.Count)
            {
                if (BufUIs.Count <= i) BufUIs.Add(Instantiate(BufUIs[0], BufUIs[0].transform.parent));
                var Bufi = Sta.Bufs[i];
                var BufD = DB.Bufs.Find(x => (int)x.Buf == Bufi.ID);
                var PowDisp = Enum_BufPowDisp.テキスト;
                if (BufD != null)
                {
                    if(BufUIs[i].BackImage.color != BufD.Col) BufUIs[i].BackImage.color = BufD.Col;
                    if(BufUIs[i].Icon.texture != BufD.Icon) BufUIs[i].Icon.texture = BufD.Icon;
                    var IconCol = BufUIs[i].Icon.texture != null ? Color.white : Color.clear;
                    if(BufUIs[i].Icon.color != IconCol) BufUIs[i].Icon.color = IconCol;
                    PowDisp = BufD.PowDisp;
                }
                else
                {
                    if(BufUIs[i].BackImage.color != Color.white) BufUIs[i].BackImage.color = Color.white;
                    if(BufUIs[i].Icon.color != Color.clear) BufUIs[i].Icon.color = Color.clear;
                }
                if(BufUIs[i].NameTx.text != ((Enum_Bufs)Bufi.ID).ToString()) BufUIs[i].NameTx.text = ((Enum_Bufs)Bufi.ID).ToString();
                if (PowDisp != Enum_BufPowDisp.割合)
                {
                    var PowStr = Bufi.Pow > 0 ? Bufi.Pow.ToString() : "";
                    if (BufUIs[i].PowTx.text != PowStr) BufUIs[i].PowTx.text = PowStr;
                }
                else if (BufUIs[i].PowTx.text != "") BufUIs[i].PowTx.text = "";
                if (PowDisp != Enum_BufPowDisp.テキスト)
                {
                    var PowPer = Float_Cuts((float)Bufi.Pow / Bufi.PowMax, 30);
                    if (BufUIs[i].PowImage.fillAmount != PowPer) BufUIs[i].PowImage.fillAmount = PowPer;
                }
                else if (BufUIs[i].PowImage.fillAmount != 0) BufUIs[i].PowImage.fillAmount = 0;
                if (Bufi.TimeMax > 0)
                {
                    var BufTimePer = Float_Cuts(1f - ((float)Bufi.Time / Bufi.TimeMax),30);
                    if(BufUIs[i].TimeImage.fillAmount != BufTimePer) BufUIs[i].TimeImage.fillAmount = BufTimePer;
                }
                else if(BufUIs[i].TimeImage.fillAmount != 0) BufUIs[i].TimeImage.fillAmount = 0;
            }
            if(BufUIs[i].gameObject.activeSelf != i < Sta.Bufs.Count) BufUIs[i].gameObject.SetActive(i < Sta.Bufs.Count);
        }
    }
}
