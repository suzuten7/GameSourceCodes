using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Suzuten_DataBase;
public class Suzuten_ShotWarpd : MonoBehaviourPun
{
    [SerializeField] Suzuten_ShotObj SObj;
    [SerializeField] Vector3Int TPtimes;
    [SerializeField] ParticleSystem WarpEffect;
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (!TimeChecks(SObj.time, TPtimes)) return;
        if (WarpEffect != null) PhotonNetwork.Instantiate(WarpEffect.name, SObj.UsePS.PosGet(), Quaternion.identity);
        SObj.UsePS.TPs(transform.position);
    }
}
