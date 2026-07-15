namespace State
{
    using UnityEngine;
    public class State_OtherAnim : MonoBehaviour
    {
        public State_StateBase Sta;
        [SerializeField] Animator Anim;
        [SerializeField] Class_AtkAnimSet[] AtkAnimSets;

        [SerializeField] int MoveID;
        [SerializeField] int AtkID;
        [SerializeField] float AtkSpeed;
        [SerializeField] int OtherID;
        [System.Serializable]
        class Class_AtkAnimSet
        {
            public Enum_AtkType Type;
            public int StaAID;
            public int AnimAID;
            public float ASpeedPer;
        }

        enum Enum_AtkType
        {
            All,
            NAtk,
            LAtk,
            RAtk,
            SAtk,
        }
        public void LateUpdate()
        {
            MoveID = Sta.AnimValues.MoveID;
            AtkID = 0;
            AtkSpeed = 1;
            OtherID = Sta.AnimValues.OtherID;
            for (int i = 0; i < AtkAnimSets.Length; i++)
            {
                var aanims = AtkAnimSets[i];
                bool atkl = false;
                bool atkr = false;
                bool atks = false;
                switch (aanims.Type)
                {
                    default:
                        atkl = true;
                        atkr = true;
                        atks = true;
                        break;
                    case Enum_AtkType.NAtk:
                        atkl = true;
                        atkr = true;
                        break;
                    case Enum_AtkType.LAtk:
                        atkl = true;
                        break;
                    case Enum_AtkType.RAtk:
                        atkr = true;
                        break;
                    case Enum_AtkType.SAtk:
                        atks = true;
                        break;
                }
                if (atkl) AtkSet(aanims, Sta.AnimValues.LAtkID, Sta.AnimValues.LAtkSpeed);
                if (atkr) AtkSet(aanims, Sta.AnimValues.RAtkID, Sta.AnimValues.RAtkSpeed);
                if (atks) AtkSet(aanims, Sta.AnimValues.SAtkID, Sta.AnimValues.SAtkSpeed);
            }
            Anim.SetInteger("MoveID", MoveID);
            Anim.SetInteger("AtkID", AtkID);
            Anim.SetFloat("AtkSpeed",AtkSpeed);
            Anim.SetInteger("OtherID", OtherID);

        }
        void AtkSet(Class_AtkAnimSet aanims,int SAID,float SASpeed)
        {
            if ((aanims.StaAID <= 0 && SAID > 0) || (aanims.StaAID > 0 && aanims.StaAID == SAID))
            {
                AtkID = aanims.AnimAID;
                AtkSpeed = SASpeed * (1f + aanims.ASpeedPer * 0.01f);
            }
        }
    }
}

