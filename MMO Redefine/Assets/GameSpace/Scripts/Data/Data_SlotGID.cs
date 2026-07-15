namespace Datas
{
    using UnityEngine;
    public class Data_SlotGID : ScriptableObject
    {
        public string Name;
        [TextArea] public string Info;
        public Texture Icon;
        public int Type;
    }
}
