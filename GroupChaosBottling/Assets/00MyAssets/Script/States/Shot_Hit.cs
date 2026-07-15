using Photon.Pun;
using UnityEngine;
using static Statics;
using static Manifesto;
public class Shot_Hit : MonoBehaviourPun
{
    [SerializeField] Shot_Obj SObj;
    [SerializeField,Tooltip(Const_Ttp_Times+"\nx,yが0だと常時")] Vector3Int HitTime;
    [SerializeField] bool WallRem;
    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine) return;
        if (SObj.USta == null) return;
        if (HitTime.x != 0 && HitTime.y != 0 && !V3IntTimeCheck(SObj.Times, HitTime)) return;
        var Hit = other.GetComponent<State_Hit>();
        if (Hit != null)
        {
            if (Hit.Sta.HP <= 0) return;
            SObj.Hits(Hit, other.ClosestPoint(transform.position));
        }
        if (WallRem && other.tag == "Wall") SObj.ShotDel();
        
    }
}
