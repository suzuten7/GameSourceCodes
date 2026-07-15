using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static DataBase;
using static GameInfos;
public class Macine : MonoBehaviourPun,IPunObservable
{
    public string MacineName;
    #region エディタ変数
    public int TaskMaxs;
    [SerializeField] Transform UITrans;
    [SerializeField] Image TaskIma;
    [SerializeField] McEventBase CompleateEvent;
    [SerializeField] MeshRenderer MRend;
    [SerializeField] Material NonMat;
    [SerializeField] Material CompMat;
    [SerializeField]float[] PerSend;
    [SerializeField] AudioSource TaskAddSEObj;
    [SerializeField] AudioSource TaskRemSEObj;
    [SerializeField] GameObject SendMakerObj;
    [SerializeField] int TaskAddScore;
    [SerializeField] int TaskCompScore;
    [SerializeField] AudioSource TaskCompSEObj;
    [SerializeField] ParticleSystem IconParticle;
    [SerializeField] Color NonIconColor;
    [SerializeField] Color CompIconColor;
    #endregion
    #region 内部変数
    public float TaskCo = 0;
    int PSend = -1;
    Dictionary<Player_F_PlanctonAction, int> FeChecks = new Dictionary<Player_F_PlanctonAction, int>();
    Dictionary<Player_Pilot_WhalesAction, int> PilotChecks = new Dictionary<Player_Pilot_WhalesAction, int>();
    public bool TaskCompleate;
    #endregion

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        #region 進捗増減処理
        var FeKeys = FeChecks.Keys.ToArray();
        float McPer = 100f;
        #region 監視者パッシブ
        for (int i = 0; i < VisitAcs.Length; i++)
        {
            if (VisitAcs[i] == null) continue;
            switch (VisitAcs[i].PSta.PassL.TryGetValue((int)Oni_Visit_PassE.マシン破損, out var OPassLV) ? OPassLV : 0)
            {
                default:break;
                case 1: McPer -= 10f; break;
                case 2: McPer -= 20f; break;
                case 3: McPer -= 30f; break;
                case 4: McPer -= 40f; break;
            }
        }
        #endregion
        #region 残逃走者数進捗影響
        int PlancCount = 0;

        for (int i=0;i<FugiAcs.Length;i++)
        {
            if (FugiAcs[i] != null && FugiAcs[i].PSta.HP > 0) PlancCount++;
        }
        McPer *= 1f / Mathf.Pow(Mathf.Max(PlancCount, 1), 0.6f);
        #endregion
        #region 増加

        for (int i = 0; i < FeKeys.Length; i++)
        {
            FeChecks[FeKeys[i]]--;
            float TaskAdd = 1;
            #region 逃走者パッシブ
            switch (FeKeys[i].PSta.PassL.TryGetValue((int)Fugi_PassE.機械高速化, out var OPassLV) ? OPassLV : 0)
            {
                default:break;
                case 1: TaskAdd += 0.1f; break;
                case 2: TaskAdd += 0.2f; break;
                case 3: TaskAdd += 0.3f; break;
                case 4: TaskAdd += 0.5f; break;
            }
            if (FeKeys[i].PSta.PassL.TryGetValue((int)Fugi_PassE.Debug_リペアース,out var ReperLV)&&ReperLV>0) TaskAdd += 9f;
            #endregion
            TaskCo += TaskAdd * McPer * 0.01f;
            if (FeChecks[FeKeys[i]] <= 0) FeChecks.Remove(FeKeys[i]);
        }
        #endregion
        #region 減少
        var PiKeys = PilotChecks.Keys.ToArray();
        for (int i = 0; i < PiKeys.Length; i++)
        {
            PilotChecks[PiKeys[i]]--;
            TaskCo -= 3 + (PlancCount - 1)/3f;
            if (PilotChecks[PiKeys[i]] <= 0) PilotChecks.Remove(PiKeys[i]);
        }
        #endregion
        TaskCo = Mathf.Clamp(TaskCo, 0, TaskMaxs);
        #endregion

        #region 途中進捗
        if (PSend < PerSend.Length -1 &&(float)TaskCo / TaskMaxs >= PerSend[PSend+1] * 0.01f)
        {
            PSend++;
            Ev_Message_Send("マシン-"+MacineName+"の進捗が" + PerSend[PSend] + "%になりました", MessageE.全員);
            PhotonNetwork.Instantiate(SendMakerObj.name, transform.position, Quaternion.identity);
            var SFeKeys = FeChecks.Keys.ToArray();
            for(int i = 0; i < SFeKeys.Length; i++)
            {
                SFeKeys[i].PSta.ScoreAdd(TaskAddScore);
            }
            PhotonNetwork.Instantiate(TaskAddSEObj.name, transform.position, Quaternion.identity);
        }
        if (PSend >= 0 && (float)TaskCo / TaskMaxs < PerSend[PSend] * 0.01f)
        {
            Ev_Message_Send("マシン-"+MacineName+"の進捗が" + PerSend[PSend] + "%に減少!!!", MessageE.全員);
            PSend--;
            var SPiKeys = PilotChecks.Keys.ToArray();
            for (int i = 0; i < SPiKeys.Length; i++)
            {
                SPiKeys[i].PSta.ScoreAdd(TaskAddScore);
            }
            PhotonNetwork.Instantiate(TaskRemSEObj.name, transform.position, Quaternion.identity);
        }
        #endregion
        #region 完了メッセージ
        if (!TaskCompleate && TaskCo >= TaskMaxs)
        {
            TaskCompleate = true;
            Ev_Message_Send("マシン-"+ MacineName + "の修復が完了した", MessageE.逃走者);
            Ev_Message_Send("マシン-"+ MacineName + "を修復された", MessageE.鬼);
            if(CompleateEvent!=null)CompleateEvent.McEvent();
            var SFeKeys = FeChecks.Keys.ToArray();
            for (int i = 0; i < SFeKeys.Length; i++)
            {
                SFeKeys[i].PSta.ScoreAdd(TaskCompScore);
            }
            PhotonNetwork.Instantiate(TaskCompSEObj.name, transform.position, Quaternion.identity);
        }
        #endregion
    }
    private void LateUpdate()
    {
        #region UI
        if (Camera.main!=null)UITrans.LookAt(Camera.main.transform);
        TaskIma.fillAmount = (float)TaskCo / TaskMaxs;
        #endregion
        #region 外見
        if (TaskCo >= TaskMaxs) MRend.material = CompMat;
        else MRend.material = NonMat;
        #endregion
        var IconPMain = IconParticle.main;
        IconPMain.startColor = TaskCo >= TaskMaxs ? CompIconColor : NonIconColor;
    }
    private void OnTriggerStay(Collider other)
    {
        var PMove = other.GetComponent<Player_Move>();
        if (PMove != null&&PMove.photonView.IsMine)
        {
            PMove.PSta.LastHitMacine = this;
            PMove.PSta.MacineHitTime = 5;
        }
        if (!photonView.IsMine) return;
        if (TaskCompleate) return;
        var PAC = other.GetComponent<Player_F_PlanctonAction>();
        if (PAC != null)
        {
            if (PAC.PSta.HP <= 0) return;
            #region 追加
            if (FeChecks.ContainsKey(PAC)) FeChecks[PAC] = 5;
            else FeChecks.Add(PAC, 5);
            #endregion
        }
        var OniPilot = other.GetComponent<Player_Pilot_WhalesAction>();
        if (OniPilot != null)
        {
            #region 追加
            if (PilotChecks.ContainsKey(OniPilot)) PilotChecks[OniPilot] = 5;
            else PilotChecks.Add(OniPilot, 5);
            #endregion
        }
    }

    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(TaskCo);
            stream.SendNext(PSend);
        }
        else
        {
            TaskCo = (float)stream.ReceiveNext();
            PSend = (int)stream.ReceiveNext();
        }
    }
}
