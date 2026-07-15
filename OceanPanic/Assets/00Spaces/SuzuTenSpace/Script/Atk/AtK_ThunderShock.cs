using Photon.Pun;
using UnityEngine;

public class AtK_ThunderShock : MonoBehaviourPun
{
    #region エディタ変数
    [SerializeField] Rigidbody Rig;
    [SerializeField] int SpakeP;
    [SerializeField] float KBPow;
    [SerializeField] int RemTime = 60;
    #endregion
    #region 内部変数
    int RemT = 0;
    #endregion
    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        #region 時間経過
        RemT++;
        if (RemT >= RemTime) PhotonNetwork.Destroy(gameObject);
        #endregion
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        var Pilot = other.GetComponent<Player_Pilot_WhalesAction>();
        if (Pilot != null)
        {
            Pilot.PSta.SpakeAdd(SpakeP);
            Pilot.PSta.KB(Rig.linearVelocity.normalized * KBPow * 0.01f);
        }
    }
}
