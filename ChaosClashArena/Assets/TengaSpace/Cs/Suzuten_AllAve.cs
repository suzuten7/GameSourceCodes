using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suzuten_AllAve : MonoBehaviour
{
    void FixedUpdate()
    {
        float Vals = 0;
        var PLs = FindObjectsOfType<Suzuten_PlayerState>();
        for(int i = 0; i < PLs.Length; i++)
        {
            Vals += PLs[i].HP;
        }
        Vals /= PLs.Length;
        for (int i = 0; i < PLs.Length; i++)
        {
            PLs[i].photonView.RPC(nameof(Suzuten_PlayerState.RpcHPSets), RpcTarget.All, Vals);
        }
        Destroy(this);
    }
}
