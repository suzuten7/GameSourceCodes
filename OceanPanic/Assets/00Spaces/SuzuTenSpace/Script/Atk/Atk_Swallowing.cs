using Photon.Pun;
using UnityEngine;

public class Atk_Swallowing : MonoBehaviourPun
{
    #region エディタ変数
    public Player_Pilot_WhalesAction Use;
    public int Damage;
    public float KBPow;
    [SerializeField] int ScoreAddCT;
    [SerializeField] int HitCT = 0;
    [SerializeField] int RemTime = 60;
    int ScoreCT = 0;
    #endregion
    #region 内部変数
    int CT = 0;
    int RemT = 0;
    #endregion
    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        #region 時間経過
        CT--;
        RemT++;
        ScoreCT--;
        if (RemT >= RemTime) PhotonNetwork.Destroy(gameObject);
        #endregion
    }
    private void OnTriggerStay(Collider other)
    {
        #region チェック用
        if (!photonView.IsMine) return;
        if (CT > 0 && CT != HitCT) return;
        if (other.tag != "Fugitive") return;
        #endregion
        var Plan = other.GetComponent<Player_Move>();
        #region 逃走者
        if (Plan != null)
        {
            if (Plan.PSta.HP <= 0 || Plan.PSta.GostModes) return;
            CT = HitCT;
            Plan.PSta.Damage(Damage);
            Plan.PSta.KB((transform.position - other.transform.position).normalized * KBPow * 0.01f);
            if (Use != null && ScoreCT <= 0)
            {
                ScoreCT = ScoreAddCT;
                Use.PSta.ScoreAdd(1, true);
            }
        }
        #endregion
        #region ダミー
        else
        {
            var RigRpc = other.GetComponent<RigRPCs>();
            if (RigRpc != null)
            {
                CT = HitCT;
                RigRpc.VectAdds((transform.position - other.transform.position).normalized * KBPow * 0.01f);
            }
        }
        #endregion
    }
}