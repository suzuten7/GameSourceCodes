using Photon.Pun;
using UnityEngine;

public class Net_GameTimeOut : MonoBehaviourPunCallbacks
{
    [Tooltip("タイムアウトまでの秒数"),SerializeField] float TimeLim;
    float times = 0;
    void Update()
    {
        if(SceneChangePanel.SChange == null) SceneChangePanel.SceneSet(0);
        if (!PhotonNetwork.InRoom)
        {
            times += Time.unscaledDeltaTime;
            if (times > TimeLim)
            {
                SceneChangePanel.SceneSet(0);
            }
        }
        else times = 0;
    }
    public override void OnLeftRoom()
    {
        times = TimeLim;
    }
}
