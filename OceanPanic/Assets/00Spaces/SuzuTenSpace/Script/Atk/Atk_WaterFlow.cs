using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameInfos;

public class Atk_WaterFlow : MonoBehaviourPun
{
    [SerializeField] float FlowPower;
    [SerializeField] int HitCT = 8;
    [SerializeField] int DeleteTime;
    [SerializeField] GameObject JammingObj;
    [SerializeField] Transform JammingPosTrans;
    public bool CreateJamming = false;
    int Times = 0;
    Dictionary<GameObject, int> HitLists = new Dictionary<GameObject, int>();
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (CreateJamming)
        {
            CreateJamming = false;
            var JaminngInsObj = PhotonNetwork.Instantiate(JammingObj.name, JammingPosTrans.position, Quaternion.identity);
            Ev_Message_Send("妨害エリアが出現", MessageE.鬼);
        }
        Times++;
        if(Times>=DeleteTime)PhotonNetwork.Destroy(gameObject);
        var HitKeys = HitLists.Keys.ToArray();
        for(int i = 0; i < HitKeys.Length; i++)
        {
            HitLists[HitKeys[i]]--;
            if (HitLists[HitKeys[i]] <= 0) HitLists.Remove(HitKeys[i]);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine) return;
        if (HitLists.ContainsKey(other.gameObject)) return;
        HitLists.Add(other.gameObject,HitCT);
        var HitPMove = other.GetComponent<Player_Move>();
        if (HitPMove != null)
        {
            HitPMove.PSta.KB(transform.forward * FlowPower * 0.01f);
        }
        var HitRigRpc = other.GetComponent<RigRPCs>();
        if (HitRigRpc != null)
        {
            HitRigRpc.VectAdds(transform.forward * FlowPower * 0.01f);
        }
    }
}
