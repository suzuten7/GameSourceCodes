namespace State
{
    using Datas;
    using UnityEngine;
    using static Datas.Data_Attack;

    public partial class State_Shot_Base
    {
        public void AddShot(Data_AddShot AddShot,int time)
        {
            if (USta == null) return;
            var sfire = AddShot.Shot.Fire;
            for(int i = 0; i < sfire.Count; i++)
            {
                var pos = transform.position;
                var rot = transform.eulerAngles;
                switch (sfire.PosBase)
                {
                    case Enum_PosBase.TargetPositon:
                        pos = USta.TargetPosGet;
                        break;
                }
                foreach (var pch in sfire.PosChange)
                {
                    pos += Quaternion.Euler(transform.eulerAngles) * Atk_TransChange(pch, i,sfire.Count, time);
                }
                switch (sfire.RotBase)
                {
                    case Enum_RotBase.Fixed:
                        rot = Vector3.forward;
                        break;
                    case Enum_RotBase.MyToTarget:
                        rot = Quaternion.LookRotation(transform.position - USta.TargetPosGet, Vector3.forward).eulerAngles;
                        rot.z = -180f;
                        break;
                    case Enum_RotBase.ShotToTarget:
                        rot = Quaternion.LookRotation(pos - USta.TargetPosGet, Vector3.forward).eulerAngles;
                        rot.z = -180f;
                        break;
                }
                foreach (var rch in sfire.RotChange)
                {
                    rot += Atk_TransChange(rch, i,sfire.Count, time);
                }
                var qrot = Quaternion.Euler(rot);
                var vect = qrot * Vector3.forward * Random.Range(sfire.Speed.x, sfire.Speed.y) * 0.01f;
                USta.ShotSet(AddShot.Shot,aslot, -2, pos, vect,qrot, LEleRideID);
            }

        }
    }
}

