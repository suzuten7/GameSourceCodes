using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Suzuten_ActionData;
using static Suzuten_DataBase;
using static Suzuten_PlayerSets;
public class Suzuten_ACMove
{
    static public void MoveM(Suzuten_PlayerState PS)
    {
        if (PS.ActionID < 0) return;
        Suzuten_ActionData AD = PS.CD.Actions[ACSetID[PS.PID, PS.ActionID]];
        if (AD.Moves == null) return;
        foreach (MovesC Move in AD.Moves)
        {
            if (TimeChecks(PS.ActionTime, Move.MoveTimes))
            {
                bool Ifs = true;
                if (Move.Ifs != null)
                {
                    for (int i = 0; i < Move.Ifs.Length; i++)
                    {
                        if (!IfsCheck(Move.Ifs[i], PS))
                        {
                            Ifs = false;
                            break;
                        }
                    }
                }
                if (Ifs)
                {
                    Vector3 Vc = Vector3.one;
                    switch (Move.RotBase)
                    {
                        case Move_RotBaseE.使用者_敵方向: Vc = PS.Target.PosGet() - PS.PosGet(); break;
                        case Move_RotBaseE.固定: Vc = Vector3.one; break;
                        case Move_RotBaseE.速度方向: Vc = PS.RigObj.velocity; break;
                    }
                    if (Move.HorizontalFixed) Vc.y = 0;
                    Vc = Vc.normalized;

                    Vector3 Vects = (Quaternion.Euler(0, 90, 0) * (Vc * Move.Pows.x)) + (Quaternion.Euler(90, 0, 0) * (Vc * Move.Pows.y)) + (Quaternion.Euler(0, 0, 0) * (Vc * Move.Pows.z)) + new Vector3(0, Move.YPower, 0);
                    PS.RigObj.velocity += Vects;
                }
            }
        }

    }
}
