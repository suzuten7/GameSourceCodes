using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
public class Suzuten_KeyTexts : MonoBehaviour
{
    [SerializeField]PlayerInput[] PIs;
    [SerializeField] int PlayerID;
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] string BackTx;
    [SerializeField] string KeyName;
    [SerializeField] string AddTx;
    void LateUpdate()
    {
        PlayerInput PI = null;
        if (PIs != null&&PIs.Length>0)
        {
            for (int i = 0; i < PIs.Length; i++)
            {
                if (PIs[i].playerIndex == PlayerID)
                {
                    PI = PIs[i];
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < Suzuten_PlayerSets.PSs.Length; i++)
            {
                if (Suzuten_PlayerSets.PSs[i] !=null&& Suzuten_PlayerSets.PSs[i].PI.playerIndex == PlayerID)
                {
                    PI = Suzuten_PlayerSets.PSs[i].PI;
                    break;
                }
            }
        }
        if (PI)
        {
            int b = PI.actions[KeyName].GetBindingIndex(InputBinding.MaskByGroup(PI.currentControlScheme));
            Text.text = BackTx + PI.actions[KeyName].GetBindingDisplayString(b) + AddTx;
            //Debug.Log(PI.playerIndex + "[" + KeyName + "]" + PI.currentControlScheme + ":" + b + ":" + Text.text);
        }
    }
}
