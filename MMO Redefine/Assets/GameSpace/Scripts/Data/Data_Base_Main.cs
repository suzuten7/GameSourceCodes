namespace Datas
{
    using System.Collections.Generic;
    using UnityEngine;
    using State;
    using Player;
    using Obj;
    using static GmSystem.GS_GlobalState;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    [CreateAssetMenu(menuName = "DataCre/TestData")]
    public class Data_Base : ScriptableObject
    {
        public const string DataPath = "Assets/GameSpace/Data/DataBase.asset";
        public Player_State PlayerBase;
        public State_DamageObj DamObj;
        public State_EXPObj EXPObj;
        public Obj_ItemObj ItemObj;

        public Color[] ElementColors;
        public Data_Model[] Models;
        public AnimatorOverrideController AOvCont;
        public List<AnimationClip> WeponAttackClips;
        public List<AnimationClip> SkillAttackClips;

        public List<Data_Buf> Bufs;
        public List<Class_Data_BufOPs> BufOPs;

        public Data_Job[] JobDatas;
        public List<Data_Attack> ALLSkills;
        public Data_Item_DBL Items;
        public Data_Consumable_DBL Consumables;
        public Data_Wepon_DBL Wepons;
        public Data_Equipment_DBL Equipments;

        public Data_AddOptionValues OpValues;
        public Data_AddOptionRand WeponOpRand;
        public Data_AddOptionRand ArmorOpRand;
        public Data_AddOptionRand AkuseOpRand;

        public List<Data_Craft> Crafts;

        public Data_Enemy_DBL Enemys;
        public Data_Acive_DBL Acives;

        [System.Serializable]
        public class Class_Data_BufOPs
        {
            public Texture OpIcon;
            public Color FlameCol;
        }
        public Enum_Element El;

#if UNITY_EDITOR
        public void SaveDatabase()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif

    }

}
