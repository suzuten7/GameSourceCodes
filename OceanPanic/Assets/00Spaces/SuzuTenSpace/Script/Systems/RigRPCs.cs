using Photon.Pun;
using UnityEngine;

public class RigRPCs : MonoBehaviourPun
{
    [SerializeField] Rigidbody Rig;

    public void VectAdds(Vector3 Vect)
    {
        photonView.RPC(nameof(Rpc_VectAdds), RpcTarget.All, Vect);
    }
    [PunRPC]
    void Rpc_VectAdds(Vector3 Vect)
    {
        if (!photonView.IsMine) return;
        Rig.linearVelocity += Vect;
    }
}
