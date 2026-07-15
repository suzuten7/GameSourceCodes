using Photon.Pun;
using UnityEngine;
using static GameInfos;
public class McEvent_Instances : McEventBase
{
    [SerializeField] GameObject InsObj;
    [SerializeField] Vector2Int InsCounts;
    [SerializeField] Vector2 InsRigSpeed;
    public override void McEvent()
    {
        Vector3 PosBase = transform.position + Vector3.up * 10;
        int ICo = Random.Range(InsCounts.x, InsCounts.y + 1);
        for (int i = 0; i < ICo; i++)
        {
            var IObj = PhotonNetwork.Instantiate(InsObj.name, PosBase, Quaternion.identity);
            var IRig = IObj.GetComponent<Rigidbody>();
            if (IRig != null)
            {
                IRig.linearVelocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized * Random.Range(InsRigSpeed.x, InsRigSpeed.y) * 0.01f;
            }
        }
        Ev_Message_Send(EventMessage, MessageE.全員);
    }
}
