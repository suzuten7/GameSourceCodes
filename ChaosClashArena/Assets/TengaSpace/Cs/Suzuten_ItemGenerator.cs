using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using static Suzuten_PlayerSets;
using static Suzuten_SEPlays;
using Photon.Pun;
public class Suzuten_ItemGenerator : MonoBehaviourPun
{
    [SerializeField] Suzuten_DataBase DB;
    [SerializeField, Tooltip("生成位置")]
    CreatePosC[] CreatePoss;
    [SerializeField, Tooltip("個数x～y")]
    Vector2Int ItemCounts = Vector2Int.one;
    [SerializeField, Tooltip("初回出現x～y秒")]
    Vector2 StartDelay = new Vector2(10, 20);
    [SerializeField,Tooltip("出現間隔x～y秒")]
    Vector2 DropCTs = new Vector2(15, 25);
    int CT;
    [System.Serializable]
    class CreatePosC
    {
        [Tooltip("生成基準")]
        public Transform BasePosTrans;
        [Tooltip("生成位置XZ範囲")]
        public Vector2 PosRanges = new Vector2(20f, 20f);
    }
    private void Start()
    {
        CT = Mathf.RoundToInt(Random.Range(StartDelay.x, StartDelay.y) * 60f * (BOP_Battle[4] * 0.01f));
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (!BattleFlag) return;

        CT--;
        if (CT <= 0)
        {
            CT = Mathf.RoundToInt(Random.Range(DropCTs.x, DropCTs.y) * 60f * (BOP_Battle[4] * 0.01f));
            int ChaosCount = 0;
            int Count = Mathf.RoundToInt(Random.Range((float)ItemCounts.x, (float)ItemCounts.y) * (BOP_Battle[5] * 0.01f));
            for (int i = 0; i < Count; i++)
            {
                Vector3 Pos = transform.position;
                if (CreatePoss.Length > 0)
                {
                    CreatePosC CPos = CreatePoss[Random.Range(0, CreatePoss.Length)];
                    Pos = CPos.BasePosTrans.position;
                    Pos += new Vector3(Random.Range(-CPos.PosRanges.x, CPos.PosRanges.x), 0, Random.Range(-CPos.PosRanges.y, CPos.PosRanges.y));
                }
                float Per = Random.Range(0, 100);
                bool Chaos = BOP_Battle[6] > Per;
                if (Chaos) ChaosCount++;
                GameObject ItemObj = Chaos ? DB.ChaosItemObjs[Random.Range(0, DB.ChaosItemObjs.Length)] : DB.ItemObjs[Random.Range(0, DB.ItemObjs.Length)];
                GameObject ItemIns = PhotonNetwork.Instantiate(ItemObj.name, Pos, Quaternion.identity);
                Rigidbody ItemRig = ItemIns.GetComponent<Rigidbody>();
                if (ItemRig)
                {
                    ItemRig.velocity += new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f).normalized * Random.Range(0f, 100f);
                }
            }
            if (ChaosCount > 0)
            {
                InfoDisplays("アイテムが出現!!!\n<color=#FF00FF><size=75%>カオスアイテム\nが" + ChaosCount + "個出現!!!</size></color>", new Vector2(0, 0), 45, 10, 3);
                SEPlays(DB.ChaosItemSE, -1);
            }
            else
            {
                InfoDisplays("アイテムが出現!!!", new Vector2(0, 0), 45, 15, 5);
                SEPlays(DB.NomalItemSE, -1);
            }
        }

    }
}
