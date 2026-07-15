namespace Player
{
    using Fusion;
    using UnityEngine;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_ChangeSet;
    public class Player_ContActives : NetworkBehaviour
    {
        [SerializeField] Player_State PSta;
        [SerializeField] GameObject[] HasObjs;

        [SerializeField] GameObject[] OtherObjs;
        private void Start()
        {
            Update();
        }

        void Update()
        {
            bool has = CanControl(Object) && PSta.BotID < 0;
            foreach (var obj in HasObjs)
            {
                if (obj != null)ChangeActive(obj, has);;
            }
            foreach (var obj in OtherObjs)
            {
                if (obj != null)ChangeActive(obj,!has);
            }
        }
    }
}
