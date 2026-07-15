using Photon.Pun;
using UnityEngine;
using static GameInfos;
public class McEvent_ThunderShock : McEventBase
{
    [SerializeField] GameObject ThunderObj;
    [SerializeField]float ThunderSpeed;
    public override void McEvent()
    {
        Vector3 PosBase = transform.position + Vector3.up * 10;
        var InsThunder = PhotonNetwork.Instantiate(ThunderObj.name, PosBase, Quaternion.identity);
        var InsRig = InsThunder.GetComponent<Rigidbody>();
        if (InsRig != null)
        {
            var PilotOni = FindFirstObjectByType<Player_Pilot_WhalesAction>();
            var Vects = Vector3.up;
            if (PilotOni != null)
            {
                Vects = PilotOni.transform.position - InsThunder.transform.position;
            }
            InsRig.linearVelocity = Vects.normalized * ThunderSpeed * 0.01f;
        }
        Ev_Message_Send(EventMessage, MessageE.全員);
    }
}
