using System.Collections.Generic;
using UnityEngine;
using static BattleManager;
using static Manifesto;

public class PCamLockdObj : MonoBehaviour
{
    public bool SetPos;
    public Enum_PCamLock_Look SetLook;


    private void Start()
    {
        BTManager.PCamLockdList.Add(this);
    }
}
