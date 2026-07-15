namespace State
{
    using UnityEngine;
    using static GmSystem.GS_GlobalState;
    using static Datas.Data_Get;
    public class State_HumanAnim : MonoBehaviour
    {
        public State_StateBase Sta;
        public Animator Anim;
        [SerializeField] AnimatorOverrideController AOvCont;

        [SerializeField] int MoveAID;
        [SerializeField] int LeftAID;
        [SerializeField] int LeftACo;
        [SerializeField] float LeftASpeed;
        [SerializeField] int RightAID;
        [SerializeField] int RightACo;
        [SerializeField] float RightASpeed;
        [SerializeField] int SkillAID;
        [SerializeField] int SkillACo;
        [SerializeField] float SkillASpeed;
        [SerializeField] int OtherAID;

        int oLaid = -1;
        int oLaco = -1;
        int oRaid = -1;
        int oRaco = -1;
        int oSaid = -1;
        int oSaco = -1;
        AnimatorOverrideController cpAOC;
        void Update()
        {
            if (cpAOC == null)
            {
                cpAOC = new AnimatorOverrideController(AOvCont != null ? AOvCont : DB.AOvCont);
                Anim.runtimeAnimatorController = cpAOC;
            }
            if (Sta != null)
            {
                MoveAID = Sta.AnimValues.MoveID;
                LeftAID = Sta.AnimValues.LAtkID;
                LeftACo = Sta.AnimValues.LAtkCo;
                LeftASpeed = Sta.AnimValues.LAtkSpeed * Sta.ValGet(Enum_StateAddsType.AtkSpeed) * 0.01f;
                RightAID = Sta.AnimValues.RAtkID;
                RightACo = Sta.AnimValues.RAtkCo;
                RightASpeed = Sta.AnimValues.RAtkSpeed * Sta.ValGet(Enum_StateAddsType.AtkSpeed) * 0.01f;
                SkillAID = Sta.AnimValues.SAtkID;
                SkillACo = Sta.AnimValues.SAtkCo;
                SkillASpeed = Sta.AnimValues.SAtkSpeed * Sta.ValGet(Enum_StateAddsType.AtkSpeed) * 0.01f;
                OtherAID = Sta.AnimValues.OtherID;
            }
            Anim.SetInteger("MoveID", MoveAID);
            Anim.SetFloat("LASpeed", LeftASpeed);
            Anim.SetFloat("RASpeed", RightASpeed);
            Anim.SetFloat("SASpeed", SkillASpeed);
            Anim.SetInteger("OtherID", OtherAID);
            if (oLaid != LeftAID || oLaco != LeftACo)
            {
                oLaid = LeftAID;
                oLaco = LeftACo;
                cpAOC["LeftAttack"] = DB.WeponAttackClips[Mathf.Clamp(LeftAID, 0, DB.WeponAttackClips.Count - 1)];
                Anim.CrossFade("LeftAttack", 0.1f, 1,0.0f);
            }

            if (oRaid != RightAID || oRaco != RightACo)
            {
                oRaid = RightAID;
                oRaco = RightACo;
                cpAOC["RightAttack"] = DB.WeponAttackClips[Mathf.Clamp(RightAID, 0, DB.WeponAttackClips.Count - 1)];
                Anim.CrossFade("RightAttack", 0.1f, 2, 0.0f);
            }
            if (oSaid != SkillAID || oSaco != SkillACo)
            {
                oSaid = SkillAID;
                oSaco = SkillACo;
                cpAOC["SkillAttack"] = DB.SkillAttackClips[Mathf.Clamp(SkillAID, 0, DB.SkillAttackClips.Count - 1)];
                Anim.CrossFade("SkillAttack", 0.1f, 3, 0.0f);
            }
        }
    }
}
