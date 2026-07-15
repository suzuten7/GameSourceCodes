namespace UIs
{
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static Datas.Data_Get;
    using static Datas.Data_Items;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_ChangeSet;
    using static UIs.UI_System;
        public class UI_Craft_Make : MonoBehaviour
        {
            public int CraftID;
            [SerializeField] List<UI_Craft_Recipe> RecipeUIs;
            [SerializeField] List<UI_Craft_ItemSet> ItemSetUIs;

            [SerializeField] TextMeshProUGUI InfoTx;
            [SerializeField] Image CraftBBack;
            void Update()
            {
                for (int i = 0; i < DB.Crafts.Count; i++)
                {
                    if (RecipeUIs.Count <= i) RecipeUIs.Add(Instantiate(RecipeUIs[0], RecipeUIs[0].transform.parent));
                    var rui = RecipeUIs[i];
                    rui.ID = i;
                    ChangeColor(rui.OutImage,OnColors(i == CraftID));
                    var itemd = DB.Crafts[i].ItemD;

                    ChangeText(rui.NameTx,itemd.Name);
                    ChangeTexture(rui.Icon, itemd.Icon, true);
                    ChangeText(rui.NoIconName, itemd.Icon != null ? "" : itemd.Name);

                }
                var CraftD = DB.Crafts[CraftID];
                for (int i = 0; i < CraftD.Recipes.Count; i++)
                {
                    var reciped = CraftD.Recipes[i];
                    var itemid = reciped.Type == Enum_CraftType.Desig ? ItemDataFindGID(reciped.Item) : -1;
                    var Va = new Vector2Int(itemid, -1);
                    if (CraftSets.Count <= i) CraftSets.Add(Va);
                    else if (itemid >= 0) CraftSets[i] = Va;
                }
                var CCheck = CraftCheck(out var oc, out int noid);
            for (int i = 0; i < Mathf.Max(ItemSetUIs.Count, CraftD.Recipes.Count); i++)
            {
                if (ItemSetUIs.Count <= i) ItemSetUIs.Add(Instantiate(ItemSetUIs[0], ItemSetUIs[0].transform.parent));
                var isui = ItemSetUIs[i];
                if (i >= CraftD.Recipes.Count)
                {
                    ChangeActive(isui.gameObject, false);
                    continue;
                }

                var reciped = CraftD.Recipes[i];
                ChangeActive(isui.gameObject, true);
                ChangeText(isui.IndexTx,"素材" + (i + 1));
                ChangeText(isui.TypeTx,reciped.Type == Enum_CraftType.Desig ? reciped.Item.Name : reciped.Type.ToString());
                isui.ItemSlot.SlotNum = i;
                var count = LPlayerVal.ItemCount(CraftSets[i].x, CraftSets[i].y);
                ChangeText(isui.CountTx,count + "/" + reciped.Count);

                var Col = Color.magenta;
                if (noid != i)Col = count >= reciped.Count ? Color.black : Color.red;
                ChangeColor(isui.CountTx, Col);
            }
                ChangeText(InfoTx,CraftD.ItemD.Info);
                ChangeColor(CraftBBack,CCheck ? Color.white : Color.red);
            }
            bool CraftCheck(out List<Vector2Int> UseCounts, out int noid)
            {
                noid = -1;
                var CraftD = DB.Crafts[CraftID];
                UseCounts = new List<Vector2Int>();
                for (int i = 0; i < CraftD.Recipes.Count; i++)
                {
                    var reciped = CraftD.Recipes[i];
                    var itemid = ItemDataFindGID(reciped.Item);
                    if (itemid < 0)
                    {
                        if (CraftSets[i].x < 0) return false;
                        itemid = CraftSets[i].x;
                    }
                    Vector2Int Va;
                    switch (ItemGIDCategoryGet(itemid))
                    {
                        case Enum_ItemID.Wepon:
                        case Enum_ItemID.Armor:
                        case Enum_ItemID.Akuse:
                            Va = new Vector2Int(itemid, CraftSets[i].y);
                            if (UseCounts.Contains(Va))
                            {
                                noid = i;
                                return false;
                            }
                            else UseCounts.Add(Va);
                            break;
                        default:
                            Va = new Vector2Int(itemid, -1);
                            var coind = UseCounts.FindIndex(0, v => v.x == Va.x);
                            if (coind >= 0)
                            {
                                Va.y += reciped.Count;
                                UseCounts[coind] = Va;
                            }
                            else
                            {
                                Va.y = reciped.Count;
                                UseCounts.Add(Va);
                            }
                            break;
                    }


                }
                for (int i = 0; i < UseCounts.Count; i++)
                {
                    var count = LPlayerVal.ItemCount(UseCounts[i].x, UseCounts[i].y);
                    switch (ItemGIDCategoryGet(UseCounts[i].x))
                    {
                        case Enum_ItemID.Wepon:
                        case Enum_ItemID.Armor:
                        case Enum_ItemID.Akuse:
                            if (count < 1) return false;
                            break;
                        default:
                            if (count < UseCounts[i].y) return false;
                            break;
                    }

                }
                UseCounts = UseCounts.OrderByDescending(x => x.y).ToList();
                return true;
            }
            public void Craft()
            {
                if (!CraftCheck(out var UCount, out var noid)) return;
                var CraftD = DB.Crafts[CraftID];
                for (int i = 0; i < UCount.Count; i++)
                {
                    Debug.Log("CraftRem:" + UCount[i]);
                    LPlayerVal.ItemRem(UCount[i].x, UCount[i].y);
                }
                LPlayerVal.ItemAdd(ItemDataFindGID(CraftD.ItemD), "");
                for (int i = 0; i < CraftD.Recipes.Count; i++)
                {
                    if (CraftD.Recipes[i].Type == Enum_CraftType.Desig) continue;
                    switch (ItemGIDCategoryGet(CraftSets[i].x))
                    {
                        case Enum_ItemID.Material:
                        case Enum_ItemID.Consumables:
                            continue;
                    }
                    CraftSets[i] = new Vector2Int(-1, -1);
                }
            }
        }

    
}
