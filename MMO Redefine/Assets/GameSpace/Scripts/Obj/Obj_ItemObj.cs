
namespace Obj
{
    using UnityEngine;
    using Fusion;
    using UnityEngine.UI;
    using static GmSystem.GS_GlobalValues;
    using static FNet.Fusion_Manager;
    using Datas;
    using Player;
    using static GmSystem.GS_SaveValues;
    using static Datas.Data_Get;
    using static Datas.Data_Equips;
    using NUnit.Framework.Constraints;

    public class Obj_ItemObj : Obj_ActionObject
    {
        [Networked] public int ItemGID { get; set; }
        [Networked] public NetworkString<_64> ItemDataStr { get; set; }
        [SerializeField]string ida;
        public Class_State_EquipmentValues? EquipVal = null;
        public float RemTime;
        [SerializeField]RawImage IconImgs;
        [SerializeField] GameObject ModelObj;
        
        int _times;
        int _bitemGID = -9999;
        private void Start()
        {
            ParentStrage(gameObject, "Drop");
        }
        private void FixedUpdate()
        {
            ModelSet();
            ida = ItemDataStr.Value;
            if (!CanControl(Object)) return;
            _times++;
            if(_times >= RemTime*60f)
            {
                Runner.Despawn(Object);
            }
        }
        void ModelSet()
        {
            if (_bitemGID != ItemGID)
            {
                _bitemGID = ItemGID;
                if (ModelObj != null) Destroy(ModelObj);

                ActionName = "アイテム";
                if (ItemGID >= 0)
                {
                    var ItemD = (Data_Item)ItemGIDDataGet(ItemGID);
                    ActionName += ItemD.Name;
                    if (ItemD.Model != null)
                    {
                        IconImgs.gameObject.SetActive(false);
                        ModelObj = Instantiate(ItemD.Model, transform.position, transform.rotation);
                        ModelObj.transform.parent = transform;
                    }
                    else
                    {
                        IconImgs.gameObject.SetActive(true);
                        IconImgs.texture = ItemD.Icon;
                    }
                }
                else ActionName += "無";

            }
        }
        public override void Spawned()
        {
            ModelSet();

        }
        public override void PlayAction(Player_State PSta)
        {
            RPC_ItemGet_Serv(PSta.Object.Id.Raw);
        }
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_ItemGet_Serv(uint pbjID)
        {

            var tobj = Runner.FindObject(new NetworkId { Raw = pbjID });
            if (tobj == null) return;
            var psta = tobj.GetComponent<Player_State>();
            if (psta == null) return;
            Debug.Log("取得:" + psta.CommonValues.Name);
            if (Runner.GameMode == GameMode.Single)
            {
                var da = ItemDataStr.Value;
                if(EquipVal != null)da = JsonUtility.ToJson(EquipVal);
                LPlayerVal.ItemAdd(ItemGID,da);
            }
            else psta.RPC_ItemAdd(ItemGID, ItemDataStr.Value);
            Runner.Despawn(Object);
        }
    }
}

