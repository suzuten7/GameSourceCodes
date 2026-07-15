using Photon.Pun;
using UnityEngine;
using static GameInfos;
using static DataBase;

public class WhaleBot : MonoBehaviourPun
{
    [SerializeField] Rigidbody Rig;
    [SerializeField] float SerchRange;
    [SerializeField] int SensingTimeM;
    [SerializeField] float GroundSpeed = 5;
    [SerializeField] float WaterSpeed = 15;
    [SerializeField] int NoSerchRotF;
    [SerializeField] Vector3 RotV;
    [SerializeField] int AtkDam;
    [SerializeField] float AtkKB;
    [SerializeField] int AtkCT;
    int SenTime = 0;
    int NSRF = 0;
    int ACT = 0;
    GameObject SenTarget=null;

    bool WaterCheck;
    public bool Water;
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (GInfo.GameTime >= GInfo.TLimits)
        {
            PhotonNetwork.Destroy(gameObject);
            return;
        }
        Water = WaterCheck;
        WaterCheck = false;
        var RigVect = Rig.linearVelocity;

        if (!Water) RigVect += Physics.gravity * 0.02f;
        else
        {
            RigVect *= 0.95f;
            RigVect += Physics.gravity * 0.002f;
        }

        SenTime--;
        NSRF--;
        ACT--;
        if(SenTime <= SensingTimeM / 3f)
        {
            SenTarget = null;
            float Dism = SerchRange;
            for(int i=0;i<Planctons.Length;i++)
            {
                if (Planctons[i] == null) continue;
                var PMo = Planctons[i].GetComponent<Player_Move>();
                if (PMo != null && PMo.PSta.HP <= 0) continue;
                float Dis = Vector3.Distance(transform.position, Planctons[i].transform.position);
                bool Checks = true;
                if(Dism > Dis)
                {
                    var RayCheck = Physics.RaycastAll(transform.position, Planctons[i].transform.position - transform.position,Dis);
                    foreach(var RHit in RayCheck)
                    {
                        if (RHit.collider.tag == "Wall")
                        {
                            Debug.DrawRay(transform.position, (Planctons[i].transform.position - transform.position), Color.red, 0.6f);
                            Debug.DrawRay(transform.position, (Planctons[i].transform.position - transform.position).normalized * RHit.distance, Color.green, 0.6f);
                            Checks = false;
                            break;
                        }
                    }
                    if (Checks)
                    {
                        Debug.DrawRay(transform.position, Planctons[i].transform.position - transform.position, Color.green, 0.6f);
                        Dism = Dis;
                        SenTarget = Planctons[i];
                    }
                }
            }
            if(SenTarget!=null)SenTime = SensingTimeM;
            else SenTime = SensingTimeM/2;
        }
        float MoveSpeed = Water ? WaterSpeed : GroundSpeed;
        if (SenTime > 0 && SenTarget != null)
        {
            Vector3 MVects = SenTarget.transform.position - transform.position;
            RigVect += MVects.normalized * 0.01f *  MoveSpeed;
        }
        else
        {
            if (NSRF <= 0)
            {
                NSRF = NoSerchRotF;
                transform.eulerAngles += new Vector3(Random.Range(-RotV.x, RotV.x), Random.Range(-RotV.y, RotV.y), Random.Range(-RotV.z, RotV.z));
            }
            Vector3 MVects = transform.forward;
            RigVect += MVects.normalized * 0.01f * MoveSpeed;
        }
        Rig.linearVelocity = RigVect;
        transform.LookAt(transform.position + RigVect);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine) return;
        if (other.tag == "Fugitive")
        {
            if (ACT > 0) return;
            var PMo = other.GetComponent<Player_Move>();
            if (PMo != null)
            {
                if (PMo.PSta.HP <= 0) return;
                ACT = AtkCT;
                PMo.PSta.Damage(AtkDam, DameTypeE.接触);
                PMo.PSta.KB(transform.forward * AtkKB * 0.01f);
            }
            else
            {
                var DesRPC = other.GetComponent<DestoryRPCs>();
                if (DesRPC != null && !DesRPC.Dels)
                {
                    ACT = AtkCT;
                    DesRPC.Destory();
                }
            }

        }
        if (other.tag == "Water") WaterCheck = true;

    }

}
