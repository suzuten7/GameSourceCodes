using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State_WeponValues : MonoBehaviour,IPunObservable
{
    public Dictionary<int, int> WeponSets = new Dictionary<int, int>();
    public Dictionary<int, Vector3> WeponPoss = new Dictionary<int, Vector3>();
    public Dictionary<int, Vector3> WeponRots = new Dictionary<int, Vector3>();
    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            var WepSetKeys = WeponSets.Keys.ToArray();
            var WepSetIDs = WeponSets.Values.ToArray();
            var WepSetPoss = WeponPoss.Values.ToArray();
            var WepSetRots = WeponRots.Values.ToArray();
            stream.SendNext(WepSetKeys);
            stream.SendNext(WepSetIDs);
            stream.SendNext(WepSetPoss);
            stream.SendNext(WepSetRots);
        }
        else
        {
            var WepSetKeys = (int[])stream.ReceiveNext();
            var WepSetIDs = (int[])stream.ReceiveNext();
            var WepSetPoss = (Vector3[])stream.ReceiveNext();
            var WepSetRots = (Vector3[])stream.ReceiveNext();
            WeponSets.Clear();
            WeponPoss.Clear();
            WeponRots.Clear();
            for (int i = 0; i < WepSetKeys.Length; i++)
            {
                WeponSets.Add(WepSetKeys[i], WepSetIDs[i]);
                WeponPoss.Add(WepSetKeys[i], WepSetPoss[i]);
                WeponRots.Add(WepSetKeys[i], WepSetRots[i]);
            }
        }
    }
}
