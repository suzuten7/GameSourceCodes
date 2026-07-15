using UnityEngine;
using Photon.Pun;
using static PlayerValue;
using static Manifesto;
using static BattleManager;
using static DataBase;
public class Net_PlayerSet : MonoBehaviour
{
    [Tooltip("プレイヤー位置範囲x～y"), SerializeField] Vector2 SetRange;
    [Tooltip("プレイヤー敵"), SerializeField] bool FriendFire;
    [Tooltip("チュートリアル用"), SerializeField] bool Tutorial;
    void Update()
    {
        if (!PhotonNetwork.InRoom) return;
        if (!Tutorial)
        {
            for (int i = 0; i < 4; i++)
            {
                PSet(i);
            }
        }
        else
        {
            PSet(-1);
        }
    }
    void PSet(int ID)
    {
        if (!PhotonNetwork.OfflineMode && ID >= 2) return;
        if (BTManager.LocalCharas[Mathf.Max(0, ID)] != null) return;
        Class_Save_PriSet PriSet = null;
        Class_Save_GeneSet GeneSt = null;
        switch (ID)
        {
            case -1:
                PriSet = new Class_Save_PriSet
                {
                    CharaID = PriSetGet.CharaID,
                    AtkF = new Class_Save_Atks
                    {
                        N_AtkID = 0,
                        S1_AtkID = 0,
                        S2_AtkID = 2,
                        E_AtkID = 0,
                    },
                    AtkB = new Class_Save_Atks
                    {
                        N_AtkID = 1,
                        S1_AtkID = 1,
                        S2_AtkID = 3,
                        E_AtkID = 1,
                    },
                    Passive = new Class_Save_Passive
                    {
                        P1_ID = 0,
                        P2_ID = 0,
                        P3_ID = 1,
                        P4_ID = 2,
                    }
                };
                GeneSt = new Class_Save_GeneSet();
                break;
            case 0: PriSet = PriSetGet; GeneSt = GeneGet; break;
            case 1:
                if (PSaves.AddSet1 >= 0)
                {
                    PriSet = PriSets[PSaves.AddSet1];
                    GeneSt= Genes.Sets[PSaves.AddSet1];
                }
                else return;
                break;
            case 2:
                if (PSaves.AddSet2 >= 0)
                {
                    PriSet = PriSets[PSaves.AddSet2];
                    GeneSt = Genes.Sets[PSaves.AddSet2];
                }
                else return;
                break;
            case 3:
                if (PSaves.AddSet3 >= 0)
                {
                    PriSet = PriSets[PSaves.AddSet3];
                    GeneSt = Genes.Sets[PSaves.AddSet3];
                }
                else return;
                break;
        }
        var Pos = transform.position;
        Pos += new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f).normalized * Random.Range(SetRange.x, SetRange.y);
        var PObj = PhotonNetwork.Instantiate(DB.Player.gameObject.name, Pos, Quaternion.identity);
        var PSta = PObj.GetComponent<State_Base>();
        PSta.PLValues.Sets = PriSet;
        PSta.PLValues.Genes = GeneSt;
        PSta.PLValues.SubID = ID;
        BTManager.LocalCharas[Mathf.Max(0, ID)] = PSta;
        if (FriendFire)
        {
            PSta.Team = 10000 + PSta.photonView.ViewID;
        }
    }
}
