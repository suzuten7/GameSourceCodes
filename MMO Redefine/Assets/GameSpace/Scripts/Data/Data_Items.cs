using UnityEngine;

namespace Datas
{
    public class Data_Items
    {
        public enum Enum_ItemID
        {
            [InspectorName("分類区分")]Category = 100000000,
            [InspectorName("種類区分")] Type = 1000000,
            [InspectorName("その他")] Other = -9999,
            [InspectorName("無")] Non = -1,
            [InspectorName("素材")] Material = Category * 0,
            [InspectorName("消耗品")] Consumables = Category * 1,
            [InspectorName("ジョブツリー")] JobTree = Category * 8,
            [InspectorName("スキル")] Skill = Category * 9,
            [InspectorName("武器")] Wepon = Category * 10,
            [InspectorName("防具")] Armor = Category * 11,
            [InspectorName("アクセ")] Akuse = Category * 12,
        }
        public enum Enum_ItemType
        {
            [InspectorName("素材")] Material,
        }
        public enum Enum_ConsumableType
        {
            [InspectorName("消耗品")] Consumables,
        }
        public enum Enum_WeponType
        {
            [InspectorName("短剣")] ShortSword,
            [InspectorName("長剣")] LongSword,
            [InspectorName("大剣")] GrateSword,
            [InspectorName("斧")] Axe,
            [InspectorName("ハンマー")] Hammer,
            [InspectorName("拳")] Grobe,
            [InspectorName("槍")] Spear = 10,
            [InspectorName("鎌")] Cyce,

            [InspectorName("盾")] Shild = 49,
            [InspectorName("拳銃")] HandGun = 50,
            [InspectorName("散弾銃")] ShotGun,
            [InspectorName("狙撃銃")] SnipGun,
            [InspectorName("RPG")] RPG,

            [InspectorName("魔術")] Magic = 60,
            [InspectorName("杖")] Staff,
            [InspectorName("本")] Book,

            [InspectorName("弓")] Bow = 70,
            [InspectorName("クロスボウ")] CrossBow,

            [InspectorName("投擲")] Throw = 80,

            [InspectorName("矢")] Arrow = 90,
        }
        public enum Enum_EquipType
        {
            [InspectorName("頭")] Head,
            [InspectorName("体")] Body,
            [InspectorName("手")] Hand,
            [InspectorName("足")] Foot,

            [InspectorName("耳飾り")] Earrings = 100,
            [InspectorName("首飾り")] Necklace,
            [InspectorName("指輪")] Ring,
        }
        public enum Enum_CraftType
        {
            [InspectorName("指定")] Desig,
            [InspectorName("任意")] Any,
            [InspectorName("素材")] Material,
            [InspectorName("消耗品")] Consum,
            [InspectorName("武器")] Wepon,
            [InspectorName("防具")] Armor,
            [InspectorName("アクセ")] Akuse,
        }
    }
}

