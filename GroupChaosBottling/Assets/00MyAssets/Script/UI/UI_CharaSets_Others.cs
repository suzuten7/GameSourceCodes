using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Manifesto;
using static DataBase;
using static PlayerValue;
using static BattleManager;
using static Statics;
using static DataColors;
public class UI_CharaSets_Others : MonoBehaviour
{
    [SerializeField] UI_CharaSets_Base SetBase;
    [SerializeField] TMP_Dropdown RandSetDr;

    [SerializeField] Image AI_SetBack;
    [SerializeField] Toggle AI_FBTo;
    [SerializeField] TextMeshProUGUI AI_FBTx;
    [SerializeField] TMP_Dropdown AI_SetDr;

    [SerializeField] TextMeshProUGUI[] AI_ValTxs;
    [SerializeField] Slider[] AI_ValBars;
    [SerializeField] TMP_Dropdown AI_ModeDr;
    [SerializeField] Toggle AI_PLTarget;
    [SerializeField] GameObject[] AI_Temps;
    [SerializeField] Toggle AI_StaysT;

    [SerializeField] TMP_InputField TextInportIn;
    public void UIUpdates()
    {
        #region AI設定
        AI_SetBack.color = !AI_FBTo.isOn ? new Color(0.75f, 0.75f, 0.75f) : new Color(0.25f, 0.25f, 0.25f);
        AI_FBTx.text = StrCol(DCol.ColSelects(!AI_FBTo.isOn));
        AI_FBTx.text += "表</color>";
        AI_FBTx.text += "/";
        AI_FBTx.text += StrCol(DCol.ColSelects(AI_FBTo.isOn));
        AI_FBTx.text += "裏</color>";
        var AISet = PriSetGet.AIGet(AI_FBTo.isOn);
        bool AITempDisp = false;
        bool[] AIDisps = new bool[AI_ValBars.Length];
        bool AITargetDisp = false;
        bool AIStayDisp = false;
        Class_Save_AIActions AIAc = null;
        switch (AI_SetDr.value)
        {
            case 0:
                AITempDisp = true;
                break;
            case 1:
                AI_ValTxs[0].text = "表裏切り替え確率 " + Float_ToSet(AISet.ChangePer / 10f, 1) + "%/s";
                AI_ValBars[0].value = MathF.Pow(AISet.ChangePer / 10f, 1 / 2f);
                AIDisps[0] = true;
                AI_ValTxs[1].text = "表裏確定時間 " + AITimeStr(AISet.ChangeTime);
                AI_ValBars[1].value = MathF.Pow(AISet.ChangeTime / 10f, 1 / 3f);
                AIDisps[1] = true;
                break;
            case 2:
                AI_ModeDr.value = AISet.AIMode;
                AI_PLTarget.isOn = AISet.PLTarget;
                AI_ValTxs[2].text = "対象適正距離" + Float_ToSet(AISet.Range / 100f, 2) + "μm";
                AI_ValBars[2].value = MathF.Pow(AISet.Range / 100f, 1 / 3f);
                AIDisps[2] = true;
                AI_ValTxs[3].text = "プレイヤー追従距離" + Float_ToSet(AISet.PLDis / 100f, 2) + "μm";
                AI_ValBars[3].value = MathF.Pow(AISet.PLDis / 100f, 1 / 3f);
                AIDisps[3] = true;
                AITargetDisp = true;
                break;
            case 3: AIAc = AISet.Jump; break;
            case 4: AIAc = AISet.Dash; break;
            case 5: AIAc = AISet.NAtk; break;
            case 6: AIAc = AISet.S1Atk; break;
            case 7: AIAc = AISet.S2Atk; break;
            case 8: AIAc = AISet.EAtk; break;
        }
        if (AIAc != null)
        {
            AI_ValTxs[4].text = "基本確率" + Float_ToSet(AIAc.PerBase / 10f, 1) + "%/s";
            AI_ValBars[4].value = MathF.Pow(AIAc.PerBase / 10f, 1 / 2f);
            AIDisps[4] = true;
            AI_ValTxs[5].text = "適正距離外確率" + Float_ToSet(AIAc.PerOuR / 10f, 1) + "%/s";
            AI_ValBars[5].value = MathF.Pow(AIAc.PerOuR / 10f, 1 / 2f);
            AIDisps[5] = true;
            AI_ValTxs[6].text = "追従時確率" + Float_ToSet(AIAc.PerPLD / 10f, 1) + "%/s";
            AI_ValBars[6].value = MathF.Pow(AIAc.PerPLD / 10f, 1 / 2f);
            AIDisps[6] = true;
            AI_ValTxs[7].text = "確定時間" + AITimeStr(AIAc.TimeIn);
            AI_ValBars[7].value = MathF.Pow(AIAc.TimeIn / 10f, 1 / 3f);
            AIDisps[7] = true;
            AI_ValTxs[8].text = "入力時間" + AITimeStr(AIAc.TimeStay);
            AI_ValBars[8].value = MathF.Pow(AIAc.TimeStay / 10f, 1 / 3f);
            AIDisps[8] = AI_SetDr.value >= 5;
            AI_StaysT.isOn = AIAc.Stays;
            AIStayDisp = AI_SetDr.value >= 5;
            AI_ValTxs[9].text = "待機時間" + AITimeStr(AIAc.TimeWait);
            AI_ValBars[9].value = MathF.Pow(AIAc.TimeWait / 10f, 1 / 3f);
            AIDisps[9] = true;
        }
        for (int i = 0; i < AI_Temps.Length; i++) AI_Temps[i].SetActive(AITempDisp);

        AI_ModeDr.gameObject.SetActive(AITargetDisp);
        AI_PLTarget.gameObject.SetActive(AITargetDisp);
        for (int i = 0; i < AI_ValBars.Length; i++)
        {
            AI_ValBars[i].gameObject.SetActive(AIDisps[i]);
            AI_ValTxs[i].gameObject.SetActive(AIDisps[i]);
        }
        AI_StaysT.gameObject.SetActive(AIStayDisp);
        #endregion
    }
    public void RandSet()
    {
        switch (RandSetDr.value)
        {
            case 1:
                CharaRand();
                FWeponRand(true);
                BWeponRand();
                PassiveRand();
                break;
            case 2:
                CharaRand();
                break;
            case 3:
                FWeponRand(false);
                break;
            case 4:
                BWeponRand();
                break;
            case 5:
                FWeponRand(true);
                BWeponRand();
                break;
            case 6:
                PassiveRand();
                break;
        }
        RandSetDr.value = 0;
        SetBase.UIUpdate();
    }
    void CharaRand()
    {
        PriSetGet.CharaID = UnityEngine.Random.Range(0, DB.Charas.Length);
    }
    void FWeponRand(bool BCheck)
    {
        while (true)
        {
            PriSetGet.AtkF.N_AtkID = UnityEngine.Random.Range(0, DB.N_Atks.Length);
            if (BCheck || PriSetGet.AtkF.N_AtkID != PriSetGet.AtkB.N_AtkID) break;
        }
        while (true)
        {
            PriSetGet.AtkF.S1_AtkID = UnityEngine.Random.Range(0, DB.S_Atks.Length);
            if (BCheck || PriSetGet.AtkF.S1_AtkID != PriSetGet.AtkB.S1_AtkID || PriSetGet.AtkF.S1_AtkID != PriSetGet.AtkB.S2_AtkID) break;
        }
        while (true)
        {
            PriSetGet.AtkF.S2_AtkID = UnityEngine.Random.Range(0, DB.S_Atks.Length);
            if (BCheck || PriSetGet.AtkF.S2_AtkID != PriSetGet.AtkB.S1_AtkID || PriSetGet.AtkF.S2_AtkID != PriSetGet.AtkB.S2_AtkID) break;
        }
        while (true)
        {
            PriSetGet.AtkF.E_AtkID = UnityEngine.Random.Range(0, DB.E_Atks.Length);
            if (BCheck || PriSetGet.AtkF.E_AtkID != PriSetGet.AtkB.E_AtkID) break;
        }
    }
    void BWeponRand()
    {
        while (true)
        {
            PriSetGet.AtkB.N_AtkID = UnityEngine.Random.Range(0, DB.N_Atks.Length);
            if (PriSetGet.AtkF.N_AtkID != PriSetGet.AtkB.N_AtkID) break;
        }
        while (true)
        {
            PriSetGet.AtkB.S1_AtkID = UnityEngine.Random.Range(0, DB.S_Atks.Length);
            if (PriSetGet.AtkF.S1_AtkID != PriSetGet.AtkB.S1_AtkID || PriSetGet.AtkF.S1_AtkID != PriSetGet.AtkB.S2_AtkID) break;
        }
        while (true)
        {
            PriSetGet.AtkB.S2_AtkID = UnityEngine.Random.Range(0, DB.S_Atks.Length);
            if (PriSetGet.AtkF.S2_AtkID != PriSetGet.AtkB.S1_AtkID || PriSetGet.AtkF.S2_AtkID != PriSetGet.AtkB.S2_AtkID) break;
        }
        while (true)
        {
            PriSetGet.AtkB.E_AtkID = UnityEngine.Random.Range(0, DB.E_Atks.Length);
            if (PriSetGet.AtkF.E_AtkID != PriSetGet.AtkB.E_AtkID) break;
        }
    }
    void PassiveRand()
    {
        PriSetGet.Passive.P1_ID = UnityEngine.Random.Range(0, DB.Passives.Length);
        PriSetGet.Passive.P2_ID = UnityEngine.Random.Range(0, DB.Passives.Length);
        PriSetGet.Passive.P3_ID = UnityEngine.Random.Range(0, DB.Passives.Length);
        PriSetGet.Passive.P4_ID = UnityEngine.Random.Range(0, DB.Passives.Length);
    }
    public void AISet(int ID)
    {
        var AISet = PriSetGet.AIGet(AI_FBTo.isOn);
        Class_Save_AIActions AIAc = null;
        switch (AI_SetDr.value)
        {
            case 3: AIAc = AISet.Jump; break;
            case 4: AIAc = AISet.Dash; break;
            case 5: AIAc = AISet.NAtk; break;
            case 6: AIAc = AISet.S1Atk; break;
            case 7: AIAc = AISet.S2Atk; break;
            case 8: AIAc = AISet.EAtk; break;
        }
        switch (ID)
        {
            case -3:
                if (AIAc == null) break;
                AIAc.Stays = AI_StaysT.isOn;
                break;
            case -2:
                AISet.AIMode = AI_ModeDr.value;
                break;
            case -1:
                AISet.PLTarget = AI_PLTarget.isOn;
                break;
            case 0:
                AISet.ChangePer = Mathf.RoundToInt(Mathf.Pow(AI_ValBars[0].value, 2f) * 10f);
                break;
            case 1:
                AISet.ChangeTime = Mathf.RoundToInt(Mathf.Pow(AI_ValBars[1].value, 3f) * 10f);
                break;
            case 2:
                AISet.Range = Mathf.RoundToInt(Mathf.Pow(AI_ValBars[2].value, 3f) * 100f);
                break;
            case 3:
                AISet.PLDis = Mathf.RoundToInt(Mathf.Pow(AI_ValBars[3].value, 3f) * 100f);
                break;
            case 4:
                if (AIAc == null) break;
                AIAc.PerBase = Mathf.RoundToInt(Mathf.Pow(AI_ValBars[4].value, 2f) * 10f);
                break;
            case 5:
                if (AIAc == null) break;
                AIAc.PerOuR = Mathf.RoundToInt(Mathf.Pow(AI_ValBars[5].value, 2f) * 10f);
                break;
            case 6:
                if (AIAc == null) break;
                AIAc.PerPLD = Mathf.RoundToInt(Mathf.Pow(AI_ValBars[6].value, 2f) * 10f);
                break;
            case 7:
                if (AIAc == null) break;
                AIAc.TimeIn = Mathf.RoundToInt(Mathf.Pow(AI_ValBars[7].value, 3f) * 10f);
                break;
            case 8:
                if (AIAc == null) break;
                AIAc.TimeStay = Mathf.RoundToInt(Mathf.Pow(AI_ValBars[8].value, 3f) * 10f);
                break;
            case 9:
                if (AIAc == null) break;
                AIAc.TimeWait = Mathf.RoundToInt(Mathf.Pow(AI_ValBars[9].value, 3f) * 10f);
                break;

        }
        SetBase.UIUpdate();
    }
    public void AITempSet(int ID)
    {
        var TmpSet = new Class_Save_AddAI();
        var AISet = PriSetGet.AIGet(AI_FBTo.isOn);

        TmpSet.ChangePer = AISet.ChangePer;
        TmpSet.ChangeTime = AISet.ChangeTime;
        TmpSet.PLTarget = AISet.PLTarget;
        TmpSet.Jump = AISet.Jump;
        TmpSet.Dash = AISet.Dash;
        TmpSet.NAtk.Stays = AISet.NAtk.Stays;
        TmpSet.S1Atk.Stays = AISet.S1Atk.Stays;
        TmpSet.S2Atk.Stays = AISet.S2Atk.Stays;
        TmpSet.EAtk.Stays = AISet.EAtk.Stays;
        switch (ID)
        {
            case 0:
                if (AISet.AIMode <= 2) TmpSet.AIMode = AISet.AIMode;
                break;
            case 1:
                if (AISet.AIMode <= 2) TmpSet.AIMode = AISet.AIMode;
                TmpSet.Range = 600;
                TmpSet.NAtk.PerOuR = 400;
                break;
            case 2:
                if (AISet.AIMode <= 2) TmpSet.AIMode = AISet.AIMode;
                TmpSet.PLDis = 3000;
                TmpSet.Range = 1500;
                TmpSet.NAtk.PerOuR = 650;
                break;
            case 3:
                TmpSet.AIMode = 3;
                TmpSet.PLDis = 1200;
                TmpSet.Range = 500;
                TmpSet.Dash.PerBase = 0;
                TmpSet.Dash.PerOuR = 200;
                TmpSet.Dash.PerPLD = 0;
                TmpSet.NAtk.PerBase = 0;
                TmpSet.NAtk.PerOuR = 0;
                TmpSet.NAtk.PerPLD = 0;
                TmpSet.S1Atk.PerBase = 500;
                TmpSet.S2Atk.PerBase = 500;
                break;
            case 4:
                if (AISet.AIMode >= 3) TmpSet.AIMode = AISet.AIMode;
                else AISet.AIMode = 3;
                TmpSet.PLDis = 500;
                TmpSet.Range = 500;
                TmpSet.Dash.PerBase = 0;
                TmpSet.Dash.PerOuR = 200;
                TmpSet.Dash.PerPLD = 0;
                TmpSet.NAtk.PerBase = 0;
                TmpSet.NAtk.PerOuR = 0;
                TmpSet.NAtk.PerPLD = 0;
                TmpSet.S1Atk.PerBase = 500;
                TmpSet.S2Atk.PerBase = 500;
                break;

        }

        if (!AI_FBTo.isOn) PriSets[PSaves.PriSetID].AddAI_F = TmpSet;
        else PriSets[PSaves.PriSetID].AddAI_B = TmpSet;
        SetBase.UIUpdate();
    }
    public void AIRevCopy()
    {
        var C_AISet = PriSetGet.AIGet(AI_FBTo.isOn);
        var R_AISet = PriSetGet.AIGet(!AI_FBTo.isOn);
        Class_Save_AIActions C_AIAc = null;
        Class_Save_AIActions R_AIAc = null;
        switch (AI_SetDr.value)
        {
            case 1:
                C_AISet.ChangePer = R_AISet.ChangePer;
                C_AISet.ChangeTime = R_AISet.ChangeTime;
                break;
            case 2:
                C_AISet.AIMode = R_AISet.AIMode;
                C_AISet.PLTarget = R_AISet.PLTarget;
                C_AISet.Range = R_AISet.Range;
                C_AISet.PLDis = R_AISet.PLDis;
                break;
            case 3: C_AIAc = C_AISet.Jump; R_AIAc = R_AISet.Jump; break;
            case 4: C_AIAc = C_AISet.Dash; R_AIAc = R_AISet.Dash; break;
            case 5: C_AIAc = C_AISet.NAtk; R_AIAc = R_AISet.NAtk; break;
            case 6: C_AIAc = C_AISet.S1Atk; R_AIAc = R_AISet.S1Atk; break;
            case 7: C_AIAc = C_AISet.S2Atk; R_AIAc = R_AISet.S2Atk; break;
            case 8: C_AIAc = C_AISet.EAtk; R_AIAc = R_AISet.EAtk; break;
        }
        if (C_AIAc != null)
        {
            C_AIAc.PerBase = R_AIAc.PerBase;
            C_AIAc.PerOuR = R_AIAc.PerOuR;
            C_AIAc.PerPLD = R_AIAc.PerPLD;
            C_AIAc.TimeIn = R_AIAc.TimeIn;
            C_AIAc.TimeStay = R_AIAc.TimeStay;
            C_AIAc.Stays = R_AIAc.Stays;
            C_AIAc.TimeWait = R_AIAc.TimeWait;
        }
        SetBase.UIUpdate();
    }


    string AITimeStr(int Val)
    {
        if (Val < 610) return Float_ToSet(Val / 10f, 1) + "秒";
        else return "∞秒";
    }

    public void Inport()
    {
        if (TextInportIn.text == "") return;
        var CutJsonStr = AesExample.DecompressFromBase64(TextInportIn.text);
        var DataJsonStr = AesExample.JsonKeyCutRev(CutJsonStr);
        var RetPriSet = JsonUtility.FromJson<Class_Save_PriSet>(DataJsonStr);
        try
        {
            RetPriSet.Disp = AesExample.DecompressFromBase64(RetPriSet.Disp);
        }
        finally { }
        try
        {
            RetPriSet.Memo = AesExample.DecompressFromBase64(RetPriSet.Memo);
        }
        finally{}
        PriSets[PSaves.PriSetID] = RetPriSet;
        SetBase.UIUpdate();
    }
    public void Export()
    {
        var CpPriSet = new Class_Save_PriSet(PriSetGet);
        CpPriSet.Disp = AesExample.CompressToBase64(CpPriSet.Disp);
        CpPriSet.Memo = AesExample.CompressToBase64(CpPriSet.Memo);
        var DataJsonStr = JsonUtility.ToJson(CpPriSet);
        var CutJsonStr = AesExample.JsonKeyCutSet(DataJsonStr);
        var B64Str = AesExample.CompressToBase64(CutJsonStr);
        TextInportIn.text = B64Str;
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
