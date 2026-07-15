
namespace Obj
{
    using UnityEngine;
    using Player;
    using State;
    using FNet;
    using static FNet.Fusion_Manager;
    using static FNet.Fusion_Reliable;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_GlobalState;
    using static Datas.Data_Get;
    public class Obj_RespawnePoint :Obj_ActionObject
    {
        public override void PlayAction(Player_State PSta)
        {
            if (!CanControl(PSta.Object)) return;
            LPlayerVal.RespawnePos = transform.position;
            Fusion_Chat.LocalMessage(Enum_MesID.System, "復活地点を変更しました", "");
        }
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            var pac = other.GetComponent<Player_Action>();
            if (pac == null) return;
            if (!CanControl(pac.PSta.Object)) return;
            PlayerRefs(pac.PSta);
        }
        void PlayerRefs(Player_State psta)
        {
            var dobj = Instantiate(DB.DamObj, psta.PosGet, Quaternion.identity);
            TeamGet_Str((int)psta.CommonValues.Team, out var oTeamCol);
            dobj.TextSet(psta,psta,100,"StateFull", oTeamCol, new Color(0.25f, 1, 0.25f));
            psta.ChangeValues.Bufs.Clear();
            psta.HP = psta.F_MHP;
            psta.MP = psta.F_MMP;
            psta.ST = psta.F_MST;
            psta.EX = State_StateBase.EXMax;
            psta.SkillCTs.Clear();
        }
    }
}
