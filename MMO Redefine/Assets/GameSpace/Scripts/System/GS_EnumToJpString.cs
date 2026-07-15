namespace GmSystem
{
    using System.Collections.Generic;
    using UnityEngine;
    using static GS_GlobalState;
    using static Datas.Data_Attack;
    public class GS_EnumToJpString
    {
        readonly static Dictionary<Enum_StateAddsOption, string> AddsOptionStrDic = new ()
        {
            [Enum_StateAddsOption.BaseConst] = "基礎定数",
            [Enum_StateAddsOption.AddRate] = "加算割合",
            [Enum_StateAddsOption.BackConst] = "後定数",
            [Enum_StateAddsOption.FinalRate] = "最終割合",
            [Enum_StateAddsOption.FinalConst] = "最終定数",
        };
        static public string EnumToJp(Enum_StateAddsOption en)
        {
            return AddsOptionStrDic.TryGetValue(en,out var str) ? str : en.ToString();
        }
        readonly static Dictionary<Enum_StateAddsType, string> AddsTypeStrDic = new ()
        {
            [Enum_StateAddsType.MaxHP] = "最大HP",
            [Enum_StateAddsType.HPRegene] = "HP回復速度",
            [Enum_StateAddsType.MaxMP] = "最大MP",
            [Enum_StateAddsType.MPRegene] = "MP回復速度",
            [Enum_StateAddsType.MaxST] = "最大ST",
            [Enum_StateAddsType.STRegene] = "ST回復速度",

            [Enum_StateAddsType.PAtk] = "物理攻撃力",
            [Enum_StateAddsType.MAtk] = "魔法攻撃力",
            [Enum_StateAddsType.PDef] = "物理防御力",
            [Enum_StateAddsType.MDef] = "魔法防御力",

            [Enum_StateAddsType.CritPer] = "会心率",
            [Enum_StateAddsType.CritMult] = "会心ダメージ",
            [Enum_StateAddsType.AtkSpeed] = "攻撃速度",
            [Enum_StateAddsType.MoveSpeed] = "移動速度",
            [Enum_StateAddsType.EXTimeCharge] = "EX時間チャージ",
            [Enum_StateAddsType.EXDamageCharge] = "EXダメージチャージ",
            [Enum_StateAddsType.EXHitCharge] = "EX命中チャージ",

            [Enum_StateAddsType.SkillCT] = "スキルCT",
            [Enum_StateAddsType.AddDamageMult] = "与ダメージ強化",
            [Enum_StateAddsType.TakeDamageRegist] = "被ダメージ軽減",
            [Enum_StateAddsType.AddHealMult] = "与回復強化",
            [Enum_StateAddsType.TakeHealMult] = "受回復強化",
            [Enum_StateAddsType.HitPer] = "命中率",
            [Enum_StateAddsType.DogePer] = "回避率",

            [Enum_StateAddsType.NormalDamageMult] = "通常攻撃倍率",
            [Enum_StateAddsType.HevDamageMult] = "重攻撃倍率",
            [Enum_StateAddsType.SkillDamageMult] = "スキル攻撃倍率",
            [Enum_StateAddsType.EXDamageMult] = "必殺攻撃倍率",

            [Enum_StateAddsType.ShortDamageMult] = "近距離攻撃倍率",
            [Enum_StateAddsType.MidleDamageMult] = "中距離攻撃倍率",
            [Enum_StateAddsType.LongDamageMult] = "遠距離攻撃倍率",

            [Enum_StateAddsType.AllEleRegist] = "全属性耐性",
            [Enum_StateAddsType.NonEleRegist] = "無属性耐性",
            [Enum_StateAddsType.FireEleRegist] = "火属性耐性",
            [Enum_StateAddsType.WaterEleRegist] = "水属性耐性",
            [Enum_StateAddsType.WindEleRegist] = "風属性耐性",
            [Enum_StateAddsType.EarthEleRegist] = "土属性耐性",
            [Enum_StateAddsType.LightEleRegist] = "光属性耐性",
            [Enum_StateAddsType.DarkEleRegist] = "闇属性耐性",

            [Enum_StateAddsType.NonEleMult] = "無属性強化",
            [Enum_StateAddsType.FireEleMult] = "火属性強化",
            [Enum_StateAddsType.WaterEleMult] = "水属性強化",
            [Enum_StateAddsType.WindEleMult] = "風属性強化",
            [Enum_StateAddsType.EarthEleMult] = "土属性強化",
            [Enum_StateAddsType.LightEleMult] = "光属性強化",
            [Enum_StateAddsType.DarkEleMult] = "闇属性強化",

        };
        static public string EnumToJp(Enum_StateAddsType en)
        {
            return AddsTypeStrDic.TryGetValue(en, out var str) ? str : en.ToString();
        }

        readonly static Dictionary<Enum_Element, string> ElementStrDic = new ()
        {
            [Enum_Element.Non] = "無",
            [Enum_Element.Fire] = "火",
            [Enum_Element.Water] = "水",
            [Enum_Element.Wind] = "風",
            [Enum_Element.Earth] = "土",
            [Enum_Element.Light] = "光",
            [Enum_Element.Dark] = "闇",

        };
        static public string EnumToJp(Enum_Element en)
        {
            return ElementStrDic.TryGetValue(en, out var str) ? str : en.ToString();
        }

        readonly static Dictionary<Enum_ValueBase, string> ValueBaseStrDic = new ()
        {
            [Enum_ValueBase.Const] = "定数",
            [Enum_ValueBase.MaxHP] = "最大HP",
            [Enum_ValueBase.HPRegene] = "HP回復速度",
            [Enum_ValueBase.CHP] = "現在HP",

            [Enum_ValueBase.MaxMP] = "最大MP",
            [Enum_ValueBase.MPRegene] = "MP回復速度",
            [Enum_ValueBase.CMP] = "現MP",

            [Enum_ValueBase.MaxST] = "最大ST",
            [Enum_ValueBase.STRegene] = "ST回復速度",
            [Enum_ValueBase.CST] = "現ST",

            [Enum_ValueBase.CEX] = "現EX",

            [Enum_ValueBase.AtkRatio] = "攻撃力比率",
            [Enum_ValueBase.DefRatio] = "防御力比率",

            [Enum_ValueBase.PAtk] = "物理攻撃力",
            [Enum_ValueBase.MAtk] = "魔法攻撃力",
            [Enum_ValueBase.PDef] = "物理防御力",
            [Enum_ValueBase.MDef] = "魔法防御力",

            [Enum_ValueBase.CritPer] = "会心率",
            [Enum_ValueBase.CritMult] = "会心ダメージ",
            [Enum_ValueBase.AtkSpeed] = "攻撃速度",
            [Enum_ValueBase.MoveSpeed] = "移動速度",
            [Enum_ValueBase.AddHealMult] = "与回復倍率",
            [Enum_ValueBase.TakeHealMult] = "受回復倍率",
            [Enum_ValueBase.EXTimeCharge] = "EX時間チャージ",
            [Enum_ValueBase.EXDamageCharge] = "EXダメージチャージ",
            [Enum_ValueBase.EXHitCharge] = "EXヒットチャージ",

            [Enum_ValueBase.AllEleRegist] = "全属性耐性",
            [Enum_ValueBase.NonEleRegist] = "無属性耐性",
            [Enum_ValueBase.FireEleRegist] = "火属性耐性",
            [Enum_ValueBase.WaterEleRegist] = "水属性耐性",
            [Enum_ValueBase.WindEleRegist] = "風属性耐性",
            [Enum_ValueBase.EarthEleRegist] = "土属性耐性",
            [Enum_ValueBase.LightEleRegist] = "光属性耐性",
            [Enum_ValueBase.DarkEleRegist] = "闇属性耐性",

            [Enum_ValueBase.AddDamage] = "与ダメージ",

        };
        static public string EnumToJp(Enum_ValueBase en)
        {
            return ValueBaseStrDic.TryGetValue(en, out var str) ? str : en.ToString();
        }
        readonly static Dictionary<Enum_BufOp, string> BufOpStrDic = new ()
        {
            [Enum_BufOp.Non] = "",
            [Enum_BufOp.Add_BaseConst] = "基礎定数増加",
            [Enum_BufOp.Add_AddRate] = "加算割合増加",
            [Enum_BufOp.Add_BackConst] = "後定数増加",
            [Enum_BufOp.Add_FinalRate] = "最終割合増加",
            [Enum_BufOp.Add_FinalConst] = "最終定数増加",

            [Enum_BufOp.Rem_BaseConst] = "基礎定数減少",
            [Enum_BufOp.Rem_AddRate] = "加算割合減少",
            [Enum_BufOp.Rem_BackConst] = "後定数減少",
            [Enum_BufOp.Rem_FinalRate] = "最終割合減少",
            [Enum_BufOp.Rem_FinalConst] = "最終定数減少",
        };
        static public string EnumToJp(Enum_BufOp en)
        {
            return BufOpStrDic.TryGetValue(en, out var str) ? str : en.ToString();
        }
        readonly static Dictionary<Enum_BufSet, string> BufSetStrDic = new ()
        {
            [Enum_BufSet.Add] = "付与",
            [Enum_BufSet.AddUp] = "付与増加",
            [Enum_BufSet.Remove] = "消去",
        };
        static public string EnumToJp(Enum_BufSet en)
        {
            return BufSetStrDic.TryGetValue(en, out var str) ? str : en.ToString();
        }

        readonly static Dictionary<Enum_Buf, string> BufsStrDic = new ()
        {
            [Enum_Buf.MaxHPChange] = "最大HP変化",
            [Enum_Buf.HPRegeneChange] = "HP回復速度変化",
            [Enum_Buf.MaxMPChange] = "最大MP変化",
            [Enum_Buf.MPRegeneChange] = "MP回復速度変化",
            [Enum_Buf.MaxSTChange] = "最大ST変化",
            [Enum_Buf.STRegeneChange] = "ST回復速度変化",
            [Enum_Buf.PAtkChange] = "物理攻撃力変化",
            [Enum_Buf.MAtkChange] = "魔法攻撃力変化",
            [Enum_Buf.PDefChange] = "物理防御力変化",
            [Enum_Buf.MDefChange] = "魔法防御力変化",

            [Enum_Buf.CritPerChange] = "会心率変化",
            [Enum_Buf.CritMultChange] = "会心ダメージ変化",
            [Enum_Buf.AtkSpeedChange] = "攻撃速度変化",
            [Enum_Buf.MoveSpeedChange] = "移動速度変化",
            [Enum_Buf.EXTimeChargeChange] = "EX時間チャージ変化",
            [Enum_Buf.EXDamageChargeChange] = "EXダメージチャージ変化",
            [Enum_Buf.EXHitChargeChange] = "EX命中チャージ変化",

            [Enum_Buf.SkillCTChange] = "スキルCT変化",

            [Enum_Buf.AddDamageMultChange] = "与ダメージ倍率変化",
            [Enum_Buf.TakeDamageRegistChange] = "被ダメージ軽減変化",
            [Enum_Buf.AddHealMultChange] = "与回復倍率変化",
            [Enum_Buf.TakeHealMultChange] = "受回復倍率変化",
            [Enum_Buf.HitPerChange] = "命中率変化",
            [Enum_Buf.DogePerChange] = "回避率変化",


            [Enum_Buf.NormalDamageMultChange] = "通常攻撃倍率変化",
            [Enum_Buf.HevDamageMultChange] = "重攻撃倍率変化",
            [Enum_Buf.SkillDamageMultChange] = "スキル攻撃倍率変化",
            [Enum_Buf.EXDamageMultChange] = "必殺攻撃倍率変化",

            [Enum_Buf.ShortDamageMultChange] = "近距離攻撃倍率変化",
            [Enum_Buf.MidleDamageMultChange] = "中距離攻撃倍率変化",
            [Enum_Buf.LongDamageMultChange] = "遠距離攻撃倍率変化",

            [Enum_Buf.AllEleRegistChange] = "全属性耐性変化",
            [Enum_Buf.NonEleRegistChange] = "無属性耐性変化",
            [Enum_Buf.FireEleRegistChange] = "火属性耐性変化",
            [Enum_Buf.WaterEleRegistChange] = "水属性耐性変化",
            [Enum_Buf.WindEleRegistChange] = "風属性耐性変化",
            [Enum_Buf.EarthEleRegistChange] = "土属性耐性変化",
            [Enum_Buf.LightEleRegistChange] = "光属性耐性変化",
            [Enum_Buf.DarkEleRegistChange] = "闇属性耐性変化",

            [Enum_Buf.NonEleMultChange] = "無属性倍率変化",
            [Enum_Buf.FireEleMultChange] = "火属性倍率変化",
            [Enum_Buf.WaterEleMultChange] = "水属性倍率変化",
            [Enum_Buf.WindEleMultChange] = "風属性倍率変化",
            [Enum_Buf.EarthEleMultChange] = "土属性倍率変化",
            [Enum_Buf.LightEleMultChange] = "光属性倍率変化",
            [Enum_Buf.DarkEleMultChange] = "闇属性倍率変化",

            [Enum_Buf.NonEleRide] = "無属性上書き",
            [Enum_Buf.FireEleRide] = "火属性上書き",
            [Enum_Buf.WaterEleRide] = "水属性上書き",
            [Enum_Buf.WindEleRide] = "風属性上書き",
            [Enum_Buf.EarthEleRide] = "土属性上書き",
            [Enum_Buf.LightEleRide] = "光属性上書き",
            [Enum_Buf.DarkEleRide] = "闇属性上書き",

            [Enum_Buf.MixEleRide] = "複合属性上書き",

            [Enum_Buf.Poison] = "毒",
            [Enum_Buf.Reiki] = "冷気",
            [Enum_Buf.Tyouhatu] = "挑発",

            [Enum_Buf.Regene] = "再生",
            [Enum_Buf.Shild] = "シールド",
            [Enum_Buf.Barria] = "バリア",
            [Enum_Buf.MPShild] = "MPシールド",
            [Enum_Buf.Revive] = "リバイブ",

            [Enum_Buf.Kakusei] = "覚醒",
            [Enum_Buf.TimeLimit] = "時間制限",



        };
        static public string EnumToJp(Enum_Buf en)
        {
            return BufsStrDic.TryGetValue(en, out var str) ? str : en.ToString();
        }

        readonly static Dictionary<Enum_BranchIfs, string> BranchIfTmpStrDic = new ()
        {
            [Enum_BranchIfs.SingleInput] = "単入力",
            [Enum_BranchIfs.LongInput] = "長入力",
            [Enum_BranchIfs.OutInput] = "離入力",
            [Enum_BranchIfs.NonInput] = "未入力",
            [Enum_BranchIfs.AddDamage] = "与ダメージ時",
            [Enum_BranchIfs.TakeDamage] = "受ダメージ時",
            [Enum_BranchIfs.AddHeal] = "与回復時",
            [Enum_BranchIfs.TakeHeal] = "受回復時",
        };

        static public string EnumToJp(Enum_BranchIfs en)
        {
            return BranchIfTmpStrDic.TryGetValue(en, out var str) ? str : en.ToString();
        }
        readonly static Dictionary<Enum_AtkType, string> AtkTypeStrDic = new ()
        {
            [Enum_AtkType.Normal] = "通常",
            [Enum_AtkType.Hev] = "重撃",
            [Enum_AtkType.Skill] = "スキル",
            [Enum_AtkType.EX] = "必殺",
            [Enum_AtkType.Other] = "その他攻撃",

        };
        static public string EnumToJp(Enum_AtkType en)
        {
            return AtkTypeStrDic.TryGetValue(en, out var str) ? str : en.ToString();
        }
        readonly static Dictionary<Enum_RangeType, string> RangeTypeStrDic = new ()
        {
            [Enum_RangeType.Short] = "近距離",
            [Enum_RangeType.Midle] = "中距離",
            [Enum_RangeType.Long] = "遠距離",
            [Enum_RangeType.Other] = "その他射程",

        };
        static public string EnumToJp(Enum_RangeType en)
        {
            return RangeTypeStrDic.TryGetValue(en, out var str) ? str : en.ToString();
        }

        readonly static Dictionary<string, string> TmpStrDic = new ()
        {
            ["a"] = "定数",
            ["b"] = "最大HP",
            ["c"] = "HP回復速度",
        };
        static public string EnumToJp(string en)
        {
            return TmpStrDic.TryGetValue(en, out var str) ? str : en.ToString();
        }

    }
}

