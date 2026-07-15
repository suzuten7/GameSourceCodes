using Photon.Pun;
using UnityEngine;
using static GameInfos;
using static DataBase;
public class Player_Visit_WhalesAction : Player_ActionBase
{
    #region 変数
    [SerializeField] Transform CamTrans;
    [SerializeField] Rigidbody Rig;

    [SerializeField] GameObject[] RaderObjs;
    [SerializeField] AudioSource RaderSEObj;

    [SerializeField] GameObject FugitiveMakerObj;
    [SerializeField] LayerMask MakerRayLayers;
    [SerializeField] GameObject PrisonMakerObj;
    [SerializeField] AudioSource MakerSEObj;

    [SerializeField] GameObject ShotWaleObj;
    [SerializeField] float ShotWaleStartSpeed;
    [SerializeField] GameObject PItemObj;

    [SerializeField] GameObject WaterFlowObj;
    [SerializeField] GameObject JammingAreaObj;

    GameObject FugiMarkerIns;
    [SerializeField] float MakerScoreDisMin;
    [SerializeField] float MakerScoreDisPer;
    #endregion
    void Update()
    {
        if (!photonView.IsMine) return;
        if (GInfo.End) return;
        #region CT減少
        for (int i = 0; i < 4; i++) CTs[i]--;
        #endregion
        #region 技1-サーチ
        if (CTs[0] <= 0 && PIns.ACs[0])
        {
            CTs[0] = ACCTs[0];
            for (int i=0;i<Planctons.Length;i++)
            {
                if (Planctons[i] == null) continue;
                var Plac = Planctons[i].GetComponent<Player_F_PlanctonAction>();
                if (Plac != null && (Plac.PSta.HP <= 0 || Plac.PSta.GostModes)) continue;
                int MarkerType = Plac == null ? 1 : 0;
                if(PSta.PassL.TryGetValue((int)Oni_Visit_PassE.スーパーサーチ,out var SSerchLV)&&SSerchLV>0)
                {
                    if (Plac != null) MarkerType = 2;
                }
                var InsObj = Instantiate(RaderObjs[MarkerType], Planctons[i].transform);
            }
            if (PSta.PassL.TryGetValue((int)Oni_Visit_PassE.座標探知,out var PosRedLV)&&PosRedLV>0)
            {
                float Dis = 1000;
                Player_F_PlanctonAction Target = null;
                for(int i=0;i<FugiAcs.Length;i++)
                {
                    if (FugiAcs[i] == null) continue;
                    float Diss = Vector3.Distance(transform.position, FugiAcs[i].transform.position);
                    if (Dis > Diss)
                    {
                        Dis = Diss;
                        Target = FugiAcs[i];
                    }
                }
                if (Target != null)
                {
                    string PosStr = "(";
                    PosStr += (Target.transform.position.x).ToString("F0") + ",";
                    PosStr += (Target.transform.position.y).ToString("F0") + ",";
                    PosStr += (Target.transform.position.z).ToString("F0") + ")";
                    Ev_Message_Send("最寄りターゲットは"+ PosStr, MessageE.鬼);
                }
                PhotonNetwork.Instantiate(RaderSEObj.name, transform.position, Quaternion.identity);
            }
        }
        #endregion
        #region 技2-逃走者マーカー
        if (CTs[1] <= 0 && PIns.ACs[1])
        {
            CTs[1] = ACCTs[1];
            float RayDis = RayChecks(1000);
            if (FugiMarkerIns != null) PhotonNetwork.Destroy(FugiMarkerIns);
            var InsObj = PhotonNetwork.Instantiate(FugitiveMakerObj.name, transform.position + RotsTrans.forward * RayDis, RotsTrans.rotation);
            FugiMarkerIns = InsObj;
            Ev_Message_Send("<color=#FF00FF>逃走者マーカー設置</color>", MessageE.鬼);
            if (PSta.PassL.TryGetValue((int)Oni_Visit_PassE.麻痺の視線,out var ParaEyeLV)&&ParaEyeLV>0 && !PSta.AddCTs.ContainsKey(AddCTsE.麻痺の視線))
            {
                bool Hits = false;
                for(int i=0;i<FugiAcs.Length;i++)
                {
                    if (FugiAcs[i] == null) continue;
                    if (FugiAcs[i].PSta.HP <= 0 || FugiAcs[i].PSta.GostModes) continue;
                    float Dis = Vector3.Distance(transform.position + RotsTrans.forward * RayDis, FugiAcs[i].transform.position);
                    if (Dis <= 2.5f)
                    {
                        Hits = true;
                        FugiAcs[i].PSta.SpakeAdd(60 * 2);
                    }
                }
                if (Hits)
                {
                    PSta.AddCTSet(AddCTsE.麻痺の視線, 60 * 10);
                    Ev_Message_Send("麻痺の視線発動!!!", MessageE.鬼);
                }
            }
            if (PSta.PassL.TryGetValue((int)Oni_Visit_PassE.牢獄マーカー,out var PrisMkLV)&&PrisMkLV>0&&!PSta.AddCTs.ContainsKey(AddCTsE.牢獄マーカー))
            {
                PSta.AddCTSet(AddCTsE.牢獄マーカー, 60 * 30);
                var PriInsObj = PhotonNetwork.Instantiate(PrisonMakerObj.name, transform.position + RotsTrans.forward * RayDis, Quaternion.identity);
                Ev_Message_Send("牢獄が設置された", MessageE.鬼);
            }
            if (PSta.PassL.TryGetValue((int)Oni_Visit_PassE.Debug_マーカーアタック,out var MkAtkLV)&&MkAtkLV>0)
            {
                for (int i=0;i<FugiAcs.Length;i++)
                {
                    if (FugiAcs[i] == null) continue;
                    if (FugiAcs[i].PSta.HP <= 0 || FugiAcs[i].PSta.GostModes) continue;
                    float Dis = Vector3.Distance(transform.position + RotsTrans.forward * RayDis, FugiAcs[i].transform.position);
                    if (Dis <= 10f)
                    {
                        FugiAcs[i].PSta.Damage(500);
                    }
                }
            }
            PhotonNetwork.Instantiate(MakerSEObj.name, transform.position, Quaternion.identity);
        }
        #endregion
        #region 技3-ショットホエール
        if (CTs[2] <= 0 && PIns.ACs[2])
        {
            CTs[2] = ACCTs[2];
            var InsObj = PhotonNetwork.Instantiate(ShotWaleObj.name, transform.position, CamTrans.rotation);
            var InsRig = InsObj.GetComponent<Rigidbody>();
            if (InsRig != null)
            {
                InsRig.linearVelocity = InsObj.transform.forward * ShotWaleStartSpeed * 0.01f;
            }
            var InsAtk = InsObj.GetComponent<Atk_ShotWale>();
            if (InsAtk != null)
            {
                InsAtk.Use = this;
                bool ItemCre = false;
                if (PSta.PassL.TryGetValue((int)Oni_Visit_PassE.アイテムクリエイター,out var ItemCreLV)&&ItemCreLV>0 && !PSta.AddCTs.ContainsKey(AddCTsE.アイテムクリエイター))
                {
                    ItemCre = true;
                    PSta.AddCTSet(AddCTsE.アイテムクリエイター, 60 * 25);
                }
                InsAtk.ItemCres = ItemCre;
            }
        }
        #endregion
        #region 技4-水流
        if (CTs[3] <= 0 && PIns.ACs[3])
        {
            CTs[3] = ACCTs[3];

            var InsObj = PhotonNetwork.Instantiate(WaterFlowObj.name, transform.position, CamTrans.rotation);
            var InsAtk = InsObj.GetComponent<Atk_WaterFlow>();
            if (InsAtk != null)
            {
                bool Jamings = false;
                if (PSta.PassL.TryGetValue((int)Oni_Visit_PassE.妨害エリア出現,out var JAreaLV)&&JAreaLV>0 && !PSta.AddCTs.ContainsKey(AddCTsE.妨害エリア出現))
                {
                    Jamings = true;
                    PSta.AddCTSet(AddCTsE.妨害エリア出現, 60 * 25);
                }
                InsAtk.CreateJamming = Jamings;
            }
        }
        #endregion
        #region マーカー距離スコア補正
        if (FugiMarkerIns != null)
        {
            float NearFugiDis = 10000;
            float NearPilotDis = 10000;
            for (int i=0;i<FugiAcs.Length;i++)
            {
                if (FugiAcs[i] == null) continue;
                float Dis = Vector3.Distance(FugiMarkerIns.transform.position, FugiAcs[i].PSta.Rig.position);
                if (NearFugiDis > Dis)
                {
                    NearFugiDis = Dis;
                }
            }
            for (int i=0;i<PilotAcs.Length;i++)
            {
                if (PilotAcs[i] == null) continue;
                float Dis = Vector3.Distance(FugiMarkerIns.transform.position, PilotAcs[i].PSta.Rig.position);
                if (NearPilotDis > Dis)
                {
                    NearPilotDis = Dis;
                }
            }
            if(NearFugiDis <= MakerScoreDisMin && NearPilotDis <= MakerScoreDisMin)
            {
                float DisAve = (NearFugiDis + NearPilotDis) / 2f;
                PSta.ScoreF += Mathf.Clamp((MakerScoreDisMin - DisAve) / MakerScoreDisMin,0.2f,1f) * MakerScoreDisPer / 60f;
            }

        }

        #endregion
    }
    /// <summary>レイ判定</summary>
    float RayChecks(float RayDis)
    {
        var Rays = Physics.RaycastAll(transform.position, RotsTrans.forward, RayDis, MakerRayLayers);
        Debug.DrawRay(transform.position, transform.position + RotsTrans.forward * RayDis, Color.yellow, 0.3f);
        foreach (var RayHit in Rays)
        {
            var Plac = RayHit.collider.GetComponent<Player_F_PlanctonAction>();
            if (Plac != null&&(Plac.PSta.HP <= 0 || Plac.PSta.GostModes))continue;
            if (RayDis > RayHit.distance) RayDis = RayHit.distance;
        }
        return RayDis;
    }

    private void LateUpdate()
    {
        if (PSta.PassL.TryGetValue((int)Oni_Visit_PassE.操縦者からの離脱, out var OutBodyLV) && OutBodyLV > 0)
        {
            Rig.isKinematic = false;
        }
        else
        {
            #region 操縦者の座標に移動
            Rig.isKinematic = true;
            var Pilots = FindAnyObjectByType<Player_Pilot_WhalesAction>();
            if (Pilots != null)
            {
                transform.position = Pilots.VisTrans.position;
                transform.rotation = Pilots.VisTrans.rotation;
            }
            #endregion
        }
    }

}
