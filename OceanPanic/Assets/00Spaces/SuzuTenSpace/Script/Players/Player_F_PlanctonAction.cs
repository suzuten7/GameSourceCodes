using Photon.Pun;
using UnityEngine;
using static GameInfos;
using static DataBase;
using UnityEngine.SocialPlatforms.Impl;

public class Player_F_PlanctonAction : Player_ActionBase
{
    #region エディタ変数
    [SerializeField] ParticleSystem PlanctonEffects;
    [SerializeField] int[] Costs;

    [SerializeField] GameObject SepPlancton;
    [SerializeField] float SepShotMovePow;
    [SerializeField] float SepShotSpeed;
    [SerializeField] AudioSource SepAudioObj;

    [SerializeField] GameObject DmPlancton;
    [SerializeField] AudioSource DmAudioObj;

    [SerializeField] AudioSource AddAudioObj;

    [SerializeField] GameObject PosPlancMarker;
    [SerializeField] AudioSource PosAudioObj;

    [SerializeField] AudioSource DeathAudioObj;
    [SerializeField] GameObject PlanctonFinal;
    [SerializeField] GameObject NoWaterMarkerObj;
    [SerializeField] Gradient LifeColors;
    [SerializeField] Gradient DeathColors;

    [SerializeField] int DeathAddScore;

    [SerializeField] float PilotScoreDisMin;
    [SerializeField] float PilotScoreDisPer;

    GameObject PosPlancIns;
    #endregion
    #region 内部変数
    bool DeathMessages =false;
    #endregion
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        var RoomHash = PhotonNetwork.CurrentRoom.CustomProperties;
        if ((PSta.HP <= 0 || PSta.GostModes) && PSta.DeathTime < 0) PSta.DeathTime = GInfo.ToGameTime;
        if (GInfo.End) return;
        #region 死亡処理
        if (PSta.HP <= 0 && !DeathMessages)
        {
            DeathMessages = true;
            Ev_Message_Send("<color=#FF0000>" + PhotonNetwork.NickName + "が捕食された!!!</color>", MessageE.逃走者);
            Ev_Message_Send("<color=#0000FF>" + PhotonNetwork.NickName + "を捕食した!!!</color>", MessageE.鬼);
            for (int i = 0; i < PilotAcs.Length; i++)
            {
                if (PilotAcs[i] == null) continue;
                PilotAcs[i].PSta.ScoreAdd(DeathAddScore);
            }
            for (int i = 0; i < VisitAcs.Length; i++)
            {
                if (VisitAcs[i] == null) continue;
                VisitAcs[i].PSta.ScoreAdd(DeathAddScore/2);
            }
            PSta.Score = Mathf.RoundToInt(PSta.Score * 0.8f);
            GInfo.Messages.TryAdd("スコア-20%",0);
            PhotonNetwork.Instantiate(DeathAudioObj.name, transform.position, Quaternion.identity);
            int DeathAddTime = 20;
            bool Revs = false;
            if (PSta.PassL.TryGetValue((int)Fugi_PassE.復活, out var RevLV) && RevLV > 0) Revs = true;
            if (PSta.PassL.TryGetValue((int)Fugi_PassE.Debug_不死,out var UDeLV)&&UDeLV>0) Revs = true;
            if (PSta.AddCTs.ContainsKey(AddCTsE.復活)) Revs = false;
            if (RoomHash.TryGetValue("GameOption2", out var Op2Val) && (bool)Op2Val) Revs = true;
            if (Revs)
            {
                PSta.AddCTSet(AddCTsE.復活, 60 * 10000);
                PSta.AddCTSet(AddCTsE.復活待ち, 60 * 10000);
                PSta.GostModes = true;
                if (PSta.AddCTs.ContainsKey(AddCTsE.復活))
                {
                    DeathAdds();
                    PSta.DeathCount++;
                }
                else DeathAddTime += 20;
            }
            else
            {
                DeathAdds();
                PSta.DeathCount++;
            }
            if (PSta.PassL.TryGetValue((int)Fugi_PassE.プランクトンファイナル, out var PFLV) && PFLV > 0&& !PSta.AddCTs.ContainsKey(AddCTsE.プランクトンファイナル))
            {
                PSta.AddCTSet(AddCTsE.プランクトンファイナル, 60 * 10000);
                var PFIns = PhotonNetwork.Instantiate(PlanctonFinal.name, transform.position, Quaternion.identity);
                var PFAtk = PFIns.GetComponent<Atk_PlanctonFinal>();
                if (PFAtk != null) PFAtk.Use = this;
            }
            TimeAdds(DeathAddTime);
        }
        #endregion
        #region 復活
        if ((PSta.HP<=0||PSta.GostModes)&&PSta.AddCTs.TryGetValue(AddCTsE.復活待ち,out var RevT)&&RevT <= 60 * 9990)
        {
            DeathMessages = false;
            PSta.GostModes = false;
            PSta.HP = 1;
            PSta.AddCTs.Remove(AddCTsE.復活待ち);
            Ev_Message_Send("<color=#FFFF00>" + PhotonNetwork.NickName + "が復活した!!!</color>", MessageE.全員);
            if (PSta.PassL.TryGetValue((int)Fugi_PassE.Debug_不死,out var UDeLV)&&UDeLV>0) PSta.AddCTs.Remove(AddCTsE.復活);
        }
        #endregion
        if (PSta.HP <= 0 || PSta.GostModes)
        {
            return;
        }
        #region スキルCT減少
        for (int i = 0; i < 4; i++) CTs[i]--;
        if (PSta.PassL.TryGetValue((int)Fugi_PassE.Debug_CT90短縮,out var CTCTLV)&&CTCTLV>0)
        {
            for (int i = 0; i < 4; i++)
            {
                if (CTs[i] > ACCTs[i] / 10) CTs[i] = ACCTs[i] / 10;
            }
        }
        #endregion
        if (PSta.Spakes <= 0)
        {
            #region 技1-プランクトン分離
            if (CTs[0] <= 0 && PIns.ACs[0] && PSta.HP > Costs[0])
            {
                CTs[0] = ACCTs[0];
                PSta.HP -= Costs[0];
                float Speeds = SepShotSpeed * 0.01f;
                if (!PSta.Water) Speeds *= 0.75f;
                PSta.Rig.linearVelocity += RotsTrans.forward * Speeds;
                var InsObj = PhotonNetwork.Instantiate(SepPlancton.name, transform.position, RotsTrans.rotation);
                var InsRig = InsObj.GetComponent<Rigidbody>();
                if (InsRig != null)
                {
                    InsRig.linearVelocity = -InsRig.transform.forward * Speeds;
                }
                PhotonNetwork.Instantiate(SepAudioObj.name, transform.position, Quaternion.identity);
            }
            #endregion
            #region 技2-プランクトン拡散
            if (CTs[1] <= 0 && PIns.ACs[1] && PSta.HP > Costs[1])
            {
                CTs[1] = ACCTs[1];
                PSta.HP -= Costs[1];
                PSta.HitMarkerTime = -180;
                for (int i = 0; i < 6; i++)
                {
                    var InsObj = PhotonNetwork.Instantiate(DmPlancton.name, transform.position, Quaternion.Euler(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f)));
                    var InsRig = InsObj.GetComponent<Rigidbody>();
                    if (InsRig != null)
                    {
                        InsRig.linearVelocity = InsRig.transform.forward * SepShotSpeed * 0.01f;
                    }
                }
                PhotonNetwork.Instantiate(DmAudioObj.name, transform.position, Quaternion.identity);
            }
            #endregion
            #region 技3-プランクトン増殖
            if (CTs[2] <= 0 && PIns.ACs[2])
            {
                CTs[2] = ACCTs[2];
                PSta.HP += Costs[2];
                PhotonNetwork.Instantiate(AddAudioObj.name, transform.position, Quaternion.identity);
            }
            #endregion
            #region 技4-座標公開挑発
            if (CTs[3] <= 0 && PIns.ACs[3])
            {
                CTs[3] = ACCTs[3];
                string PosStr = "(";
                PosStr += transform.position.x.ToString("F0") + ",";
                PosStr += transform.position.y.ToString("F0") + ",";
                PosStr += transform.position.z.ToString("F0");
                PosStr += ")";
                Ev_Message_Send(PhotonNetwork.NickName + ">>私の座標は" + PosStr + "です!!!", MessageE.全員);
                if (PosPlancIns != null) PhotonNetwork.Destroy(PosPlancIns);
                var InsObj = PhotonNetwork.Instantiate(PosPlancMarker.name, transform.position, Quaternion.identity);
                PosPlancIns = InsObj;
                PhotonNetwork.Instantiate(PosAudioObj.name, transform.position, Quaternion.identity);
            }
            #endregion
        }
        #region 対操縦者距離スコア補正
        for(int i=0;i<PilotAcs.Length;i++)
        {
            if (PilotAcs[i] == null) continue;
            float DisP = Vector3.Distance(PSta.Rig.position, PilotAcs[i].PSta.Rig.position);
            float DisM = PosPlancIns != null ? Vector3.Distance(PosPlancIns.transform.position, PilotAcs[i].PSta.Rig.position) : 10000;
            float Dis = Mathf.Min(DisP, DisM);
            if (Dis <= PilotScoreDisMin)
            {
                PSta.ScoreF += Mathf.Clamp((PilotScoreDisMin - Dis) / PilotScoreDisMin, 0.2f, 1f) * PilotScoreDisPer / 60f;
            }
        }
        #endregion
    }
    private void LateUpdate()
    {
        #region プランクトンエフェクト設定
        var PS_a = PlanctonEffects.main;
        if (!PSta.GostModes&&PSta.HP>0)
        {
            PS_a.startColor = new ParticleSystem.MinMaxGradient(LifeColors);
            int MParticle = (int)((float)PSta.HP / PSta.MHP * 2000);
            if (PSta.PassL.TryGetValue((int)Fugi_PassE.ステルス,out var StelsLV)&&StelsLV>0) MParticle /= 4;
            PS_a.maxParticles = Mathf.Max(20, MParticle);
            if (PSta.HitMarkerTime < 0) PlanctonEffects.Clear();
            if(PSta.HitMarkerTime >= 0 && PSta.HP > 0)NoWaterMarkerObj.SetActive(!PSta.Water||PSta.HitMarkerTime>0);
            else NoWaterMarkerObj.SetActive(false);
        }
        else
        {
            PS_a.startColor = new ParticleSystem.MinMaxGradient(DeathColors);
            if(photonView.IsMine) PS_a.maxParticles = 2000;
            else PS_a.maxParticles = 0;
            NoWaterMarkerObj.SetActive(false);
        }
        #endregion
    }
}
