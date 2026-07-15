using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Suzuten_ActionData;
using static Suzuten_DataBase;
using static Suzuten_PlayerSets;
public class Suzuten_SEPlays : MonoBehaviourPun
{
    [SerializeField] SEDatas SEData;
    [SerializeField] AudioSource AS;
    static Suzuten_SEPlays SEPlay;
    static List<PSEsC> PlaySEs = new List<PSEsC> { };
    class PSEsC
    {
        public AudioSource AS;
        public int PlayNum;
        public bool Voice;
    }
    private void Start()
    {
        SEData.SEDic_Set();
        SEPlay = this;
    }
    static public void SEM(Suzuten_PlayerState PS)
    {
        if (PS.ActionID >= 0)
        {
            Suzuten_ActionData AD = PS.CD.Actions[ACSetID[PS.PID, PS.ActionID]];
            if (AD.SEs == null) return;
            foreach (SEsC SEC in AD.SEs)
            {
                if (TimeChecks(PS.ActionTime, SEC.SETimes))
                {
                    bool Ifs = true;
                    if (SEC.Ifs != null)
                    {
                        for (int i = 0; i < SEC.Ifs.Length; i++)
                        {
                            if (!IfsCheck(SEC.Ifs[i], PS))
                            {
                                Ifs = false;
                                break;
                            }
                        }
                    }
                    if (Ifs)
                    {
                        for (int i = 0; i < SEC.SEPlays.Length; i++)
                        {
                            SEPlays(SEC.SEPlays[i],PS.PID);
                        }
                    }
                }
            }
        }
    }
    static public void SEPlays(SEPlayC SEPlays,int PlayNum)
    {
        if (SEPlays.SEFile == null) return;
        if (PhotonNetwork.InRoom)
        {
            Vector2Int SEID = SEDatas.SEIDGet(SEPlays.SEFile.name);
            SEPlay.photonView.RPC(nameof(RpcSEs), RpcTarget.All, SEID.x,SEID.y, SEPlays.Volume, SEPlays.Pitch, (int)SEPlays.SEOP, SEPlays.SERePlay, PlayNum);
        }
        else SEPlay.SEPlaysD(SEPlays.SEFile, SEPlays.Volume, SEPlays.Pitch, (int)SEPlays.SEOP, SEPlays.SERePlay, PlayNum);
    }
    [PunRPC]
    void RpcSEs(int SEIDx,int SEIDy,float Volume,float Pitch,int SEOP,bool RePlay, int PlayNum)
    {
        AudioClip AuSo = SEDatas.SEGetID(new Vector2Int(SEIDx, SEIDy));
        if (AuSo!=null)
        {
            SEPlaysD(AuSo, Volume, Pitch, SEOP, RePlay, PlayNum);
        }
    }
    void SEPlaysD(AudioClip Ac, float Volume, float Pitch, int SEOP, bool RePlay, int PlayNum)
    {
        if (AS == null || SEPlay == null || Ac == null) return;
        AudioSource ASs = null;
        if (SEOP != (int)SEOPE.複数再生)
        {
            for (int i = PlaySEs.Count - 1; i >= 0; i--)
            {
                if (PlaySEs[i].AS == null) PlaySEs.RemoveAt(i);
                else
                {
                    bool Ifs = true;
                    if (PlaySEs[i].PlayNum != PlayNum) Ifs = false;
                    if (SEOP != (int)SEOPE.ボイス)
                    {
                        if (PlaySEs[i].AS.clip != Ac) Ifs = false;
                        if (PlaySEs[i].AS.pitch != Pitch) Ifs = false;
                        if (PlaySEs[i].Voice) Ifs = false;
                    }
                    else if (!PlaySEs[i].Voice) Ifs = false;

                    if (Ifs)
                    {
                        ASs = PlaySEs[i].AS;
                        break;
                    }
                }
            }
        }
        if (ASs == null)
        {
            ASs = Instantiate(AS);
            ASs.clip = Ac;
            ASs.pitch = Pitch;
            ASs.volume = Volume / 4f;
            ASs.loop = false;
            ASs.Play();
            ASs.time = Pitch >= 0 ? 0 : (Ac.length - 0.02f);

            ASs.gameObject.AddComponent<SEEndDeltes>();
            if (SEOP != (int)SEOPE.複数再生) PlaySEs.Add(new PSEsC { AS = ASs, PlayNum = PlayNum, Voice = SEOP == (int)SEOPE.ボイス });
        }
        if (ASs != null)
        {
            switch (SEOP)
            {
                default: break;
                case (int)SEOPE.単体最大音量:
                    ASs.volume = Mathf.Max(ASs.volume, Volume / 4f);
                    if (RePlay) ASs.time = Pitch >= 0 ? 0 : (Ac.length - 0.02f);
                    break;
                case (int)SEOPE.単体音量加算:
                    ASs.volume += Volume / 4f;
                    if (RePlay) ASs.time = Pitch >= 0 ? 0 : (Ac.length - 0.02f);
                    break;
                case (int)SEOPE.ボイス:
                    if (ASs.clip != Ac || ASs.pitch != Pitch)
                    {
                        ASs.clip = Ac;
                        ASs.pitch = Pitch;
                        ASs.time = Pitch >= 0 ? 0 : (Ac.length - 0.02f);
                        ASs.Play();
                    }
                    if (RePlay)
                    {
                        ASs.time = Pitch >= 0 ? 0 : (Ac.length - 0.02f);
                        ASs.Play();
                    }
                    ASs.volume = Volume / 4f;
                    break;
            }
        }

    }
    class SEEndDeltes:MonoBehaviourPun
    {
        public AudioSource AS;
        private void Start()
        {
            AS = GetComponent<AudioSource>();
        }
        private void Update()
        {
            if(!AS.isPlaying) Destroy(AS.gameObject);
        }
    }
}
