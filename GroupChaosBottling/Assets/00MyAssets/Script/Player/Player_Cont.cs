using DG.Tweening;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static BattleManager;
using static Manifesto;
using static Statics;
using static DataBase;
public class Player_Cont : MonoBehaviourPun
{
    public bool IsMoblie = false;
    [SerializeField] State_Base Sta;
    [SerializeField] GameObject[] ContObjs;
    [SerializeField] Transform CamObj;
    [SerializeField] PlayerInput PI;
    [SerializeField] Player_Moblie Moblie;
    [SerializeField] UI_CursolCheck CursolCh;
    [SerializeField] GameObject[] MoblieUIs;
    public Vector2 Move;
    public Vector2 Look;
    public bool Jump_Enter;
    public bool Dash_Enter;
    public bool Change_Enter;
    public bool Target_Enter;
    public bool NAtk_Enter;
    public bool NAtk_Stay;
    public int NAtk_StayFl;
    public bool NAtk_Exit;
    public bool S1Atk_Enter;
    public bool S1Atk_Stay;
    public int S1Atk_StayFl;
    public bool S1Atk_Exit;
    public bool S2Atk_Enter;
    public bool S2Atk_Stay;
    public int S2Atk_StayFl;
    public bool S2Atk_Exit;
    public bool EAtk_Enter;
    public bool EAtk_Stay;
    public int EAtk_StayFl;
    public bool EAtk_Exit;

    bool NAtk_ExitT = false;
    bool S1Atk_ExitT = false;
    bool S2Atk_ExitT = false;
    bool EAtk_ExitT = false;

    int SecTime = 0;
    int[] AIInTimes = new int[7];
    int[] AISyTimes = new int[6];
    int[] AIWaTimes = new int[6];

    private void Start()
    {
        if (!photonView.IsMine) return;
        IsMoblie = false;
        if (Application.platform == RuntimePlatform.Android) IsMoblie = true;
        if (Application.platform == RuntimePlatform.IPhonePlayer) IsMoblie = true;
        for(int i=0;i< ContObjs.Length; i++)
        {
            ContObjs[i].SetActive(Sta.PLValues.SubID <= 0);
        }
        for(int i = 0; i < MoblieUIs.Length; i++) MoblieUIs[i].SetActive(IsMoblie);
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        for (int i = 0; i < MoblieUIs.Length; i++) MoblieUIs[i].SetActive(IsMoblie);
        ResetInput();
        int AIMode = Sta.PLValues.SubID <= 0 ? 0 : (Sta.PLValues.Sets.AIGet(Sta.PLValues.Backs).AIMode + 1);
        switch (AIMode)
        {
            case 0: PICont(); PIMoblie(); ExitSet(); break;

            case 1: AI_Cont(0, false); break;
            case 2: AI_Cont(1, false); break;
            case 3: AI_Cont(-1, false); break;

            case 4: AI_Cont(100, true); break;
            case 5: AI_Cont(0, true); break;
            case 6: AI_Cont(-1, true); break;
            case 7: AI_Cont(1, true); break;
        }
        if (AIMode > 0)
        {
            var AISet = Sta.PLValues.Sets.AIGet(Sta.PLValues.Backs);
            bool EnterIn = Random.value <= 0.5f;
            if (AISyTimes[2] > 0 && !AISet.NAtk.Stays) NAtk_Enter = EnterIn; NAtk_Exit = !EnterIn;
            if (AISyTimes[3] > 0 && !AISet.S1Atk.Stays) S1Atk_Enter = EnterIn; S1Atk_Exit = !EnterIn;
            if (AISyTimes[4] > 0 && !AISet.S2Atk.Stays) S2Atk_Enter = EnterIn; S2Atk_Exit = !EnterIn;
            if (AISyTimes[5] > 0 && !AISet.EAtk.Stays) EAtk_Enter = EnterIn; EAtk_Exit = !EnterIn;
        }
        if (SecTime >= 60) SecTime = 0;

    }
    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (Sta.PLValues.SubID > 0)
        {
            SecTime++;
            for (int i = 0; i < AIInTimes.Length; i++)
            {
                AIInTimes[i]++;
            }
            for (int i = 0; i < AISyTimes.Length; i++)
            {
                AISyTimes[i]--;
                AIWaTimes[i]--;
            }
            var AISet = Sta.PLValues.Sets.AIGet(Sta.PLValues.Backs);
            if (AISet.NAtk.Stays)NAtk_Stay = (NAtk_Enter && AISyTimes[2] <= 0) || AISyTimes[2] > 2;
            if (AISet.S1Atk.Stays)S1Atk_Stay = (S1Atk_Enter && AISyTimes[3] <= 0) || AISyTimes[3] > 2;
            if (AISet.S2Atk.Stays)S2Atk_Stay = (S2Atk_Enter && AISyTimes[4] <= 0) || AISyTimes[4] > 2;
            if (AISet.EAtk.Stays)EAtk_Stay = (EAtk_Enter && AISyTimes[5] <= 0) || AISyTimes[5] > 2;
        }
        StayFlSet();
    }
    void ResetInput()
    {
        Move = Vector2.zero;
        Look = Vector2.zero;
        Jump_Enter = false;
        Dash_Enter = false;
        Change_Enter = false;
        NAtk_Enter = false;
        NAtk_Stay= false;
        S1Atk_Enter= false;
        S1Atk_Stay= false;
        S2Atk_Enter = false;
        S2Atk_Stay = false;
        EAtk_Enter = false;
        EAtk_Stay= false;
    }
    void StayFlSet()
    {
        if (NAtk_Stay) NAtk_StayFl++;
        else NAtk_StayFl = 0;
        if (S1Atk_Stay) S1Atk_StayFl++;
        else S1Atk_StayFl = 0;
        if (S2Atk_Stay) S2Atk_StayFl++;
        else S2Atk_StayFl = 0;
        if (EAtk_Stay) EAtk_StayFl++;
        else EAtk_StayFl = 0;
    }
    void ExitSet()
    {
        if (NAtk_Stay) NAtk_ExitT = true;
        else
        {
            NAtk_Exit = NAtk_ExitT;
            NAtk_ExitT = false;
        }
        if (S1Atk_Stay) S1Atk_ExitT = true;
        else
        {
            S1Atk_Exit = S1Atk_ExitT;
            S1Atk_ExitT = false;
        }
        if (S2Atk_Stay) S2Atk_ExitT = true;
        else
        {
            S2Atk_Exit = S2Atk_ExitT;
            S2Atk_ExitT = false;
        }
        if (EAtk_Stay) EAtk_ExitT = true;
        else
        {
            EAtk_Exit = EAtk_ExitT;
            EAtk_ExitT = false;
        }
    }
    void PICont()
    {
        Move = PI.actions["Move"].ReadValue<Vector2>();
        Look = PI.actions["Look"].ReadValue<Vector2>();
        Jump_Enter = PI.actions["Jump"].triggered;
        Dash_Enter = PI.actions["Dash"].triggered;
        Change_Enter = PI.actions["Change"].triggered;
        Target_Enter = PI.actions["Target"].triggered;
        NAtk_Enter = PI.actions["N_Atk"].triggered;
        NAtk_Stay = PI.actions["N_Atk"].IsPressed();
        S1Atk_Enter = PI.actions["S1_Atk"].triggered;
        S1Atk_Stay = PI.actions["S1_Atk"].IsPressed();
        S2Atk_Enter = PI.actions["S2_Atk"].triggered;
        S2Atk_Stay = PI.actions["S2_Atk"].IsPressed();
        EAtk_Enter = PI.actions["E_Atk"].triggered;
        EAtk_Stay = PI.actions["E_Atk"].IsPressed();

        if (PI.currentControlScheme == "Keyboard&Mouse" && SoftwareCursorPositionAdjuster.Vis) Look = Vector2.zero;
        if (PI.currentControlScheme != "Touch" && !CursolCh.Check)
        {
            Jump_Enter = false;
            Dash_Enter = false;
            Change_Enter = false;
            Target_Enter = false;
            NAtk_Enter = false;
            NAtk_Stay = false;
            S1Atk_Enter = false;
            S1Atk_Stay = false;
            S2Atk_Enter = false;
            S2Atk_Stay = false;
            EAtk_Enter = false;
            EAtk_Stay = false;
        }
        if(PI.currentControlScheme != "Keyboard&Mouse" && PI.currentControlScheme != "Touch" && SoftwareCursorPositionAdjuster.Vis)
        {
            Move = Vector2.zero;
        }

    }
    void PIMoblie()
    {
        if (!IsMoblie) return;
        if (Moblie.MoveInputs.magnitude >= 0.1f) Move = Moblie.MoveInputs;
        if (Moblie.LookInputs.magnitude >= 0.1f) Look = Moblie.LookInputs;
        if (Moblie.Jump_Enter) Jump_Enter = true;
        if (Moblie.Dash_Enter) Dash_Enter = true;
        if (Moblie.Change_Enter) Change_Enter = true;
        if (Moblie.Target_Enter) Target_Enter = true;
        if (Moblie.Atk_Enters[0]) NAtk_Enter = true;
        if (Moblie.Atk_Stays[0]) NAtk_Stay = true;
        if (Moblie.Atk_Enters[1]) S1Atk_Enter = true;
        if (Moblie.Atk_Stays[1]) S1Atk_Stay = true;
        if (Moblie.Atk_Enters[2]) S2Atk_Enter = true;
        if (Moblie.Atk_Stays[2]) S2Atk_Stay = true;
        if (Moblie.Atk_Enters[3]) EAtk_Enter = true;
        if (Moblie.Atk_Stays[3]) EAtk_Stay = true;

    }

    void AI_Cont(int BossCheck, bool Sapo)
    {
        if (BTManager.End) return;
        Look = Vector2.zero;
        var AISet = Sta.PLValues.Sets.AIGet(Sta.PLValues.Backs);
        Change_Enter = SecTime >= 60 && Random.value <= AISet.ChangePer / 1000f;
        int TimeInVal = (AISet.ChangeTime < 610) ? (AISet.ChangeTime * 6) : int.MaxValue;
        if (AIInTimes[6] >= TimeInVal) Change_Enter = true;
        if (Change_Enter)
        {
            Debug.Log("チェンジ");
            for (int i = 0; i < AIInTimes.Length; i++)
            {
                AIInTimes[i] = 0;
            }
            for (int i = 0; i < AISyTimes.Length; i++)
            {
                AISyTimes[i] = 0;
                AIWaTimes[i] = 0;
            }
        }


        NAtk_Enter = false;
        S1Atk_Enter = false;
        S2Atk_Enter = false;
        EAtk_Enter = false;

        if (!Sapo) AI_TargetSet(Sta.PosGet(), false, BossCheck);
        else
        {
            if (BossCheck <= 1) AI_TargetSet(Sta.PosGet(), false, BossCheck);
            else Sta.Target = Sta;
            if (Sta.Target != null) AI_TargetSet(Sta.Target.PosGet(), true, 0);
        }
        var PDis = HoriDistance(Sta.PosGet(), BTManager.LocalCharas[0].PosGet());
        var PLDisCheck = PDis > AISet.PLDis * 0.01f;
        int PerType = 0;
        if (Sta.Target != null)
        {
            var RayMax = 250f;
            var TDis = RayMax;
            var MPos = Sta.PosGet();
            MPos.y = Sta.Target.PosGet().y;
            foreach (var RayHit in Physics.RaycastAll(MPos, Sta.Target.PosGet() - MPos, TDis, DB.HitLayer))
            {
                bool Check = false;
                foreach (var Tra in Sta.Target.transform.GetComponentsInChildren<Collider>())
                {
                    if (RayHit.collider == Tra) Check = true;
                }
                if (!Check) continue;
                TDis = Mathf.Min(TDis, Mathf.Max(0, RayHit.distance - 1f));
            }
            if (TDis >= RayMax) TDis = HoriDistance(Sta.PosGet(), Sta.Target.PosGet());

            if (TDis > AISet.Range * 0.01f)
            {
                Move = new Vector2(0f, 1f);
                PerType = 1;
            }
            else if (TDis <= AISet.Range * 0.01f * 0.75f)
            {
                Move = new Vector2(0, -1f);
            }
            CamObj.LookAt(Sta.Target.PosGet());
        }
        if (PLDisCheck)
        {
            Move = new Vector2(0, 1f);
            CamObj.LookAt(BTManager.LocalCharas[0].PosGet());
            PerType = 2;
        }
        AI_UseCheck(AISet.Jump, PerType, 0);
        AI_UseCheck(AISet.Dash, PerType, 1);
        AI_UseCheck(AISet.NAtk, PerType, 2);
        AI_UseCheck(AISet.S1Atk, PerType, 3);
        AI_UseCheck(AISet.S2Atk, PerType, 4);
        AI_UseCheck(AISet.EAtk, PerType, 5);

        if (AISyTimes[0] > 2) Jump_Enter = true;
        if (AISyTimes[1] > 2) Dash_Enter = true;
    }
    void AI_UseCheck(Class_Save_AIActions AIAc,int PerType, int Slot)
    {
        bool Check = false;
        int TimeInVal = (AIAc.TimeIn < 610) ? (AIAc.TimeIn * 6) : int.MaxValue;
        if (AIInTimes[Slot] > TimeInVal) Check = true;
        else
        {
            int Per = AIAc.PerBase;
            switch (PerType)
            {
                case 1: Per = AIAc.PerOuR; break;
                case 2: Per = AIAc.PerPLD; break;
            }
            Check = SecTime >= 60 && Random.value < Per * 0.001f;
        }
        if (Slot >= 0 && AISyTimes[Slot] > 0) Check = false;
        if (AIWaTimes[Slot] > 0) Check = false;
        if (Check)
        {
            switch (Slot)
            {
                default: Jump_Enter = true; break;
                case 1: Dash_Enter = true; break;
                case 2: NAtk_Enter = true; break;
                case 3: S1Atk_Enter = true; break;
                case 4: S2Atk_Enter = true; break;
                case 5: EAtk_Enter = true; break;
            }
            AIInTimes[Slot] = 0;
            if (Slot >= 0) AISyTimes[Slot] = (AIAc.TimeStay < 610) ? Mathf.Max(AIAc.TimeStay * 6,3) : int.MaxValue;
            AIWaTimes[Slot] = AIAc.TimeWait;
        }
    }
    void AI_TargetSet(Vector3 Pos,bool FTeam, int BossCheck)
    {
        float NearDis = -1;
        Sta.Target = null;
        if (Sta.Fixation != null && Sta.BufCheck(Enum_Bufs.標的固定))
        {
            Sta.Target = Sta.Fixation;
            return;
        }
        if (Sta.PLValues.Sets.AIGet(Sta.PLValues.Backs).PLTarget && BTManager.LocalCharas[0].TargetHit != null)
        {
            Sta.Target = BTManager.LocalCharas[0].TargetHit.Sta;
            return;
        }
        foreach (var TSta in BTManager.StateList)
        {
            if (TSta == null) continue;
            if (!TeamCheck(Sta, TSta, !FTeam, FTeam, false)) continue;
            if (BossCheck >= 1 && !TSta.Boss) continue;
            if (BossCheck <= -1 && TSta.Boss) continue;
            if (TSta.HP <= 0) continue;
            if (!TSta.gameObject.activeInHierarchy) continue;
            float Dis = HoriDistance(Pos, TSta.PosGet());
            if (NearDis < 0 || NearDis > Dis)
            {
                NearDis = Dis;
                Sta.Target = TSta;
            }
        }
        if (BossCheck != 0 && Sta.Target == null) AI_TargetSet(Pos,FTeam, 0);
    }
}
