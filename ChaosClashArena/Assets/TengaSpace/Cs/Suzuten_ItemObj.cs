using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Suzuten_ActionData;
using static Suzuten_PlayerSets;
using static Suzuten_ItemData;
using Photon.Pun;
public class Suzuten_ItemObj : MonoBehaviourPun
{
    [SerializeField] string ItemName;
    [SerializeField] Suzuten_ItemData[] ItemDatas;
    [SerializeField] Vector2Int Counts = Vector2Int.one;
    [SerializeField] int RemTime;
    int Time = 0;

    // Start is called before the first frame update
    //void Start()
    //{
    //    //RemTime = RemTime;
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        Time++;
        if (Time >= RemTime)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (!BattleFlag) return;
        if (!photonView.IsMine) return;
        if (col.tag == "Player")
        {
            Suzuten_PlayerMove PMv = col.GetComponent<Suzuten_PlayerMove>();
            if (PMv)
            {
                string Infost = Gets(PMv.PS);
                Vector2 InfoPos = new Vector2(-100, 100);
                InfoPos += PMv.PS.PI.playerIndex == 1 ? new Vector2(200, 200): new Vector2(-200, -200);
                string InfoStr = PhotonNetwork.OfflineMode ? "Player" + (PMv.PS.PI.playerIndex + 1) : PMv.PS.photonView.Owner.NickName;
                InfoStr += "は" + ItemName + "を拾った!!!" + Infost;
                InfoDisplays(InfoStr, InfoPos,25,15,5);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    string Gets(Suzuten_PlayerState PS)
    {
        string Infostr = "";
        List<Suzuten_ItemData> ItemLists = ItemDatas.ToList();
        int Count = Random.Range(Counts.x, Counts.y + 1);
        while (ItemLists.Count > 0 && Count > 0)
        {
            Count--;
            int Indexs = Random.Range(0, ItemLists.Count);
            if (ItemLists[Indexs] != null)
            {
                if (ItemLists[Indexs].ItemMessage != "") Infostr += "\n" + ItemLists[Indexs].ItemMessage;
                if (ItemLists[Indexs].ItemDs != null)
                {
                    foreach (ItemDsC Ds in ItemLists[Indexs].ItemDs)
                    {
                        switch (Ds.Targets)
                        {
                            case TargetsE.使用者自身: Dss(PS, Ds, PS); break;
                            case TargetsE.使用者ターゲット: if (PS.Target != null) Dss(PS.Target, Ds, PS); break;
                            case TargetsE.使用者以外:
                                foreach (var PSs in FindObjectsOfType<Suzuten_PlayerState>())
                                {
                                    if (PSs != PS) Dss(PSs, Ds, PS);
                                }
                                break;
                            case TargetsE.全員:
                                foreach (var PSs in FindObjectsOfType<Suzuten_PlayerState>()) Dss(PSs, Ds, PS);
                                break;
                        }

                    }
                }
                if (ItemLists[Indexs].GetEffect != null) Instantiate(ItemLists[Indexs].GetEffect, PS.PosGet(), Quaternion.identity);
            }
            ItemLists.RemoveAt(Indexs);
        }
        return Infostr;
    }

    void Dss(Suzuten_PlayerState PS, ItemDsC Ds, Suzuten_PlayerState BasePS)
    {
        if (Ds.Parametors != null)
        {
            foreach (ParametorsC MParas in Ds.Parametors)
            {
                PS.ItemChanges((int)MParas.Parametor, MParas.Val, MParas.MaxPers);
            }
        }
        if (Ds.Bufs != null)
        {
            for (int i = 0; i < Ds.Bufs.Length; i++) PS.BufSets(Ds.Bufs[i]);
        }
        if (Ds.TPs != null)
        {
            for (int i = 0; i < Ds.TPs.Length; i++)
            {
                Suzuten_PlayerState TPObjs = null;
                switch (Ds.TPs[i].TPsTarget)
                {
                    case TPsTargetE.使用者: TPObjs = BasePS; break;
                    case TPsTargetE.使用者ターゲット: if (PS.Target != null) TPObjs = BasePS.Target; break;
                    case TPsTargetE.ランダム:
                        Suzuten_PlayerState[] PSs = FindObjectsOfType<Suzuten_PlayerState>();
                        TPObjs = PSs[Random.Range(0, PSs.Length)];
                        break;
                }
                if (TPObjs != null) PS.TPs(TPObjs.RigObj.transform.position);
            }
        }
        if (Ds.InstObj != null)
        {
            for (int i = 0; i < Ds.InstObj.Length; i++)
            {
                if(Ds.InstObj[i]!=null) PhotonNetwork.Instantiate(Ds.InstObj[i].name, PS.PosGet(),Quaternion.identity);
            }
        }
    }
}
