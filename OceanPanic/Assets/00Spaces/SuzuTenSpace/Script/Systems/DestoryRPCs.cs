using Photon.Pun;
using UnityEngine;

public class DestoryRPCs : MonoBehaviourPun
{
    [System.NonSerialized]public bool Dels = false;
    public bool Destory()
    {
        if (Dels) return false;
        Dels = true;
        photonView.RPC(nameof(Rpc_Destory), RpcTarget.All);
        return true;
    }
    [PunRPC]
    void Rpc_Destory()
    {
        if (!photonView.IsMine) return;
        PhotonNetwork.Destroy(gameObject);
    }
}
