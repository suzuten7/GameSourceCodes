using System.Collections.Generic;
using UnityEngine;

public class UI_BranchInfo : MonoBehaviour
{
    [SerializeField] State_Base Sta;
    [SerializeField] UI_Sin_Branch CurrentBranchUI;
    [SerializeField] List<UI_Sin_Branch> NextBranchUIs;

    void Update()
    {
        if (Sta.AtkD != null)
        {
            CurrentBranchUI.gameObject.SetActive(true);
            CurrentBranchUI.UISet(Sta, Sta.AtkBranch, "");
            for (int i = 0; i < Mathf.Max(Sta.AtkD.Branchs.Length,NextBranchUIs.Count); i++)
            {
                if (i < Sta.AtkD.Branchs.Length)
                {
                    if (NextBranchUIs.Count <= i) NextBranchUIs.Add(Instantiate(NextBranchUIs[0], NextBranchUIs[0].transform.parent));
                    var Branch = Sta.AtkD.Branchs[i];
                    bool NumCheck = false;
                    for (int j = 0; j < Branch.BranchNums.Length; j++) if (Branch.BranchNums[j] == Sta.AtkBranch) NumCheck = true;
                    if (NumCheck && Branch.FutureNum != Sta.AtkBranch)
                    {
                        NextBranchUIs[i].gameObject.SetActive(true);
                        string IfStr = "";
                        for (int j = 0; j < Branch.Ifs.Length; j++) IfStr += Branch.Ifs[j];
                        NextBranchUIs[i].UISet(Sta, Branch.FutureNum, IfStr);
                    }
                    else NextBranchUIs[i].gameObject.SetActive(false);


                }
                else NextBranchUIs[i].gameObject.SetActive(false);
            }
        }
        else
        {
            CurrentBranchUI.gameObject.SetActive(false);
            for (int i = 0; i < Mathf.Max(NextBranchUIs.Count); i++)
            {
                NextBranchUIs[i].gameObject.SetActive(false);
            }
        }
    }
}
