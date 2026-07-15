using Photon.Pun;
using UnityEngine;
using static Statics;
public class State_TestAtk : MonoBehaviourPun
{
    [SerializeField] State_Base Sta;
    [SerializeField] int Dam;
    [SerializeField] float HitCT;
    int ct = 0;
    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        ct--;
    }
    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine) return;
        if (Sta.HP <= 0) return;
        if (ct > 0) return;
        var Hit = other.GetComponent<State_Hit>();
        if (Hit == null) return;
        if (!TeamCheck(Sta, Hit.Sta)) return;
        if (Hit.Sta.HP <= 0) return;
        ct = Mathf.RoundToInt(HitCT * 60);
        Hit.Sta.Damage(other.ClosestPoint(transform.position), Dam,0,false,photonView.ViewID);
    }
}
