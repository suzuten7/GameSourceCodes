namespace State
{
    using Fusion;
    using State;
    using System;
    using UnityEngine;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalState;
    using static GmSystem.GS_GlobalValues;
    public class State_Shot_Moves : NetworkBehaviour
    {
        [SerializeField] State_Shot_Base Shot;
        [SerializeField] Rigidbody Rig;
        [SerializeField] Class_ShotMoves[] Moves;
        enum Enum_ShotMoveMode
        {
            [InspectorName("速度方向")] VectRot = -1,
            [InspectorName("加速")] Boost = 0,
            [InspectorName("減速")] SlowDown,
            [InspectorName("回転")] Rots = 50,
            [InspectorName("ホーミング")] Homing = 100,
            [InspectorName("水平ホーミング")] Vert_Homing,
            [InspectorName("ターゲット方向")] TargetLook,
            [InspectorName("水平ターゲット方向")] Vert_TargetLook,
            [InspectorName("ターゲット位置")] TargetPos=200,
            [InspectorName("ターゲット向き")] TargetRot,
        }
        enum Enum_HomingTarget
        {
            [InspectorName("近敵")] Near = 0,
            [InspectorName("ターゲット")] Target,
            [InspectorName("ターゲット優先近敵")] TargetNear,
            [InspectorName("自身")] My = 100,
        }
        [System.Serializable]
        class Class_ShotMoves
        {
            public Enum_ShotMoveMode Mode;
            public Vector3 Times;
            public Vector3 Vect;
            public float Per;
            public Enum_HomingTarget HomingTarget;
            public float HomingCT;
            [NonSerialized] public int hct = 0;
            [NonSerialized]public GameObject ctarget = null;
            public Vector3 ctPosGet
            {
                get
                {
                    var sta = ctarget.GetComponent<State_StateBase>();
                    if (sta != null) return sta.PosGet;
                    var shit = ctarget.GetComponent<State_StateHit>();
                    if (shit != null) return shit.PosGet;
                    return ctarget.transform.position;
                }
            }
            public Vector3 ctRotGet
            {
                get
                {
                    var sta = ctarget.GetComponent<State_StateBase>();
                    if (sta != null) return sta.RotGet;
                    return ctarget.transform.eulerAngles;
                }

            }
        }
        void FixedUpdate()
        {
            if (!CanControl(Object)) return;

            var rvect = Rig.linearVelocity;
            for(int i = 0; i < Moves.Length; i++)
            {
                var Mv = Moves[i];
                if (!V3TimeCheck(Shot._times, Mv.Times)) continue;
                switch (Mv.Mode)
                {
                    case Enum_ShotMoveMode.VectRot:
                        transform.LookAt(transform.position + rvect);
                        break;
                    case Enum_ShotMoveMode.Boost:
                        rvect += transform.rotation * Mv.Vect * 0.01f;
                        break;
                    case Enum_ShotMoveMode.SlowDown:
                        rvect *= 1f - Mv.Per * 0.01f;
                        break;
                    case Enum_ShotMoveMode.Homing:
                        HomingSet(Mv);
                        if (Mv.ctarget != null)
                        {
                            var hvect = Mv.ctPosGet - transform.position;
                            rvect = rvect.magnitude * Vector3.Slerp(rvect.normalized, hvect.normalized, Mv.Per * 0.01f);
                        }
                        break;
                    case Enum_ShotMoveMode.Rots:
                        transform.eulerAngles += Mv.Vect;
                        break;
                    case Enum_ShotMoveMode.Vert_Homing:
                        HomingSet(Mv);
                        if (Mv.ctarget != null)
                        {
                            var hvect = Mv.ctPosGet - transform.position;
                            hvect.y = rvect.y;
                            rvect = rvect.magnitude * Vector3.Slerp(rvect.normalized, hvect.normalized, Mv.Per * 0.01f);
                        }
                        break;
                    case Enum_ShotMoveMode.TargetLook:
                        HomingSet(Mv);
                        if (Mv.ctarget != null)
                        {
                            var hvect = Mv.ctPosGet - transform.position;
                            var lrot = Quaternion.LookRotation(hvect).eulerAngles;
                            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles,lrot, Mv.Per * 0.01f);
                        }
                        break;
                    case Enum_ShotMoveMode.Vert_TargetLook:
                        HomingSet(Mv);
                        if (Mv.ctarget != null)
                        {
                            var hvect = Mv.ctPosGet - transform.position;
                            hvect.y = 0;
                            var lrot = Quaternion.LookRotation(hvect).eulerAngles;
                            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, lrot, Mv.Per * 0.01f);
                        }
                        break;
                    case Enum_ShotMoveMode.TargetPos:
                        HomingSet(Mv);
                        if (Mv.ctarget != null)
                        {
                            transform.position = Mv.ctPosGet;
                        }
                        break;
                    case Enum_ShotMoveMode.TargetRot:
                        HomingSet(Mv);
                        if (Mv.ctarget != null)
                        {
                            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, Mv.ctRotGet, Mv.Per * 0.01f);
                        }
                        break;
                }
            }
            Rig.linearVelocity = rvect;
        }
        void HomingSet(Class_ShotMoves Mv)
        {
            Mv.hct--;
            if (Mv.hct > 0) return;
            Mv.hct = Mathf.RoundToInt(Mv.HomingCT * 60);
            Mv.ctarget = null;
            float near = float.MaxValue;
            switch (Mv.HomingTarget)
            {
                case Enum_HomingTarget.Near:
                    foreach (var shit in StHitList)
                    {
                        if (shit == null) continue;
                        if (shit.State == null) continue;
                        var sta = shit.State;
                        if (sta.TeamCheck(Shot.USta.CommonValues.Team) != Enum_TeamCheck.Enemy) continue;
                        var dis = Vector3.Distance(transform.position, shit.transform.position);
                        if (near <= dis) continue;
                        near = dis;
                        Mv.ctarget = shit.gameObject;
                    }
                    break;
                case Enum_HomingTarget.TargetNear:
                    if (Shot.USta.TargetStaGet != null)
                    {
                        Mv.ctarget = Shot.USta.TargetObjGet;
                        break;
                    }
                    foreach (var shit in StHitList)
                    {
                        if (shit == null) continue;
                        if (shit.State == null) continue;
                        var sta = shit.State;
                        if (sta.TeamCheck(Shot.USta.CommonValues.Team) != Enum_TeamCheck.Enemy) continue;
                        var dis = Vector3.Distance(transform.position, shit.transform.position);
                        if (near <= dis) continue;
                        near = dis;
                        Mv.ctarget = shit.gameObject;
                    }
                    break;
                case Enum_HomingTarget.Target:
                    Mv.ctarget = Shot.USta.TargetObjGet;
                    break;
                case Enum_HomingTarget.My:
                    Mv.ctarget = Shot.USta.gameObject;
                    break;
            }
        }
    }
}

