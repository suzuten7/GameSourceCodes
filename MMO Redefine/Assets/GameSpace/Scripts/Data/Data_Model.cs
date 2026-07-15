namespace Datas
{
    using UnityEngine;
    [CreateAssetMenu(menuName = "DataCre/Model")]
    public class Data_Model : ScriptableObject
    {
        public string Name;
        [TextArea]public string Info;
        public Texture Icon;
        public GameObject Model;
    }
}

