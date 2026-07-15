using System.IO;
using UnityEngine;
using static Manifesto;
using static DataBase;


public class PlayerValue
{
    static string Paths { get { return Application.persistentDataPath+"/"; } }
    static public int StageID = 0;
    static public int DifeMode = 1;
    static public bool ChaosSet = false;
    static public Class_Save_PSaves PSaves;
    static public Class_Save_PriSet[] PriSets = new Class_Save_PriSet[10];
    static public Class_Save_PriSet PriSetGet => PriSets[PSaves.PriSetID];
    static public Class_Save_Stages Stages;
    static public Class_Save_Genes Genes;
    static public Class_Save_GeneSet GeneGet => Genes.Sets[PSaves.PriSetID];
    static public int GeneLimit = 200;
    static public void Save()
    {
        var PSaves_Json = JsonUtility.ToJson(PSaves);
        SaveFile("PSaves.data",PSaves_Json);
        var DebugStr = "SaveJson\n(PSaves)\n" + PSaves_Json;
        for (int i = 0; i < PriSets.Length; i++)
        {
            var PriSet_Json = JsonUtility.ToJson(PriSets[i]);
            SaveFile("Priset_"+(i+1)+".data", PriSet_Json);
            DebugStr += "\n(PriSet_" + (i + 1) + ")\n" + PriSet_Json;
        }
        var Stages_Json = JsonUtility.ToJson(Stages);
        SaveFile("Stages.data", Stages_Json);
        DebugStr += "StagesJson\n(Stages)\n" + Stages_Json;

        var Genes_Json = JsonUtility.ToJson(Genes);
        SaveFile("Genes.data", Genes_Json);
        DebugStr += "GenesJson\n(Genes)\n" + Genes_Json;

        Debug.Log(DebugStr);
    }
    static public void Load()
    {

        PSaves = new Class_Save_PSaves();
        var PSaves_Json = LoadFile("PSaves.data");
        if (PSaves_Json != "")
        {
            var PSaves_C = JsonUtility.FromJson<Class_Save_PSaves>(PSaves_Json);
            PSaves = PSaves_C;
        }
        for (int i = 0; i < PriSets.Length; i++)
        {
            PriSets[i] = new Class_Save_PriSet();
            var PriSet_Json = LoadFile("Priset_" + (i + 1) + ".data");
            if (PriSet_Json != "")
            {
                try
                {
                    PriSets[i] = JsonUtility.FromJson<Class_Save_PriSet>(PriSet_Json);
                }
                catch
                {
                    Debug.Log("セット" + (i+1) + "ロードエラー");
                }

            }
        }

        Stages = new Class_Save_Stages();
        var Stages_Json = LoadFile("Stages.data");
        if (Stages_Json != "")
        {
            var Stages_C = JsonUtility.FromJson<Class_Save_Stages>(Stages_Json);
            Stages = Stages_C;
        }
        for (int i = Stages.SoloStars.Count - 1; i < DB.Stages.Count; i++)
        {
            Stages.SoloStars.Add(0);
        }
        for (int i = Stages.MultStars.Count - 1; i < DB.Stages.Count; i++)
        {
            Stages.MultStars.Add(0);
        }

        Genes = new Class_Save_Genes();
        var Genes_Json = LoadFile("Genes.data");
        if (Genes_Json != "")
        {
            var Genes_C = JsonUtility.FromJson<Class_Save_Genes>(Genes_Json);
            Genes = Genes_C;
        }

    }

    static void SaveFile(string FileName,string Str)
    {
        if (Application.isEditor) FileName = "Edit_" + FileName;
        var Writer = new StreamWriter(Paths + FileName);
        Writer.Write(Str);
        Writer.Flush();
        Writer.Close();
    }
    static string LoadFile(string FileName)
    {
        try
        {
            if (Application.isEditor) FileName = "Edit_" + FileName;
            var reader = new StreamReader(Paths + FileName);
            string Str = reader.ReadToEnd();
            reader.Close();
            return Str;
        }
        catch
        {
            return "";
        }
    }



    #region 因子
    static public Class_Save_GeneData GeneAdds(string S_Name, int S_Type,int S_Format = -1, int S_Main = -1, int S_Sub1 = -1, int S_Sub2 = -1, int S_Sub3 = -1)
    {
        var GeneD = new Class_Save_GeneData();
        if (S_Name != "") GeneD.Name = S_Name;
        if (S_Type < 0) GeneD.Type = Random.Range(0, DB.Genes.Length);
        else GeneD.Type = S_Type;
        if (S_Format < 0) GeneD.Format = Random.Range(0, (int)Enum_GeneFormat.終);
        else GeneD.Format = S_Format;
        if (S_Main < 0) GeneD.Main = GeneOpTablesGet();
        else GeneD.Main = S_Main;
        GeneDOP(GeneD,S_Sub1,S_Sub2,S_Sub3);

        return GeneD;
    }
    static public void GeneLVAdd(int GeneIndex)
    {
        var GeneD = Genes.Datas[GeneIndex];
        GeneD.LV++;
        switch (Random.Range(0, 3))
        {
            case 0:
                GeneD.Add1 += GeneAddRand();
                break;
            case 1:
                GeneD.Add2 += GeneAddRand();
                break;
            case 2:
                GeneD.Add3 += GeneAddRand();
                break;
        }
    }
    static public void GeneReset(int GeneIndex)
    {
        var GeneD = Genes.Datas[GeneIndex];
        GeneD.LV = 1;
        GeneD.Add1 = 0;
        GeneD.Add1 = GeneAddRand();
        GeneD.Add2 = 0;
        GeneD.Add2 = GeneAddRand();
        GeneD.Add3 = 0;
        GeneD.Add3 = GeneAddRand();
    }
    static public void GeneDelete(int GeneIndex)
    {
        Genes.Datas.RemoveAt(GeneIndex);
        for(int i = 0; i < Genes.Sets.Count; i++)
        {
            var GSet = Genes.Sets[i];
            if (GSet.G1_ID == GeneIndex) GSet.G1_ID = -1;
            else if(GSet.G1_ID > GeneIndex) GSet.G1_ID--;

            if (GSet.G2_ID == GeneIndex) GSet.G2_ID = -1;
            else if (GSet.G2_ID > GeneIndex) GSet.G2_ID--;

            if (GSet.G3_ID == GeneIndex) GSet.G3_ID = -1;
            else if (GSet.G3_ID > GeneIndex) GSet.G3_ID--;

            if (GSet.G4_ID == GeneIndex) GSet.G4_ID = -1;
            else if (GSet.G4_ID > GeneIndex) GSet.G4_ID--;

            if (GSet.G5_ID == GeneIndex) GSet.G5_ID = -1;
            else if (GSet.G5_ID > GeneIndex) GSet.G5_ID--;
        }
    }
    static void GeneDOP(Class_Save_GeneData GeneD, int S_Sub1, int S_Sub2, int S_Sub3)
    {
        int OpCount = Random.Range(0, 4);
        if (S_Sub1 >= 0) GeneD.Sub1 = S_Sub1;
        else if (OpCount >= 1) GeneD.Sub1 = GeneOpTablesGet();

        if (S_Sub2 >= 0)GeneD.Sub2 = S_Sub2;
        else if (OpCount >= 2)GeneD.Sub2 = GeneOpTablesGet();

        if (S_Sub3 >= 0) GeneD.Sub3 = S_Sub3;
        else if (OpCount >= 3)GeneD.Sub3 = GeneOpTablesGet();

        GeneD.Add1 = 0;
        GeneD.Add1 = GeneAddRand();
        GeneD.Add2 = 0;
        GeneD.Add2 = GeneAddRand();
        GeneD.Add3 = 0;
        GeneD.Add3 = GeneAddRand();
    }
    static int GeneAddRand()
    {
        switch (Random.Range(0, 5))
        {
            case 0: return 10;
            default: return 13;
            case 3: return 16;
            case 4: return 20;
        }
    }
    static int GeneOpTablesGet()
    {
        var TPer = 0f;
        for (int i = 0; i < DB.GeneDropTables.Length; i++)
        {
            TPer += DB.GeneDropTables[i].P;
        }
        var Val = Random.Range(0, TPer);
        var PStack = 0f;
        for (int i = 0; i < DB.GeneDropTables.Length; i++)
        {
            if (Val <= DB.GeneDropTables[i].P + PStack) return i;
            else PStack += DB.GeneDropTables[i].P;
        }
        return 0;
    }

    static public float GenePowGet(Enum_GeneOptions Option, int P)
    {
        switch (Option)
        {
            case Enum_GeneOptions.最大HP:return 200 * P;
            case Enum_GeneOptions.HP回復速度:return 1.5f * P;
            case Enum_GeneOptions.最大MP:return 0.5f * P;
            case Enum_GeneOptions.MP回復速度:return 0.05f * P;
            case Enum_GeneOptions.SP回復速度:return 0.01f * P;
            case Enum_GeneOptions.SP回復量:return 0.02f * P;
            case Enum_GeneOptions.攻撃力:return 25 * P;
            case Enum_GeneOptions.防御力:return 30 * P;
            case Enum_GeneOptions.近ダメージ:return 0.15f * P;
            case Enum_GeneOptions.遠ダメージ:return 0.1f * P;
            case Enum_GeneOptions.通常ダメージ: return 0.1f * P;
            case Enum_GeneOptions.重撃ダメージ: return 0.15f * P;
            case Enum_GeneOptions.落下ダメージ: return 0.15f * P;
            case Enum_GeneOptions.スキルダメージ: return 0.15f * P;
            case Enum_GeneOptions.必殺ダメージ: return 0.2f * P;

        }
        return 0;
    }
    static public float GenePowT(Class_Save_GeneSet GSet, Enum_GeneOptions Option)
    {
        var Val = 0f;
        for (int i = 0; i < 5; i++)
        {
            var Num = -1;
            switch (i)
            {
                case 0: Num = GSet.G1_ID; break;
                case 1: Num = GSet.G2_ID; break;
                case 2: Num = GSet.G3_ID; break;
                case 3: Num = GSet.G4_ID; break;
                case 4: Num = GSet.G5_ID; break;
            }
            if (Num >= 0)
            {
                var GeneD = Genes.Datas[Num];
                if((Enum_GeneOptions)GeneD.Main == Option)Val += GenePowGet(Option, GeneD.LV * 30);
                if ((Enum_GeneOptions)GeneD.Sub1 == Option) Val += GenePowGet(Option, GeneD.Add1);
                if ((Enum_GeneOptions)GeneD.Sub2 == Option) Val += GenePowGet(Option, GeneD.Add2);
                if ((Enum_GeneOptions)GeneD.Sub3 == Option) Val += GenePowGet(Option, GeneD.Add3);
            }
        }
        return Val;

    }
    static public int GeneIDGet(int SelectID)
    {
        switch ((Enum_SetSlot)SelectID)
        {
            case Enum_SetSlot.因子1:return GeneGet.G1_ID;
            case Enum_SetSlot.因子2: return GeneGet.G2_ID;
            case Enum_SetSlot.因子3: return GeneGet.G3_ID;
            case Enum_SetSlot.因子4: return GeneGet.G4_ID;
            case Enum_SetSlot.因子5: return GeneGet.G5_ID;
        }
        return -1;
    }
    static public string GeneInfo(Class_Save_GeneData GeneD,bool SetCheck)
    {
        var Str = "「" + GeneD.Name + "」";
        Str += "\n" + (Enum_GeneTypes)GeneD.Type + ":" + (Enum_GeneFormat)GeneD.Format;
        Str += "\n" + GeneD.LV + "/10LV";
        Str += "\n" + (Enum_GeneOptions)GeneD.Main;
        Str += "\n" + GenePowGet((Enum_GeneOptions)GeneD.Main, GeneD.LV * 30).ToString("F1");
        if (GeneD.Sub1 >= 0)
        {
            Str += "\n<size=75%>(1)" + (Enum_GeneOptions)GeneD.Sub1;
            Str += GenePowGet((Enum_GeneOptions)GeneD.Sub1, GeneD.Add1).ToString("F1");
            Str += "</size>";
        }
        if (GeneD.Sub2 >= 0)
        {
            Str += "\n<size=75%>(2)" + (Enum_GeneOptions)GeneD.Sub2;
            Str += GenePowGet((Enum_GeneOptions)GeneD.Sub2, GeneD.Add2).ToString("F1");
            Str += "</size>";
        }
        if (GeneD.Sub3 >= 0)
        {
            Str += "\n<size=75%>(3)" + (Enum_GeneOptions)GeneD.Sub3;
            Str += GenePowGet((Enum_GeneOptions)GeneD.Sub3, GeneD.Add3).ToString("F1");
            Str += "</size>";
        }
        var GType = DB.Genes[GeneD.Type];
        Str += "\n" + GType.Info;
        var SetCount = 0;
        if (SetCheck)
        {
            SetCount = GeneSetCount(GeneGet,(Enum_GeneTypes)GeneD.Type);
            Str += "\n×" + SetCount;
            Str += SetCount >= 2 ? "<color=#FFFF00>" : "<color=#888888>";
        }
        else Str += "<color=#FFFFFF>";
        Str += "\n<2セット>\n<size=75%>" + GType.Set2 + "</size></color>";
        if (SetCheck) Str += SetCount >= 4 ? "<color=#FFFF00>" : "<color=#888888>";
        else Str += "<color=#FFFFFF>";
        Str += "\n<4セット>\n<size=75%>" + GType.Set4 + "</size></color>";
        return Str;
    }
    static public int GeneSetCount(Class_Save_GeneSet GSet,Enum_GeneTypes GType)
    {
        int TCount = 0;
        for (int k = 0; k < 5; k++)
        {
            var GID = -1;
            switch (k)
            {
                case 0:GID = GSet.G1_ID;break;
                case 1: GID = GSet.G2_ID; break;
                case 2: GID = GSet.G3_ID; break;
                case 3: GID = GSet.G4_ID; break;
                case 4: GID = GSet.G5_ID; break;
            }
            if (GID >= 0)
            {
                var GeneD = Genes.Datas[GID];
                if ((Enum_GeneTypes)GeneD.Type == GType) TCount++;
            }
        }
        return TCount;
    }
    #endregion
}
