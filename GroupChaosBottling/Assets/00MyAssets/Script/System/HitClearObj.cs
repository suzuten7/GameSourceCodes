using Photon.Pun;
using UnityEngine;
using static BattleManager;
public class HitClearObj : MonoBehaviour
{
    bool Hit = false;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient) return;
        if (Hit) return;
        var SHit = other.GetComponent<State_Hit>();
        if(SHit != null && SHit.Sta.Player)
        {
            Hit = true;
            BTManager.Clear();
        }
    }
}
