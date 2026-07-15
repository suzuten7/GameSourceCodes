using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Suzuten_ActionData;
using static Suzuten_DataBase;
using static Suzuten_SEPlays;
using Photon.Pun;
public class Suzuten_AddShot : MonoBehaviourPun
{
    [SerializeField,Tooltip("弾オブジェクト(Suzuten_ShotObj)")]
    Suzuten_ShotObj SObj;
    [SerializeField,Tooltip("発射弾設定")]
    ShotSC[] Shots;
    
    [System.Serializable]
    class ShotSC
    {
        [Tooltip("アクションデータ")]
        public Suzuten_ActionData ShotsAD;
        [Tooltip("時間条件:x～yの間zの間隔")]
        public Vector3Int Times;
        [Tooltip("キャラ命中時")]
        public bool CharaHits;
        [Tooltip("壁命中時")]
        public bool WallHits;
        [Tooltip("弾破棄時")]
        public bool Deletes;
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        for (int i = 0; i < Shots.Length; i++)
        {
            ShotSC SC = Shots[i];
            if (TimeChecks(SObj.time, SC.Times))
            {
                if (Shots[i].ShotsAD.Shots != null)Shotd(Shots[i].ShotsAD.Shots, SObj.UsePS, transform);  
                if (Shots[i].ShotsAD.SEs != null)SEs(Shots[i].ShotsAD.SEs); 
            }
        }
    }
    public void ShotM(int HitType)
    {

        for (int i = 0; i < Shots.Length; i++)
        {
            ShotSC SC = Shots[i];
            bool Ifs = false;
            switch (HitType)
            {
                case 0: if (SC.CharaHits) Ifs = true; break;
                case 1: if (SC.WallHits) Ifs = true; break;
                case 2: if (SC.Deletes) Ifs = true; break;
            }
            if (Ifs)
            {
                if (Shots[i].ShotsAD.Shots != null) Shotd(Shots[i].ShotsAD.Shots, SObj.UsePS, transform);
                if (Shots[i].ShotsAD.SEs != null) SEs(Shots[i].ShotsAD.SEs);
            }
        }
    }
    void Shotd(ShotsC[] Shotss,Suzuten_PlayerState PS,Transform BaseTr)
    {
        for (int k = 0; k < Shotss.Length; k++)
        {
            var Shot = Shotss[k];
            foreach (var Firing in Shot.Firings)
            {
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
                if (Ifs && Shot.ShotObj)
                {
                    int SCount = Mathf.Max(1, Firing.ShotCount);
                    for (int i = 0; i < SCount; i++)
                    {
                        Vector3 Pos = transform.position;
                        Pos += transform.right * Firing.PosOffSet.x + transform.up * Firing.PosOffSet.y + transform.forward * Firing.PosOffSet.z;
                        Pos.y += Firing.YOffSet;
                        GameObject Obj = PhotonNetwork.Instantiate(Shot.ShotObj.name, Pos, Quaternion.identity);
                        Vector3 Vc = Vector3.zero;
                        switch (Firing.RotBase)
                        {
                            case Shot_RotBaseE.弾_敵方向: Vc = PS.Target.PosGet() - Obj.transform.position; break;
                            case Shot_RotBaseE.使用者_敵方向: Vc = PS.Target.PosGet() - PS.PosGet(); break;
                            case Shot_RotBaseE.固定: Vc = Vector3.zero; break;
                            case Shot_RotBaseE.速度方向: Vc = BaseTr.forward; break;
                        }
                        if (Firing.HorizontalFixed) Vc.y = 0;
                        if (Vc != Vector3.zero) Obj.transform.LookAt(Obj.transform.position + Vc);

                        Vector3 Rot = Obj.transform.eulerAngles;
                        Rot += Firing.RotChange;
                        Rot += new Vector3(Random.Range(-Firing.RotRand.x, Firing.RotRand.x), Random.Range(-Firing.RotRand.y, Firing.RotRand.y), Random.Range(-Firing.RotRand.z, Firing.RotRand.z));
                        Rot += Firing.RotWay * (i - (SCount - 1) / 2f);
                        Obj.transform.eulerAngles = Rot;
                        Rigidbody Rig = Obj.GetComponent<Rigidbody>();
                        if (Rig)
                        {
                            Rig.velocity = Obj.transform.forward * Random.Range(Firing.ShotSpeed.x, Firing.ShotSpeed.y) * 0.02f;
                        }
                        Suzuten_ShotObj InsSObj = Obj.GetComponent<Suzuten_ShotObj>();
                        if (InsSObj)
                        {
                            InsSObj.UsePS = PS;
                            InsSObj.Shots = Shot;
                            InsSObj.ACNames = SObj.ACNames;
                        }
                    }
                }
            }
        }
    }

    void SEs(SEsC[] SECs)
    {
        for (int k = 0; k < SECs.Length; k++)
        {
            SEsC SEC = SECs[k];
            for (int i = 0; i < SEC.SEPlays.Length; i++)
            {
                SEPlays(SEC.SEPlays[i], SObj.UsePS.PID);
            }
        }
    }
}
