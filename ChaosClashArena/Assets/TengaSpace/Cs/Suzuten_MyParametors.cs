using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Suzuten_ActionData;
using static Suzuten_DataBase;
using static Suzuten_PlayerSets;
public class Suzuten_MyParametors : MonoBehaviour
{
    static public void ParametorM(Suzuten_PlayerState PS)
    {
        if (PS.ActionID < 0) return;
        Suzuten_ActionData AD = PS.CD.Actions[ACSetID[PS.PID, PS.ActionID]];
        if (AD.MyParametors == null) return;
        foreach (MyParametorsC MParas in AD.MyParametors)
        {
            if (TimeChecks(PS.ActionTime, MParas.ParaTimes))
            {
                bool Ifs = true;
                if (MParas.Ifs != null)
                {
                    for (int i = 0; i < MParas.Ifs.Length; i++)
                    {
                        if (!IfsCheck(MParas.Ifs[i], PS))
                        {
                            Ifs = false;
                            break;
                        }
                    }
                }
                if (Ifs)
                {
                    switch (MParas.Parametor)
                    {
                        case ParametorE.HP:
                            if(MParas.Val>=0)PS.HP += MParas.Val;
                            else PS.Damage(Mathf.RoundToInt(-MParas.Val), 0.05f, false, "アクションの自傷");
                            break;
                        case ParametorE.HP_SP無変:
                            if (MParas.Val >= 0) PS.HP += MParas.Val;
                            else PS.Damage(Mathf.RoundToInt(-MParas.Val), 0.05f, false, "アクションの自傷", true);
                            break;
                        case ParametorE.MP: PS.MP += MParas.Val; break;
                        case ParametorE.SP: PS.SP += MParas.Val; break;
                        case ParametorE.キャラスタン値:
                            if (MParas.Val >= 0) PS.CHST += MParas.Val;
                            else if (PS.ActionID < 0) PS.StanSets(new Vector2Int(Mathf.RoundToInt(-MParas.Val), 120));
                            break;
                        case
                        ParametorE.アクションスタン値:
                            if (MParas.Val >= 0) PS.ACST += MParas.Val;
                            else if (PS.ActionID >= 0) PS.StanSets(new Vector2Int(Mathf.RoundToInt(-MParas.Val), 120));
                            break;
                        case ParametorE.アクション1CT: PS.ACCTs[0] += Mathf.RoundToInt(MParas.Val); break;
                        case ParametorE.アクション2CT: PS.ACCTs[1] += Mathf.RoundToInt(MParas.Val); break;
                        case ParametorE.アクション3CT: PS.ACCTs[2] += Mathf.RoundToInt(MParas.Val); break;
                        case ParametorE.アクション4CT: PS.ACCTs[3] += Mathf.RoundToInt(MParas.Val); break;
                        case ParametorE.アクションSPCT: PS.ACCTs[4] += Mathf.RoundToInt(MParas.Val); break;


                    }
                }
            }
        }

    }
}
