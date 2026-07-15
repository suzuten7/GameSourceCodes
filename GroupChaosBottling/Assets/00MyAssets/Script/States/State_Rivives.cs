using UnityEngine;
using Photon.Pun;
using static Manifesto;
public class State_Rivives : MonoBehaviourPun
{
    [SerializeField] State_Base Sta;
    [SerializeField] int RiviveSecond;
    [SerializeField] float HPPer;
    bool Flag = false;

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (Sta.HP > 0) Flag = false;
        else if (!Sta.BufCheck(Enum_Bufs.復活待機))
        {
            if (Flag)
            {
                Sta.HP = Sta.FMHP * HPPer * 0.01f;
                if (Sta.Team == 0 || Sta.Team >= 100000)
                {
                    Sta.BufSets(Enum_Bufs.シールド, -4040, Enum_BufSet.付与, 300, Mathf.RoundToInt(Sta.FMHP * 5));
                }
            }
            else
            {
                Sta.BufSets(Enum_Bufs.復活待機,0,Enum_BufSet.付与,RiviveSecond * 60,0);
                Flag = true;
            }
        }
    }
}
