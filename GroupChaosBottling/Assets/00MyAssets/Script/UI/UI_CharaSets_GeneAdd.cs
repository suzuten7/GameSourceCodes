using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Manifesto;
using static DataBase;
using static PlayerValue;

public class UI_CharaSets_GeneAdd : MonoBehaviour
{
    [SerializeField] UI_CharaSets_Base SetBase;
    [SerializeField] TextMeshProUGUI ChaosGrainTx;
    [SerializeField] TextMeshProUGUI GeneCountTx;
    [SerializeField] TMP_Dropdown[] Drs;
    [SerializeField] TextMeshProUGUI[] CostMults;
    [SerializeField] TextMeshProUGUI TMultsTx;
    [SerializeField] TextMeshProUGUI CostTx;
    [SerializeField] TMP_InputField NameIns;
    [SerializeField] GameObject CreateViewUI;
    [SerializeField] TextMeshProUGUI CreateViewTx;
    [SerializeField] RawImage CreateViewIcon;

    int AddCost = 2000;
    private void Start()
    {
        CreateViewUI.SetActive(false);
    }
    public void UIUpdates()
    {
        var Mult = 1f;
        for (int i = 0; i < 6; i++)
        {
            Drs[i].options.Clear();
            Drs[i].options.Add(new TMP_Dropdown.OptionData { text = "ランダム" });
            var Str = "×1";
            switch (i)
            {
                case 0:
                    var TypeKeys = Enum.GetValues(typeof(Enum_GeneTypes));
                    for (int j = 0; j < TypeKeys.Length - 1; j++)
                    {
                        Drs[i].options.Add(new TMP_Dropdown.OptionData { text = TypeKeys.GetValue(j).ToString() });
                    }
                    if (Drs[i].value != 0)
                    {
                        Str = "×3";
                        Mult *= 3;
                    }
                    break;
                case 1:
                    var FormatKeys = Enum.GetValues(typeof(Enum_GeneFormat));
                    for (int j = 0; j < FormatKeys.Length - 1; j++)
                    {
                        Drs[i].options.Add(new TMP_Dropdown.OptionData { text = FormatKeys.GetValue(j).ToString() });
                    }
                    if (Drs[i].value != 0)
                    {
                        Str = "×4";
                        Mult *= 4;
                    }

                    break;
                default:
                    var OptionKeys = Enum.GetValues(typeof(Enum_GeneOptions));
                    for (int j = 0; j < OptionKeys.Length - 1; j++)
                    {
                        Drs[i].options.Add(new TMP_Dropdown.OptionData { text = OptionKeys.GetValue(j).ToString() });
                    }
                    if (Drs[i].value != 0)
                    {
                        var Mlt = DB.GeneDropTables[Drs[i].value - 1].Mult;
                        if (i == 2) Mlt *= 3;
                        Str = "×" + Mlt;
                        Mult *= Mlt;

                    }
                    break;
            }
            CostMults[i].text = Str;
        }
        ChaosGrainTx.text = "カオスグライン:" + Genes.ChaosGrain;
        GeneCountTx.text = "因子数:" + Genes.Datas.Count + "/" + GeneLimit;
        GeneCountTx.color = Genes.Datas.Count >= GeneLimit ? Color.red : Color.white;
        TMultsTx.text = "×" + Mult;
        AddCost = Mathf.RoundToInt(2000 * Mult);
        CostTx.text = "-" + AddCost;
    }
    public void Adds()
    {
        UIUpdates();
        if (Genes.ChaosGrain < AddCost) return;
        if (Genes.Datas.Count >= GeneLimit) return;
        Genes.ChaosGrain -= AddCost;
        var CreGeneD = GeneAdds(NameIns.text,Drs[0].value - 1, Drs[1].value - 1, Drs[2].value - 1, Drs[3].value - 1, Drs[4].value - 1, Drs[5].value - 1);
        Genes.Datas.Add(CreGeneD);
        SetBase.UIUpdate();
        CreateViewTx.text = GeneInfo(CreGeneD, false);
        CreateViewIcon.texture = DB.Genes[CreGeneD.Type].Images[CreGeneD.Format];
        CreateUIOC(true);
    }
    public void CreateUIOC(bool Open)
    {
        CreateViewUI.SetActive(Open);
    }
}
