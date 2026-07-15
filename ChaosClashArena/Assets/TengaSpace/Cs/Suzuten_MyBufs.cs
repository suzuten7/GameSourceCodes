using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Suzuten_ActionData;
using static Suzuten_DataBase;
using static Suzuten_PlayerSets;
public class Suzuten_MyBufs
{
    static public void BufM(Suzuten_PlayerState PS)
    {
        if (PS.ActionID < 0) return;
        Suzuten_ActionData AD = PS.CD.Actions[ACSetID[PS.PID, PS.ActionID]];
        if (AD.MyBufs == null) return;
        foreach (MyBufsC MBufs in AD.MyBufs)
        {
            if (TimeChecks(PS.ActionTime, MBufs.BufTimes))
            {
                #region 条件分岐
                bool Ifs = true;
                if (MBufs.Ifs != null)
                {
                    for (int i = 0; i < MBufs.Ifs.Length; i++)
                    {
                        if (!IfsCheck(MBufs.Ifs[i], PS))
                        {
                            Ifs = false;
                            break;
                        }
                    }
                }
                #endregion
                #region 実行
                if (Ifs)
                {
                    foreach (var buf in MBufs.Bufs)
                    {
                        PS.BufSets(buf);
                    }
                }
                #endregion
            }
        }

    }
}
