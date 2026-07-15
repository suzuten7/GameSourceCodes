namespace Datas
{
    using UnityEngine;
    using static Data_Equips;
    [CreateAssetMenu(menuName = "DataCre/JobTree")]
    public class Data_JobTree : ScriptableObject
    {
        public string Name;
        [TextArea] public string Info;
        public Texture Icon;
        public int Point;
        public int LVMax;

        public Class_EquipmentAdds[] StateAdds;
        public Class_EquipmentTrigger[] TriggerAttacks;
        public Data_Attack[] SkillAdds;
    }
}
