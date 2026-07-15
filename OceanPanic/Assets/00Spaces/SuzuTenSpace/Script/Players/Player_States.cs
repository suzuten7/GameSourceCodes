using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using static DataBase;
using static GlovalValues;
using static GameInfos;
public class Player_States : MonoBehaviourPun,IPunObservable
{
    #region エディタ変数
    public Rigidbody Rig;
    [SerializeField] GameObject SpakesEffects;
    public TypesE Types;

    public int MHP;
    public int HP;
    public int MSP;
    public int SP;

    public bool GostModes = false;
    public float HPRegene = 1;
    public float SPRegene = 1;
    public int SPRegeneT = 120;
    public float SPLowPer=35f;

    public int Damages = 0;
    public int LastUseSPT;
    public bool LowSP;
    public int Spakes;
    public bool WaterCheck;
    public bool Water;

    public int Score;
    public int DeathTime = -1;
    public int DeathCount = 0;

    #endregion
    #region 内部変数
    static public Player_States MyPSta;
    float HPRegs = 0;
    float SPRegs = 0;
    public float ScoreF = 0;

    public Dictionary<DataBase.AddCTsE, int> AddCTs = new Dictionary<DataBase.AddCTsE, int>();
    public Dictionary<int,int> PassL = new Dictionary<int, int>();

    public Macine LastHitMacine=null;
    public int MacineHitTime = 0;
    public int HitMarkerTime = 0;
    #endregion
    private void Start()
    {
        if (!photonView.IsMine) return;
        MyPSta = this;
        #region 初期化
        PassChecks();
        HP = MHP;
        SP = MSP;
        LastUseSPT = 999;
        #endregion
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        LastUseSPT++;
        #region HP処理
        if (HP > 0)
        {
            DeathTime = -1;
            float RegeAddPer = 0;
            if (Types == TypesE.逃げ)
            {
                if (!GInfo.End) ScoreF += 1f / 60f;
                switch (PassL.TryGetValue((int)Fugi_PassE.再生, out var OPassReg) ? OPassReg : 0)
                {
                    default:break;
                    case 1: RegeAddPer += 10f; break;
                    case 2: RegeAddPer += 20f; break;
                    case 3: RegeAddPer += 35f; break;
                    case 4: RegeAddPer += 50f; break;
                }
            }
            HPRegs += HPRegene * 0.02f * (1f + RegeAddPer*0.01f);
            if (HPRegs >= 1)
            {
                HP += (int)HPRegs;
                HPRegs -= (int)HPRegs;
            }
            if (!GInfo.End)
            {
                if (ScoreF >= 1f)
                {
                    ScoreF--;
                    Score++;
                }
            }
        }
        HP = Mathf.Clamp(HP, 0, MHP);
        #endregion
        #region スタミナ処理
        if (SP <= 0)
        {
            SP = 0;
            LowSP = true;
        }
        if (LastUseSPT > SPRegeneT)
        {
            SPRegs += SPRegene * 0.02f;
            if (SPRegs >= 1)
            {
                SP += (int)SPRegs;
                SPRegs -= (int)SPRegs;
            }
        }
        if(SP >= MSP * SPLowPer * 0.01f)LowSP = false;
        SP = Mathf.Clamp(SP, 0, MSP);
        #endregion
        #region ダメージ・ショック
        if (Damages > 30) Damages = (int)(Damages * 0.95f);
        else Damages--;
        Damages = Mathf.Max(Damages, 0);
        Spakes = Mathf.Max(Spakes - 1, 0);
        #endregion
        #region 追加効果CT
        var AddCTKeys = AddCTs.Keys.ToArray();
        for(int i = 0; i < AddCTKeys.Length; i++)
        {
            AddCTs[AddCTKeys[i]]--;
            if (AddCTs[AddCTKeys[i]] <= 0) AddCTs.Remove(AddCTKeys[i]);
        }
        #endregion
        MacineHitTime--;
        if (HitMarkerTime > 0) HitMarkerTime--;
        if (HitMarkerTime < 0) HitMarkerTime++;
    }
    void LateUpdate()
    {
        if (SpakesEffects != null) SpakesEffects.SetActive(Spakes > 0);
    }
    #region メソッド
    /// <summary>ダメージを与える</summary>
    public void Damage(int Val, DameTypeE DType = DameTypeE.他)
    {
        photonView.RPC(nameof(Rpc_Damage), RpcTarget.All, Val,(int)DType);
    }
    [PunRPC]
    void Rpc_Damage(int Val, int DType)
    {
        if (!photonView.IsMine) return;
        if (GostModes) return;
        HP -= Val;
        Damages += Val;
        if (Types == TypesE.逃げ)
        {
            if (PassL.TryGetValue((int)Fugi_PassE.パニック,out var PanicLV)&&PanicLV>0&&DType == (int)DameTypeE.接触)
            {
                AddCTSet(AddCTsE.パニック, 60 * 5);
            }
        }
    }
    /// <summary>ショックを与える</summary>
    public void SpakeAdd(int Val)
    {
        photonView.RPC(nameof(Rpc_SpakeAdd), RpcTarget.All, Val);
    }
    [PunRPC]
    void Rpc_SpakeAdd(int Val)
    {
        if (!photonView.IsMine) return;
        Spakes += Val;
    }
    /// <summary>吹き飛ばしを与える</summary>
    public void KB(Vector3 Vect)
    {
        photonView.RPC(nameof(Rpc_KB), RpcTarget.All, Vect);
    }
    [PunRPC]
    void Rpc_KB(Vector3 Vect)
    {
        if (!photonView.IsMine) return;
        if (GostModes) return;
        if (Rig != null)
        {
            Rig.linearVelocity += Vect;
        }
    }
    void PassChecks()
    {
        switch (Types)
        {
            default: PassL = new Dictionary<int,int>(); break;
            case TypesE.逃げ:
                PassL = Pass_Fugi;
                float HPAddPer = 0;
                switch (PassL.TryGetValue((int)Fugi_PassE.プランクトン増加, out var OPassHPAdd) ? OPassHPAdd : 0)
                {
                    default:break;
                    case 1: HPAddPer += 10f; break;
                    case 2: HPAddPer += 20f; break;
                    case 3: HPAddPer += 30f; break;
                    case 4: HPAddPer += 45f; break;
                    case 5: HPAddPer += 60f; break;
                }
                MHP = Mathf.CeilToInt(MHP * (1f + HPAddPer * 0.01f));
                switch (PassL.TryGetValue((int)Fugi_PassE.持久, out var ZikyuLV) ? ZikyuLV : 0)
                {
                    default:break;
                    case 1: MSP = (int)(MSP * 1.15f); break;
                    case 2: MSP = (int)(MSP * 1.3f); break;
                    case 3: MSP = (int)(MSP * 1.5f); break;
                }
                break;
            case TypesE.鬼操縦者:
                PassL = Pass_OniP;
                break;
            case TypesE.鬼監視者:
                PassL = Pass_OniV;
                break;
        }
    }
    public void HitMarkerSet(int Val)
    {
        photonView.RPC(nameof(Rpc_HitMarkerSet), RpcTarget.All, Val);
    }
    [PunRPC]
    void Rpc_HitMarkerSet(int Val)
    {
        if (!photonView.IsMine) return;
        HitMarkerTime = Mathf.Max(HitMarkerTime, Val);
    }
    /// <summary>追加効果CTを設定</summary>
    public void AddCTSet(AddCTsE AddC,int val)
    {
        if (AddCTs.ContainsKey(AddC)) AddCTs[AddC] = Mathf.Max(AddCTs[AddC], val);
        else AddCTs.Add(AddC, val);
    }
    public void ScoreAdd(int Val,bool NoMessage = false)
    {
        photonView.RPC(nameof(Rpc_ScoreAdd), RpcTarget.All, Val,NoMessage);
    }
    [PunRPC]
    void Rpc_ScoreAdd(int Val,bool NoMessage)
    {
        if (!photonView.IsMine) return;
        if(!NoMessage)GameInfos.GInfo.Messages.TryAdd("スコア+" + Val, 0);
        Score += Val;
    }
    #endregion
    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(HP);
            stream.SendNext(Spakes);

            List<int> PIDs = PassL.Keys.ToList();
            List<int> PLVs = new List<int>();
            for(int i = 0; i < PIDs.Count; i++)
            {
                PLVs.Add(PassL[PIDs[i]]);
            }
            stream.SendNext(PIDs.ToArray());
            stream.SendNext(PLVs.ToArray());

            stream.SendNext(Water);
            stream.SendNext(HitMarkerTime);
            stream.SendNext(Score);
            stream.SendNext(DeathTime);
            stream.SendNext(DeathCount);
            stream.SendNext(GostModes);
        }
        else
        {
            HP = (int)stream.ReceiveNext();
            Spakes = (int)stream.ReceiveNext();

            var Stre_PIDs = (int[])stream.ReceiveNext();
            var Stre_PLVs = (int[])stream.ReceiveNext();
            PassL.Clear();
            for(int i = 0; i < Stre_PIDs.Length; i++)
            {
                PassL.Add(Stre_PIDs[i], Stre_PLVs[i]);
            }

            Water = (bool)stream.ReceiveNext();
            HitMarkerTime =(int)stream.ReceiveNext();
            Score =(int)stream.ReceiveNext();
            DeathTime =(int)stream.ReceiveNext();
            DeathCount =(int)stream.ReceiveNext();
            GostModes = (bool)stream.ReceiveNext();
        }
    }
}
