using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;
using static GameInfos;
using static DataBase;
public class Player_Pilot_WhalesAction : Player_ActionBase
{
    #region エディタ変数
    [SerializeField] int HitDam = 600;
    [SerializeField] float HitKB = 20;
    [SerializeField] int HitCT = 50;
    [SerializeField] int PlHitScore;
    [SerializeField] AudioSource HitPlAudioObj;
    [SerializeField] int DmHitScore;
    [SerializeField] AudioSource HitDmAudioObj;

    [SerializeField] GameObject ATSerchMaker;
    [SerializeField] GameObject SwallowingObj;

    [SerializeField] GameObject SonicWaveSerchObj;
    [SerializeField] AudioSource SerchAudioObj;

    [SerializeField] GameObject WhirlpoolsObj;
    [SerializeField] GameObject WhaleBotObj;

    [SerializeField] Volume PoorVisibilityVolume;
    [SerializeField] AudioSource VisAudioObj;
    public Transform VisTrans;
    #endregion
    #region 内部変数
    /// <summary>接触CT</summary>
    int CT = 0;
    GameObject SwallowIns;
    #endregion
    private void Start()
    {
        PoorVisibilityVolume.weight = 1f;
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (GInfo.End) return;
        #region CT
        for (int i = 0; i < 4; i++)
        {
            CTs[i]--;
            if (GInfo.Ramps)
            {
                if(PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.極限暴走, out var URampLV) && URampLV > 0) CTs[i]-=2;
                else CTs[i]--;
            }
        }
        if (PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.Debug_ファーストCT,out var FCTLV)&&FCTLV>0)
        {
            for (int i = 0; i < 4; i++)
            {
                if (CTs[i] > ACCTs[i] / 10) CTs[i] = ACCTs[i] / 10;
            }
        }
        #endregion
        #region オートサーチ
        if (PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.オートサーチ,out var AtSerchLV)&&AtSerchLV>0&&!PSta.AddCTs.ContainsKey(AddCTsE.オートサーチ))
        {
            PSta.AddCTSet(AddCTsE.オートサーチ, 60 * 15);
            for (int i=0;i<Planctons.Length;i++)
            {
                if (Planctons[i] == null) continue;
                var Plac = Planctons[i].GetComponent<Player_F_PlanctonAction>();
                if (Plac != null && (Plac.PSta.HP <= 0 || Plac.PSta.GostModes)) continue;
                var InsObj = Instantiate(ATSerchMaker, Planctons[i].transform);
            }
            PhotonNetwork.Instantiate(SerchAudioObj.name, transform.position, Quaternion.identity);
        }
        #endregion
        if (PSta.Spakes <= 0)
        {
            #region 技1-飲み込み
            if (CTs[0] <= 0 && PIns.ACs[0])
            {
                CTs[0] = ACCTs[0];
                if (SwallowIns != null) PhotonNetwork.Destroy(SwallowIns);
                var InsObj = PhotonNetwork.Instantiate(SwallowingObj.name, transform.position, transform.rotation);
                var SwaAtk = InsObj.GetComponent<Atk_Swallowing>();
                if (SwaAtk != null)
                {
                    if (PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.粉砕飲み込み,out var ClEartLV)&&ClEartLV>0)
                    {
                        SwaAtk.Damage *= 4;
                        SwaAtk.KBPow *= 0.3f;
                    }
                    SwaAtk.Use = this;
                }
                SwallowIns = InsObj;
            }
            #endregion
            #region 技2-ウェーブサーチ
            if (CTs[1] <= 0 && PIns.ACs[1])
            {
                CTs[1] = ACCTs[1];
                var InsObj = PhotonNetwork.Instantiate(SonicWaveSerchObj.name, transform.position, transform.rotation);
                PhotonNetwork.Instantiate(SerchAudioObj.name, transform.position, Quaternion.identity);
            }
            #endregion
            #region 技3-渦潮呼び出し
            if (CTs[2] <= 0 && PIns.ACs[2])
            {
                CTs[2] = ACCTs[2];
                var InsObj = PhotonNetwork.Instantiate(WhirlpoolsObj.name, transform.position, transform.rotation);
                var InsRig = InsObj.GetComponent<Rigidbody>();
                if (InsRig != null)
                {
                    InsRig.linearVelocity = transform.forward * 5f;
                }
                var WhiAtk = InsObj.GetComponent<Atk_Swallowing>();
                if (WhiAtk != null)
                {
                    WhiAtk.Use = this;
                }
                if (PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.ホエールBOT,out var WBotLV)&&WBotLV>0)
                {
                    var WBots = FindObjectsByType<WhaleBot>(FindObjectsSortMode.None);
                    if (WBots.Length <= GInfo.FePlanTotal)
                    {
                        var InsWBotObj = PhotonNetwork.Instantiate(WhaleBotObj.name, transform.position, transform.rotation);
                    }
                }
            }
            #endregion
            #region 技4-視界開放
            if (CTs[3] <= 0 && PIns.ACs[3])
            {
                CTs[3] = ACCTs[3];
                int Vald = 600;
                if (PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.視界良好,out var NVisLV)&&NVisLV >0) Vald = (int)(Vald * 1.5f);
                PSta.AddCTSet(AddCTsE.視界開放, Vald);
                PhotonNetwork.Instantiate(VisAudioObj.name, transform.position, Quaternion.identity);
            }
            #endregion
        }
        #region 飲み込み用
        if (SwallowIns != null)
        {
            SwallowIns.transform.position = transform.position;
            SwallowIns.transform.rotation = transform.rotation;
        }
        #endregion
        #region 視界処理
        if (PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.Debug_Uクリア,out var UClearLV)&&UClearLV>0)
        {
            PoorVisibilityVolume.weight = 0;
        }
        else
        {
            float VolWei = PoorVisibilityVolume.weight;
            if (PSta.AddCTs.ContainsKey(AddCTsE.視界開放)) VolWei = Mathf.Clamp01(VolWei - 0.03f);
            else VolWei = Mathf.Clamp01(VolWei + 0.01f);
            PoorVisibilityVolume.weight = VolWei;
        }
        #endregion
        CT--;
    }
    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine) return;
        if (GInfo.End) return;
        if (PSta.Spakes > 0) return;
        #region 接触処理
        if (other.tag == "Fugitive")
        {
            if (CT > 0) return;
            var PMo = other.GetComponent<Player_Move>();
            if (PMo != null)
            {
                #region 逃走者
                if (PMo.PSta.HP <= 0 || PMo.PSta.GostModes) return;
                CT = HitCT;
                int HitDams = HitDam;
                #region パッシブ処理
                switch (PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.強噛, out var PowEatLV) ? PowEatLV : 0)
                {
                    default:break;
                    case 1: HitDams = (int)(HitDams * 1.15f); break;
                    case 2: HitDams = (int)(HitDams * 1.3f); break;
                    case 3: HitDams = (int)(HitDams * 1.5f); break;
                }
                if (GInfo.Ramps)
                {
                    if(PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.極限暴走, out var URampLV)&&URampLV>0) HitDams = (int)(HitDams * 1.6f);
                    else HitDams = (int)(HitDams * 1.3f);
                }
                switch (PMo.PSta.PassL.TryGetValue((int)Fugi_PassE.抵抗, out var OPassPregLV) ? OPassPregLV : 0)
                {
                    default:break;
                    case 1: HitDams = (int)(HitDams * 0.9f); break;
                    case 2: HitDams = (int)(HitDams * 0.8f); break;
                    case 3: HitDams = (int)(HitDams * 0.7f); break;
                    case 4: HitDams = (int)(HitDams * 0.55f); break;
                }
                #endregion
                PSta.SP += 40;
                PMo.PSta.Damage(Mathf.Min(HitDams,PMo.PSta.MHP-1),DameTypeE.接触);
                PMo.PSta.KB(transform.forward * HitKB * 0.01f);
                PSta.Score += PlHitScore;
                PhotonNetwork.Instantiate(HitPlAudioObj.name, transform.position, Quaternion.identity);
                #endregion
            }
            else
            {
                #region ダミー
                var DesRPC = other.GetComponent<DestoryRPCs>();
                if (DesRPC != null && !DesRPC.Dels)
                {
                    CT = HitCT;
                    PSta.SP += 20;
                    PSta.Score++;
                    DesRPC.Destory();
                    Ev_Message_Send("ダミープランクトンを捕食した!!!</color>", MessageE.鬼);
                    PSta.Score += DmHitScore;
                    PhotonNetwork.Instantiate(HitDmAudioObj.name, transform.position, Quaternion.identity);
                }
                #endregion
            }

        }
        #endregion
    }
}
