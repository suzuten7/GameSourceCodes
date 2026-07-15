using Photon.Pun;
using UnityEngine;
using static Manifesto;
using static Statics;
using static State_Atk;
public class Shot_Add : MonoBehaviourPun
{
    [SerializeField] Shot_Obj SObj;
    [SerializeField] Class_Shot_Add[] Adds;
    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        for(int i = 0; i < Adds.Length; i++)
        {
            if (!V3IntTimeCheck(SObj.Times, Adds[i].Times)) continue;
            ShotAdd(SObj.USta, SObj.BranchNum, Adds[i].AddShot, SObj.Times, transform.position, transform.eulerAngles,SObj,SObj.AtkNum);
        }
    }
}
