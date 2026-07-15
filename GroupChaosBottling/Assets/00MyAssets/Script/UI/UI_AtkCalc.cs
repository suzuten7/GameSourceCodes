using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Windows;
using static DataBase;
using static Manifesto;
public class UI_AtkCalc : MonoBehaviour,UI_Sin_Set.SetReturn
{
    [SerializeField] TMP_Dropdown SelectDr;
    [SerializeField] UI_Sin_Set SetUI;
    [SerializeField] List<UI_Sin_Set> SelectUIs;
    [SerializeField] List<UI_Sin_Set> InputUIs;
    [SerializeField] TMP_Dropdown TimeCheckDr;
    [SerializeField] Slider HitTimeSlider;
    [SerializeField] TextMeshProUGUI HitTimeTx;
    [SerializeField] TextMeshProUGUI OutTx;
    public int SetID = -1;
    public List<int> Inputs;
    void Start()
    {
        UIUpdate();
    }
    public void UIUpdate()
    {
        Data_Atk[] AtkDatas = new Data_Atk[0];
        Data_Atk AData = null;
        List<Class_Atk_BranchInfo> AtkBInfos = new List<Class_Atk_BranchInfo>();
        if (SetID < 0)
        {
            switch (SelectDr.value)
            {
                default: AtkDatas = DB.N_Atks; break;
                case 1: AtkDatas = DB.S_Atks; break;
                case 2: AtkDatas = DB.E_Atks; break;
            }
        }
        else
        {
            switch (SelectDr.value)
            {
                default: AData = DB.N_Atks[SetID]; break;
                case 1: AData = DB.S_Atks[SetID]; break;
                case 2: AData = DB.E_Atks[SetID]; break;
            }
            if (AData.Branchs != null) AtkBInfos = AData.BranchInfos;
        }
        for (int i = 0; i < Mathf.Max(SelectUIs.Count, AtkDatas.Length, AtkBInfos.Count); i++)
        {
            if (SelectUIs.Count <= i) SelectUIs.Add(Instantiate(SelectUIs[0], SelectUIs[0].transform.parent));
            var SinUI = SelectUIs[i];
            SinUI.ID = i;
            SinUI.Returns = this;
            bool Disp = false;
            if (AData == null)
            {
                if (i < AtkDatas.Length)
                {
                    Disp = true;
                    SinUI.Type = "Atk";
                    SinUI.Name.text = AtkDatas[i].Name;
                    SinUI.Icon.texture = AtkDatas[i].Icon;
                    SinUI.Icon.color = Color.white;
                }
            }
            else
            {
                if (i < AtkBInfos.Count)
                {
                    if(Inputs.Count<=0)Disp = true;
                    else
                    {
                        bool Check = false;
                        var BIndex = AData.BranchInfos[Inputs[Inputs.Count - 1]].BID;
                        var FID = AData.BranchInfos[i].BID;
                        for (int j = 0; j < AData.Branchs.Length; j++)
                        {
                            var BDa = AData.Branchs[j];
                            for (int k = 0; k < BDa.BranchNums.Length; k++)
                            {
                                if (BDa.BranchNums[k] == BIndex && BDa.FutureNum == FID)
                                {
                                    Disp = true;
                                    Check = true;
                                    break;
                                }
                            }
                            if (Check) break;
                        }
                    }
                    SinUI.Type = "Branch";
                    SinUI.Name.text = AtkBInfos[i].Name;
                    SinUI.Icon.color = Color.clear;
                }

            }
            SinUI.gameObject.SetActive(Disp);
        }
        for(int i = 0; i < Mathf.Max(InputUIs.Count, Inputs.Count); i++)
        {
            if (InputUIs.Count <= i) InputUIs.Add(Instantiate(InputUIs[0], InputUIs[0].transform.parent));
            var SinUI = InputUIs[i];
            SinUI.Returns = this;
            var Disp = false;
            if(i < Inputs.Count)
            {
                Disp = true;
                var DBranch = AData.BranchInfos[Inputs[i]];
                SinUI.Name.text = DBranch.Name;
            }
            SinUI.gameObject.SetActive(Disp);
        }
        SelectDr.gameObject.SetActive(AData == null);
        SetUI.gameObject.SetActive(AData != null);
        OutTx.text = "";
        int HitF = Mathf.RoundToInt(Mathf.Pow(HitTimeSlider.value, 3)) + 1;
        HitTimeTx.text = Manifesto.T("LABEL_HIT") + HitF + "f";
        if (AData != null)
        {
            SetUI.Name.text = AData.Name;
            SetUI.Icon.texture = AData.Icon;
            SetUI.Returns = this;
            if (AData.Branchs == null || AData.Branchs.Length <= 0)
            {
                AData.DamGets(0, HitF, 0, out var oDam, out var oHit, out var oBreak, out var oHeal);
                OutTx.text = AData.InfoDams(AData.EndTime,oDam,oHit,oBreak,oHeal);
                OutTx.text += "\n" + AData.InfoGetBranchs(0);
            }
            else if (Inputs.Count > 0)
            {
                var BTime = 0;
                var BDam = 0;
                var BHit = 0;
                var BBreak = 0f;
                var BHeal = 0;
                var FBI = AData.BranchInfos[Inputs[Inputs.Count - 1]];
                var STime = 0;
                var BST = 0;
                for (int i = 0; i < Inputs.Count - 1; i++)
                {
                    var CBID = AData.BranchInfos[Inputs[i]].BID;
                    var NBID = AData.BranchInfos[Inputs[i + 1]].BID;
                    int LT = -1;
                    int LC = 0;
                    for (int j = 0; j < AData.Branchs.Length; j++)
                    {
                        var BDa = AData.Branchs[j];

                        for (int k = 0; k < BDa.BranchNums.Length; k++)
                        {
                            if (BDa.BranchNums[k] == CBID && BDa.FutureNum == NBID)
                            {
                                switch (TimeCheckDr.value)
                                {
                                    case 0:
                                        if (LT < 0 || LT > BDa.Times.x)
                                        {
                                            LT = BDa.Times.x;
                                            STime = BDa.FutureTime;
                                        }
                                        break;
                                    case 1:
                                        LT += (BDa.Times.x + BDa.Times.y) / 2;
                                        STime += BDa.FutureTime;
                                        LC++;
                                        break;
                                    case 2:
                                        if (LT < 0 || LT < BDa.Times.y)
                                        {
                                            LT = BDa.Times.y;
                                            STime = BDa.FutureTime;
                                        }
                                        break;
                                }

                                break;
                            }
                        }
                    }
                    if (TimeCheckDr.value == 1)
                    {
                        LT /= Mathf.Max(1, LC);
                        STime /= Mathf.Max(1, LC);
                    }
                    AData.DamGets(CBID, HitF, BST, out var oDam, out var oHit, out var oBreak, out var oHeal);
                    InputUIs[i].Info.text = oDam + ":" + LT + "f";
                    BTime += LT;
                    BDam += oDam;
                    BHit += oHit;
                    BBreak += oBreak;
                    BHeal += oHeal;
                    BST = STime;
                }
                var EndTimed = FBI.ChangeEndTime > 0 ? FBI.ChangeEndTime : AData.EndTime;
                AData.DamGets(FBI.BID, HitF, STime, out var oFDam, out var oFHit, out var oFBreak, out var oFHeal);
                BTime += EndTimed;
                BDam += oFDam;
                BHit += oFHit;
                BBreak += oFBreak;
                BHeal += oFHeal;
                InputUIs[Inputs.Count - 1].Info.text = oFDam + ":" + EndTimed + "f";
                OutTx.text = AData.InfoDams(BTime, BDam, BHit,BBreak,BHeal);
                for(int i = 0; i < Inputs.Count; i++)
                {
                    var BInfo = AData.BranchInfos[Inputs[i]];
                    OutTx.text += "\n[" + BInfo.Name+ "]\n" + AData.InfoGetBranchs(BInfo.BID);
                }

            }
        }
    }
    public void InputClear()
    {
        Inputs.Clear();
        UIUpdate();
    }
    public void SetRem()
    {
        SetID = -1;
        InputClear();
    }
    public void InputRem()
    {
        if(Inputs.Count>0)Inputs.RemoveAt(Inputs.Count - 1);
        UIUpdate();
    }
    void UI_Sin_Set.SetReturn.ReturnID(string Type, int ID)
    {
        switch (Type)
        {
            case "Atk":SetID = ID;break;
            case "Branch":Inputs.Add(ID); break;
        }
        UIUpdate();
    }
}
