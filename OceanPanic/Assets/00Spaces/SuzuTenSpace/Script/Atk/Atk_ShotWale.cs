using Photon.Pun;
using UnityEngine;
using static DataBase;
using static GameInfos;

public class Atk_ShotWale : MonoBehaviourPun
{
    public Player_Visit_WhalesAction Use;
    [SerializeField] int PlHitAddScore;
    [SerializeField] int DmHitAddScore;
    [SerializeField] Rigidbody Rig;
    [SerializeField] int Damage;
    [SerializeField] float AtkKB;
    [SerializeField] float AddSpeed;
    [SerializeField] int HitMarkerTime;
    [SerializeField] GameObject HitEffect;
    [SerializeField] float HitRange;
    [SerializeField] LayerMask HitLayer;
    [SerializeField] int DeleteTime = 0;
    [SerializeField] GameObject CreateItem;
    public bool ItemCres = false;
    int Times = 0;
    bool Hitd = false;
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        Rig.linearVelocity += Rig.linearVelocity.normalized * AddSpeed * 0.01f;
        Times++;
        if (Times >= DeleteTime&&!Hitd)
        {
            Hits();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (Hitd) return;
        if (other.tag == "Fugitive")
        {
            Hits();
        }
        if(other.tag == "Wall")
        {
            Hits();
        }
    }
    void Hits()
    {
        Hitd = true;
        PhotonNetwork.Instantiate(HitEffect.name, transform.position, Quaternion.identity);

        foreach (var Hits in Physics.OverlapSphere(transform.position, HitRange, HitLayer))
        {
            var PMo = Hits.GetComponent<Player_Move>();
            if (PMo != null)
            {
                if (PMo.PSta.HP <= 0) return;
                PMo.PSta.Damage(Damage, DameTypeE.接触);
                PMo.PSta.KB(transform.forward * AtkKB * 0.01f);
                PMo.PSta.HitMarkerSet(HitMarkerTime);
                if (Use != null) Use.PSta.ScoreAdd(PlHitAddScore, true);
            }
            else
            {
                var DesRPC = Hits.GetComponent<DestoryRPCs>();
                if (DesRPC != null && !DesRPC.Dels)
                {
                    DesRPC.Destory();
                    if (Use != null) Use.PSta.ScoreAdd(DmHitAddScore, true);
                }
            }
        }
        if (ItemCres)
        {
            var PItInsObj = PhotonNetwork.Instantiate(CreateItem.name, transform.position, Quaternion.identity);
            Ev_Message_Send("Pアイテムが作られた!!!", MessageE.鬼);
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
