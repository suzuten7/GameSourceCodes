using Photon.Pun;
using UnityEngine;

public class NEffectDels_PView : MonoBehaviourPun
{
    [SerializeField] ParticleSystem[] ParSs;
    void Update()
    {
        if (!photonView.IsMine) return;
        for(int i = 0; i < ParSs.Length; i++)
        {
            if (ParSs[i] != null) return;
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
