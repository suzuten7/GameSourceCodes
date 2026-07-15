using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Manifesto;
using static PlayerValue;
using static DataBase;
using static DataColors;
public class UI_CharaSets_GeneList : MonoBehaviour,UI_Sin_Set.SetReturn
{
    [SerializeField] UI_CharaSets_Base SetBase;
    [SerializeField] TextMeshProUGUI ChaosGrainTx;
    [SerializeField] TextMeshProUGUI GeneCountTx;
    [SerializeField] TMP_Dropdown TypeDr;
    [SerializeField] TMP_Dropdown FormatDr;
    [SerializeField] TMP_Dropdown MStateDr;
    [SerializeField] List<UI_Sin_Set> GeneUIs;
    [SerializeField] TextMeshProUGUI GeneInfoTx;
    [SerializeField] TMP_InputField GeneNameIn;
    [SerializeField] Toggle GeneLockTo;
    [SerializeField] TextMeshProUGUI AddCGCostTx;
    int GeneSelID = -1;

    int AddCGCost
    {
        get
        {
            if (GeneSelID < 0) return 0;
            if (Genes.Datas[GeneSelID].LV >= 10) return 0;
            return Mathf.RoundToInt(1000 * MathF.Pow(Genes.Datas[GeneSelID].LV, 2f));
        }
    }
    [SerializeField] TextMeshProUGUI ResetCGGetTx;
    int ResetCGGet
    {
        get
        {
            if (GeneSelID < 0) return 0;
            float CG = 0;
            for (int i = Genes.Datas[GeneSelID].LV; i > 1; i--)
            {
                CG += 1000 * MathF.Pow(Genes.Datas[GeneSelID].LV, 2f);
            }
            return Mathf.RoundToInt(CG / 2);
        }
    }
    [SerializeField] TextMeshProUGUI DeleteCGGetTx;

    public void UIUpdates()
    {
        ChaosGrainTx.text = "カオスグライン:" + Genes.ChaosGrain;
        GeneCountTx.text = "因子数:" + Genes.Datas.Count + "/" + GeneLimit;
        GeneCountTx.color = Genes.Datas.Count >= GeneLimit ? Color.red : Color.white;
        for (int i = 0; i < MathF.Max(GeneUIs.Count, Genes.Datas.Count); i++)
        {
            if (GeneUIs.Count <= i) GeneUIs.Add(Instantiate(GeneUIs[0], GeneUIs[0].transform.parent));
            var SinUI = GeneUIs[i];
            bool Disp = false;
            if (i < Genes.Datas.Count)
            {
                var GeneD = Genes.Datas[i];
                Disp = true;
                if (TypeDr.value > 0 && GeneD.Type != (TypeDr.value - 1)) Disp = false;
                if (FormatDr.value > 0 && GeneD.Format != (FormatDr.value - 1)) Disp = false;
                if (MStateDr.value > 0 && GeneD.Main != (MStateDr.value - 1)) Disp = false;
                if (Disp)
                {
                    SinUI.ID = i;
                    SinUI.Returns = this;
                    SinUI.Name.text = "「" + GeneD.Name + "」";
                    SinUI.Name.text += "\n<size=75%>" + (Enum_GeneTypes)GeneD.Type + ":" + (Enum_GeneFormat)GeneD.Format;
                    SinUI.Info.text = GeneD.Lock ? "★" : "";
                    SinUI.Icon.texture = DB.Genes[GeneD.Type].Images[GeneD.Format];
                    SinUI.BackImage.color = DCol.ColSelects(i == GeneSelID);
                }
            }
            SinUI.gameObject.SetActive(Disp);
        }
        if (GeneSelID >= 0)
        {
            GeneNameIn.text = Genes.Datas[GeneSelID].Name;
            GeneLockTo.isOn = Genes.Datas[GeneSelID].Lock;
            GeneInfoTx.text = GeneInfo(Genes.Datas[GeneSelID],false);
        }
        else
        {
            GeneNameIn.text = "";
            GeneLockTo.isOn = false;
            GeneInfoTx.text = "";
        }
        AddCGCostTx.text = "-" + AddCGCost;
        ResetCGGetTx.text = "+" + ResetCGGet;
        DeleteCGGetTx.text = "+" + (ResetCGGet + 1000);
    }
    public void GeneAction(int ID)
    {
        int GeneID = GeneSelID;
        if (GeneID < 0) return;
        var GeneD = Genes.Datas[GeneID];
        switch (ID)
        {
            case 0:
                GeneD.Name = GeneNameIn.text;
                break;
            case 1:
                if (GeneD.LV < 10)
                {
                    if (Genes.ChaosGrain < AddCGCost) return;
                    Genes.ChaosGrain -= AddCGCost;
                    GeneLVAdd(GeneID);
                }
                break;
            case 2:
                if (!GeneD.Lock && GeneD.LV > 1)
                {
                    Genes.ChaosGrain += ResetCGGet;
                    GeneReset(GeneID);
                }
                break;
            case 3:
                Genes.Datas[GeneID].Lock = GeneLockTo.isOn;
                break;
            case 4:
                if (!GeneD.Lock)
                {
                    Genes.ChaosGrain += ResetCGGet + 1000;
                    GeneSelID = Mathf.Min(GeneSelID, Genes.Datas.Count - 2);
                    GeneDelete(GeneID);
                }
                break;
        }
        SetBase.UIUpdate();
    }
    void UI_Sin_Set.SetReturn.ReturnID(string Type, int ID)
    {
        GeneSelID = ID;
        UIUpdates();
    }
    void Start()
    {
        TypeDr.options.Clear();
        TypeDr.options.Add(new TMP_Dropdown.OptionData { text = "タイプ" });
        var TypeKeys = Enum.GetValues(typeof(Enum_GeneTypes));
        for (int i = 0; i < TypeKeys.Length; i++)
        {
            if ((int)TypeKeys.GetValue(i) == (int)Enum_GeneTypes.終) continue;
            TypeDr.options.Add(new TMP_Dropdown.OptionData { text = TypeKeys.GetValue(i).ToString() });
        }
        FormatDr.options.Clear();
        FormatDr.options.Add(new TMP_Dropdown.OptionData { text = "形状" });
        var FormatKeys = Enum.GetValues(typeof(Enum_GeneFormat));
        for (int i = 0; i < FormatKeys.Length; i++)
        {
            if ((int)FormatKeys.GetValue(i) == (int)Enum_GeneFormat.終) continue;
            FormatDr.options.Add(new TMP_Dropdown.OptionData { text = FormatKeys.GetValue(i).ToString() });
        }
        MStateDr.options.Clear();
        MStateDr.options.Add(new TMP_Dropdown.OptionData { text = "メイン\nステータス" });
        var MStaKeys = Enum.GetValues(typeof(Enum_GeneOptions));
        for (int i = 0; i < MStaKeys.Length; i++)
        {
            if ((int)MStaKeys.GetValue(i) == (int)Enum_GeneOptions.終) continue;
            MStateDr.options.Add(new TMP_Dropdown.OptionData { text = MStaKeys.GetValue(i).ToString() });
        }
    }
}
