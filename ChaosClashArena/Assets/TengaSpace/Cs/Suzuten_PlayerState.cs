
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Suzuten_DataBase;
using static Suzuten_ActionData;
using static Suzuten_PlayerSets;
using static CameraSettings_Gabu;
using static Suzuten_SEPlays;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using UnityEngine.InputSystem.Haptics;

public class Suzuten_PlayerState : MonoBehaviourPun, IPunObservable
{
    #region 変数
    [Header("参照")]
    public Suzuten_DataBase DB;
    public Suzuten_CharaData CD;
    public PlayerInput PI;
    public Suzuten_PlayerInputs Inputs;
    public Rigidbody RigObj;
    public Camera Cam;
    public Camera[] CamSubs;
    public Transform CPos;
    public Transform StartCPos;
    public Transform BattleCPos;
    public Transform RotsT;
    public Transform CharaArrow;
    public GameObject BPUI;

    public Animator Anim;
    [SerializeField] GameObject GuardObj;

    public bool TargetLook;
    [Header("変数")]
    public int PID;
    public float HP;
    public float MP;
    public float SP;
    public float CHST;
    public float ACST;

    public int Moves;
    public bool Grounds;
    public bool Guard;
    public float RigPow;
    public float PhisPow;
    public int LastDamTime = 0;
    public int ActionTime = 0;
    public int ActionID = -1;
    public int StanTime;
    public int[] ACCTs = new int[5];
    public Suzuten_PlayerState Target;
    [System.NonSerialized] public bool[] Stints = new bool[5];
    [System.NonSerialized] public Dictionary<int, BufsC> Bufs = new Dictionary<int, BufsC>();
    [System.NonSerialized] public string LastDamName = "";
    [System.NonSerialized] public Suzuten_ActionData[] ADs = new Suzuten_ActionData[5];
    Dictionary<int, GameObject> BufEffects = new Dictionary<int, GameObject>();
    GameObject InsTrail = null;

    bool StartSEPlay = false;
    bool DownSEPlay = false;
    bool EndSEPlay = false;
    bool Downs = false;
    bool Mo = false;
    float vibrations;

    public class BufsC
    {
        public int BufTime;
        public int BufSTime;
        public int BufPower;
    }
    #endregion
    private void OnValidate()
    {
        PI.defaultControlScheme = DB.KeyBoardUse ? "<Any>" : "Gamepad";
    }
    private void Start()
    {
        ActionTime = 0;
        ActionID = -1;
        LastDamTime = 999;
        //コントローラー振動
        StartCoroutine(ControllerVibrations());
    }
    void FixedUpdate()
    {
        #region PID設定
        if (PhotonNetwork.OfflineMode) PID = PI.playerIndex;
        else PID = photonView.OwnerActorNr - 1;
        #endregion
        BufEffectSet();
        if (PSsCheck())
        {
            #region 操作
            if (!PhotonNetwork.OfflineMode)
            {
                if (!Mo || PSs[photonView.OwnerActorNr - 1 == 0 ? 1 : 0].PI.actions == PI.actions)
                {
                    Mo = true;
                    PI.enabled = false;
                    PI.enabled = true;
                }
            }
            #endregion
            #region 毎フレーム処理
            if (PhotonNetwork.OfflineMode) Target = PSs[PID == 0 ? 1 : 0];
            else
            {
                Target = PSs[(PhotonNetwork.LocalPlayer.ActorNumber - 1) == 0 ? 1 : 0];
                if (!photonView.IsMine) return;
            }
            //フラグチェック,アニメーションセット
            if (Anim != null)
            {
                Anim.SetBool("NBattleF", !BattleFlag);
                Anim.SetBool("Down", HP <= 0);
                Anim.SetBool("WIN", WinNums == PID);
                Anim.SetBool("End", WinNums >= -1);
            }

            if (!StartSEPlay)
            {
                StartSEPlay = true;
                SEPlays(CD.StartSE, PID);
            }
            if (!DownSEPlay && HP <= 0)
            {
                DownSEPlay = true;
                SEPlays(CD.DownSE, PID);
            }
            if (!EndSEPlay && WinNums >= -1)
            {
                EndSEPlay = true;
                if (WinNums == PID) SEPlays(CD.WINSE, PID);
                else SEPlays(CD.LoseSE, PID);
            }

            if (HP <= 0 && !Downs)
            {
                Downs = true;

                Vector2 DisPos = new Vector2(-100, 100);
                DisPos += PI.playerIndex == 1 ? new Vector2(200, 200) : new Vector2(-200, -200);
                string InfoString = PhotonNetwork.OfflineMode ? "Player" + (PI.playerIndex + 1) : photonView.Owner.NickName;
                InfoString += "は" + LastDamName + "で倒された!!!";
                InfoDisplays(InfoString, DisPos, 45, 9, 3, photonView.OwnerActorNr);
            }
            if (!photonView.IsMine) return;
            if (!BattleFlag || HP <= 0)
            {
                Guard = false;
                return;
            }
            if (Grounds && !Guard&&!Bufs.ContainsKey((int)BufsE.時間停止))
            {
                MP += CD.MPRegene / 60f * (BOP_HMSP[3] * 0.01f);
            }
            if (HP > 0)
            {
                Downs = false;
                if (!Bufs.ContainsKey((int)BufsE.時間停止)) HP += CD.HPRegene / 60f * (BOP_HMSP[1] * 0.01f);

                if (Bufs.TryGetValue((int)BufsE.炎上, out BufsC Buf_Fire))
                {
                    HP -= Buf_Fire.BufPower * 0.1f * (BOP_AtkStan[6] * 0.01f);
                    if (ActionTime > 0) ACST -= Buf_Fire.BufPower * 0.002f * (BOP_AtkStan[6] * 0.01f);
                    else CHST -= Buf_Fire.BufPower * 0.002f * (BOP_AtkStan[6] * 0.01f) * (ActionTime < 0 ? 0.1f : 1f);

                }
                if (Bufs.TryGetValue((int)BufsE.毒, out BufsC Buf_Poison))
                {
                    HP -= Buf_Poison.BufPower * 0.1f * (BOP_AtkStan[6] * 0.01f);
                    MP -= Buf_Poison.BufPower * 0.001f * (BOP_AtkStan[6] * 0.01f);
                }
                if (Bufs.ContainsKey((int)BufsE.過力)) HP -= CD.MHP * 0.0005f * (BOP_AtkStan[6] * 0.01f);
                HP = Mathf.Max(1, HP);
            }
            if (!Bufs.ContainsKey((int)BufsE.時間停止)) SP += CD.SPRegene / 60f * (BOP_HMSP[5] * 0.01f);

            HP = Mathf.Min(HP, CD.MHP * (BOP_HMSP[0] * 0.01f));
            MP = Mathf.Clamp(MP, 0, CD.MMP * (BOP_HMSP[2] * 0.01f));
            SP = Mathf.Clamp(SP, 0, CD.MSP * (BOP_HMSP[4] * 0.01f));
            CHST = Mathf.Clamp(CHST, 0, CD.StanRegist * (BOP_AtkStan[2] * 0.01f));
            if (ActionID >= 0) ACST = Mathf.Min(ACST, CD.Actions[ActionID].StanRegist * (BOP_AtkStan[3] * 0.01f));
            LastDamTime++;
            #endregion
            #region アクション処理
            if (!Bufs.ContainsKey((int)BufsE.時間停止))
            {
                if (ActionTime != 0) ActionTime++;
                for (int i = 0; i < 5; i++)
                {
                    if (!Bufs.ContainsKey((int)BufsE.CT停滞)) ACCTs[i] = Mathf.Max(0, ACCTs[i] - 1);
                    int IDss = ACSetID[PID, i];
                    Suzuten_ActionData AD = IDss >= 0 ? CD.Actions[IDss] : null;
                    if (AD != null)
                    {
                        bool ifs = true;
                        if (ActionID >= 0) ifs = false;
                        if (ActionTime != 0) ifs = false;
                        if (ACCTs[i] > 0) ifs = false;
                        if (!Inputs.ACInput_Enter[i]) ifs = false;
                        if (SP < AD.SPCost) ifs = false;
                        if (ifs)
                        {
                            SP -= AD.SPCost;
                            if (AD.SPAC)
                            {
                                Vector2 InfoPos = new Vector2(-50, 50);
                                InfoPos += PI.playerIndex == 1 ? new Vector2(400, 400) : new Vector2(-400, -400);
                                string InfoString = PhotonNetwork.OfflineMode ? "Player" + (PI.playerIndex + 1) : photonView.Owner.NickName;
                                InfoString += "の必殺\n「" + AD.ACName + "」!!!";
                                InfoDisplays(InfoString, InfoPos, 45, 15, 5, photonView.OwnerActorNr);
                            }
                            ActionTime = 1;
                            ActionID = i;
                            ACST = AD.StanRegist;
                            ACCTs[i] = Mathf.RoundToInt(AD.CT * 60 * (BOP_AtkStan[7] * 0.01f));
                        }
                    }
                }
                Suzuten_PlayerShot.ShotM(this);
                Suzuten_ACMove.MoveM(this);
                Suzuten_MyBufs.BufM(this);
                Suzuten_MyParametors.ParametorM(this);
                Suzuten_SEPlays.SEM(this);
                for (int i = 0; i < Stints.Length; i++)
                {
                    Stints[i] = ActionTime != 0;
                }
                if (ActionID >= 0 && ActionTime > 0)
                {
                    Suzuten_ActionData AD = CD.Actions[ACSetID[PID, ActionID]];
                    foreach (var stint in AD.Stints)
                    {
                        Stints[(int)stint] = false;
                    }
                    if (AD.IfTimeBranch != null)
                    {
                        for (int i = 0; i < AD.IfTimeBranch.Length; i++)
                        {
                            IfTimeBranchC ITB = AD.IfTimeBranch[i];
                            if (ITB.Times.x <= ActionTime && ActionTime <= ITB.Times.y)
                            {
                                bool IfB = true;
                                if (ITB.Ifs != null)
                                {
                                    for (int j = 0; j < ITB.Ifs.Length; j++)
                                    {
                                        if (!IfsCheck(ITB.Ifs[j], this))
                                        {
                                            IfB = false;
                                            break;
                                        }
                                    }
                                }
                                if (IfB)
                                {
                                    ActionTime = ITB.TimeChange;
                                    ACCTs[ActionID] -= Mathf.RoundToInt(ITB.CTChange * 60f * (BOP_AtkStan[7] * 0.01f));
                                }
                            }
                        }
                    }
                    if (ActionTime >= AD.EndTime) ActionTime = 0;
                }
                if (ActionTime == 0)
                {
                    ActionID = -1;
                    CHST += CD.STRegene / 60f * (BOP_AtkStan[4] * 0.01f);
                    CHST = Mathf.Min(CHST, CD.StanRegist * (BOP_AtkStan[2] * 0.01f));

                }
                for (int i = 0; i < 5; i++) Inputs.ACInput_Enter[i] = false;
                if (Anim != null)
                {
                    Anim.SetInteger("ActionTime", ActionTime);
                    Anim.SetInteger("ActionID", ActionID >= 0 ? CD.Actions[ACSetID[PID, ActionID]].AnimID : -1);

                }
            }
            #endregion
            #region バフ処理
            List<int> BufE = Bufs.Keys.ToList();
            for (int i = 0; i < BufE.Count; i++)
            {
                if (Bufs.TryGetValue(BufE[i], out BufsC BufC))
                {
                    BufC.BufTime--;
                    if (BufC.BufTime < 0) BufRem(BufE[i]);
                }
            }
            #endregion
        }
        #region 衝突攻撃
        Vector3 RVc = RigObj.velocity;
        RigPow = new Vector2(RVc.x, RVc.z).magnitude + Mathf.Abs(RVc.y * 2f);
        PhisPow = RigPow * CD.PhisPow * 1f * (BOP_AtkStan[0] * 0.01f);
        if (Bufs.ContainsKey((int)BufsE.過力)) PhisPow *= 1.5f;
        if (PhisPow >= 2500f || RigPow >= 30f)
        {
            if (!InsTrail && CD.SpeedTrail) InsTrail = Instantiate(CD.SpeedTrail, RigObj.transform).gameObject;
        }
        else if (InsTrail)
        {
            InsTrail.transform.parent = null;
            InsTrail = null;
        }
        #endregion
    }
    private void LateUpdate()
    {
        if (!PSsCheck()) return;
        #region カメラ設定
        if (MultiDisplayMode) Cam.targetDisplay = PI.playerIndex;
        else Cam.targetDisplay = 0;
        for (int i = 0; i < CamSubs.Length; i++)
        {
            CamSubs[i].targetDisplay = Cam.targetDisplay;
            CamSubs[i].fieldOfView = Cam.fieldOfView;
            CamSubs[i].rect = Cam.rect;
            CamSubs[i].transform.position = Cam.transform.position;
            CamSubs[i].transform.rotation = Cam.transform.rotation;
        }
        #endregion
        //ガード表示
        GuardObj.SetActive(Guard);
        #region 画面外矢印
        if (Target && BattleFlag && CameraSettings_Gabu.Settings_Bool[PI.playerIndex, 0])
        {
            CharaArrow.gameObject.SetActive(!CamTargetChecks());
            CharaArrow.position = PosGet() + new Vector3(0, 1.5f, 0);
            CharaArrow.LookAt(Target.PosGet());
        }
        else CharaArrow.gameObject.SetActive(false);
        #endregion
        //バトルフラグ
        if (!BattleFlag || HP <= 0) return;
        #region UI非表示
        if (!MultiDisplayMode) BPUI.SetActive(!Settings_Bool[0, 1]);
        else BPUI.SetActive(!Settings_Bool[PI.playerIndex, 1]);
        #endregion
    }
    #region 各メソッド
    /// <summary>座標取得</summary>
    public Vector3 PosGet()
    {
        return RigObj.transform.position + new Vector3(0, CD.YPos, 0);
    }
    /// <summary>ダメージ</summary>
    public void Damage(int Val, float HitStop = 0f, bool Guards = false, string DamName = "???", bool SPnons = false)
    {
        photonView.RPC(nameof(RpcDam), RpcTarget.All, Val, HitStop, Guards, DamName,SPnons);
    }
    [PunRPC]
    void RpcDam(int Val, float HitStop = 0f, bool Guards = false, string DamName = "???", bool SPnons = false)
    {
        if (!photonView.IsMine) return;
        if (Bufs.TryGetValue((int)BufsE.はいバリアー, out BufsC Buf_HiBarria))
        {
            int V = Val;
            Val = Mathf.Max(Val - Buf_HiBarria.BufPower, 0);
            Buf_HiBarria.BufPower -= V;
            if (Buf_HiBarria.BufPower <= 0) Bufs.Remove((int)BufsE.はいバリアー);
            if (Val <= 0)
            {
                SEPlays(CD.DamGuardSE, PID);
                return;
            }
        }
        HP -= Val;
        if(!SPnons)SP += Val * 0.1f * (CD.DamageSPPer * 0.01f) * (BOP_HMSP[7] * 0.01f);
        LastDamTime = 0;
        LastDamName = DamName;
        Cam.DOComplete();
        Cam.DOShakeRotation(HitStop, new Vector3(1f, 2f, 1f));
        vibrations += Val;

        StopTime = Mathf.Max(StopTime, HitStop < 0.02f ? (HitStop + StopTime) : HitStop);

        if (Guards) SEPlays(CD.DamGuardSE, PID);
        else SEPlays(CD.DamageSE, PID);
    }
    /// <summary>スタン</summary>
    public void StanSets(Vector2Int StanPow)
    {
        photonView.RPC(nameof(RpcStan), RpcTarget.All, StanPow.x, StanPow.y);
    }
    [PunRPC]
    void RpcStan(int StanPow, int StanTimed)
    {
        if (!photonView.IsMine) return;
        int StanTimes = Mathf.RoundToInt(StanTimed * (BOP_AtkStan[5] * 0.01f));
        if (ActionTime > 0)
        {
            ACST -= StanPow;
            if (ACST <= 0)
            {
                ActionTime = -StanTimes / 3;
                StanTime = StanTimes / 3;
                SEPlays(CD.StanSE, PID);

                Vector2 InfoPos = new Vector2(-100, 100);
                InfoPos += PI.playerIndex == 1 ? new Vector2(300, 300) : new Vector2(-300, -300);
                string InfoString = PhotonNetwork.OfflineMode ? "Player" + (PI.playerIndex + 1) : photonView.Owner.NickName;
                InfoString += "は\n" + CD.Actions[ACSetID[PID, ActionID]].ACName + "中に" + "\nスタンした!!";
                InfoDisplays(InfoString, InfoPos, 35, 15, 5, photonView.OwnerActorNr);

            }
        }
        else
        {
            CHST -= StanPow * (ActionTime < 0 ? 0.1f : 1f);
            if (CHST <= 0)
            {
                ActionTime = -StanTimes;
                StanTime = StanTimes;
                CHST = CD.StanRegist * (BOP_AtkStan[2] * 0.01f);
                SEPlays(CD.StanSE, PID);

                Vector2 InfoPos = new Vector2(-100, 100);
                InfoPos += PI.playerIndex == 1 ? new Vector2(300, 300) : new Vector2(-300, -300);
                string InfoString = PhotonNetwork.OfflineMode ? "Player" + (PI.playerIndex + 1) : photonView.Owner.NickName;
                InfoString += "は\nスタンした!!";
                InfoDisplays(InfoString, InfoPos, 35, 15, 5, photonView.OwnerActorNr);
            }
        }
    }
    /// <summary>バフ付与</summary>
    public void BufSets(BufAddsC BufAdd)
    {
        photonView.RPC(nameof(RpcBufSets), RpcTarget.All, (int)BufAdd.Buf, (int)BufAdd.BufOP, BufAdd.BufTime, BufAdd.BufPower, BufAdd.BufTimeMax, BufAdd.BufPowerMax);
    }
    [PunRPC]
    void RpcBufSets(int Bufe, int BufOP, int BufTime, int BufPower, int BufMTime, int BufMPower)
    {
        if (!photonView.IsMine) return;
        BufsC Buf;
        if (!Bufs.TryGetValue(Bufe, out Buf))
        {
            if (BufOP != (int)BufOPE.解除)
            {
                Buf = new BufsC
                {
                    BufTime = BufTime,
                    BufSTime = BufTime,
                    BufPower = BufPower,
                };
                Bufs.TryAdd(Bufe, Buf);
            }
        }
        else
        {
            switch (BufOP)
            {
                case (int)BufOPE.付与:
                    Buf.BufTime = Mathf.Max(Buf.BufTime, BufTime);
                    Buf.BufSTime = Mathf.Max(Buf.BufTime, Buf.BufSTime);
                    Buf.BufPower = Mathf.Max(Buf.BufPower, BufPower);
                    break;
                case (int)BufOPE.増加:
                    Buf.BufTime = Mathf.Min(Buf.BufTime + BufTime, BufMTime);
                    Buf.BufSTime = Mathf.Max(Buf.BufTime, Buf.BufSTime);
                    Buf.BufPower = Mathf.Min(Buf.BufPower + BufPower, BufMPower);
                    break;
                case (int)BufOPE.上書き:
                    Buf.BufTime = BufTime;
                    Buf.BufSTime = BufTime;
                    Buf.BufPower = BufPower;
                    break;
                case (int)BufOPE.解除:
                case (int)BufOPE.切り替え:
                    BufRem(Bufe);
                    break;
            }

        }
    }
    /// <summary>バフ消去</summary>
    public void BufRem(int BufE)
    {
        BufsC Buf;
        if (Bufs.TryGetValue(BufE, out Buf))
        {
            Bufs.Remove(BufE);
        }
    }
    /// <summary>バフエフェクト設定</summary>
    public void BufEffectSet()
    {
        var BuKeys = Bufs.Keys.ToArray();
        for(int i = 0; i < BuKeys.Length; i++)
        {
            if (!BufEffects.TryGetValue(BuKeys[i], out GameObject BufEffect))
            {
                BufEffects.Add(BuKeys[i], Instantiate(DB.BufEffectObjs[BuKeys[i]], RigObj.transform));
            }
            else if (BufEffect == null) BufEffects[BuKeys[i]] = Instantiate(DB.BufEffectObjs[BuKeys[i]], RigObj.transform);
        }
        var EfKeys = BufEffects.Keys.ToArray();
        for(int i = 0; i < EfKeys.Length; i++)
        {
            if (!Bufs.ContainsKey(EfKeys[i]))
            {
                GameObject EffectObj = BufEffects[EfKeys[i]];
                if (EffectObj != null)
                {
                    Suzuten_Effects Effect = EffectObj.GetComponent<Suzuten_Effects>();
                    if (Effect) Effect.EffectStop();
                    else Destroy(EffectObj);
                }
                BufEffects.Remove(EfKeys[i]);
            }
        }
    }

    /// <summary>ノックバック</summary>
    public void KBs(Vector3 Vects)
    {
        photonView.RPC(nameof(RpcKBs), RpcTarget.All, Vects);
    }
    [PunRPC]
    void RpcKBs(Vector3 Vects)
    {
        if (photonView.IsMine) RigObj.velocity += Vects;
    }
    /// <summary>アイテムステータス変動</summary>
    public void ItemChanges(int Type, float Val, bool MaxPers)
    {
        photonView.RPC(nameof(RpcItemChanges), RpcTarget.All, Type, Val, MaxPers);
    }
    [PunRPC]
    void RpcItemChanges(int Type, float Val, bool MaxPers)
    {
        if (!photonView.IsMine) return;
        float Vals;
        switch (Type)
        {
            case (int)ParametorE.HP:
                Vals = MaxPers ? CD.MHP * (Val * 0.01f) : Val;
                if (Vals >= 0) HP += Vals;
                else Damage(Mathf.RoundToInt(-Vals), 0.05f, false, "アイテムの効果");
                break;
            case (int)ParametorE.HP_SP無変:
                Vals = MaxPers ? CD.MHP * (Val * 0.01f) : Val;
                if (Vals >= 0) HP += Vals;
                else Damage(Mathf.RoundToInt(-Vals), 0.05f, false, "アイテムの効果",true);
                break;
            case (int)ParametorE.MP: MP += MaxPers ? CD.MMP * (Val * 0.01f) : Val; break;
            case (int)ParametorE.SP: SP += MaxPers ? CD.MSP * (Val * 0.01f) : Val; break;
            case (int)ParametorE.キャラスタン値:
                Vals = MaxPers ? CD.StanRegist * (Val * 0.01f) : Val;
                if (Vals >= 0) CHST += Vals;
                else if (ActionID < 0) StanSets(new Vector2Int(Mathf.RoundToInt(-Vals), 120));
                break;
            case (int)ParametorE.アクションスタン値:
                if (ActionID >= 0)
                {
                    Vals = MaxPers ? CD.Actions[ActionID].StanRegist * (Val * 0.01f) : Val;
                    if (Vals >= 0) CHST += Vals;
                    else StanSets(new Vector2Int(Mathf.RoundToInt(-Vals), 120));
                }
                break;
            case (int)ParametorE.アクション1CT: ACCTs[0] = Mathf.Max(0, ACCTs[0]) + Mathf.RoundToInt(MaxPers ? CD.Actions[ACSetID[PID,0]].CT / 60f * (Val * 0.01f) : Val); break;
            case (int)ParametorE.アクション2CT: ACCTs[1] = Mathf.Max(0, ACCTs[1]) + Mathf.RoundToInt(MaxPers ? CD.Actions[ACSetID[PID, 1]].CT / 60f * (Val * 0.01f) : Val); break;
            case (int)ParametorE.アクション3CT: ACCTs[2] = Mathf.Max(0, ACCTs[2]) + Mathf.RoundToInt(MaxPers ? CD.Actions[ACSetID[PID, 2]].CT / 60f * (Val * 0.01f) : Val); break;
            case (int)ParametorE.アクション4CT: ACCTs[3] = Mathf.Max(0, ACCTs[3]) + Mathf.RoundToInt(MaxPers ? CD.Actions[ACSetID[PID, 3]].CT / 60f * (Val * 0.01f) : Val); break;
            case (int)ParametorE.アクションSPCT: ACCTs[4] = Mathf.Max(0, ACCTs[4]) + Mathf.RoundToInt(MaxPers ? CD.Actions[ACSetID[PID, 4]].CT / 60f * (Val * 0.01f) : Val); break;
        }
    }
    /// <summary>テレポート</summary>
    public void TPs(Vector3 Pos)
    {
        photonView.RPC(nameof(RpcTPs), RpcTarget.All, Pos);
    }
    [PunRPC]
    void RpcTPs(Vector3 Pos)
    {
        RigObj.transform.position = Pos;
    }
    /// <summary>カメラ内判定</summary>
    bool CamTargetChecks()
    {
        if (!Target) return false;
        Vector3 TScreenPos = Cam.WorldToScreenPoint(Target.PosGet());
        if (TScreenPos.x < Cam.pixelRect.xMin) return false;
        if (TScreenPos.x > Cam.pixelRect.xMax) return false;
        if (TScreenPos.y < Cam.pixelRect.yMin) return false;
        if (TScreenPos.y > Cam.pixelRect.yMax) return false;
        return true;

    }

    [PunRPC]
    public void RpcHPSets(float Val)
    {
        HP = Val;
    }


    /// <summary>コントローラ振動Start内で呼び出し</summary>
    private IEnumerator ControllerVibrations()
    {
        //PlayerInputに接続されてるゲームパッド取得
        if (PI.devices.FirstOrDefault(x => x is IDualMotorRumble) is not IDualMotorRumble gamepad)
        {
            Debug.Log("P" + (PI.playerIndex + 1) + "デバイス未接続");
            yield break;
        }
        while (true)
        {
            float high = Settings_Int[PI.playerIndex, 5] * 0.01f;//振動力設定
            float vibrationSeconds = 0.2f;//振動秒数
            //ダメージ振動計算 最大HPに対する割合で変化(35%以上で最大)
            float Damvib = Mathf.Clamp(vibrations / (CD.MHP * BOP_HMSP[0] * 0.01f) / 0.35f, 0f, 1f);

            if (Damvib >= 0.03f)//ダメージ振動が0.03以上なら振動
            {
                float highs = Damvib * high * 2f;
                Debug.Log(vibrations + ":" + highs);
                //ダメージ振動減少
                vibrations *= 0.25f;
                gamepad.SetMotorSpeeds(highs, highs);
                yield return new WaitForSeconds(vibrationSeconds);
            }
            else if (HP >= 0 && ActionTime < 0)//スタン
            {
                gamepad.SetMotorSpeeds(high * 1.4f, high * 1.4f);
                yield return new WaitForSeconds(0.8f * vibrationSeconds);
                gamepad.SetMotorSpeeds(0f,0f);
                yield return new WaitForSeconds(0.6f * vibrationSeconds);
            }
            else if (HP>=0&&HP <= (CD.MHP * BOP_HMSP[0] * 0.01f) * 0.25f)//ピンチ用
            {
                gamepad.SetMotorSpeeds(high * 0.3f, high * 0.3f);
                yield return new WaitForSeconds(0.5f * vibrationSeconds);
                gamepad.SetMotorSpeeds(high * 0.2f, high * 0.2f);
                yield return new WaitForSeconds(2f * vibrationSeconds);
            }
            gamepad.SetMotorSpeeds(0f, 0f);
            yield return null;
        }
    }

    private IEnumerator ResetControllerVibration()
    {
        //PlayerInputに接続されてるゲームパッド取得
        if (PI.devices.FirstOrDefault(x => x is IDualMotorRumble) is not IDualMotorRumble gamepad)
        {
            Debug.Log("P" + (PI.playerIndex + 1) + "デバイス未接続");
            yield break;
        }
        UnityEngine.Debug.Log("reset vibration is ok");
        gamepad.SetMotorSpeeds(0f, 0f);
        yield return new WaitForSeconds(1f);
    }
#endregion
    /// <summary>同期用</summary>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(PID);
            stream.SendNext(HP);
            stream.SendNext(MP);
            stream.SendNext(SP);
            stream.SendNext(CHST);
            stream.SendNext(ACST);
            stream.SendNext(ActionTime);
            stream.SendNext(ActionID);
            stream.SendNext(StanTime);
            stream.SendNext(LastDamTime);
            stream.SendNext(ACCTs);
            stream.SendNext(Guard);

            var BufsIDs = Bufs.Keys.ToArray();
            var BufsTime = new List<int>();
            var BufsSTime = new List<int>();
            var BufsPower = new List<int>();
            for (int i = 0; i < BufsIDs.Length; i++)
            {
                BufsTime.Add(Bufs[BufsIDs[i]].BufTime);
                BufsSTime.Add(Bufs[BufsIDs[i]].BufSTime);
                BufsPower.Add(Bufs[BufsIDs[i]].BufPower);
            }
            stream.SendNext(BufsIDs);
            stream.SendNext(BufsTime.ToArray());
            stream.SendNext(BufsSTime.ToArray());
            stream.SendNext(BufsPower.ToArray());
        }
        else
        {
            PID = (int)stream.ReceiveNext();
            HP = (float)stream.ReceiveNext();
            MP = (float)stream.ReceiveNext();
            SP = (float)stream.ReceiveNext();
            CHST = (float)stream.ReceiveNext();
            ACST = (float)stream.ReceiveNext();
            ActionTime = (int)stream.ReceiveNext();
            ActionID = (int)stream.ReceiveNext();
            StanTime = (int)stream.ReceiveNext();
            LastDamTime = (int)stream.ReceiveNext();
            ACCTs = (int[])stream.ReceiveNext();
            Guard = (bool)stream.ReceiveNext();
            var BufsIDs = (int[])stream.ReceiveNext();
            var BufsTime = (int[])stream.ReceiveNext();
            var BufsSTime = (int[])stream.ReceiveNext();
            var BufsPower = (int[])stream.ReceiveNext();

            Bufs.Clear();
            for (int i = 0; i < BufsIDs.Length; i++)
            {
                Bufs.Add(BufsIDs[i], new BufsC { BufTime = BufsTime[i], BufSTime = BufsSTime[i], BufPower = BufsPower[i] });
            }
        }
    }
}
