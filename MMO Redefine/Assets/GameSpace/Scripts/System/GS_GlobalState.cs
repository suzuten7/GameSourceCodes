namespace GmSystem
{
    using UnityEngine;
    public class GS_GlobalState
    {
        public enum Enum_Element
        {
            [InspectorName("無")] Non = 0,
            [InspectorName("火")] Fire,
            [InspectorName("水")] Water,
            [InspectorName("風")] Wind,
            [InspectorName("土")] Earth,
            [InspectorName("光")] Light,
            [InspectorName("闇")] Dark,
        }
        public enum Enum_StateAddsType
        {
            [InspectorName("最大HP")] MaxHP,
            [InspectorName("HP回復速度")] HPRegene,
            [InspectorName("最大MP")] MaxMP,
            [InspectorName("MP回復速度")] MPRegene,
            [InspectorName("最大ST")] MaxST,
            [InspectorName("ST回復速度")] STRegene,

            [InspectorName("物理攻撃力")] PAtk = 100,
            [InspectorName("魔法攻撃力")] MAtk,
            [InspectorName("物理防御力")] PDef,
            [InspectorName("魔法防御力")] MDef,

            [InspectorName("会心率")] CritPer = 200,
            [InspectorName("会心ダメージ")] CritMult,
            [InspectorName("攻撃速度")] AtkSpeed,
            [InspectorName("移動速度")] MoveSpeed,
            [InspectorName("EX時間チャージ")] EXTimeCharge = 210,
            [InspectorName("EXダメージチャージ")] EXDamageCharge,
            [InspectorName("EX命中チャージ")] EXHitCharge,

            [InspectorName("スキルCT")] SkillCT = 220,

            [InspectorName("与ダメージ強化")] AddDamageMult = 204,
            [InspectorName("被ダメージ軽減")] TakeDamageRegist,

            [InspectorName("与回復強化")] AddHealMult,
            [InspectorName("受回復強化")] TakeHealMult,

            [InspectorName("命中率")] HitPer,
            [InspectorName("回避率")] DogePer,

            [InspectorName("通常ダメージ強化")] NormalDamageMult = 230,
            [InspectorName("重撃ダメージ強化")] HevDamageMult,
            [InspectorName("スキルダメージ強化")] SkillDamageMult,
            [InspectorName("必殺ダメージ強化")] EXDamageMult,

            [InspectorName("近距離ダメージ強化")] ShortDamageMult = 240,
            [InspectorName("中距離ダメージ強化")] MidleDamageMult,
            [InspectorName("遠距離ダメージ強化")] LongDamageMult,


            [InspectorName("全属性耐性")] AllEleRegist = 300,
            [InspectorName("無属性耐性")] NonEleRegist,
            [InspectorName("火属性耐性")] FireEleRegist,
            [InspectorName("水属性耐性")] WaterEleRegist,
            [InspectorName("風属性耐性")] WindEleRegist,
            [InspectorName("土属性耐性")] EarthEleRegist,
            [InspectorName("光属性耐性")] LightEleRegist,
            [InspectorName("闇属性耐性")] DarkEleRegist,

            [InspectorName("無属性強化")] NonEleMult = 400,
            [InspectorName("火属性強化")] FireEleMult,
            [InspectorName("水属性強化")] WaterEleMult,
            [InspectorName("風属性強化")] WindEleMult,
            [InspectorName("土属性強化")] EarthEleMult,
            [InspectorName("光属性強化")] LightEleMult,
            [InspectorName("闇属性強化")] DarkEleMult,
        }
        public enum Enum_Buf
        {
            [InspectorName("最大HP変化")] MaxHPChange = 0,
            [InspectorName("HP回復速度変化")] HPRegeneChange,
            [InspectorName("最大MP変化")] MaxMPChange,
            [InspectorName("MP回復速度変化")] MPRegeneChange,
            [InspectorName("最大ST変化")] MaxSTChange,
            [InspectorName("ST回復速度変化")] STRegeneChange,

            [InspectorName("物理攻撃力変化")] PAtkChange = 100,
            [InspectorName("魔法攻撃力変化")] MAtkChange,
            [InspectorName("物理防御力変化")] PDefChange,
            [InspectorName("魔法防御力変化")] MDefChange,

            [InspectorName("会心率変化")] CritPerChange = 200,
            [InspectorName("会心ダメージ変化")] CritMultChange,
            [InspectorName("攻撃速度変化")] AtkSpeedChange,
            [InspectorName("移動速度変化")] MoveSpeedChange,
            [InspectorName("EX時間チャージ変化")] EXTimeChargeChange = 210,
            [InspectorName("EXダメージチャージ変化")] EXDamageChargeChange,
            [InspectorName("EX命中チャージ変化")] EXHitChargeChange,

            [InspectorName("スキルCT変化")] SkillCTChange = 220,


            [InspectorName("与ダメージ倍率変化")] AddDamageMultChange = 204,
            [InspectorName("被ダメージ軽減変化")] TakeDamageRegistChange,
            [InspectorName("与回復倍率変化")] AddHealMultChange,
            [InspectorName("受回復倍率変化")] TakeHealMultChange,

            [InspectorName("命中率変化")] HitPerChange,
            [InspectorName("回避率変化")] DogePerChange,

            [InspectorName("通常ダメージ倍率変化")] NormalDamageMultChange = 230,
            [InspectorName("重撃ダメージ倍率変化")] HevDamageMultChange,
            [InspectorName("スキルダメージ倍率変化")] SkillDamageMultChange,
            [InspectorName("必殺ダメージ倍率変化")] EXDamageMultChange,

            [InspectorName("近距離ダメージ倍率変化")] ShortDamageMultChange = 240,
            [InspectorName("中距離ダメージ倍率変化")] MidleDamageMultChange,
            [InspectorName("遠距離ダメージ倍率変化")] LongDamageMultChange,


            [InspectorName("全属性耐性変化")] AllEleRegistChange = 300,
            [InspectorName("無属性耐性変化")] NonEleRegistChange,
            [InspectorName("火属性耐性変化")] FireEleRegistChange,
            [InspectorName("水属性耐性変化")] WaterEleRegistChange,
            [InspectorName("風属性耐性変化")] WindEleRegistChange,
            [InspectorName("土属性耐性変化")] EarthEleRegistChange,
            [InspectorName("光属性耐性変化")] LightEleRegistChange,
            [InspectorName("闇属性耐性変化")] DarkEleRegistChange,

            [InspectorName("無属性倍率変化")] NonEleMultChange = 400,
            [InspectorName("火属性倍率変化")] FireEleMultChange,
            [InspectorName("水属性倍率変化")] WaterEleMultChange,
            [InspectorName("風属性倍率変化")] WindEleMultChange,
            [InspectorName("土属性倍率変化")] EarthEleMultChange,
            [InspectorName("光属性倍率変化")] LightEleMultChange,
            [InspectorName("闇属性倍率変化")] DarkEleMultChange,

            [InspectorName("無属性上書き")] NonEleRide = 500,
            [InspectorName("火属性上書き")] FireEleRide,
            [InspectorName("水属性上書き")] WaterEleRide,
            [InspectorName("風属性上書き")] WindEleRide,
            [InspectorName("土属性上書き")] EarthEleRide,
            [InspectorName("光属性上書き")] LightEleRide,
            [InspectorName("闇属性上書き")] DarkEleRide,

            [InspectorName("複合属性上書き")] MixEleRide = 520,

            [InspectorName("毒")] Poison = 1000,
            [InspectorName("冷気")] Reiki,
            [InspectorName("挑発")] Tyouhatu,

            [InspectorName("再生")] Regene = 2000,
            [InspectorName("シールド")] Shild,
            [InspectorName("バリア")] Barria,
            [InspectorName("MPシールド")] MPShild,
            [InspectorName("リバイブ")] Revive,

            [InspectorName("覚醒")] Kakusei = 9999,
            [InspectorName("時間制限")] TimeLimit = 10000,
        }

        public enum Enum_StateAddsOption
        {
            [InspectorName("基礎定数")] BaseConst,
            [InspectorName("加算割合")] AddRate,
            [InspectorName("後定数")] BackConst,
            [InspectorName("最終割合")] FinalRate,
            [InspectorName("最終定数")] FinalConst,
        }
        public enum Enum_BufOp
        {
            [InspectorName("無")] Non,
            [InspectorName("増加_基礎定数")] Add_BaseConst,
            [InspectorName("増加_加算割合")] Add_AddRate,
            [InspectorName("増加_後定数")] Add_BackConst,
            [InspectorName("増加_最終割合")] Add_FinalRate,
            [InspectorName("増加_最終定数")] Add_FinalConst,

            [InspectorName("減少_基礎定数")] Rem_BaseConst,
            [InspectorName("減少_加算割合")] Rem_AddRate,
            [InspectorName("減少_後定数")] Rem_BackConst,
            [InspectorName("減少_最終割合")] Rem_FinalRate,
            [InspectorName("減少_最終定数")] Rem_FinalConst,
        }
        public enum Enum_BufSet
        {
            [InspectorName("付与")] Add,
            [InspectorName("付与増加")] AddUp,
            [InspectorName("消去")] Remove = 99,
        }
        public enum Enum_ValState
        {
            HP,
            MP,
            ST,
            EX,
        }

        public enum Enum_ValueBase
        {
            [InspectorName("定数")] Const = 0,
            [InspectorName("最大HP")] MaxHP = 1,
            [InspectorName("HP回復速度")] HPRegene,
            [InspectorName("現HP")] CHP,

            [InspectorName("最大MP")] MaxMP,
            [InspectorName("MP回復速度")] MPRegene,
            [InspectorName("現MP")] CMP,

            [InspectorName("最大ST")] MaxST,
            [InspectorName("ST回復速度")] STRegene,
            [InspectorName("現ST")] CST,

            [InspectorName("現EX")] CEX,

            [InspectorName("攻撃力比率")] AtkRatio = 100,
            [InspectorName("防御力比率")] DefRatio,

            [InspectorName("物理攻撃力")] PAtk = 110,
            [InspectorName("魔法攻撃力")] MAtk,
            [InspectorName("物理防御力")] PDef,
            [InspectorName("魔法防御力")] MDef,

            [InspectorName("会心率")] CritPer = 500,
            [InspectorName("会心ダメージ")] CritMult,
            [InspectorName("攻撃速度")] AtkSpeed,
            [InspectorName("移動速度")] MoveSpeed,
            [InspectorName("与回復倍率")] AddHealMult,
            [InspectorName("受回復倍率")] TakeHealMult,
            [InspectorName("EX時間チャージ")] EXTimeCharge,
            [InspectorName("EXダメージチャージ")] EXDamageCharge,
            [InspectorName("EXヒットチャージ")] EXHitCharge,

            [InspectorName("全属性耐性")] AllEleRegist = 600,
            [InspectorName("無属性耐性")] NonEleRegist,
            [InspectorName("火属性耐性")] FireEleRegist,
            [InspectorName("水属性耐性")] WaterEleRegist,
            [InspectorName("風属性耐性")] WindEleRegist,
            [InspectorName("土属性耐性")] EarthEleRegist,
            [InspectorName("光属性耐性")] LightEleRegist,
            [InspectorName("闇属性耐性")] DarkEleRegist,

            [InspectorName("与ダメージ")] AddDamage = 1000,
        }
        public enum Enum_TeamCheck
        {
            [InspectorName("無関心")] Non = -1,
            [InspectorName("敵対")] Enemy = 0,
            [InspectorName("味方")] Friend = 1,

        }

        static public string TeamGet_Str(int teamID, out Color teamCol, Color? baseCol = null)
        {
            var teamStr = "Enemy";
            teamCol = baseCol == null ? Color.red : (Color)baseCol;
            switch (teamID)
            {
                case -9999: teamStr = "独立"; teamCol = Color.magenta; break;
                case -1: teamStr = "Aチーム"; teamCol = Color.yellow; break;
                case -2: teamStr = "Bチーム"; teamCol = Color.green; break;
                case -3: teamStr = "Cチーム"; teamCol = Color.cyan; break;

                case 1000: teamStr = "中立"; teamCol = Color.gray; break;
            }
            return teamStr;
        }
        static public bool V3TimeCheck(int itime, Vector3 timeIfs)
        {
            if (itime < Mathf.RoundToInt(timeIfs.x * 60)) return false;
            if (itime > Mathf.RoundToInt(Mathf.Max(timeIfs.x, timeIfs.y) * 60)) return false;
            if (timeIfs.z > 0 && (itime - Mathf.RoundToInt(timeIfs.x * 60)) % Mathf.RoundToInt((timeIfs.z) * 60) != 0) return false;
            return true;
        }

    }
}

