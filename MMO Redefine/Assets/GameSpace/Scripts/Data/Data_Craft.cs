namespace Datas
{
    using System.Collections.Generic;
    using UnityEngine;
    using static Data_Items;
    [CreateAssetMenu(menuName = "DataCre/Craft")]
    public class Data_Craft : ScriptableObject
    {
        public Data_Item ItemD;
        public List<Class_Data_CraftRecipe> Recipes;
        [System.Serializable]
        public class Class_Data_CraftRecipe
        {
            public Enum_CraftType Type;
            public Data_Item Item;
            public int Count;
        }
    }
}
