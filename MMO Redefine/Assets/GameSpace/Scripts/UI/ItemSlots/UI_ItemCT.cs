namespace UIs
{
    using UnityEngine;
    using UnityEngine.UI;
    using static Datas.Data_Get;
    using Datas;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_ChangeSet;

    public class UI_ItemCT : MonoBehaviour
    {
        [SerializeField] UI_ItemSlotBase ItemSlot;
        [SerializeField] Image CTFill;
        [SerializeField] Image NoUse;
        private void Update()
        {
            var gid = ItemSlot.ItemGID;
            if (gid < 0)
            {
                ChangeFill(CTFill,0);
                ChangeColor(NoUse, Color.clear);
                return;
            }
            
            var ItemData = ItemGIDDataGet(gid);
            float mct = 0;
            bool check = true;
            Data_Attack.Class_AttackData_Costs[] costs = null;
            var ncol = Color.clear;
            switch (ItemData)
            {
                case Data_Consumables:
                    mct = ((Data_Consumables)ItemData).Attack.CT;
                    check = LPlayerVal.ConsumablesDic.ContainsKey(gid);
                    if (!check)
                    {
                        ncol = new Color(1.0f, 0.5f, 1.0f, 0.5f);
                    }
                    break;
                case Data_Attack:
                    mct = ((Data_Attack)ItemData).CT;
                    costs = ((Data_Attack)ItemData).Costs;
                    check = SkillCheck((Data_Attack)ItemData, ItemSlot.CharaUI.LChara);
                    if (!check)
                    {
                        ncol = new Color(1.0f, 0.5f, 0.5f, 0.5f);
                    }
                    break;
            }
            var Sta = ItemSlot.CharaUI.Sta;
            if (Sta && check)
            {
                var cost = true;
                if (ItemSlot.CharaUI.CharaID < 0 && costs != null)
                {
                    cost = GmSystem.GS_GlobalValues.MyPlayer.AttackCostCheck(costs);
                }
                if (!cost) ncol = new Color(0.0f, 0.0f, 1.0f, 0.25f);
                else ncol = Color.clear;

                if (Sta.SkillCTs.ContainsKey(gid))
                {
                    ChangeFill(CTFill, Sta.SkillCTs[gid] / Mathf.Max(1f, mct * 60f));
                }
                else ChangeFill(CTFill, 0f);
            }
            else
            {
               ChangeFill(CTFill, 0f);
            }
            ChangeColor(NoUse, ncol);
        }
    }
}
