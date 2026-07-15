using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static DataBase;
using static GlovalValues;

public class Title_SkillSets : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI RoleTx;
    [SerializeField] TextMeshProUGUI SkillCostTx;
    [SerializeField] List<Title_Sin_PassiveSkill> PassUIs;
    [SerializeField] bool StartRoalSet;

    int CostMaxs = 0;
    int UseCost = 0;
    Data_Player_Passive[] Passives = new Data_Player_Passive[0];
    Dictionary<int, int> PassSetL = new Dictionary<int, int>();
    private void Start()
    {
        if (StartRoalSet) RoalSet(0);
    }
    public void RoalSet(int Type)
    {
        switch (Type)
        {
            default:
                RoleTx.text = "<color=#FFFF00>観戦</color>";
                CostMaxs = 0;
                Passives = new Data_Player_Passive[0];
                PassSetL = new Dictionary<int, int>();
                break;
            case 0:
                RoleTx.text = "逃走者\n(プランクトン)";
                CostMaxs = 6;
                Passives = DB.Fugitive_Pass;
                PassSetL = Pass_Fugi;
                break;
            case 1:
                RoleTx.text = "鬼操縦者\n(クジラ)";
                CostMaxs = 20;
                Passives = DB.Oni_Pilot_Pass;
                PassSetL = Pass_OniP;
                break;
            case 2:
                RoleTx.text = "鬼監視者\n(コバンザメ?)";
                CostMaxs = 20;
                Passives = DB.Oni_Visit_Pass;
                PassSetL = Pass_OniV;
                break;

            case -1:
                RoleTx.text =  "ロール設定待ち";
                break;
            case -2:
                RoleTx.text = "";
                break;
        }
    }

    public void LateUpdate()
    {
        UseCost = 0;
        for (int i = 0; i < Mathf.Max(Passives.Length, PassUIs.Count); i++)
        {
            if (i < Passives.Length)
            {
                var PassUIBase = PassUIs[0];
                if (i >= PassUIs.Count) PassUIs.Add(Instantiate(PassUIBase, PassUIBase.transform.parent));
                var SinUIs = PassUIs[i];
                if (Passives[i].Debug)
                {
                    bool Debugs = false;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                    Debugs = true;
#endif
                    if (!Debugs)
                    {
                        SinUIs.gameObject.SetActive(false);
                        continue;
                    }
                }
                SinUIs.gameObject.SetActive(true);
                int SType = (int)Passives[i].SkillColor;
                if (SType != 6)
                {
                    var SColor = DB.SkillTypeColors[SType];
                    SinUIs.BackIm.color = SColor;
                    SinUIs.NameBackIm.color = SColor * 0.7f;
                }
                else
                {
                    var SColor = SinUIs.BackIm.color;
                    Color GColor;
                    Color.RGBToHSV(SColor, out GColor.r, out GColor.g, out GColor.b);
                    SColor = Color.HSVToRGB(Mathf.Repeat(GColor.r + 0.01f, 1f), 1, 1);
                    SinUIs.BackIm.color = SColor;
                    SinUIs.NameBackIm.color = SColor * 0.7f;
                }
                SinUIs.SkillID = i;
                SinUIs.SkillNatx.text = Passives[i].Names;
                SinUIs.SkillInfotx.text = Passives[i].Info;

                int LVCost = 0;
                for (int j = 0; j < Mathf.Max(Passives[i].PassLVs.Length, SinUIs.PassLVUIs.Count); j++)
                {
                    if (j < Passives[i].PassLVs.Length)
                    {
                        if (SinUIs.PassLVUIs.Count <= j)
                        {
                            var CPSinUI = Instantiate(SinUIs.PassLVUIs[0], SinUIs.PassLVUIs[0].transform.parent);
                            SinUIs.PassLVUIs.Add(CPSinUI);
                        }
                        var PSinUI = SinUIs.PassLVUIs[j];
                        var PassLVD = Passives[i].PassLVs[j];
                        PSinUI.gameObject.SetActive(true);
                        PSinUI.LV = j;
                        LVCost += PassLVD.Cost;
                        PSinUI.CostTx.text = LVCost.ToString();
                        if (j > 0) SinUIs.SkillInfotx.text += "/";
                        if (PassSetL.ContainsKey(i))
                        {
                            if (PassSetL[i] <= j)
                            {
                                PSinUI.BackImage.color = new Color(1, 0.7f, 0.3f);
                                SinUIs.SkillInfotx.text += "<color=#888888>";
                            }
                            else
                            {
                                PSinUI.BackImage.color = new Color(0, 1f, 0.3f);
                                UseCost += PassLVD.Cost;
                                if (PassSetL[i] - 1 == j) SinUIs.SkillInfotx.text += "<color=#FFFF00>";
                                else SinUIs.SkillInfotx.text += "<color=#FFFFFF>";
                            }
                        }
                        else
                        {
                            PSinUI.BackImage.color = new Color(1, 0.7f, 0.3f);
                            SinUIs.SkillInfotx.text += "<color=#888888>";
                        }
                        SinUIs.SkillInfotx.text += PassLVD.LVInfo;
                        SinUIs.SkillInfotx.text += "</color>";
                    }
                    else
                    {
                        SinUIs.PassLVUIs[j].gameObject.SetActive(false);
                    }
                }
                SinUIs.SkillInfotx.text += Passives[i].EndInfo;
            }
            else
            {
                PassUIs[i].gameObject.SetActive(false);
            }
        }
        SkillCostTx.text = "スキルコスト" + UseCost + "/" + CostMaxs;
    }


    public void PassReset()
    {
        PassSetL.Clear();
        SkillSave();
    }
    public void PassRandom()
    {
        PassSetL.Clear();
        UseCost = 0;
        for (int i = 0; i < 100; i++)
        {
            if (UseCost >= CostMaxs) break;
            var SPassID = Random.Range(0, Passives.Length);
            var SPass = Passives[SPassID];
            if (!PassSetL.ContainsKey(SPassID)) PassSetL.Add(SPassID, 0);
            if (PassSetL[SPassID] < SPass.PassLVs.Length && !SPass.NRand)
            {
                int Cost = SPass.PassLVs[PassSetL[SPassID]].Cost;
                if (Cost <= (CostMaxs - UseCost))
                {
                    PassSetL[SPassID]++;
                    UseCost += Cost;
                }
            }
        }
        SkillSave();
    }

    public void Bu_PassSetCh(int ID, int LV)
    {

        if (!PassSetL.ContainsKey(ID)) PassSetL.Add(ID, 0);
        if (PassSetL[ID] == LV + 1)
        {
            PassSetL[ID]--;
        }
        else if (PassSetL[ID] <= LV)
        {
            int Cost = 0;
            for (int i = PassSetL[ID]; i <= LV; i++) Cost += Passives[ID].PassLVs[i].Cost;
            Debug.Log(Cost + ":" + (CostMaxs - UseCost));
            if (Cost <= (CostMaxs - UseCost)) PassSetL[ID] = LV + 1;
        }
        else
        {
            PassSetL[ID] = LV + 1;
        }
        SkillSave();
    }

    void SkillSave()
    {
        for (int i = 0; i < DB.Fugitive_Pass.Length; i++)
        {
            PlayerPrefs.SetInt("FugiPassLVs" + i, Pass_Fugi.TryGetValue(i, out var PassLV) ? PassLV : 0);
        }
        for (int i = 0; i < DB.Oni_Pilot_Pass.Length; i++)
        {
            PlayerPrefs.SetInt("OniPilotPassLVs" + i, Pass_OniP.TryGetValue(i, out var PassLV) ? PassLV : 0);
        }
        for (int i = 0; i < DB.Oni_Visit_Pass.Length; i++)
        {
            PlayerPrefs.SetInt("OniVisitPassLVs" + i, Pass_OniV.TryGetValue(i, out var PassLV) ? PassLV : 0);
        }

    }
    static public void SkillLoad()
    {
        Pass_Fugi.Clear();
        for (int i = 0; i < DB.Fugitive_Pass.Length; i++)
        {
            Pass_Fugi.Add(i, PlayerPrefs.GetInt("FugiPassLVs" + i, 0));
        }
        Pass_OniP.Clear();
        for (int i = 0; i < DB.Oni_Pilot_Pass.Length; i++)
        {
            Pass_OniP.Add(i, PlayerPrefs.GetInt("OniPilotPassLVs" + i, 0));
        }
        Pass_OniV.Clear();
        for (int i = 0; i < DB.Oni_Visit_Pass.Length; i++)
        {
            Pass_OniV.Add(i, PlayerPrefs.GetInt("OniVisitPassLVs" + i, 0));
        }
    }
}
