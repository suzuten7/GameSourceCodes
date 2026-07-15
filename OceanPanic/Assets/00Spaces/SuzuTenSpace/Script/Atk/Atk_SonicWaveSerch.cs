using Photon.Pun;
using UnityEngine;

public class Atk_SonicWaveSerch : MonoBehaviourPun
{
    #region 変数
    [SerializeField] GameObject MakerObj;
    [SerializeField] int RemTime = 60;
    int RemT = 0;
    #endregion
    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        #region 時間経過破棄
        RemT++;
        if (RemT >= RemTime) PhotonNetwork.Destroy(gameObject);
        #endregion
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (other.tag != "Fugitive") return;
        var Plac = other.GetComponent<Player_F_PlanctonAction>();
        if (Plac != null && (Plac.PSta.HP <= 0 || Plac.PSta.GostModes)) return;
        Instantiate(MakerObj,other.transform);
    }
}