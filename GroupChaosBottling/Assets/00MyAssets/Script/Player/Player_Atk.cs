using Photon.Pun;
using UnityEngine;
using static DataBase;
using static PlayerValue;
public class Player_Atk :MonoBehaviourPun
{
    [SerializeField] State_Base Sta;
    [SerializeField] Player_Cont PCont;

    private void Update()
    {
        if (!photonView.IsMine) return;
        if (PCont.Change_Enter)
        {
            var SPF = Sta.SP;
            Sta.SP = Sta.SPB;
            Sta.SPB = SPF;
            Sta.PLValues.Backs = !Sta.PLValues.Backs;
        }
        var Atks = Sta.PVal_AtkGet(Sta.PLValues.Backs);
        Sta.AtkInput(0, DB.N_Atks[Atks.N_AtkID], PCont.NAtk_Enter);
        Sta.AtkInput(1, DB.S_Atks[Atks.S1_AtkID], PCont.S1Atk_Enter);
        Sta.AtkInput(2, DB.S_Atks[Atks.S2_AtkID], PCont.S2Atk_Enter);
        Sta.AtkInput(10, DB.E_Atks[Atks.E_AtkID], PCont.EAtk_Enter);
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        var Atks = Sta.PVal_AtkGet(Sta.PLValues.Backs);
        switch (Sta.AtkSlot)
        {
            case 0:
                Sta.AtkBranchSet(DB.N_Atks[Atks.N_AtkID], PCont.NAtk_Enter, PCont.NAtk_Stay, PCont.NAtk_StayFl, PCont.NAtk_Exit);
                break;
            case 1:
                Sta.AtkBranchSet(DB.S_Atks[Atks.S1_AtkID], PCont.S1Atk_Enter, PCont.S1Atk_Stay, PCont.S1Atk_StayFl, PCont.S1Atk_Exit);
                break;
            case 2:
                Sta.AtkBranchSet(DB.S_Atks[Atks.S2_AtkID], PCont.S2Atk_Enter, PCont.S2Atk_Stay, PCont.S2Atk_StayFl, PCont.S2Atk_Exit);
                break;
            case 10:
                Sta.AtkBranchSet(DB.E_Atks[Atks.E_AtkID], PCont.EAtk_Enter, PCont.EAtk_Stay, PCont.EAtk_StayFl, PCont.EAtk_Exit);
                break;
        }
        Sta.SP = Mathf.Min(Sta.SP,DB.E_Atks[Sta.PVal_AtkGet(Sta.PLValues.Backs).E_AtkID].SPUse);
        Sta.SPB = Mathf.Min(Sta.SPB, DB.E_Atks[Sta.PVal_AtkGet(!Sta.PLValues.Backs).E_AtkID].SPUse);
    }
}
