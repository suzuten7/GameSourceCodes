using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BattleManager;
using static DataBase;
using static Manifesto;
using static PlayerValue;
using static DataColors;
public class UI_EndResult : MonoBehaviour,UI_Sin_Set.SetReturn
{
    [SerializeField] GameObject[] EndUIs;
    [SerializeField] Image BackImage;
    [SerializeField] TextMeshProUGUI WinsTx;
    [SerializeField] TextMeshProUGUI StarsTx;
    [SerializeField] TextMeshProUGUI TimeTx;
    [SerializeField] TextMeshProUGUI DeathTx;
    [SerializeField] List<UI_Sin_Set> DropGeneUIs;
    [SerializeField] TextMeshProUGUI DropGeneInfoTx;
    int CheckGeneID = -1;
    private void Start()
    {
        DropGeneInfoTx.text = "";
    }
    void Update()
    {
        for (int i = 0; i < EndUIs.Length; i++)
        {
            if (EndUIs[i].activeSelf != BTManager.End) EndUIs[i].SetActive(BTManager.End);
        }
        if (!BTManager.End) return;
        UISet_Base();
    }
    void UISet_Base()
    {
        Color BackCol;
        float Alpha = BackImage.color.a;
        var WinStr = "";
        if (BTManager.Win)
        {
            BackCol = Color.gray;
            WinStr = "勝利";
        }
        else
        {
            BackCol = Color.red;
            WinStr = "敗北";
        }
        BackCol.a = Alpha;
        if(BackImage.color != BackCol) BackImage.color = BackCol;
        if(WinsTx.text != WinStr) WinsTx.text = WinStr;

        var StarStr = "";
        StarsTx.text = "";
        int StarLim = 2 + BTManager.Dife;
        if (BTManager.Chaos) StarLim++;
        for (int i = 0; i < StarLim; i++) StarStr += i < BTManager.Star ? "★" : "☆";
        if (StarsTx.text != StarStr) StarsTx.text = StarStr;
        var TimeStr = "";
        if (BTManager.StageD != null)
        {
            if (BTManager.StageD.DifencePer <= 0)
            {
                TimeStr = (BTManager.Time / 3600).ToString("D2") + ":" + (BTManager.Time / 60 % 60).ToString("D2");
            }
            else
            {
                var DifHPPer = 0f;
                for (int i = 0; i < BTManager.BossList.Count; i++)
                {
                    if (BTManager.BossList[i] == null) continue;
                    if (BTManager.StageD.DifencePer > 0 && BTManager.BossList[i].Team == 0)
                    {
                        DifHPPer = BTManager.BossList[i].HP / Mathf.Max(1f, BTManager.BossList[i].FMHP) * 100f;
                    }
                }
                TimeStr = "耐久値" + Mathf.Max(0, DifHPPer).ToString("F1") + "%";
            }
        }
        if (TimeTx.text != TimeStr) TimeTx.text = TimeStr;
        var DeathStr = "Death:" + BTManager.DeathCount;
        if (DeathTx.text != DeathStr) DeathTx.text = DeathStr;
        for(int i = 0; i < Mathf.Max(DropGeneUIs.Count, BTManager.DropGenes.Count); i++)
        {
            if(DropGeneUIs.Count <= i) DropGeneUIs.Add(Instantiate(DropGeneUIs[0], DropGeneUIs[0].transform.parent));
            var SinUI = DropGeneUIs[i];
            if(i < BTManager.DropGenes.Count)
            {
                var DGeneD = BTManager.DropGenes[i];
                var TGeneD = DB.Genes[DGeneD.Type];
                SinUI.Returns = this;
                SinUI.ID = i;
                SinUI.BackImage.color = DCol.ColSelects(CheckGeneID == i);
                SinUI.Icon.texture = TGeneD.Images[DGeneD.Format];
                SinUI.Name.text = "「" + DGeneD.Name+ "」";
                SinUI.Name.text += "\n<size=75%>" + (Enum_GeneTypes)DGeneD.Type + ":" + (Enum_GeneFormat)DGeneD.Format + "</size>";

            }
            SinUI.gameObject.SetActive(i < BTManager.DropGenes.Count);
        }
        if (CheckGeneID >= 0) DropGeneInfoTx.text = GeneInfo(BTManager.DropGenes[CheckGeneID], false);
    }
    void UI_Sin_Set.SetReturn.ReturnID(string Type, int ID)
    {
        CheckGeneID = ID;
    }
}
