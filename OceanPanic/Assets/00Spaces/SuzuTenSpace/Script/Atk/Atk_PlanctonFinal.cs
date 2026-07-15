using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Atk_PlanctonFinal : MonoBehaviourPun
{
    public Player_F_PlanctonAction Use;
    [SerializeField] int AddScore;
    [SerializeField] int DeleteTime;
    [SerializeField] int StanTime;
    [SerializeField] int HitWaitTime;
    List<Player_Pilot_WhalesAction> HitList = new List<Player_Pilot_WhalesAction>();
    int Times = 0;
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        Times++;
        if (Times >= DeleteTime) PhotonNetwork.Destroy(gameObject);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine) return;
        if (Times < HitWaitTime) return;
        var Oni_Pi = other.GetComponent<Player_Pilot_WhalesAction>();
        if (Oni_Pi != null&& !HitList.Contains(Oni_Pi))
        {
            HitList.Add(Oni_Pi);
            Oni_Pi.PSta.SpakeAdd(StanTime);
            GameInfos.GInfo.Messages.TryAdd("プランクトンファイナルが命中!!!", 0);
            if (Use!=null) Use.PSta.ScoreAdd(AddScore);

        }
    }
}
