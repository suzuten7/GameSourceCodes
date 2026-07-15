namespace UIs
{
    using TMPro;
    using UnityEngine;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_ChangeSet;
    public class UI_Death : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI WaitRevTx;

        void LateUpdate()
        {
            if (MyPlayer == null) return;
            var time = MyPlayer.RevTime - (MyPlayer.ChangeValues.DeathTic / 60);
            var revstr = "その場で復活";
            if(time > 0) revstr += "\n(" + time.ToString("F0") + "秒後)";
            ChangeText(WaitRevTx, revstr);
        }
        public void RevButton(bool wait)
        {
            if (wait && MyPlayer.ChangeValues.DeathTic < Mathf.RoundToInt(MyPlayer.RevTime * 60)) return;
            MyPlayer.Respawne(wait);
            
        }
    }
}


