
using Photon.Pun;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using static BattleManager;

public class Calculation : MonoBehaviour
{
    public const string TooltipStr = "文字列計算式(例)U.ATKdfmT.DEF[自攻撃力{防計}敵防御]" +
        "\n演算式\n[max=左右の大きい値][min=左右の小さい値][or=左右を50%の確率でどちらか][dfm=防御計算]" +
        "\nステータス値\n[U=使用者][T=対象者]+\n[.HP=現在HP][.MHP=最大HP][.MP=現在MP][.MMP=最大MP][.ATK=攻撃力][.DEF=防御]";
    [TextArea] public string Cals;
    [SerializeField] State_Base Sta1;
    [SerializeField] State_Base Sta2;
    public double Out;
    public bool Play;

    #region デバッグ用
    private void OnValidate()
    {
        if (!Play) return;
        Play = false;
        Out = Cal(Cals, Sta1, Sta2);
    }
    private void Update()
    {
        if (!Play) return;
        Play = false;
        Out = Cal(Cals, Sta1, Sta2);
    }
    #endregion

    /// <summary>値取得</summary>
    static double GetValue(string ValStr, State_Base Sta1, State_Base Sta2)
    {
        ValStr = CutOpe(ValStr);
        double Vald;
        bool Neg = ValStr.Contains("-");
        var NNegStr = ValStr.Replace("-", "");
        switch (NNegStr)
        {
            default: Vald = double.TryParse(NNegStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var oVal) ? oVal : 0; break;
            case "U.HP": Vald = Sta1.HP; break;
            case "U.MHP": Vald = Sta1.FMHP; break;
            case "U.MP": Vald = Sta1.MP; break;
            case "U.MMP": Vald = Sta1.FMMP; break;
            case "U.ATK": Vald = Sta1.FAtk; break;
            case "U.DEF": Vald = Sta1.FDef; break;
            case "T.HP": Vald = Sta2.HP; break;
            case "T.MHP": Vald = Sta2.FMHP; break;
            case "T.MP": Vald = Sta2.MP; break;
            case "T.MMP": Vald = Sta2.FMMP; break;
            case "T.ATK": Vald = Sta2.FAtk; break;
            case "T.DEF": Vald = Sta2.FDef; break;
            case "PCOUNT":Vald = BTManager!=null ? BTManager.PlayerList.Count : 1;break;
        }
        if (Neg) Vald *= -1;
        return Vald;
    }
    /// <summary>文字列数式計算</summary>
    static public double Cal(string Ca, State_Base Sta1, State_Base Sta2,bool RandAve=false)
    {
        if (Ca == null) return 0;
        if (Ca == "") return 0;
        while (true)
        {
            #region ()取得
            int BackIndex = 0;
            int BackNest = 0;
            int Nest = 0;
            string Strs = "";
            string MaxStrs = Ca;
            int MaxNest = -1;
            int MaxSIndex = -1;
            for (int i = 0; i < Ca.Length; i++)
            {
                var CutOne = Ca.Substring(i, 1);
                if (CutOne == "(")
                {
                    Nest++;
                }
                if (CutOne == ")")
                {
                    Nest--;
                }
                if (Nest != BackNest)
                {
                    if (MaxNest < BackNest)
                    {
                        MaxNest = BackNest;
                        MaxStrs = Strs;
                        MaxSIndex = BackIndex;
                        BackIndex = i;
                    }
                    BackNest = Nest;
                    Strs = "";
                }
                else Strs += CutOne;
            }
            var Get = MaxStrs;
            #endregion
            Get = ChangeReplace(Get);
            #region 計算
            var OpeCuts = Get.Split('|').ToList();
            //他
            StrOther(OpeCuts, Sta1, Sta2,RandAve);
            // べき乗
            StrPow(OpeCuts, Sta1, Sta2);
            // 掛け算・割り算
            StrMD(OpeCuts, Sta1, Sta2);
            // 足し算・引き算
            StrAR(OpeCuts, Sta1, Sta2);
            #endregion
            #region 書き換え
            if (MaxSIndex >= 0)
            {
                Ca = Ca.Remove(MaxSIndex, MaxStrs.Length + 2);
                Ca = Ca.Insert(MaxSIndex, OpeCuts[0]);
            }
            else
            {
                Ca = OpeCuts[0];
                break;
            }
            #endregion
        }

        string LogStr = CutOpe(Ca);
        return GetValue(LogStr, Sta1, Sta2);
    }

    static public string CalStr(string Ca,bool Enters)
    {
        if (Ca == null) return "";
        if (Ca == "") return "";


        Ca = Ca.Replace("U.", "自");
        Ca = Ca.Replace("T.", "対");
        Ca = Ca.Replace("MHP", "最大H#P");
        Ca = Ca.Replace("HP", "現在HP");
        Ca = Ca.Replace("MMP", "最大M#P");
        Ca = Ca.Replace("MP", "現在MP");
        Ca = Ca.Replace("ATK", "攻撃力");
        Ca = Ca.Replace("DEF", "防御力");
        Ca = Ca.Replace("PCOUNT", "プレイヤー数");
        Ca = Ca.Replace("#", "");

        var CaReplace = ChangeReplace(Ca);
        var CaCut = CaReplace.Split('|').ToList();
        string FCa = "";
        for(int i = 0; i < CaCut.Count; i++)
        {
            if (CaCut[i].Length >= 3 && CaCut[i].Substring(0,3) == "mul")
            {
                if (double.TryParse(CutOpe(CaCut[i]), NumberStyles.Any, CultureInfo.InvariantCulture, out var oVal))
                {
                    CaCut[i] = (oVal*100).ToString("F1", CultureInfo.InvariantCulture) + "%";
                }
            }
            FCa += CaCut[i];
        }
        Ca = ChangeRestoration(FCa);
        Ca = Ca.Replace("dfm", "{防計}\n");
        if(!Enters)Ca = Ca.Replace("\n", "");
        return Ca;
    }

    #region 処理用
    static string AddRemStrFixes(string Str)
    {
        while (true)
        {
            bool nChange = true;
            if (Str.IndexOf("++") >= 0)
            {
                Str = Str.Replace("++", "+");
                nChange = false;
            }
            if (Str.IndexOf("+-") >= 0)
            {
                Str = Str.Replace("+-", "-");
                nChange = false;
            }
            if (Str.IndexOf("-+") >= 0)
            {
                Str = Str.Replace("-+", "-");
                nChange = false;
            }
            if (Str.IndexOf("--") >= 0)
            {
                Str = Str.Replace("--", "+");
                nChange = false;
            }
            if (nChange) break;
        }
        return Str;
    }
    static string CutOpe(string Str)
    {
        Str = Str.Replace("rnd", "");
        Str = Str.Replace("m_a", "");
        Str = Str.Replace("m_i", "");
        Str = Str.Replace("o_r", "");
        Str = Str.Replace("d_m", "");

        Str = Str.Replace("pow", "");

        Str = Str.Replace("mul", "");
        Str = Str.Replace("div", "");

        Str = Str.Replace("add", "");
        Str = Str.Replace("rem", "-");
        return Str;
    }
    static string ChangeReplace(string Get)
    {
        //連続+-修正
        Get = AddRemStrFixes(Get);
        //空白・改行
        Get = Get.Replace(" ", "");
        Get = Get.Replace("\n", "");
        Get = Get.Replace("\r", "");
        //負値
        //他
        Get = Get.Replace("~-", "|rndneg");
        Get = Get.Replace("max-", "|m_aneg");
        Get = Get.Replace("min-", "|m_inneg");
        Get = Get.Replace("or-", "|o_rneg");
        Get = Get.Replace("dfm-", "|d_mneg");
        //乗算
        Get = Get.Replace("^-", "|powneg");
        //掛除
        Get = Get.Replace("*-", "|mulneg");
        Get = Get.Replace("/-", "|divneg");
        //正値
        //他
        Get = Get.Replace("~", "|rnd");
        Get = Get.Replace("max", "|m_a");
        Get = Get.Replace("min", "|m_i");
        Get = Get.Replace("or", "|o_r");
        Get = Get.Replace("dfm", "|d_m");
        //乗算
        Get = Get.Replace("^", "|pow");
        //掛除
        Get = Get.Replace("*", "|mul");
        Get = Get.Replace("/", "|div");
        //加減
        Get = Get.Replace("+", "|add");
        Get = Get.Replace("-", "|rem");
        //負変換
        Get = Get.Replace("neg", "-");
        if (Get.Substring(0, 1) == "|") Get = Get.Remove(0, 1);
        return Get;
    }
    static string ChangeRestoration(string Get)
    {
        //連続+-修正
        Get = AddRemStrFixes(Get);
        //空白・改行
        Get = Get.Replace(" ", "");
        Get = Get.Replace("\n", "");
        Get = Get.Replace("\r", "");
        //負値
        //他
        Get = Get.Replace("rndneg", "~-");
        Get = Get.Replace("m_aneg", "max-");
        Get = Get.Replace("m_inneg", "min-");
        Get = Get.Replace("o_rneg", "or-");
        Get = Get.Replace("d_mneg", "dfm-");
        //乗算
        Get = Get.Replace("powneg", "^-");
        //掛除
        Get = Get.Replace("mulneg", "*-");
        Get = Get.Replace("divneg", "/-");
        //正値
        //他
        Get = Get.Replace("rnd", "~");
        Get = Get.Replace("m_a", "max");
        Get = Get.Replace("m_i", "min");
        Get = Get.Replace("o_r", "or");
        Get = Get.Replace("d_m", "dfm");
        //乗算
        Get = Get.Replace("pow", "^");
        //掛除
        Get = Get.Replace("mul", "*");
        Get = Get.Replace("div", "/");
        //加減
        Get = Get.Replace("add", "+");
        Get = Get.Replace("rem", "-");

        if (Get.Substring(0, 1) == "|") Get = Get.Remove(0, 1);
        return Get;
    }
    static void StrOther(List<string> OpeCuts, State_Base Sta1, State_Base Sta2, bool RandAve)
    {
        while (true)
        {
            bool Change = false;
            for (int i = 1; i < OpeCuts.Count; i++)
            {
                string Ope;
                try { Ope = OpeCuts[i].Substring(0, 3); }
                catch { Ope = ""; }
                if (Ope == "rnd" || Ope == "m_a" || Ope == "m_i" || Ope == "o_r"||Ope=="d_m")
                {
                    var Val1 = GetValue(OpeCuts[i - 1], Sta1, Sta2);
                    var Val2 = GetValue(OpeCuts[i], Sta1, Sta2);
                    switch (Ope)
                    {
                        case "rnd": 
                            if(!RandAve)OpeCuts[i] = Random.Range((float)Val1, (float)Val2).ToString(CultureInfo.InvariantCulture);
                            else OpeCuts[i] = ((Val1 + Val2)/2f).ToString(CultureInfo.InvariantCulture);
                             break;
                        case "m_a": OpeCuts[i] = System.Math.Max(Val1, Val2).ToString(CultureInfo.InvariantCulture); break;
                        case "m_i": OpeCuts[i] = System.Math.Min(Val1, Val2).ToString(CultureInfo.InvariantCulture); break;
                        case "o_r":
                            if (!RandAve) OpeCuts[i] = (Random.value < 0.5f ? Val1 : Val2).ToString(CultureInfo.InvariantCulture);
                            else OpeCuts[i] = ((Val1 + Val2) / 2f).ToString(CultureInfo.InvariantCulture);
                            break;
                        case "d_m":
                            if (Val1 < Val2 * 2) OpeCuts[i] = ((Val1 * Val1) / (Val2 * 4)).ToString(CultureInfo.InvariantCulture);
                            else OpeCuts[i] = (Val1 - Val2).ToString(CultureInfo.InvariantCulture);
                            break;
                    }
                    OpeCuts[i] = "add" + OpeCuts[i];
                    OpeCuts.RemoveAt(i - 1);
                    Change = true;
                    break;
                }
            }
            if (!Change) break;
        }
    }
    static void StrPow(List<string> OpeCuts, State_Base Sta1, State_Base Sta2)
    {
        while (true)
        {
            bool Change = false;
            for (int i = 1; i < OpeCuts.Count; i++)
            {
                string Ope;
                try { Ope = OpeCuts[i].Substring(0, 3); }
                catch { Ope = ""; }
                if (Ope == "pow")
                {
                    var Val1 = GetValue(OpeCuts[i - 1], Sta1, Sta2);
                    var Val2 = GetValue(OpeCuts[i], Sta1, Sta2);
                    OpeCuts[i] = System.Math.Pow(Val1, Val2).ToString(CultureInfo.InvariantCulture);
                    OpeCuts[i] = "add" + OpeCuts[i];
                    OpeCuts.RemoveAt(i - 1);
                    Change = true;
                    break;
                }
            }
            if (!Change) break;
        }
    }
    static void StrMD(List<string> OpeCuts, State_Base Sta1, State_Base Sta2)
    {
        while (true)
        {
            bool Change = false;
            for (int i = 1; i < OpeCuts.Count; i++)
            {
                string Ope;
                try { Ope = OpeCuts[i].Substring(0, 3); }
                catch { Ope = ""; }
                if (Ope == "mul" || Ope == "div")
                {
                    var Val1 = GetValue(OpeCuts[i - 1], Sta1, Sta2);
                    var Val2 = GetValue(OpeCuts[i], Sta1, Sta2);
                    if (Ope == "mul") OpeCuts[i] = (Val1 * Val2).ToString(CultureInfo.InvariantCulture);
                    else OpeCuts[i] = (Val1 / Val2).ToString(CultureInfo.InvariantCulture);
                    OpeCuts[i] = "add" + OpeCuts[i];
                    OpeCuts.RemoveAt(i - 1);
                    Change = true;
                    break;
                }
            }
            if (!Change) break;
        }
    }
    static void StrAR(List<string> OpeCuts, State_Base Sta1, State_Base Sta2)
    {
        while (true)
        {
            bool Change = false;
            for (int i = 1; i < OpeCuts.Count; i++)
            {
                string Ope;
                try { Ope = OpeCuts[i].Substring(0, 3); }
                catch { Ope = ""; }
                if (Ope == "add" || Ope == "rem")
                {
                    var Val1 = GetValue(OpeCuts[i - 1], Sta1, Sta2);
                    var Val2 = GetValue(OpeCuts[i], Sta1, Sta2);
                    OpeCuts[i] = (Val1 + Val2).ToString(CultureInfo.InvariantCulture);
                    OpeCuts.RemoveAt(i - 1);
                    Change = true;
                    break;
                }
            }
            if (!Change) break;
        }
    }
    #endregion
}

