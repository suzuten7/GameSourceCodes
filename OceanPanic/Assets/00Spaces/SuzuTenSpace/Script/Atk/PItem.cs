using Photon.Pun;
using UnityEngine;
using static GameInfos;
public class PItem : MonoBehaviourPun
{
    bool Gets = false;
    private void OnTriggerEnter(Collider other)
    {
        if (Gets) return;
        var PilotM = other.GetComponent<Player_Pilot_WhalesAction>();
        if (PilotM == null) return;
        Gets = true;
        PilotM.PSta.SP += 50;
        Ev_Message_Send("PアイテムでSPが回復した!", MessageE.鬼);
        photonView.RPC(nameof(Deletes), RpcTarget.All);
    }

    [PunRPC]
    void Deletes()
    {
        if (!photonView.IsMine) return;
        PhotonNetwork.Destroy(gameObject);
    }
}
