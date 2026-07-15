namespace Player
{
    using System.Collections.Generic;
    using UnityEngine;
    using static Datas.Data_Equips;
    using static GmSystem.GS_SaveValues;
    public partial class Player_State
    {
        [System.Serializable]
        public class Class_State_PlayerValues
        {
            public Class_State_EquipmentValues[] SetWepons = new Class_State_EquipmentValues[4];
            public int[] WeponSkin = new int[] {-2,-2,-2,-2 };
            public Class_State_EquipmentValues[] SetArmors = new Class_State_EquipmentValues[4];
            public Class_State_EquipmentValues[] SetAkuses = new Class_State_EquipmentValues[4];
            public bool WepBack;
            public Class_State_Player_Job[] Jobs = new Class_State_Player_Job[2];
            public byte ModelMode;
            public int ModelID;
            public string ModelExID;
            public Class_Save_2DImageBase Model2DSet;
            public Vector2Int ModelExScale;
            public bool ShortCutBack;
            public Class_State_PlayerValues()
            {
                SetWepons = new Class_State_EquipmentValues[4];
                WeponSkin = new int[] { -2, -2, -2, -2 };
                SetArmors = new Class_State_EquipmentValues[4];
                SetAkuses = new Class_State_EquipmentValues[4];
                Jobs = new Class_State_Player_Job[2];
            }
        }
        [System.Serializable]
        public class Class_State_Player_Job
        {
            public byte ID;
            public List<Class_JobTrees> Trees = new List<Class_JobTrees>();
        }
    }
}
