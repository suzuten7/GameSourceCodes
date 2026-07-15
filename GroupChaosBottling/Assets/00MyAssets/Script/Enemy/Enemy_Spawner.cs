using UnityEngine;
using Photon.Pun;
using static Statics;
public class Enemy_Spawner : MonoBehaviourPun,IPunObservable
{
    [SerializeField] GameObject EnemyObj;
    [SerializeField] Vector2Int Counts;
    [SerializeField] Vector2 Dis;
    [SerializeField] float SpawenCT;
    [SerializeField] float CT = 0;

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        CT -= 1 / 60f;
        if (CT <= 0)
        {
            CT = SpawenCT;
            var Count = V2Int_Rand(Counts);
            for(int i = 0; i < Count; i++)
            {
                Vector3 Pos = transform.position;
                Pos += new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f).normalized * V2_Rand_Float(Dis);
                PhotonNetwork.InstantiateRoomObject(EnemyObj.name, Pos, Quaternion.identity);
            }
        }
    }
    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(CT);
        }
        else
        {
            CT = (float)stream.ReceiveNext();
        }
    }
}
