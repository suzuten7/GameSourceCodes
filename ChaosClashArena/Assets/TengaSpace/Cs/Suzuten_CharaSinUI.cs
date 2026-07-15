using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Suzuten_PlayerSets;
using static Suzuten_SelectUI;
public class Suzuten_CharaSinUI : MonoBehaviour
{
    public int PID;
    public int IDs;
    public Suzuten_SelectUI SUI;
    public TextMeshProUGUI CharaNameTx;
    public Image BackImage;
    public RawImage CharaImage;
    public void Clicks()
    {
        if (SUI.CUIs[PID].ACUIss[2].activeSelf)
        {
            if (!SUI.SelACSetB[PID])
            {
                SUI.SelACSetSlot[PID] = ACSetID[PID, SUI.SelACID[PID]];
            }
            else
            {
                int Used = -1;
                if (SUI.SelACSetSlot[PID] >= 0)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (SUI.SelACSetSlot[PID] == ACSetID[PID, k]) Used = k;
                    }
                }
                if (Used < 0) ACSetID[PID, SUI.SelACID[PID]] = SUI.SelACSetSlot[PID];
                else
                {
                    int SlotB = ACSetID[PID, SUI.SelACID[PID]];
                    ACSetID[PID, SUI.SelACID[PID]] = SUI.SelACSetSlot[PID];
                    ACSetID[PID, Used] = SlotB;
                }
                ACIDSave(SUI.DB, PID);
            }
            SUI.SelACSetB[PID] = !SUI.SelACSetB[PID];
        }
        int Charay = (SUI.DB.Charas.Length+RaCharaCo) / SUI.CharaXcounts;
        Vector2Int Slot = SelectSlots[PID];
        if (IDs >= 0)
        {
            if (IDs < SUI.CharaXcounts) Slot.y -= 1;
            if (IDs >= SUI.CharaXcounts * 2) Slot.y += 1;
            Slot.x = IDs % SUI.CharaXcounts;

        }
        else if (IDs == -1) Slot.y -= 3;
        else if (IDs == -2) Slot.y += 3;
        Slot.y = (int)Mathf.Repeat(Slot.y, Charay + 1);
        Slot.x = (int)Mathf.Repeat(Slot.x, SUI.CharaXcounts);
        SelectSlots[PID] = Slot;
    }
}
