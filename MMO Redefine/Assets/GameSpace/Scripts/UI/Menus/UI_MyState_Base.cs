namespace UIs
{
    using TMPro;
    using UnityEngine;
    using static UI_System;
    using UnityEngine.UI;
    using Player;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_GlobalState;
    using static Datas.Data_Get;
    using static State.State_StateBase;
    using static GmSystem.GS_EnumToJpString;
    using static GmSystem.GS_ChangeSet;
    public class UI_MyState_Base : MonoBehaviour
    {
        [SerializeField] UI_CharaBase CharaUI;
        [SerializeField] RawImage CharaImage;
        [SerializeField] TextMeshProUGUI JobTx;
        [SerializeField] TMP_Dropdown TeamDrops;
        [SerializeField] TextMeshProUGUI MainTxs;
        [SerializeField] TextMeshProUGUI AddTxs;
        [SerializeField] TextMeshProUGUI ViewTx;
        [SerializeField] TMP_InputField LVSetIn;

        bool ViewCurrent;

        bool startDropSets = false;

        [SerializeField] Player_State MSta;
        private void LateUpdate()
        {
            var Sta = CharaUI.Sta;
            DropSet();
            if (Sta != null)ChangeValue(TeamDrops,-(int)Sta.CommonValues.Team - 1);
            var JobStr = DB.JobDatas[CharaUI.LChara.JobIDs[0]].Name + "×" + DB.JobDatas[CharaUI.LChara.JobIDs[1]].Name;
            ChangeText(JobTx,JobStr);
            var ModelImg = CharaUI.LChara.PlayerIconGet(out var ModelName);

            if (CharaUI.CharaID >= 0)ChangeTexture(CharaImage,ModelImg, true);

            var ViewStr = "ステータス表示(";
            ViewStr += (!ViewCurrent ? "<color=#FFFF00>基礎</color>/現在" : "基礎/<color=#FFFF00>現在</color>") + ")";
            ChangeText(ViewTx,ViewStr);
            ChangeText(LVSetIn, CharaUI.LChara.SetLV.ToString(), true);
            MainStrs();
            AddStrs();
        }
        void DropSet()
        {
            if (startDropSets) return;
            TeamDrops.ClearOptions();
            var Ops_Team = new System.Collections.Generic.List<TMP_Dropdown.OptionData>()
        {
            new (){ text = "<color=#FFFF00>チームA</color>" },
            new (){ text = "<color=#00FF00>チームB</color>" },
            new (){ text = "<color=#00FFFF>チームC</color>" },
        };
            TeamDrops.AddOptions(Ops_Team);

            startDropSets = true;
        }
        Player_State StateMake()
        {
            MSta.CashRem();
            MSta.CharaID = CharaUI.CharaID >= 0 ? CharaUI.CharaID : LPlayerVal.UseChara;
            MSta.LocalSetChara();
            MSta.PStateSet();
            return MSta;
        }
        void MainStrs()
        {
            var Sta = CharaUI.Sta;
            if (!ViewCurrent || Sta == null) Sta = StateMake();
            var MainStr = "<size=150%>=== HP・MP・スタミナ ===</size>";
            MainStr += "\n最大HP : " + ValueStrings(Sta.F_MHP);
            MainStr += "\nHP回復速度 : " + ValueStrings(Sta.ValGet(Enum_StateAddsType.HPRegene)) + "/s";

            MainStr += "\n最大MP : " + ValueStrings(Sta.F_MMP);
            MainStr += "\nMP回復速度 : " + ValueStrings(Sta.ValGet(Enum_StateAddsType.MPRegene)) + "/s";

            MainStr += "\n最大スタミナ : " + ValueStrings(Sta.F_MST);
            MainStr += "\nスタミナ回復速度 : " + ValueStrings(Sta.ValGet(Enum_StateAddsType.STRegene)) + "/s";

            MainStr += "\n<size=150%>=== 攻撃力/防御力 ===</size>";
            MainStr += "\n物理攻撃力 : " + ValueStrings(Sta.F_PAtk);
            MainStr += "\n魔法攻撃力 : " + ValueStrings(Sta.F_MAtk);
            MainStr += "\n物理防御力 : " + ValueStrings(Sta.F_PDef);
            MainStr += "\n魔法防御力 : " + ValueStrings(Sta.F_MDef);
            ChangeText(MainTxs, MainStr);
        }

        void AddStrs()
        {
            var Sta = CharaUI.Sta;
            if (!ViewCurrent || Sta == null) Sta = StateMake();

            var AddStr = "<size=150%>=== 追加ステータス ===</size>";
            AddStr += "\n会心率 : " + Sta.ValGet(Enum_StateAddsType.CritPer).ToString("F1") + "%";
            AddStr += "\n会心ダメージ : " + Sta.ValGet(Enum_StateAddsType.CritMult).ToString("F1") + "%";
            AddStr += "\n攻撃速度 : " + Sta.ValGet(Enum_StateAddsType.AtkSpeed).ToString("F1") + "%";
            AddStr += "\n移動速度 : " + Sta.ValGet(Enum_StateAddsType.MoveSpeed).ToString("F1") + "%";
            AddStr += "\nEX時間チャージ : " + Sta.ValGet(Enum_StateAddsType.EXTimeCharge).ToString("F1") + "%/m";
            AddStr += "\nEXダメージチャージ : " + Sta.ValGet(Enum_StateAddsType.EXDamageCharge).ToString("F1") + "%";
            AddStr += "\nEX命中チャージ : " + Sta.ValGet(Enum_StateAddsType.EXHitCharge).ToString("F1") + "%";
            AddStr += "\n<size=150%>=== その他 ===</size>";
            AddStr += "\n与ダメージ強化 : " + Sta.ValGet(Enum_StateAddsType.AddDamageMult).ToString("F1") + "%";
            AddStr += "\n被ダメージ軽減 : " + Sta.ValGet(Enum_StateAddsType.TakeDamageRegist).ToString("F1") + "%";
            AddStr += "\n与回復強化 : " + Sta.ValGet(Enum_StateAddsType.TakeHealMult).ToString("F1") + "%";
            AddStr += "\n受回復強化 : " + Sta.ValGet(Enum_StateAddsType.AddHealMult).ToString("F1") + "%";

            AddStr += "\n命中率 : " + Sta.ValGet(Enum_StateAddsType.HitPer).ToString("F1") + "%";
            AddStr += "\n回避率 : " + Sta.ValGet(Enum_StateAddsType.DogePer).ToString("F1") + "%";

            AddStr += "\n通常ダメージ強化 : " + Sta.ValGet(Enum_StateAddsType.NormalDamageMult).ToString("F1") + "%";
            AddStr += "\n重撃ダメージ強化 : " + Sta.ValGet(Enum_StateAddsType.HevDamageMult).ToString("F1") + "%";
            AddStr += "\nスキルダメージ強化 : " + Sta.ValGet(Enum_StateAddsType.SkillDamageMult).ToString("F1") + "%";
            AddStr += "\n必殺ダメージ強化 : " + Sta.ValGet(Enum_StateAddsType.EXDamageMult).ToString("F1") + "%";
            AddStr += "\n近距離ダメージ強化 : " + Sta.ValGet(Enum_StateAddsType.ShortDamageMult).ToString("F1") + "%";
            AddStr += "\n中距離ダメージ強化 : " + Sta.ValGet(Enum_StateAddsType.MidleDamageMult).ToString("F1") + "%";
            AddStr += "\n遠距離ダメージ強化 : " + Sta.ValGet(Enum_StateAddsType.LongDamageMult).ToString("F1") + "%";

            AddStr += "\n<size=150%>=== 属性別耐性 ===</size>";
            for (int i = 0; i <= (int)Enum_Element.Dark; i++)
            {
                AddStr += "\n" + EnumToJp((Enum_Element)i) + "属性耐性 : " + Sta.ElementRegistGet((Enum_Element)i).ToString("F1") + "%";
            }
            AddStr += "\n<size=150%>=== 属性別強化 ===</size>";
            for (int i = 0; i <= (int)Enum_Element.Dark; i++)
            {
                AddStr += "\n" + EnumToJp((Enum_Element)i) + "属性強化 : " + Sta.ElementAddGet((Enum_Element)i).ToString("F1") + "%";
            }
            ChangeText(AddTxs,AddStr);

        }

        public void TeamChange()
        {
            var Sta = CharaUI.Sta;
            if (Sta != null) Sta.CommonValues.Team = (Enum_Team)(-TeamDrops.value - 1);
        }

        public void ViewChange()
        {
            ViewCurrent = !ViewCurrent;
        }
        public void LVSetChange()
        {
            if(int.TryParse(LVSetIn.text, out var outLV)) CharaUI.LChara.SetLV = outLV;
        }

    }
}
