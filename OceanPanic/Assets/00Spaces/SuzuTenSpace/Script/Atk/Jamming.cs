using Photon.Pun;
using UnityEngine;

public class Jamming : MonoBehaviourPun
{
    private void OnTriggerStay(Collider other)
    {
        var FPMove = other.GetComponent<Player_F_PlanctonAction>();
        if (FPMove == null) return;
        if (!FPMove.photonView.IsMine) return;
        FPMove.PSta.Rig.linearVelocity *= 0.85f;
    }
}
