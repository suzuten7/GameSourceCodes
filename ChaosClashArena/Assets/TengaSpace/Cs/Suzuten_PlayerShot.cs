using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Suzuten_ActionData;
using static Suzuten_DataBase;
using static Suzuten_PlayerSets;
using Photon.Pun;
public class Suzuten_PlayerShot
{
    static public void ShotM(Suzuten_PlayerState PS)
    {
        if (PS.ActionID < 0) return;
        Suzuten_ActionData AD = PS.CD.Actions[ACSetID[PS.PID, PS.ActionID]];
        if (AD.Shots == null) return;
        foreach (var Shot in AD.Shots)
        {
            foreach (var Firing in Shot.Firings)
            {
                #region 条件分岐
                if (!TimeChecks(PS.ActionTime, Firing.ShotTimes)) continue;
                bool Ifs = true;
                if (Firing.Ifs != null)
                {
                    for (int i = 0; i < Firing.Ifs.Length; i++)
                    {
                        if (!IfsCheck(Firing.Ifs[i], PS))
                        {
                            Ifs = false;
                            break;
                        }
                    }
                }
                #endregion
                if (Ifs && Shot.ShotObj)
                {
                    int SCount = Mathf.Max(1, Firing.ShotCount);
                    for (int i = 0; i < SCount; i++)
                    {
                        #region 初期位置設定
                        Vector3 Pos = PS.PosGet();
                        Pos += PS.RigObj.transform.right * Firing.PosOffSet.x + PS.RigObj.transform.up * Firing.PosOffSet.y + PS.RigObj.transform.forward * Firing.PosOffSet.z;
                        Pos.y += Firing.YOffSet;
                        #endregion
                        GameObject Obj = PhotonNetwork.Instantiate(Shot.ShotObj.name, Pos, Quaternion.identity);
                        #region 角度設定
                        Vector3 Vc = Vector3.zero;
                        switch (Firing.RotBase)
                        {
                            case Shot_RotBaseE.弾_敵方向: Vc = PS.Target.PosGet() - Obj.transform.position; break;
                            case Shot_RotBaseE.使用者_敵方向: Vc = PS.Target.PosGet() - PS.PosGet(); break;
                            case Shot_RotBaseE.固定: Vc = Vector3.zero; break;
                            case Shot_RotBaseE.速度方向: Vc = PS.RigObj.velocity; break;
                        }
                        if (Firing.HorizontalFixed) Vc.y = 0;
                        if (Vc != Vector3.zero) Obj.transform.LookAt(Obj.transform.position + Vc);

                        Vector3 Rot = Obj.transform.eulerAngles;
                        Rot += Firing.RotChange;
                        Rot += new Vector3(Random.Range(-Firing.RotRand.x, Firing.RotRand.x), Random.Range(-Firing.RotRand.y, Firing.RotRand.y), Random.Range(-Firing.RotRand.z, Firing.RotRand.z));
                        Rot += Firing.RotWay * (i - (SCount - 1) / 2f);
                        Obj.transform.eulerAngles = Rot;
                        #endregion
                        #region 他コンポーネント処理
                        Rigidbody Rig = Obj.GetComponent<Rigidbody>();
                        if (Rig)
                        {
                            Rig.velocity = Obj.transform.forward * Random.Range(Firing.ShotSpeed.x, Firing.ShotSpeed.y) * 0.02f;
                        }
                        Suzuten_ShotObj SObj = Obj.GetComponent<Suzuten_ShotObj>();
                        if (SObj)
                        {
                            SObj.UsePS = PS;
                            SObj.Shots = Shot;
                            SObj.ACNames = AD.ACName;
                        }
                        #endregion
                    }
                }

            }
        }

    }

}
