
namespace UIs
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static Datas.Data_JobTree_Group;
    using static Datas.Data_Get;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_ChangeSet;
    using static UIs.UI_System;
    public class UI_JobTree_Make : MonoBehaviour
    {
        [SerializeField] UI_CharaBase CharaUI;
        public int JobID;
        public float PosScale;
        public Transform BackParent;
        public TextMeshProUGUI PointTx;
        public Image BackBase;
        [SerializeField]UI_JobTree_Select[] JCSel;
        [SerializeField] List<UI_JobTree_Select> JSelect;
        public UI_JobTree_Single JTUI_SingleBase;

        List<List<UI_JobTree_Single>> JTUIs = new ();
        List<List<List<Image>>> JTBacks = new ();

        int JID
        {
            get
            {
                if (JobID == -1) return CharaUI.LChara.JobIDs[0];
                if (JobID == -2) return CharaUI.LChara.JobIDs[1];
                return JobID;
            }
        }
        int[] bids = { -1, -1 };
        public List<Class_Data_JobTreeGroupSet> JTDGroupsGet
        {
            get{return DB.JobDatas[JID].JTGroupSet;}
        }
        public List<Class_JobTrees> JTSGroupGet
        {
            get { return CharaUI.LChara.JobTrees[JID].Groups; }
        }
        public int PreLVsGet(Class_Data_JobTree jtd,Class_JobTrees jtsc)
        {
            var PreLVs = 0;
            for (int i = 0; i < jtd.BTreeIDs.Length; i++)
            {
                PreLVs += jtsc.LVs[jtd.BTreeIDs[i]];
            }
            return PreLVs;
        }
        public int MaxPointGet => MPoint(LPlayerVal.LV);
        int MPoint(int LV)
        {
            return 10 + (LV-1) * 2;
        }
        public int UsePointGet => UPoint(LPlayerVal.LV);
        int UPoint(int LV)
        {
            var Up = MPoint(LV);
            var JTGroups = JTDGroupsGet;
            for (int i = 0; i < JTGroups.Count; i++)
            {
                var JTs = JTGroups[i];
                for (int k = 0; k < JTs.JTreeGroup.JobTrees.Count; k++)
                {
                    var JD = JTs.JTreeGroup.JobTrees[k];
                    Up -= JD.CTree.Point * JTSGroupGet[i].LVs[k];
                }
            }
            return Up;
        }
        void Start()
        {
            ChangeActive(JTUI_SingleBase.gameObject, false);
            ChangeActive(BackBase.gameObject, false);
            TreeUpdate();
        }
        private void LateUpdate()
        {
            for(int i = 0; i < bids.Length; i++)
            {
                if (bids[i] != CharaUI.LChara.JobIDs[i])
                {
                    bids[i] = CharaUI.LChara.JobIDs[i];
                    TreeUpdate();
                }
            }

        }
        public void TreeUpdate()
        {
            JTExpansion();
            for (int i = 0; i < JCSel.Length; i++)
            {
                ChangeText(JCSel[i].NameTx, DB.JobDatas[CharaUI.LChara.JobIDs[i]].Name);
                ChangeColor(JCSel[i].Flame, OnColors(JobID == -(i + 1)));
            }
            for (int i = 0; i < DB.JobDatas.Length; i++)
            {
                if (JSelect.Count <= i) JSelect.Add(Instantiate(JSelect[0], JSelect[0].transform.parent));
                JSelect[i].JobID = i;
                ChangeText(JSelect[i].NameTx, DB.JobDatas[i].Name);
                ChangeColor(JSelect[i].Flame,OnColors(JobID == i));
            }
            var JTGroups = JTDGroupsGet;
            UIClear();
            for (int i = 0; i < JTGroups.Count; i++)
            {
                JTUI(i, JTGroups[i]);
                JTBack(i, JTGroups[i]);
            }
            var pstr = "ポイント" + UsePointGet;
            if (CharaUI.LChara.SetLV > 0) pstr += "(固定" + UPoint(Mathf.Min(LPlayerVal.LV, CharaUI.LChara.SetLV)) + ")";
            pstr += "/" + MaxPointGet;
            ChangeText(PointTx, pstr);

        }
        void UIClear()
        {
            for(int i = 0; i < JTUIs.Count; i++)
            {
                for (int k = 0; k < JTUIs[i].Count; k++)
                {
                    ChangeActive(JTUIs[i][k].gameObject, false);
                }
            }
            for(int i = 0; i < JTBacks.Count; i++)
            {
                for(int k = 0; k < JTBacks[i].Count; k++)
                {
                    for (int m = 0; m < JTBacks[i][k].Count; m++)
                    {
                        ChangeActive(JTBacks[i][k][m].gameObject, false);
                    }
                }
            }
        }
        void JTExpansion()
        {
            var LChara = CharaUI.LChara;
            for (int i = 0; i < DB.JobDatas.Length; i++)
            {
                if (LChara.JobTrees.Count <= i) LChara.JobTrees.Add(new Class_Save_JTreeBase());
                for (int k = 0; k < DB.JobDatas[i].JTGroupSet.Count; k++)
                {
                    if (LChara.JobTrees[i].Groups.Count <= k) LChara.JobTrees[i].Groups.Add(new Class_JobTrees());
                    for (int m = 0; m < DB.JobDatas[i].JTGroupSet[k].JTreeGroup.JobTrees.Count; m++)
                    {
                        if (LChara.JobTrees[i].Groups[k].LVs.Count <= m) LChara.JobTrees[i].Groups[k].LVs.Add(0);
                    }
                }
            }

        }
        void JTUI(int gid,Class_Data_JobTreeGroupSet JTGroupSet)
        {
            if (JTUIs.Count <= gid)JTUIs.Add(new List<UI_JobTree_Single>());
            for (int i = 0; i < JTGroupSet.JTreeGroup.JobTrees.Count; i++)
            {
                if (JTUIs[gid].Count <= i)JTUIs[gid].Add(Instantiate(JTUI_SingleBase, JTUI_SingleBase.transform.parent));
                var sui = JTUIs[gid][i];
                var jtd = JTGroupSet.JTreeGroup.JobTrees[i];
                var jtsc = JTSGroupGet[gid];
                ChangeActive(sui.gameObject, true);
                sui.GID = gid;
                sui.SID = i;

                if (jtsc.LVs[i] >= jtd.CTree.LVMax)ChangeColor(sui.BackImage, Color.yellow);
                else if (jtsc.LVs[i] > 0)ChangeColor(sui.BackImage, Color.green);
                else if (jtd.BTreeIDs.Length > 0 && PreLVsGet(jtd,jtsc) < jtd.PrereLV)ChangeColor(sui.BackImage, Color.gray);
                else ChangeColor(sui.BackImage, Color.magenta);
                ChangeTexture(sui.Icon, jtd.CTree.Icon, true);
                ChangeText(sui.NameTx, jtd.CTree.Icon != null ? "" : jtd.CTree.Name);

                ChangeText(sui.LvTx,"\nLV:" + jtsc.LVs[i] + "/" + jtd.CTree.LVMax);
                var LPos = (Vector3)(jtd.Pos + JTGroupSet.OffSet) * PosScale;
                sui.transform.localPosition = LPos;
            }
        }
        void JTBack(int gid,Class_Data_JobTreeGroupSet JTGroupSet)
        {
            if (JTBacks.Count <= gid) JTBacks.Add(new List<List<Image>>());
            for (int i = 0; i < JTGroupSet.JTreeGroup.JobTrees.Count; i++)
            {
                var cjt = JTGroupSet.JTreeGroup.JobTrees[i];
                if (JTBacks[gid].Count <= i) JTBacks[gid].Add(new List<Image>());
                for (int k = 0; k < cjt.BTreeIDs.Length; k++)
                {
                    if (JTBacks[gid][i].Count <= k) JTBacks[gid][i].Add(Instantiate(BackBase, BackParent));
                    var bui = JTBacks[gid][i][k];
                    var bjt = JTGroupSet.JTreeGroup.JobTrees[cjt.BTreeIDs[k]];
                    ChangeActive(bui.gameObject, true);
                    var LPos = (Vector3)((bjt.Pos + cjt.Pos) / 2f + JTGroupSet.OffSet) * PosScale;
                    bui.transform.localPosition = LPos;
                    var BVect = bjt.Pos - cjt.Pos;
                    float angle = Mathf.Atan2(BVect.y, BVect.x) * Mathf.Rad2Deg;
                    bui.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
                    bui.transform.localScale = new Vector3(1, BVect.magnitude*0.5f, 1);
                }
            }
        }
        public void GetTree(int gid,int sid)
        {
            var JTDGroups = JTDGroupsGet;
            var JTData = JTDGroups[gid].JTreeGroup.JobTrees[sid];
            var JTSGroup = JTSGroupGet[gid];
            var JTSC = JTSGroup.LVs[sid];

            if (JTSC >= JTData.CTree.LVMax) return;
            if (JTData.BTreeIDs.Length > 0 && PreLVsGet(JTData, JTSGroup) < JTData.PrereLV) return;
            if (UsePointGet < JTData.CTree.Point) return;
            JTSGroup.LVs[sid]++;
            TreeUpdate();
        }
        public void PointReset()
        {
            var LChara = CharaUI.LChara;
            for (int i=0;i< LChara.JobTrees[JID].Groups.Count; i++)
            {
                for(int k = 0; k < LChara.JobTrees[JID].Groups[i].LVs.Count; k++)
                {
                    LChara.JobTrees[JID].Groups[i].LVs[k] = 0;
                }
            }
            TreeUpdate();
        }
    }

}
