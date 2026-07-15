
namespace Datas
{
    using System.Collections.Generic;
    using UnityEngine;
    [CreateAssetMenu(menuName ="DataCre/Shop")]
    public class Data_Shop : ScriptableObject
    {
        [System.Serializable]
        public class Class_ShopBuy
        {
            public Data_Item ItemD;
            public float Cost;
        }
        public List<Class_ShopBuy> Buys;
    }
}

