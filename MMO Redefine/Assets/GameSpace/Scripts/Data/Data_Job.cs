
namespace Datas
{
    using System.Collections.Generic;
    using UnityEngine;
    using static Data_Equips;
    using static Data_JobTree_Group;
    [CreateAssetMenu(menuName = "DataCre/Job")]
    public class Data_Job : ScriptableObject
    {
        public string Name;
        [TextArea]public string Info;
        [Header("職補正値")]
        public float MHP;
        public float HPRegene;
        public float MMP;
        public float MPRegene;
        public float MST;
        public float STRegene;
        public float PAtk;
        public float MAtk;
        public float PDef;
        public float MDef;

        public Class_EquipmentAdds[] Adds;
        public List<Data_Attack> SkillAttacks;
        public List<Class_Data_JobTreeGroupSet> JTGroupSet;
    }
}
