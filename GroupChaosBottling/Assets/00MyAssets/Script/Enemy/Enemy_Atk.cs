using Photon.Pun;
using UnityEngine;
using static Statics;
using static Manifesto;
using static BattleManager;
public class Enemy_Atk : MonoBehaviourPun
{
    [SerializeField] State_Base Sta;
    [SerializeField] Data_EnemyAI[] EnemyAI;
    [SerializeField] int timer;
    [SerializeField] int Patturn = 0;
    int StayFl = 0;

    private void Start()
    {
        timer = 0;
        Patturn = 0;
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        var EAI = EnemyAI[Patturn];
        if (Sta.Target == null)
        {
            if (EAI.NoTargetResetTime) timer = 0;
        }
        if (timer == 0) Patturn = Random.Range(0, EnemyAI.Length);
        timer++;
        if (timer > EAI.TimerLim) timer = 0;
        bool Inputs = false;
        bool Stays = false;
        for(int i = 0; i < EAI.AtkAIs.Length; i++)
        {
            var AtkAI = EAI.AtkAIs[i];
            bool InputIfs = Sta.Target != null && V3IntTimeCheck(timer, (Vector3Int)AtkAI.TimeIf);
            for(int j = 0; j < AtkAI.OtherIfs.Length; j++)
            {
                if (!InputIfs) break;
                var OtherIf = AtkAI.OtherIfs[j];
                float Vals;
                switch (OtherIf.Ifs)
                {
                    case Enum_OtherIfs.HP割合_x以下:
                        if ((float)Sta.HP/ Sta.MHP > OtherIf.Val.x * 0.01f) InputIfs = false;
                        break;
                    case Enum_OtherIfs.HP割合_x以上:
                        if ((float)Sta.HP / Sta.MHP > OtherIf.Val.x * 0.01f) InputIfs = false;
                        break;
                    case Enum_OtherIfs.ターゲット距離_x以上:
                        Vals = HoriDistance(Sta.PosGet(), Sta.Target.PosGet());
                        if (Vals < OtherIf.Val.x) InputIfs = false;
                        break;
                    case Enum_OtherIfs.ターゲット距離_x以下:
                        Vals = HoriDistance(Sta.PosGet(), Sta.Target.PosGet());
                        if (Vals > OtherIf.Val.x) InputIfs = false;
                        break;
                    case Enum_OtherIfs.カオスバフ無し:
                        if(BTManager.Chaos) InputIfs = false;
                        break;
                    case Enum_OtherIfs.カオスバフ有り:
                        if (!BTManager.Chaos) InputIfs = false;
                        break;
                }
            }
            if (InputIfs)Inputs = true;
            if (InputIfs && AtkAI.StayInput)Stays = true;
            Sta.AtkInput(AtkAI.AtkSlot, AtkAI.AtkD,InputIfs);
        }
        if (Sta.Target == null)
        {
            Inputs = false;
            Stays = false;
        }
        Sta.AtkBranchSet(Sta.AtkD, Inputs, Stays, StayFl, Inputs && !Stays);
        if (Stays) StayFl++;
        else StayFl = 0;
    }
}
