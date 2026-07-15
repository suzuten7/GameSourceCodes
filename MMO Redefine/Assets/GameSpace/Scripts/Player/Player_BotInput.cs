namespace Player
{
    using State;
    using System.Collections.Generic;
    using UnityEngine;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;

    public class Player_BotInput : Player_Controle
    {
        [SerializeField] Player_State PSta;
        [SerializeField] Transform CamTrans;
        [SerializeField] float BotMaxRange = 60;
        public int Mode;
        public GameObject Target;
        int atkFBTimes;
        int shortCutTimes;
        int randMoveTimes;
        int randTargetTimes;
        Vector2 randMoveVect;

        int useid = -1;
       
        protected override void Start(){}
        private void FixedUpdate()
        {
            if (!CanControl(PSta.Object)) return;
            if (PSta.BotID < 0)
            {
                PSta.Cont = PCont;
                return;
            }
            atkFBTimes++;
            shortCutTimes++;
            randMoveTimes++;
            randTargetTimes++;
        }
        protected override void Update()
        {
            if (!CanControl(PSta.Object)) return;
            if (PSta.BotID < 0)
            {
                PSta.Cont = PCont;
                return;
            }
            PSta.Cont = this;
            InputClear();
            if (PSta.BotID == 0) return;
            V2_Look = Vector2.zero;
            Tr_LockOn = true;

            BotMove(Mode);
            Atks();
            CamTrans.position = PSta.PosGet;
            CamTrans.LookAt(PSta.TargetPosGet);


        }
        void BotMove(int ID)
        {
            var BotOp = LPlayerCharas[PSta.CharaID].BotOption;
            var pteam = MyPlayer.CommonValues.Team == PSta.CommonValues.Team;
            if (pteam)
            {
                if (ID == 0) Target = MyPlayer.TargetObjGet;
                if (ID == 2 && Target == null || Target == MyPlayer.gameObject) Target = MyPlayer.TargetObjGet;
            }
            else
            {
                if (randTargetTimes >= BotOp.RandModeTargetSec * 6)
                {
                    randTargetTimes = 0;
                    var near = BotMaxRange;
                    foreach (var st in StateList)
                    {
                        if (st == null) continue;
                        if (st.HP <= 0) continue;
                        if (PSta.TeamCheck(st.CommonValues.Team) == GmSystem.GS_GlobalState.Enum_TeamCheck.Friend) continue;
                        var dis = Vector3.Distance(PSta.PosGet, st.PosGet);
                        if (near > dis)
                        {
                            near = dis;
                            Target = st.gameObject;
                        }
                    }
                }
            }
            if (ID == 1 || Target == null || (pteam && MyPlayer.TargetObjGet == null)) Target = MyPlayer.gameObject;

            var tmdis = Vector3.Distance(PSta.PosGet, PSta.TargetPosGet);
            var tpdis = Vector3.Distance(MyPlayer.PosGet, PSta.TargetPosGet);

            if (ID!=1 && ((pteam && Target == MyPlayer.gameObject) || tpdis > BotMaxRange))
            {
                BotMove(1);
                return;
            }
            switch (ID)
            {
                case 0:
                    if (randMoveTimes >= BotOp.RandModeMoveSec * 6)
                    {
                        randMoveTimes = 0;
                        randMoveVect = new Vector2(Random.value - 0.5f, Random.value - 0.5f) * 2f;
                    }
                    if (tmdis > BotOp.MoveRange.y * 0.1f)
                    {
                        V2_Move = new Vector2(0.2f * randMoveVect.x, 1.0f);
                        In_Dash = true;
                        Stay_Dash = true;
                    }
                    else if (tmdis < BotOp.MoveRange.x * 0.1f)
                    {
                        V2_Move = new Vector2(0.2f * randMoveVect.x, -1.0f);
                    }
                    else V2_Move = randMoveVect;
                    break;
                case 1:
                    if (tmdis > BotMaxRange)
                    {
                        var vect = PSta.PosGet - MyPlayer.PosGet;
                        var pos = MyPlayer.PosGet + vect.normalized * 3;
                        PSta.SettingValues.Rig.position = pos;
                    }
                    else if (tmdis > 4f) V2_Move = new Vector2(0.0f, 0.3f);
                    else V2_Move = new Vector2(0.0f, 0.0f);
                    break;
                case 2:
                    if(randTargetTimes >= BotOp.RandModeTargetSec * 6)
                    {
                        randTargetTimes = 0;
                        var near = BotMaxRange;
                        foreach (var st in StateList)
                        {
                            if (st == null) continue;
                            if (st.HP <= 0) continue;
                            if (PSta.TeamCheck(st.CommonValues.Team) == GmSystem.GS_GlobalState.Enum_TeamCheck.Friend) continue;
                            var dis = Vector3.Distance(PSta.PosGet, st.PosGet);
                            if (near > dis)
                            {
                                near = dis;
                                Target = st.gameObject;
                            }
                        }
                    }
                    if (randMoveTimes >= BotOp.RandModeMoveSec * 6)
                    {
                        randMoveTimes = 0;
                        randMoveVect = new Vector2(Random.value - 0.5f, Random.value - 0.5f) * 2f;
                    }
                    V2_Move = randMoveVect;
                    break;
            }
        }

        void Atks()
        {
            var BotOp = LPlayerCharas[PSta.CharaID].BotOption;
            var NoEnemy = Target == null || (MyPlayer.CommonValues.Team == PSta.CommonValues.Team && Target == MyPlayer.gameObject);
            In_Atk1 = !NoEnemy;
            Stay_Atk1 = !NoEnemy;
            In_Atk2 = !NoEnemy;
            Stay_Atk2 = !NoEnemy;
            if (!NoEnemy)
            {
                if (shortCutTimes >= BotOp.ShortCutTrySec * 6)
                {
                    shortCutTimes = 0;
                    if (BotOp.ShortCutChance * 0.1f >= Random.value * 100) ShortCutUse();
                }
                if (atkFBTimes >= BotOp.AtkFBTrySec * 6)
                {
                    atkFBTimes = 0;
                    if (BotOp.AtkFBChance * 0.1f >= Random.value * 100) In_AtkFBC = true;
                }
            }
            else
            {
                useid = -1;
                atkFBTimes = 0;
                shortCutTimes = 0;
            }
        }
        void ShortCutUse()
        {
            var BotOp = LPlayerCharas[PSta.CharaID].BotOption;
            var Set = LPlayerCharas[PSta.CharaID].BotSets;
            var id = 0;
            switch (BotOp.ShortCutUse)
            {
                case 1:
                    var rlist = new List<int>();
                    for(int i = 0; i < Set.Count; i++) if (Set[i] > 0)rlist.Add(i);
                    if(rlist.Count > 0) id = rlist[Random.Range(0, rlist.Count)] + 1;
                    break;
                case 2:
                    for(int i = 0; i < Set.Count; i++)
                    {
                        useid = (int)Mathf.Repeat(useid + 1, Set.Count);
                        if (Set[useid] > 0)
                        {
                            id = useid + 1;
                            break;
                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < Set.Count / 5; i++)
                    {
                        useid = (int)Mathf.Repeat(useid + 1, Set.Count / 5);
                        var slist = new List<int>();
                        for (int k = useid * 5; k < (useid+1) * 5; k++) if (Set[k] > 0) slist.Add(k);
                        if (slist.Count > 0)
                        {
                            id = slist[Random.Range(0, slist.Count)] + 1;
                            break;
                        }
                    }
                    break;
            }
            In_ShortCutSet = id;
        }
    }
}

