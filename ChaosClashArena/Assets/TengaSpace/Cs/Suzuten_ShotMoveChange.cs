using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Suzuten_DataBase;
public class Suzuten_ShotMoveChange : MonoBehaviourPun
{
    [SerializeField] Suzuten_ShotObj SObj;
    [SerializeField] Rigidbody Rig;
    [SerializeField] MoveOPC[] MoveOPs;
    Suzuten_PlayerState OwnerPS = null;
    [System.Serializable]
    class MoveOPC
    {
        public Vector3Int MoveTime;
        [Header("動き(_の後はPowerの使用する軸)")]
        public MoveModeE MoveMode;
        [Tooltip("水平化(一部のみ)")]
        public bool HorizontalFixed;
        public Vector3 Power;
        public TargetE Target;
        public int TargetCT;
        [System.NonSerialized]public int TCT = 0;
        [System.NonSerialized] public Suzuten_PlayerState TObj;
    }

    enum MoveModeE
    {
        速度方向向き,
        加速_x,
        速度変化_x,
        Y加速_x,
        Y移動_x,
        対象ホーミング_x,
        対象位置に移動,
        重力落下_x,
        停止,
    }
    enum TargetE
    {
        敵,
        使用者,
        使用者ターゲット,
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (OwnerPS != SObj.UsePS)
        {
            OwnerPS = SObj.UsePS;
            for(int i = 0; i < MoveOPs.Length; i++)
            {
                MoveOPs[i].TCT = 0;
            }
        }
        for(int i = 0; i < MoveOPs.Length; i++)
        {
            MoveOPC MOP = MoveOPs[i];
            if(TimeChecks(SObj.time,MOP.MoveTime))
            {
                Vector3 Vects;
                Vector3 VectBase = Rig.velocity;
                Vector3 PosBase = Rig.transform.position;
                switch (MOP.MoveMode)
                {
                    case MoveModeE.速度方向向き:
                        Vects = Rig.velocity;
                        if (MOP.HorizontalFixed) Vects.y = 0;
                        Rig.transform.LookAt(PosBase + Vects);
                        break;
                    case MoveModeE.加速_x:
                        float Speed = VectBase.magnitude;
                        Speed += MOP.Power.x * 0.01f;
                        VectBase = VectBase.normalized * Speed;
                        break;
                    case MoveModeE.速度変化_x:
                        VectBase = VectBase.normalized * MOP.Power.x * 0.01f;
                        break;
                    case MoveModeE.Y加速_x: VectBase += new Vector3(0, MOP.Power.x*0.01f, 0);break;
                    case MoveModeE.Y移動_x:PosBase += new Vector3(0, MOP.Power.x, 0);break;
                    case MoveModeE.重力落下_x:VectBase += Physics.gravity * MOP.Power.x * 0.01f;break;
                    case MoveModeE.対象ホーミング_x:
                        MOP.TCT--;
                        if (MOP.TCT <= 0)
                        {
                            MOP.TCT = MOP.TargetCT;
                            TargetSet(MOP);
                        }
                        if (MOP.TObj)
                        {
                            Vector3 Vect2 = MOP.TObj.PosGet() - SObj.transform.position;
                            if (MOP.HorizontalFixed) Vect2.y = 0;
                            Vector3 Vect3 = VectBase.normalized * (1f - MOP.Power.x * 0.01f) + Vect2.normalized * (MOP.Power.x * 0.01f);
                            VectBase = Vect3.normalized * Rig.velocity.magnitude;
                        }
                        break;
                    case MoveModeE.対象位置に移動:
                        MOP.TCT--;
                        if (MOP.TCT <= 0)
                        {
                            MOP.TCT = MOP.TargetCT;
                            TargetSet(MOP);
                        }
                        if (MOP.TObj)PosBase = MOP.TObj.PosGet();
                        break;
                    case MoveModeE.停止:
                        VectBase = Vector3.zero;
                        break;
                }
                Rig.velocity = VectBase;
                Rig.transform.position = PosBase;
            }
        }
    }
    void TargetSet(MoveOPC MOP)
    {
        float Dis = 999;
        switch (MOP.Target)
        {
            case TargetE.敵:
                foreach (var PSs in FindObjectsOfType<Suzuten_PlayerState>())
                {
                    if (PSs != SObj.UsePS)
                    {
                        float Diss = Vector3.Distance(transform.position, PSs.PosGet());
                        if (Dis > Diss)
                        {
                            Dis = Diss;
                            MOP.TObj = PSs;
                        }
                    }
                }
                break;
            case TargetE.使用者: MOP.TObj = SObj.UsePS;break;
            case TargetE.使用者ターゲット: MOP.TObj = SObj.UsePS.Target; break;
        }
    }
}
