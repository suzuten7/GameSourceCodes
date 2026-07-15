namespace UIs
{
    using Obj;
    using TMPro;
    using UnityEngine;
    using static GmSystem.GS_ChangeSet;
    using static GmSystem.GS_GlobalValues;
    public class UI_AreaView : MonoBehaviour
    {
        [SerializeField] CanvasGroup Canv;
        [SerializeField] TextMeshProUGUI NameTx;
        [SerializeField] TextMeshProUGUI LvTx;
        [SerializeField] float FadeInTime = 1.0f;
        [SerializeField] float StayTime = 1.5f;
        [SerializeField] float FadeOutTime = 0.7f;
        Obj_AreaView backView = null;
        float times = 9999;
        private void Start()
        {
            Canv.gameObject.SetActive(false);
        }
        void LateUpdate()
        {
            if(backView != AreaGet)
            {
                backView = AreaGet;
                times = 0;
                if(backView != null)
                {
                    ChangeText(NameTx,backView.Name);
                    ChangeText(LvTx,"ザコ敵Lv" + backView.LvRanges.x + "～" + backView.LvRanges.y);
                }
                else
                {
                    ChangeText(NameTx,"未定義エリア");
                    ChangeText(LvTx,"ザコ敵Lv???～???");
                }
            }
            times += Time.deltaTime;
            var alpha = 1f;
            if(times < FadeInTime) alpha = times / FadeInTime;
            else if(times > StayTime) alpha = 1f - (times - StayTime) / FadeOutTime;
            if(Canv.alpha != alpha) Canv.alpha = alpha;
            ChangeActive(Canv.gameObject, Canv.alpha > 0);
        }
    }
}

