using Photon.Pun;
using UnityEngine;

public class Player_ActionBase : MonoBehaviourPun
{
    [SerializeField] protected Player_Inputd PIns;
    public Player_States PSta;
    [SerializeField] protected Transform RotsTrans;

    public int[] ACCTs = new int[4];
    public float[] ACMoveSpeedPerCh = new float[4];

    public int[] CTs = new int[4];
    void Update()
    {
        if (!photonView.IsMine) return;
        for (int i = 0; i < 4; i++) CTs[i]--;
    }
}