
namespace GmSystem
{
    using System.Collections.Generic;
    using UnityEngine;
    using static GS_SaveValues;
    using static GmSystem.GS_GlobalState;
    using State;
    public class GS_DebugValueView : MonoBehaviour
    {
        [SerializeField] Class_Local_PlayerValues LPVal;
        [SerializeField] List<Class_Local_CharaSet> LCharas;
        [SerializeField] Class_Save_State Save;
        [SerializeField] List<Class_Save_Chara> Save_Charas;
        [SerializeField] List<Class_Save_2DImageBase> Save_2DImages;

        [SerializeField] State_StateBase Sta;
        [TextArea]public string SValstr;
        void Update()
        {
            LPVal = LPlayerVal;
            LCharas = LPlayerCharas;
            Save = GetSave_State;
            Save_Charas = GetSave_Charas;
            Save_2DImages = GetSave_2DImages;
            if (Sta != null)
            {
                var str = Sta.CommonValues.Name;
                str += "\nFHPM:" + Sta.F_MHP;
                str += "\nFHPR:" + Sta.ValGet(Enum_StateAddsType.HPRegene, Enum_StateAddsOption.FinalRate);
                str += "\nFMPM:" + Sta.F_MMP;
                str += "\nFMPR:" + Sta.ValGet(Enum_StateAddsType.MPRegene, Enum_StateAddsOption.FinalRate);
                str += "\nFSTM:" + Sta.F_MST;
                str += "\nFSTR:" + Sta.ValGet(Enum_StateAddsType.STRegene, Enum_StateAddsOption.FinalRate);
                str += "\nFPAtk:" + Sta.F_PAtk;
                str += "\nFMAtk:" + Sta.F_MAtk;
                str += "\nFPDef:" + Sta.F_PDef;
                str += "\nFMDef:" + Sta.F_MDef;

                str += "\n会心率:" + Sta.ValGet(Enum_StateAddsType.CritPer, Enum_StateAddsOption.FinalRate);
                str += "\n会心ダメージ:" + Sta.ValGet(Enum_StateAddsType.CritMult, Enum_StateAddsOption.FinalRate);
                str += "\n攻撃速度:" + Sta.ValGet(Enum_StateAddsType.AtkSpeed, Enum_StateAddsOption.FinalRate);
                str += "\n移動速度:" + Sta.ValGet(Enum_StateAddsType.MoveSpeed, Enum_StateAddsOption.FinalRate);

                str += "\n与ダメージ:" + Sta.ValGet(Enum_StateAddsType.AddDamageMult, Enum_StateAddsOption.FinalRate);
                str += "\n被ダメ軽減:" + Sta.ValGet(Enum_StateAddsType.TakeDamageRegist, Enum_StateAddsOption.FinalRate);
                str += "\n与回復:" + Sta.ValGet(Enum_StateAddsType.TakeHealMult, Enum_StateAddsOption.FinalRate);
                str += "\n受回復:" + Sta.ValGet(Enum_StateAddsType.AddHealMult, Enum_StateAddsOption.FinalRate);

                for (int i = 0; i < (int)Enum_Element.Dark; i++)
                {
                    var ele = (Enum_Element)i;
                    str += "\n" + ele + "強化:" + Sta.ElementAddGet(ele);
                    str += "\n" + ele + "耐性:" + Sta.ElementRegistGet(ele);
                }
                ;


                SValstr = str;
            }
        }
    }
}
