using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Suzuten_ActionData;
using static Suzuten_DataBase;
using static Suzuten_PlayerSets;
using Photon.Pun;
public class Suzuten_ShotObj : MonoBehaviourPun
{
    [System.NonSerialized]public ShotsC Shots;
    [SerializeField, Tooltip("破棄されるまでの時間")]
    int DelTime = 10;
    [SerializeField, Tooltip("キャラに触れると破棄されるか")]
    bool CharaHitRem = false;
    [SerializeField, Tooltip("壁に触れると破棄されるか")]
    bool WallHit = false;
    [SerializeField, Tooltip("反射耐性")]
    RefRegE RefReg = 0;
    [SerializeField,Tooltip("破棄時に分離されるオブジェクト(エフェクトなど)")]
    GameObject[] SeparationObj;
    [SerializeField, Tooltip("追加弾")]
    Suzuten_AddShot[] AddShots;


    public Suzuten_PlayerState UsePS;
    [System.NonSerialized]public string ACNames;
    [System.NonSerialized]public int time = 0;
    enum RefRegE
    {
        LV0_反射耐性無 = 0,
        LV1_設置反射無視 = 1,
        LV2_状態反射無視 = 2,
        LV3_全反射無視=3,
    }
    int HitCount = 0;
    int HitCT = 0;
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        time++;
        HitCT--;
        if (time >= DelTime)
        {
            Dels();
        }
    }
    private void OnTriggerStay(Collider col)
    {
        if (!photonView.IsMine) return;
        switch (col.tag)
        {
            case "Player":
                if ((HitCT <= 0||HitCT == Shots.Damages.HitCT) && BattleFlag)
                {
                    Suzuten_PlayerMove HitPSM = col.gameObject.GetComponent<Suzuten_PlayerMove>();
                    #region 条件判定
                    if (!HitPSM) break;
                    if (HitPSM.PS.HP < 0) break;
                    if (HitPSM.PS == UsePS && !Shots.OwnerHits) break;
                    #endregion
                    #region 反射
                    if (HitPSM.PS.Bufs.ContainsKey((int)BufsE.弾反射))
                    {
                        if(Refrects(HitPSM.PS, 0,2))return;
                    }
                    #endregion
                    #region ダメージ計算
                    var SDams = Shots.Damages;
                    float Damage = UsePS.CD.Atk * SDams.Damage;

                    float DamMul = (HitPSM.PS.Guard ? 1f - SDams.GuardReg * 0.01f : 1f) * (BOP_AtkStan[1] * 0.01f);

                    int HitCo = Mathf.Min(HitCount, SDams.HitMax);
                    if (SDams.HitsDam > 0) DamMul *= 1 + HitCo * SDams.HitsDam * 0.01f;
                    else DamMul /= (1 + HitCo * -SDams.HitsDam * 0.01f);

                    if (SDams.TimeDam > 0) DamMul *= Mathf.Clamp(SDams.TimeDam * 0.01f * time/60f, SDams.TimeMin * 0.01f, 1f);
                    if (SDams.TimeDam < 0) DamMul *= Mathf.Clamp(1f - (SDams.TimeDam * 0.01f * time / 60f), SDams.TimeMin * 0.01f, 1f);

                    if (UsePS.Bufs.ContainsKey((int)BufsE.過力)) DamMul *= 1.5f;
                    if (UsePS.Bufs.TryGetValue((int)BufsE.パワーアップ, out Suzuten_PlayerState.BufsC Buf_PowerUP))
                    {
                        Damage *= 1f + (Buf_PowerUP.BufPower * 0.01f);
                    }
                    HitPSM.PS.StanSets(new Vector2Int(Mathf.RoundToInt(SDams.StanPow.x * DamMul), SDams.StanPow.y));

                    if (HitPSM.PS.Bufs.TryGetValue((int)BufsE.バリア, out Suzuten_PlayerState.BufsC buf_barria))
                    {
                        HitPSM.PS.BufSets(new BufAddsC
                        {
                            Buf = BufsE.バリア,
                            BufOP = BufOPE.増加,
                            BufTime = 0,
                            BufTimeMax = buf_barria.BufSTime,
                            BufPower = -Mathf.RoundToInt(Damage * DamMul),
                            BufPowerMax = buf_barria.BufPower
                        });
                        if (buf_barria.BufPower <= 0)
                        {
                            HitPSM.PS.BufSets(new BufAddsC { Buf = BufsE.バリア, BufOP = BufOPE.解除, });
                        }

                        Damage *= 0.2f;
                    }
                    if (HitPSM.PS.Bufs.TryGetValue((int)BufsE.反撃, out Suzuten_PlayerState.BufsC buf_counter))
                    {
                        if (buf_counter.BufPower > 0)
                        {
                            float Pows = buf_counter.BufPower;
                            HitPSM.PS.BufSets(new BufAddsC
                            {
                                Buf = BufsE.反撃,
                                BufOP = BufOPE.増加,
                                BufTime = 0,
                                BufTimeMax = buf_counter.BufSTime,
                                BufPower = -Mathf.RoundToInt(Damage * DamMul),
                                BufPowerMax = buf_counter.BufPower
                            });
                            Damage = Mathf.Max(1, Damage - Pows);
                        }
                    }
                    int LDam = Mathf.RoundToInt(Damage * DamMul);
                    #endregion
                    #region 命中対象各処理
                    HitPSM.PS.Damage(LDam, SDams.HitStopTime * DamMul, HitPSM.PS.Guard, ACNames);
                    if (SDams.HPDrain != 0)
                    {
                        int DVal = Mathf.RoundToInt(LDam * SDams.HPDrain * 0.01f);
                        UsePS.HP += DVal;
                    }
                    if (Shots.AddBufs != null)
                    {
                        for (int i = 0; i < Shots.AddBufs.Length; i++) HitPSM.PS.BufSets(Shots.AddBufs[i]);
                    }
                    if (SDams.HitEffect)
                    {
                        GameObject InsHitEffect = PhotonNetwork.Instantiate(SDams.HitEffect.name, col.ClosestPoint(transform.position), Quaternion.identity);
                    }
                    #endregion
                    #region 弾各処理
                    if (AddShots != null)
                    {
                        foreach (var AS in AddShots) AS.ShotM(0);
                    }
                    if (Shots.KBs != null)
                    {
                        for (int k = 0; k < Shots.KBs.Length; k++)
                        {
                            KBsC KB = Shots.KBs[k];
                            Rigidbody ShotRig;
                            Vector3 KBRot = Vector3.one;
                            switch (KB.KBRotBase)
                            {
                                default: KBRot = Vector3.one; break;
                                case KB_RotBaseE.弾向き: KBRot = transform.forward; break;
                                case KB_RotBaseE.弾速度: ShotRig = GetComponent<Rigidbody>(); if (ShotRig != null) KBRot = ShotRig.velocity; break;
                                case KB_RotBaseE.使用者向き: KBRot = UsePS.RigObj.transform.forward; break;
                                case KB_RotBaseE.使用者速度: KBRot = UsePS.RigObj.velocity; break;
                                case KB_RotBaseE.敵弾中心向き: KBRot = HitPSM.transform.position - transform.position; break;
                            }
                            KBRot = KBRot.normalized;
                            Vector3 KBVect = (Quaternion.Euler(0, 90, 0) * (KBRot * KB.KBPow.x)) + (Quaternion.Euler(90, 0, 0) * (KBRot * KB.KBPow.y)) + (Quaternion.Euler(0, 0, 0) * (KBRot * KB.KBPow.z)) + new Vector3(0, KB.KBPowY, 0);
                            KBVect *= DamMul;
                            HitPSM.PS.KBs(KBVect);
                        }
                    }
                    HitCount++;
                    HitCT = SDams.HitCT > 0 ? SDams.HitCT : 999999;
                    if (CharaHitRem)
                    {
                        Dels();
                    }
                    #endregion

                }
                break;
            case "Wall":
                if (WallHit)
                {
                    if (AddShots != null)
                    {
                        foreach (var AS in AddShots) AS.ShotM(1);
                    }
                    Dels();
                }
                break;
        }
    }
    void Dels()
    {
        photonView.RPC(nameof(RpcDel), RpcTarget.All);
    }
    [PunRPC]
    void RpcDel()
    {
        if (SeparationObj != null)
        {
            foreach (var SepObj in SeparationObj)
            {
                SepObj.transform.parent = null;
                var Effect = SepObj.GetComponent<Suzuten_Effects>();
                if (Effect) Effect.EffectStop();
            }
        }
        if (photonView.IsMine)
        {
            if (AddShots != null)
            {
                foreach (var AS in AddShots) AS.ShotM(2);
            }
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public bool Refrects(Suzuten_PlayerState PS,int RefType,int RefLV)
    {
        if (RefLV <= (int)RefReg) return false;
        if (PhotonNetwork.OfflineMode) Refs(PS, RefType);
        else photonView.RPC(nameof(RefRpc), RpcTarget.All, PS.photonView.ViewID, RefType);
        return true;
    }
    [PunRPC]
    void RefRpc(int ViewID, int RefType)
    {
        if (!photonView.IsMine) return;
        foreach(var PSs in FindObjectsOfType<Suzuten_PlayerState>())
        {
            if (PSs.photonView.ViewID == ViewID)
            {
                Refs(PSs, RefType);
                return;
            }
        }
    }
    void Refs(Suzuten_PlayerState PS, int RefType)
    {
        Rigidbody Rig = GetComponent<Rigidbody>();
        if (Rig != null)
        {
            switch (RefType)
            {
                default: Rig.velocity = -Rig.velocity;break;
                case 1:
                    if (UsePS != null) Rig.velocity = Rig.velocity.magnitude * (UsePS.PosGet() - transform.position).normalized;
                    else Rig.velocity = -Rig.velocity;
                    break;
            }
            transform.LookAt(transform.position + Rig.velocity);
        }
        UsePS = PS;
    }

}
