using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DataBase;
using static PlayerValue;
using static Manifesto;
using static Statics;
using static BattleManager;
using static DataColors;
using UnityEngine.SceneManagement;
using System.Linq;

public class UI_Player : UI_State
{
    [SerializeField] Player_Cont PCont;
    [SerializeField] Player_Target PTarget;
    [SerializeField] Camera PCam;

    [SerializeField] Image DeathPanel;
    [SerializeField] float DeathAlphaSpeed;
    [SerializeField] float DeathAlphaMax;

    [SerializeField] Image MPBar;
    [SerializeField] Image MPFill;

    [SerializeField] Image AtkTimeBar;
    [SerializeField] TextMeshProUGUI AtkNameTx;
    [SerializeField] TextMeshProUGUI AtkBranchTx;
    [SerializeField] RectTransform BranchTrans;
    [SerializeField] List<Slider> BranchSliders;

    [SerializeField] Image TargetImage;

    [SerializeField] TextMeshProUGUI AtkFBTx;
    [SerializeField] Image SPBImage;
    [SerializeField] UI_Sin_Atk[] AtkUIs;
    [SerializeField] GameObject OtherPlayerView;
    [SerializeField] List<UI_Sin_BossUI> OtherPlayerUIs;
    [SerializeField] List<UI_Sin_BossUI> BossUIs;
    [SerializeField] TextMeshProUGUI StateInfoTx;
    [SerializeField] BinaryUIAnimationo binaryUIAnimationo;
    [SerializeField] GameObject DebugUI;

    float DeathAlpha = 0;
    private void Start()
    {
        var DeathColor = DeathPanel.color;
        DeathColor.a = 0;
        DeathPanel.color = DeathColor;
    }
    override protected void LateUpdate()
    {
        BaseSet(200);

        var DeathColor = DeathPanel.color;
        if (Sta.HP <= 0) DeathAlpha += 0.01f * DeathAlphaSpeed;
        else DeathAlpha -= 0.05f * DeathAlphaSpeed;
        DeathAlpha = Mathf.Clamp(DeathAlpha, 0f, DeathAlphaMax);
        DeathColor.a = Float_Cuts(DeathAlpha, 30);
        if (DeathPanel.color != DeathColor) DeathPanel.color = DeathColor;

        var MPPer = Float_Cuts(Mathf.Abs(Sta.MP) / Mathf.Max(1, Sta.FMMP),30);
        if (MPBar.fillAmount != MPPer) MPBar.fillAmount = MPPer;
        var MPCol = DCol.MP_Base;
        if (Sta.LowMP) MPCol = Sta.MP >= 0 ? DCol.MP_Low : DCol.MP_Neg;
        if(MPFill.color!=MPCol)MPFill.color = MPCol;

        var TargetColor = PTarget.TargetOff ? Color.white : new Color(1, 0.5f, 0);
        TargetColor.a = TargetImage.color.a;
        if(TargetImage.color != TargetColor) TargetImage.color = TargetColor;

        if (Sta.AtkD != null)
        {
            var AtkD = Sta.AtkD;


            var BrachTx = "";
            var EndTime = AtkD.EndTime;
            var NoEnd = false;
            if (AtkD.BranchInfos.Count > 0)
            {
                var BranchGet = AtkD.BranchInfos.Find(x => x.BID == Sta.AtkBranch);
                if (BranchGet != null)
                {
                    BrachTx = BranchGet.Name;
                    if(BranchGet.ChangeEndTime>0) EndTime = BranchGet.ChangeEndTime;
                    NoEnd = BranchGet.NoEnd;
                }
            }
            var AtkTimePer = Float_Cuts(Sta.AtkTime / Mathf.Max(1f, EndTime), 120);
            if (AtkTimeBar.fillAmount != AtkTimePer) AtkTimeBar.fillAmount = AtkTimePer;
            if (AtkNameTx.text != AtkD.Name) AtkNameTx.text = AtkD.Name;
            if (AtkBranchTx.text != BrachTx) AtkBranchTx.text = BrachTx;
            var BranchIndexs = new List<int>();
            if(AtkD.Branchs!=null)
                for(int i = 0; i < AtkD.Branchs.Length; i++)
                {
                    var BranchD = AtkD.Branchs[i];
                    for(int j = 0; j < BranchD.BranchNums.Length; j++)
                    {
                        if(BranchD.BranchNums[j] < 0 || Sta.AtkBranch == BranchD.BranchNums[j])
                        {
                            BranchIndexs.Add(i);
                            break;
                        }
                    }
                }
            var ListSTimeBase = new Dictionary<int, int>();
            for (int i = 0; i < Mathf.Max(BranchIndexs.Count, BranchSliders.Count); i++)
            {
                if (BranchSliders.Count <= i) BranchSliders.Add(Instantiate(BranchSliders[0], BranchSliders[0].transform.parent));

                var SinUI = BranchSliders[i];
                if (i < BranchIndexs.Count)
                {
                    var BranchInfo = AtkD.Branchs[BranchIndexs[i]];
                    if (BranchInfo.HideUI)
                    {
                        SinUI.gameObject.SetActive(false);
                        continue;
                    }
                    ListSTimeBase.Add(i,BranchInfo.Times.x);
                    var SliderVal = Float_Cuts((BranchInfo.Times.x + BranchInfo.Times.y) / 2f / Mathf.Max(1f, EndTime),120);
                    if(SinUI.value != SliderVal) SinUI.value = SliderVal;
                    var TRectSize = SinUI.targetGraphic.rectTransform.sizeDelta;
                    TRectSize.x = Float_Cuts(BranchTrans.sizeDelta.x * ((BranchInfo.Times.y - BranchInfo.Times.x + 1f) / Mathf.Max(1f, EndTime)),120);
                    if(SinUI.targetGraphic.rectTransform.sizeDelta != TRectSize) SinUI.targetGraphic.rectTransform.sizeDelta = TRectSize;
                    Color SliderCol = Color.black;
                    if (BranchInfo.BranchColor.a > 0) SliderCol = BranchInfo.BranchColor;
                    else if (BranchInfo.Ifs.Length > 0)
                    {
                        SliderCol = Color.green;
                        for (int j = 0; j < BranchInfo.Ifs.Length; j++)
                        {
                            if (BranchInfo.Ifs[j] == Enum_AtkIf.攻撃単入力 || BranchInfo.Ifs[j] == Enum_AtkIf.攻撃入力離)
                            {
                                SliderCol = Color.yellow;
                                break;
                            }

                            if (BranchInfo.Ifs[j] == Enum_AtkIf.攻撃長入力)
                            {
                                SliderCol = Color.magenta;
                                break;
                            }
                            if (BranchInfo.Ifs[j] == Enum_AtkIf.空中)
                            {
                                SliderCol = Color.cyan;
                                break;
                            }
                            if (BranchInfo.Ifs[j] == Enum_AtkIf.攻撃未入力 || BranchInfo.Ifs[j] == Enum_AtkIf.攻撃未長入力)
                            {
                                SliderCol = Color.white;
                                break;
                            }
                            if (BranchInfo.Ifs[j] == Enum_AtkIf.MP無し)
                            {
                                SliderCol = new Color(1f,0.8f,0.8f);
                                break;
                            }
                        }

                    }

                    SliderCol.a = SinUI.targetGraphic.color.a;
                    if(SinUI.targetGraphic.color != SliderCol) SinUI.targetGraphic.color = SliderCol;


                }
                if(SinUI.gameObject.activeSelf != i < BranchIndexs.Count) SinUI.gameObject.SetActive(i < BranchIndexs.Count);
            }
            var ListSTimeSorts =  ListSTimeBase.OrderBy(x => x.Value).ToDictionary(x=> x.Key, x => x.Value);
            var STimeKeys = ListSTimeSorts.Keys.ToArray();
            for (int i = 0; i < STimeKeys.Length; i++)
            {
                BranchSliders[STimeKeys[i]].transform.SetAsLastSibling();
            }

        }
        else
        {
            if(AtkTimeBar.fillAmount != 0) AtkTimeBar.fillAmount = 0;
            if (AtkNameTx.text != "") AtkNameTx.text = "";
            if (AtkBranchTx.text != "") AtkBranchTx.text = "";
            for(int i = 0; i < BranchSliders.Count; i++)
            {
                if(BranchSliders[i].gameObject.activeSelf != false) BranchSliders[i].gameObject.SetActive(false);
            }
        }
        var FBStr = !Sta.PLValues.Backs ? "表" : "裏";
        if (AtkFBTx.text != FBStr) AtkFBTx.text = FBStr;
        SPBImage.fillAmount = Float_Cuts(Sta.SPB / Mathf.Max(1f, DB.E_Atks[Sta.PVal_AtkGet(!Sta.PLValues.Backs).E_AtkID].SPUse), 30);
        binaryUIAnimationo.UpdateAnimation(Sta.PLValues.Backs);
        for (int i = 0; i < AtkUIs.Length; i++)
        {
            Data_Atk AtkD = null;
            bool Input = false;
            int Slot = i;

            var Atks = Sta.PVal_AtkGet(Sta.PLValues.Backs);
            switch (i)
            {
                case 0:
                    AtkD = DB.N_Atks[Atks.N_AtkID];
                    Input = PCont.NAtk_Stay;
                    break;
                case 1:
                    AtkD = DB.S_Atks[Atks.S1_AtkID];
                    Input = PCont.S1Atk_Stay;
                    break;
                case 2:
                    AtkD = DB.S_Atks[Atks.S2_AtkID];
                    Input = PCont.S2Atk_Stay;
                    break;
                case 3:
                    AtkD = DB.E_Atks[Atks.E_AtkID];
                    Input = PCont.EAtk_Stay;
                    Slot = 10;
                    break;
            }
            foreach (var animation in AtkUIs[i].imageAnimation)
            {
                // 入力があった場合にアニメーションをPressedに変更
                animation.ChangeStatu(Input ? UISystem_Gabu.AnimatorStatu.Pressed : UISystem_Gabu.AnimatorStatu.Normal);
            }
            Sta.AtkCTs.TryGetValue(Slot, out var AtkCTs);

            for (int j = 0; j < AtkUIs[i].ChengedImages.Length; j++)
            {
                if (AtkUIs[i].Name[j] != null && AtkUIs[i].Name[j].text != AtkD.Name)
                {
                    AtkUIs[i].Name[j].text = AtkD.Name;
                }
                if (AtkUIs[i].Icon[j] != null && AtkUIs[i].Icon[j].texture != AtkD.Icon)
                {
                    AtkUIs[i].Icon[j].texture = AtkD.Icon;
                }
                if (AtkUIs[i].CTImage[j] != null)
                {
                    if (AtkCTs != null)
                    {
                        var CTPer = Float_Cuts((float)AtkCTs.CT / Mathf.Max(1, AtkCTs.CTMax), 30);
                        if(AtkUIs[i].CTImage[j].fillAmount != CTPer) AtkUIs[i].CTImage[j].fillAmount = CTPer;
                    }
                    else if(AtkUIs[i].CTImage[j].fillAmount != 0) AtkUIs[i].CTImage[j].fillAmount = 0;
                }
                if (AtkUIs[i].ChargeImage[j] != null)
                {
                    if (AtkD.SPUse > 0)
                    {
                        var ChargePer = Float_Cuts((float)Sta.SP / AtkD.SPUse, 30);
                        if(AtkUIs[i].ChargeImage[j].fillAmount != ChargePer) AtkUIs[i].ChargeImage[j].fillAmount = ChargePer;
                    }
                    else if(AtkUIs[i].ChargeImage[j].fillAmount != 0)AtkUIs[i].ChargeImage[j].fillAmount = 0;
                }
                var FBIf = (j == 0 ? true : false) == !Sta.PLValues.Backs;
                if(AtkUIs[i].ChengedImages[j].activeSelf != FBIf) AtkUIs[i].ChengedImages[j].SetActive(FBIf);
            }
            var FullIf = AtkD.SPUse > 0 && Sta.SP >= AtkD.SPUse;
            if(AtkUIs[i].FullImage.activeSelf != FullIf) AtkUIs[i].FullImage.SetActive(FullIf);
        }
        bool OPlayers = false;
        for(int i = 0; i < Mathf.Max(BTManager.PlayerList.Count,OtherPlayerUIs.Count); i++)
        {
            if (OtherPlayerUIs.Count <= i) OtherPlayerUIs.Add(Instantiate(OtherPlayerUIs[0], OtherPlayerUIs[0].transform.parent));
            var SinUI = OtherPlayerUIs[i];
            if (i < BTManager.PlayerList.Count)
            {
                var PSta = BTManager.PlayerList[i];
                if (PSta != null && PSta != Sta)
                {
                    SinUI.Sta = PSta;
                    SinUI.BaseSet(100);
                    SinUI.DisSet(Sta.PosGet(), PCam);
                    if(SinUI.Icon.texture != DB.Charas[PSta.PLCharaSet.CharaID].Icon)SinUI.Icon.texture = DB.Charas[PSta.PLCharaSet.CharaID].Icon;
                    if (SinUI.gameObject.activeSelf != true) SinUI.gameObject.SetActive(true);
                    OPlayers = true;
                }
                else if(SinUI.gameObject.activeSelf != false) SinUI.gameObject.SetActive(false);
            }
            else if (SinUI.gameObject.activeSelf != false) SinUI.gameObject.SetActive(false);
        }
        if (OtherPlayerView.activeSelf != OPlayers) OtherPlayerView.SetActive(OPlayers);
        for (int i = 0; i < Mathf.Max(BossUIs.Count, BTManager.BossList.Count); i++)
        {
            bool NDisp = false;
            if (i < BTManager.BossList.Count)
            {
                if (BossUIs.Count <= i) BossUIs.Add(Instantiate(BossUIs[0], BossUIs[0].transform.parent));
                var BSta = BTManager.BossList[i];
                var BUI = BossUIs[i];
                if (BSta == null)
                {
                    NDisp = true;
                }
                else
                {
                    BUI.Sta = BSta;
                    BUI.BaseSet(400);
                    BUI.DisSet(Sta.PosGet(), PCam);
                }
            }
            var BossDispIf = i < BTManager.BossList.Count && !NDisp;
            if (BossUIs[i].gameObject.activeSelf != BossDispIf) BossUIs[i].gameObject.SetActive(BossDispIf);
        }
        var StateStr = "MHP:" + Sta.FMHP;
        StateStr += "\nMMP:" + Sta.FMMP;
        StateStr += "\nAtk:" + Sta.FAtk;
        StateStr += "\nDef:" + Sta.FDef;
        if (StateInfoTx.text != StateStr) StateInfoTx.text = StateStr;
        var DebugScene = SceneManager.GetActiveScene().buildIndex == 1;
        if (DebugUI.activeSelf != DebugScene) DebugUI.SetActive(DebugScene);
    }
}
