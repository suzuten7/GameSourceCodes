
namespace State
{
    using Fusion;
    using UnityEngine;
    using System.Linq;
    using static Datas.Data_Attack;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalState;
    using static GmSystem.GS_GlobalValues;
    public partial class State_StateBase : NetworkBehaviour
    {
        public Class_State_SettingValues SettingValues;
        public Class_State_CommonValues CommonValues;
        public Class_State_BaseValues BaseValues;
        [SerializeField] Class_State_BaseLv1 BaseLv1;
        public Class_State_AddValues AddValues;
        public Class_State_ChangeValues ChangeValues;
        public Class_State_AnimValues AnimValues;

        virtual protected void Start()
        {
            ParentStrage(gameObject, "Enemy");
            StateList.Add(this);
            if(SettingValues.Boss)BossList.Add(this);
            HP = F_MHP;
            MP = F_MMP;
            ST = F_MST;
            EX = 0;
            ChangeValues.LastAtkTime = 9999;
            ChangeValues.LastLookTime = 9999;
            if (ChangeValues.TimeLim > 0) BufSet(new Class_BufAdd
            {
                ID = Enum_Buf.TimeLimit,
                Index = 0,
                Set = Enum_BufSet.Add,
                Times = new Vector2(ChangeValues.TimeLim,0),
                Pows = Vector2Int.zero,
            }) ;
        }
        private void FixedUpdate()
        {
            DeathMoves();
            if (!CanControl(Object))
            {
                NetsLocalSet();
                return;
            }
            BaseUpdate();
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEditor.Selection.objects.Contains(gameObject)) return;
            LVStateChange(true,1);
        }
#endif
        
    }
}
