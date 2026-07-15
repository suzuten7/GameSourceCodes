namespace UIs
{
    using TMPro;
    using UnityEngine;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static UI_System;
    using static GmSystem.GS_ChangeSet;
    public class UI_ShopButton_Sin : MonoBehaviour
    {
        [SerializeField] UI_ItemSlotBase ItemSlot;
        [SerializeField] TextMeshProUGUI CostTx;
        private void LateUpdate()
        {
            ChangeText(CostTx, ValueStrings(Cost) + "ゴールド");
        }
        public void Buy()
        {
            if (LPlayerVal.Gold < Cost) return;
            LPlayerVal.Gold -= Cost;
            LPlayerVal.ItemAdd(ItemSlot.ItemGID, "");
        }
        float Cost
        {
            get
            {
                if (CurrentShop.ShopD.Buys.Count <= ItemSlot.SlotNum)
                    return 0;
                var BuyD = CurrentShop.ShopD.Buys[ItemSlot.SlotNum];
                return BuyD.Cost > 0 ? BuyD.Cost : BuyD.ItemD.BuyGold;
            }
        }
    }
}
