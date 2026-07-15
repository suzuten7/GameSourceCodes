using TMPro;
using UnityEngine;
using static Manifesto;
using static PlayerValue;
public class UI_SData_IO : MonoBehaviour
{
    [SerializeField] TMP_InputField TextInportIn;
    public void Inport()
    {
        if (TextInportIn.text == "") return;
        var CutSData = "";
        try
        {
            CutSData = AesExample.DecompressFromBase64(TextInportIn.text);
        }
        catch
        {
            CutSData = TextInportIn.text;
        }
        var JsonSData = "";
        try
        {
            JsonSData = AesExample.JsonKeyCutRev(CutSData);
        }
        catch
        {
            JsonSData = TextInportIn.text;
        }
        var SDatas = JsonUtility.FromJson<Class_SDatas>(JsonSData);
        for (int i = 0; i < SDatas.PriSets.Length; i++)
        {
            SDatas.PriSets[i].Disp = AesExample.Decrypt(SDatas.PriSets[i].Disp);
            SDatas.PriSets[i].Memo = AesExample.Decrypt(SDatas.PriSets[i].Memo);
        }
        for (int i = 0; i < SDatas.Genes.Datas.Count; i++)
        {
            SDatas.Genes.Datas[i].Name = AesExample.Decrypt(SDatas.Genes.Datas[i].Name);
        }
        PSaves = SDatas.PSaves;
        PriSets = SDatas.PriSets;
        Stages = SDatas.Stages;
        Genes = SDatas.Genes;
        SceneChangePanel.SceneSet(0);
    }
    public void Export()
    {
        var SDatas = new Class_SDatas();
        SDatas.PSaves = PSaves;
        SDatas.PriSets = new Class_Save_PriSet[PriSets.Length];
        for (int i = 0; i < PriSets.Length; i++)
        {
            SDatas.PriSets[i] = new Class_Save_PriSet(PriSets[i]);
            SDatas.PriSets[i].Disp = AesExample.Encrypt(SDatas.PriSets[i].Disp);
            SDatas.PriSets[i].Memo = AesExample.Encrypt(SDatas.PriSets[i].Memo);
        }
        SDatas.Stages = Stages;
        SDatas.Genes = new Class_Save_Genes(Genes);
        for(int i = 0; i < SDatas.Genes.Datas.Count; i++)
        {
            SDatas.Genes.Datas[i].Name = AesExample.Encrypt(SDatas.Genes.Datas[i].Name);
        }

        var JsonSData = JsonUtility.ToJson(SDatas);
        var CutSData = AesExample.JsonKeyCutSet(JsonSData);
        var Base64SData = AesExample.CompressToBase64(CutSData);
        TextInportIn.text = Base64SData;
    }
    public void Copy()
    {
        GUIUtility.systemCopyBuffer = TextInportIn.text;
    }
    public void Past()
    {
        TextInportIn.text = GUIUtility.systemCopyBuffer;
    }
}
