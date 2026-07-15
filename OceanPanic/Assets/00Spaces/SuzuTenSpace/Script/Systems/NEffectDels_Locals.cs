using Photon.Pun;
using UnityEngine;

public class NEffectDels_Locals : MonoBehaviour
{
    [SerializeField] ParticleSystem[] ParSs;
    void Update()
    {
        for(int i = 0; i < ParSs.Length; i++)
        {
            if (ParSs[i] != null) return;
        }
        Destroy(gameObject);
    }
}
