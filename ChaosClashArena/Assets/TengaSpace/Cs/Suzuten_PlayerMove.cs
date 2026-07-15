using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Suzuten_DataBase;
using static Suzuten_PlayerState;
using static Suzuten_PlayerSets;
using static Suzuten_SEPlays;
using Photon.Pun;
public class Suzuten_PlayerMove : MonoBehaviourPunCallbacks
{
    #region 変数
    public Suzuten_PlayerState PS;
    int JumpCT=0;
    int BoostCT = 0;
    int PhisAtkCT = 0;
    Vector3 BaseScale;
    float Scaled;
    bool Ground = false;
    int GuardTime = 0;
    int GRefCT = 0;
    int GRefs = 0;
    #endregion

    void Start()
    {
        BaseScale = transform.localScale;
        Scaled = 1f;
    }
    private void LateUpdate()
    {
        if (!photonView.IsMine) return;
        #region スタート前
        Vector3 PosVect = PS.Target !=null ? PS.Target.PosGet() - PS.PosGet():Vector3.one;
        Vector3 Pos_S = PS.PosGet() + PosVect * 5f;
        PS.StartCPos.position = Pos_S;
        #endregion
        #region バトル中カメラ基準
        if (!PS.Inputs.Look_Stay&&PS.Target)
        {
            Vector3 CamPos = PS.BattleCPos.position;
            Vector3 PosVect1 = PS.PosGet() - CamPos;
            Vector3 PosVect2 = PS.Target.PosGet() - PS.PosGet();
            float RotSpeed = Mathf.Clamp01(PS.DB.RotSpeed * (CameraSettings_Gabu.Settings_Int[PS.PI.playerIndex,0]*0.01f) );
            Vector3 PosVect3 = (PosVect1.normalized * (1f - RotSpeed)) + PosVect2.normalized * RotSpeed;
            Vector3 Pos_B = PS.PosGet() - PosVect3.normalized * 20f;
            PS.BattleCPos.position = Pos_B;

        }
        else
        {
            Vector3 CamPos = PS.BattleCPos.position;
            Vector3 PosVect1 = (PS.PosGet() - CamPos).normalized;
            PS.RotsT.LookAt(PS.RotsT.position + PosVect1);

            Vector2 RotIn = PS.Inputs.LookInput * (CameraSettings_Gabu.Settings_Int[PS.PI.playerIndex,1]*0.01f);
            Vector3 EluerRot = PS.RotsT.eulerAngles;
            EluerRot += new Vector3(-RotIn.y, RotIn.x,0);
            EluerRot.x = Mathf.Clamp(Mathf.Repeat(EluerRot.x+180f,360f)-180f, -85f, 85f);
            PS.RotsT.eulerAngles = EluerRot;
            Vector3 PosVect3 = PS.RotsT.forward;
            Vector3 Pos_B = PS.PosGet() - PosVect3.normalized * 20f;
            PS.BattleCPos.position = Pos_B;
        }
        #endregion
        #region カメラ位置変更
        if (StartTime > 120 || WinNums >= -1)
        {
            Vector3 Pos = PS.StartCPos.position;
            Vector3 Pos2 = new Vector3(Pos.x, Mathf.Max(Pos.y + 0.75f, PS.PosGet().y), Pos.z);
            Vector3 Pos3 = Pos2 - PS.PosGet();
            Vector3 Pos4 = PS.PosGet() + Pos3.normalized * 5f;
            Pos4.y += 1f;
            PS.CPos.transform.position = Pos4;
            PS.CPos.transform.LookAt(PS.PosGet());
            Vector3 CRot = PS.CPos.transform.eulerAngles;
            CRot.x += 3;
            if (StartTime > 0) CRot.y += ((StartTime - 120) * 0.1f - 10f) * (PS.PI.playerIndex % 2 == 0 ? 1f : -1f);
            PS.CPos.transform.eulerAngles = CRot;
        }
        else
        {

            Vector3 Pos = PS.BattleCPos.position;
            Vector3 Pos3 = Pos - PS.PosGet();
            PS.CPos.transform.position = PS.PosGet() + Pos3.normalized * Mathf.Max(0.1f, 20f * (CameraSettings_Gabu.Settings_Int[PS.PI.playerIndex, 2] * 0.01f));
            PS.CPos.transform.LookAt(PS.PosGet());
            PS.CPos.transform.position += -PS.CPos.right * 4f* (CameraSettings_Gabu.Settings_Int[PS.PI.playerIndex, 3] * 0.01f) * (CameraSettings_Gabu.Settings_Int[PS.PI.playerIndex, 2] * 0.01f);
            PS.CPos.transform.position += PS.CPos.up * 1f * (CameraSettings_Gabu.Settings_Int[PS.PI.playerIndex, 4] * 0.01f) * (CameraSettings_Gabu.Settings_Int[PS.PI.playerIndex, 2] * 0.01f);
            Vector3 CRot = PS.CPos.transform.eulerAngles;
            PS.CPos.transform.eulerAngles = CRot;

        }
        #endregion
        #region キャラターゲット向き
        if (PS.TargetLook && PS.Target)
        {
            transform.LookAt(PS.Target.PosGet());
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        #endregion
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        #region ジャンプCT重力
        if (JumpCT > 0) JumpCT--;
        else
        {
            float Grav = 1;
            if (PS.Bufs.ContainsKey((int)BufsE.ハイフライ)) Grav *= 0.15f;
            PS.RigObj.velocity += Physics.gravity * (PS.CD.GravPower * 0.01f) * 0.02f*Grav;
        }
        if (PS.Bufs.ContainsKey((int)BufsE.重力不定))
        {
            Vector3 GVects = PS.RigObj.velocity;
            GVects.y = (BattleTime + Random.Range(-6, 7)) % 30 < 15 ? -6.2f : 3.1f;
            PS.RigObj.velocity = GVects;
        }
        #endregion
        //フラグチェック
        if (!BattleFlag || PS.HP <= 0) return;
        if (PS.Bufs.ContainsKey((int)BufsE.時間停止))
        {
            PS.RigObj.velocity = Vector3.zero;
            return;
        }

        #region 毎フレーム処理
        PS.Moves = 0;
        BoostCT--;
        PhisAtkCT--;
        #endregion
        #region サイズ変化
        float Scale = 1;
        if (PS.Bufs.TryGetValue((int)BufsE.巨大化, out BufsC Buf_Bigs)) Scale *= (Buf_Bigs.BufPower * 0.01f)+1f;
        if (Scaled < Scale) Scaled = Mathf.Min(Scaled + 0.01f, Scale);
        if (Scaled > Scale) Scaled = Mathf.Max(Scaled - 0.01f, Scale);
        transform.localScale = BaseScale * Scaled;
        #endregion
        #region 速度バフ
        float SpeedMult = 1;
        if (PS.Bufs.ContainsKey((int)BufsE.ハイパーUnityちゃん)) SpeedMult *= 3;
        if (PS.Bufs.TryGetValue((int)BufsE.スピードアップ, out BufsC Buf_Speed)) SpeedMult *= (Buf_Speed.BufPower * 0.01f) + 1;
        if (PS.Bufs.TryGetValue((int)BufsE.冷気, out BufsC Buf_Snows)) SpeedMult /= (Buf_Snows.BufPower *0.02f) + 1;
        #endregion
        #region 落下
        Vector3 RVect = PS.RigObj.velocity;
        if (!PS.Stints[3] && PS.Inputs.Down_Stay)
        {
            RVect.y -= PS.CD.DownPower * 0.02f;
            if (!PS.Grounds)
            {
                PS.Moves = Mathf.Max(PS.Moves, 4);
                SEPlays(PS.CD.FallSE, PS.PID);
                if (PS.MP >= PS.DB.FallHomSPCost)
                {
                    Vector3 HoVects = (PS.Target.RigObj.position - PS.PosGet());
                    HoVects.y = 0;
                    if (HoVects.magnitude <= PS.CD.PhisRange * 3f)
                    {
                        PS.Moves = Mathf.Max(PS.Moves, 5);
                        if (HoVects.magnitude >= PS.CD.PhisRange * 0.75f)
                        {

                            PS.MP -= PS.DB.FallHomSPCost;
                            RVect += HoVects.normalized * PS.CD.DownPower * 0.05f * (BOP_Moves[4] * 0.01f);
                        }
                    }
                }
            }
        }
        PS.RigObj.velocity = new Vector3(RVect.x * 0.9f, RVect.y, RVect.z * 0.9f);
        #endregion
        #region 移動とガード
        Vector2 MInput = PS.Inputs.MoveInput;
        PS.Guard = false;
        GRefCT--;
        GRefs--;
        #region ガード
        if (!PS.Stints[4] && MInput.magnitude <= 0.3f && PS.Inputs.Boost_Stay)
        {
            GuardTime++;
            PS.Guard = true;
            PS.Moves = Mathf.Max(PS.Moves, 3);
            SEPlays(PS.CD.GuardSE, PS.PID);
            if (GuardTime<=2&&(GRefCT <= 0||(PS.MP >= PS.DB.GuardRefCost&& GRefs <= 0)))
            {
                if (GRefCT <= 0) GRefCT = 120;
                else
                {
                    GRefs = 30;
                    PS.MP -= PS.DB.GuardRefCost;
                }
                PS.BufSets(new BufAddsC
                {
                    Buf = BufsE.弾反射,
                    BufOP = BufOPE.付与,
                    BufTime = 15,
                    BufPower = 0,
                });
            }
        }
        else GuardTime = 0;
        #endregion
        #region 移動
        if (!PS.Stints[0] && MInput.magnitude > 0.3f)
        {
            if (PS.Bufs.ContainsKey((int)BufsE.混乱)) MInput *= -1;
            Vector3 MVects = PS.CPos.transform.forward * MInput.y + PS.CPos.transform.right * MInput.x;
            MVects.y = 0;
            Vector3 MoveVect = MVects.normalized * 0.02f * (PS.Guard ? 0.3f : 1f)
                * (PS.Grounds ? PS.CD.GroundSpeed * (BOP_Moves[0] * 0.01f) : PS.CD.AirSpeed * (BOP_Moves[1] * 0.01f));
            MoveVect *= SpeedMult;
            PS.RigObj.velocity += MoveVect;
            PS.Moves = Mathf.Max(PS.Moves, 1);
            //ブースト
            if (!PS.Stints[2] && PS.Inputs.Boost_Stay)
            {
                Vector3 BoostVect = Vector3.zero;
                if (PS.Grounds && PS.MP >= PS.DB.BoostSPCost)
                {
                    PS.MP -= PS.DB.BoostSPCost;
                    BoostVect = MVects.normalized * PS.CD.BoostSpeed * 0.02f * (BOP_Moves[2] * 0.01f);
                }
                if (!PS.Grounds && PS.MP >= PS.DB.BoostSPCost * 0.5f)
                {
                    PS.MP -= PS.DB.BoostSPCost * 0.5f;
                    BoostVect = MVects.normalized * PS.CD.BoostSpeed * 0.01f * (BOP_Moves[2] * 0.01f);
                }
                BoostVect *= SpeedMult;
                PS.RigObj.velocity += BoostVect;
            }
        }
        #endregion
        #endregion
        #region #地面判定
        PS.Grounds = Ground;
        Ground = false;
        if (!PS.Grounds) PS.Moves = Mathf.Max(PS.Moves, 2);
        #endregion
        if (PS.Anim!=null) PS.Anim.SetInteger("Moves", PS.Moves);
    }
    private void Update()
    {
        if (!photonView.IsMine) return;
        //フラグチェック
        if (!BattleFlag || PS.HP <= 0) return;
        if (PS.Bufs.ContainsKey((int)BufsE.時間停止)) return;
        Vector3 RVect = PS.RigObj.velocity;
        #region 速度バフ
        float SpeedMult = 1;
        if (PS.Bufs.ContainsKey((int)BufsE.ハイパーUnityちゃん)) SpeedMult *= 3;
        if (PS.Bufs.TryGetValue((int)BufsE.スピードアップ, out BufsC Buf_Speed)) SpeedMult *= (Buf_Speed.BufPower * 0.01f) + 1;
        if (PS.Bufs.TryGetValue((int)BufsE.冷気, out BufsC Buf_Snows)) SpeedMult /= (Buf_Snows.BufPower * 0.02f) + 1;
        #endregion
        #region ジャンプ


        if (JumpCT <= 0 && !PS.Stints[1] && PS.Inputs.Jump_Enter && (PS.Grounds || PS.MP >= PS.DB.JDSPCost))
        {
            JumpCT = 10;
            if (!PS.Grounds) PS.MP -= PS.DB.JDSPCost;
            float JPow = PS.CD.JumpPower * (BOP_Moves[3] * 0.01f) * (PS.Grounds ? 0.2f : 0.1f);
            RVect.y = Mathf.Max(JPow, RVect.y + JPow);
            SEPlays(PS.CD.JumpSE, PS.PID);
        }
        #endregion
        #region ダッシュ
        Vector2 MInput = PS.Inputs.MoveInput;
        if (PS.Bufs.ContainsKey((int)BufsE.混乱)) MInput *= -1;
        Vector3 MVects = PS.CPos.transform.forward * MInput.y + PS.CPos.transform.right * MInput.x;
        MVects.y = 0;

        if (BoostCT <= 0 && !PS.Stints[2] && MVects.magnitude > 0.1f)
        {
            if (PS.Inputs.Boost_Enter && PS.MP >= PS.DB.JDSPCost)
            {
                BoostCT = 10;
                PS.MP -= PS.DB.JDSPCost;
                Vector3 BoostVect = MVects.normalized * PS.CD.BoostSpeed * 0.35f * (BOP_Moves[2] * 0.01f);
                BoostVect *= SpeedMult;
                RVect += BoostVect;
                SEPlays(PS.CD.BoostSE, PS.PID);
            }
        }
        #endregion
        PS.RigObj.velocity = RVect;
    }
    private void OnCollisionEnter(Collision col)
    {
        if (!photonView.IsMine) return;
        #region 衝突攻撃
        PS.Grounds = true;
        Ground = true;
        if ((PS.PhisPow >= 2500f || PS.RigPow >= 30f) && PhisAtkCT <= 0 && BattleFlag)
        {
            PhisAtkCT = 15;
            foreach (var hit in Physics.OverlapSphere(transform.position, PS.CD.PhisRange, PS.DB.PlayerLayer))
            {
                float PhisDamage = PS.PhisPow;
                if (PS.Bufs.TryGetValue((int)BufsE.パワーアップ, out BufsC Buf_PowerUP))
                {
                    PhisDamage *= 1f + (Buf_PowerUP.BufPower * 0.01f);
                }
                Suzuten_PlayerMove HitPMv = hit.GetComponent<Suzuten_PlayerMove>();
                if (HitPMv && HitPMv.PS != PS&&HitPMv.PS.HP>0)
                {
                    float Dis = Vector3.Distance(transform.position, hit.transform.position) / PS.CD.PhisRange*100f;
                    float DamPer = Mathf.Clamp((100f - Dis) + 50f,50f,100f);
                    int PhisLDam = Mathf.RoundToInt(PhisDamage*(DamPer*0.01f));
                    //Debug.Log("衝突基礎"+PhisDamage +",距離:"+Dis.ToString("F1")+"%"+DamPer+"ダメージ:" + PhisLDam);
                    HitPMv.PS.Damage(PhisLDam, 0.3f,false,"衝突攻撃");
                    HitPMv.PS.StanSets(new Vector2Int(Mathf.RoundToInt(PS.PhisPow * 0.02f), PS.CD.PhisStanTime));
                    if (PS.CD.PhisHitEffect)
                    {
                        GameObject InsHitEffect = PhotonNetwork.Instantiate(PS.CD.PhisHitEffect.name, hit.ClosestPoint(transform.position), Quaternion.identity);
                    }

                }
            }
            if (PS.CD.PhisAtkEffect)
            {
                GameObject InsAtkEffect = PhotonNetwork.Instantiate(PS.CD.PhisAtkEffect.name, transform.position, Quaternion.identity);
            }
            SEPlays(PS.CD.PhisAtkSE, PS.PID);
        }
        #endregion
    }
    private void OnCollisionStay(Collision col)
    {
        if (!photonView.IsMine) return;
        #region #地面判定
        Ground = true;
        #endregion
    }
}
