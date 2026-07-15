using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Suzuten_TimeMarker : MonoBehaviourPun
{
    [SerializeField] Suzuten_ShotObj SObj;
    [SerializeField] GameObject UseObj;
    [SerializeField] GameObject EnemyObj;
    bool Use = false;
    void FixedUpdate()
    {
        if (Use) return;
        Use = true;
        if (SObj.photonView.IsMine)
        {
            Suzuten_TimeMarker Markerd = null;
            var Markers = FindObjectsOfType<Suzuten_TimeMarker>();
            foreach (var Marker in Markers)
            {
                if(Marker!=this&&Marker.SObj.UsePS == SObj.UsePS)
                {
                    Markerd = Marker;
                    break;
                }
            }
            if (Markerd == null)
            {
                UseObj.transform.position = SObj.UsePS.PosGet();
                EnemyObj.transform.position = SObj.UsePS.Target.PosGet();
            }
            else
            {
                SObj.UsePS.TPs(Markerd.UseObj.transform.position);
                SObj.UsePS.Target.TPs(Markerd.EnemyObj.transform.position);
                PhotonNetwork.Destroy(Markerd.gameObject);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
