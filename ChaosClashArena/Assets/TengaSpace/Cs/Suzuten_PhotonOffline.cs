using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Suzuten_PhotonOffline : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonPrefabs PrePool;
    void Start()
    {
        PrePool.PrefabPoolSet();
        PhotonNetwork.Disconnect();
        PhotonNetwork.OfflineMode = true;
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.OfflineMode = true;
    }
}
