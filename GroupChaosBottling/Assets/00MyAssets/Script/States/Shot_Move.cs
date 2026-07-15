using UnityEngine;
using static Statics;
using static BattleManager;
using static Manifesto;
using Photon.Pun;
using System.Collections.Generic;

public class Shot_Move : MonoBehaviourPun
{
    [SerializeField] Shot_Obj SObj;
    [SerializeField] Class_Shot_Move[] Moves;

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (SObj.USta == null) return;
        for (int i = 0; i < Moves.Length; i++)
        {
            var Moved = Moves[i];
            Moved.TCT--;
            if (!V3IntTimeCheck(SObj.Times, Moved.Times)) continue;
            var RigVect = SObj.Rig.linearVelocity;
            var RigRot = SObj.Rig.angularVelocity;
            switch (Moved.MoveMode)
            {
                case Enum_MoveMode.重力落下_x:
                    RigVect += Physics.gravity * Moved.Pow.x * 0.01f;
                    break;
                case Enum_MoveMode.速度向き:
                    SObj.transform.LookAt(SObj.transform.position + RigVect);
                    break;
                case Enum_MoveMode.加速_x:
                    RigVect += RigVect.normalized * Moved.Pow.x * 0.01f;
                    break;
                case Enum_MoveMode.速度変化_x:
                    RigVect = RigVect.normalized * Moved.Pow.x * 0.01f;
                    break;
                case Enum_MoveMode.物理ランダム回転_xyz:
                    RigRot = new Vector3(Float_NegRand(Moved.Pow.x), Float_NegRand(Moved.Pow.y), Float_NegRand(Moved.Pow.z));
                    break;
                case Enum_MoveMode.物理指定回転_xyz:
                    RigRot = Moved.Pow;
                    break;
                case Enum_MoveMode.オブジェランダム回転_xyz:
                    SObj.transform.Rotate(new Vector3(Float_NegRand(Moved.Pow.x), Float_NegRand(Moved.Pow.y), Float_NegRand(Moved.Pow.z)));
                    break;
                case Enum_MoveMode.オブジェ指定回転_xyz:
                    SObj.transform.Rotate(Moved.Pow);
                    break;
                case Enum_MoveMode.直線補間ホーミング_x距離_y変値:
                    TargetSet(Moved);
                    if (Moved.Target != null || Moved.TargetHit != null)
                    {
                        var TVect = (Moved.Target!=null ? Moved.Target.PosGet() : Moved.TargetHit.PosGet()) - SObj.transform.position;
                        RigVect = Vector3.Lerp(RigVect.normalized, TVect.normalized, Moved.Pow.y * 0.01f).normalized * RigVect.magnitude;
                    }
                    break;
                case Enum_MoveMode.曲線補間ホーミング_x距離_y変値:
                    TargetSet(Moved);
                    if (Moved.Target != null || Moved.TargetHit != null)
                    {
                        var TVect = (Moved.Target != null ? Moved.Target.PosGet() : Moved.TargetHit.PosGet()) - SObj.transform.position;
                        RigVect = Vector3.Slerp(RigVect.normalized, TVect.normalized, Moved.Pow.y * 0.01f).normalized * RigVect.magnitude;
                    }
                    break;
                case Enum_MoveMode.瞬間移動_x距離:
                    TargetSet(Moved);
                    if (Moved.Target != null || Moved.TargetHit != null)
                    {
                        SObj.Rig.position = Moved.Target != null ? Moved.Target.PosGet() : Moved.TargetHit.PosGet();
                    }
                    break;
                case Enum_MoveMode.向き合わせ_x距離_y変値:
                    TargetSet(Moved);
                    if (Moved.Target != null || Moved.TargetHit != null)
                    {
                        var TRot = Quaternion.Euler(Moved.Target != null ? Moved.Target.RotGet() : Moved.TargetHit.Sta.RotGet());
                        var TVect = TRot * Vector3.forward;
                        SObj.transform.LookAt(SObj.transform.position + Vector3.Slerp(SObj.transform.forward, TVect.normalized, Moved.Pow.y * 0.01f));
                    }
                    break;
            }
            SObj.Rig.linearVelocity = RigVect;
            SObj.Rig.angularVelocity = RigRot;
        }
    }
    void TargetSet(Class_Shot_Move Moved)
    {
        if (Moved.TCT <= 0)
        {
            Moved.Target = null;
            Moved.TargetHit = null;
            Moved.TCT = Moved.TargetCT;
            if(Moved.TargetMode == Enum_TargetMode.ターゲット || Moved.TargetMode == Enum_TargetMode.近敵ターゲット優先)
            {
                    Moved.Target = SObj.USta.Target;
                    Moved.TargetHit = SObj.USta.TargetHit;
            }
            if (Moved.TargetMode == Enum_TargetMode.自身) Moved.Target = SObj.USta;
            if (Moved.Target != null || Moved.TargetHit!=null) return;
            float NearDis = -1;
            var RandomLists = new List<State_Hit>();
            foreach (var THit in BTManager.HitList)
            {
                if (THit == null) continue;
                bool Enemy = false;
                bool Flend = false;
                switch (Moved.TargetMode)
                {
                    case Enum_TargetMode.近敵ターゲット優先:
                    case Enum_TargetMode.近敵:
                    case Enum_TargetMode.ランダム敵:
                        Enemy = true;
                        break;
                    case Enum_TargetMode.味方:
                    case Enum_TargetMode.ランダム味方:
                        Flend = true;
                        break;
                }
                if (!TeamCheck(SObj.USta, THit.Sta, Enemy, Flend)) continue;
                if (THit.Sta.HP <= 0) continue;
                var Dis = Vector3.Distance(SObj.USta.PosGet(), THit.PosGet());
                if (Dis > Moved.Pow.x) continue;
                if (Moved.TargetMode == Enum_TargetMode.ランダム敵 || Moved.TargetMode == Enum_TargetMode.ランダム味方)
                {
                    RandomLists.Add(THit);
                }
                else if (NearDis <= 0 || NearDis > Dis)
                {
                    NearDis = Dis;
                    Moved.TargetHit = THit;
                }
            }
            if (RandomLists.Count > 0) Moved.TargetHit = RandomLists[Random.Range(0, RandomLists.Count)];

        }
    }
}
