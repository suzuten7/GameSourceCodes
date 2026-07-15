namespace UIs
{
    using Datas;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static Datas.Data_Get;
    using static UI_System;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_ChangeSet;
    public class UI_Shop_Sell : MonoBehaviour
    {
        [SerializeField] UI_ItemSlotBase ItemSlot;
        public Slider CountSlider;
        public TMP_InputField CountIn;
        public TextMeshProUGUI SellTx;
        int count = 1;

        void LateUpdate()
        {
            ChangeMax(CountSlider, Mathf.Max(1, ItemSlot.ItemCount));
            ChangeValue(CountSlider,count);
            ChangeText(CountIn, count.ToString(),true);
            var ItemD = ItemGIDDataGet(ItemSlot.ItemGID);
            if (ItemD == null)
            {
                ChangeText(SellTx, "");
            }
            else
            {
                var cost = ((Data_Item)ItemD).BuyGold * 0.5f;
                var str = ValueStrings(cost * count) + "ゴールド";
                str += "\n" + ValueStrings(cost) + "ゴールド/個";
                ChangeText(SellTx, str);
            }
        }
        public void SliderSet()
        {
            count = (int)CountSlider.value;
        }
        public void InSet()
        {
            count = int.TryParse(CountIn.text,out var co) ? co : count;
            count = Mathf.Clamp(count,1, ItemSlot.ItemCount);
        }
        public void Sell()
        {
            var ItemD = ItemGIDDataGet(ItemSlot.ItemGID);
            if (ItemD == null) return;
            LPlayerVal.Gold += ((Data_Item)ItemD).BuyGold * 0.5f * count;
            switch (ItemGIDCategoryGet(ItemSlot.ItemGID))
            {
                case Data_Items.Enum_ItemID.Wepon:
                case Data_Items.Enum_ItemID.Armor:
                case Data_Items.Enum_ItemID.Akuse:
                    LPlayerVal.ItemRem(ItemSlot.ItemGID,ItemSlot.SlotNum);
                    break;
                default:
                    LPlayerVal.ItemRem(ItemSlot.ItemGID, count);
                    break;
            }

            ItemSlot.USID = -1;
        }
    }
}

