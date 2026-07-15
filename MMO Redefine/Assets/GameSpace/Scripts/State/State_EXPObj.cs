
namespace State
{
    using UnityEngine;
    using Player;
    using static Datas.Data_Get;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalState;
    public class State_EXPObj : MonoBehaviour
    {
        [SerializeField] float DelTime;
        public int EXP;
        int _times = 0;
        private void Start()
        {
            ParentStrage(gameObject, "Drop");
        }
        private void FixedUpdate()
        {
            _times++;
            if (_times >= DelTime * 60) Destroy(gameObject);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<State_StateHit>(out var shit)) return;
            var psta = shit.State.GetComponent<Player_State>();
            if (psta != null && CanControl(psta.Object) && psta.BotID < 0)
            {
                LPlayerVal.EXP += EXP;
                var dobj = Instantiate(DB.DamObj, psta.PosGet, Quaternion.identity);
                TeamGet_Str((int)psta.CommonValues.Team, out var oTeamCol);
                dobj.TextSet(psta,psta,70,"EXP+" + EXP,oTeamCol,new Color(1,1,0.5f));
                Destroy(gameObject);
            }
        }
    }
}
