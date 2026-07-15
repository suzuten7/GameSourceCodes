namespace State
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Player;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalState;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_ChangeSet;
    public class State_DamageObj : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI DamTx;
        [SerializeField] float DelTime;
        [SerializeField] float UpSpeed;
        float _times = 0;

        private void Start()
        {
            ParentStrage(gameObject, "Damage");
        }
        void LateUpdate()
        {
            if (Camera.main != null)
            {
                transform.LookAt(Camera.main.transform);
            }
            transform.position += Vector3.up * Time.deltaTime * UpSpeed;
            var alpha = 1f - (_times / DelTime);
            ChangeColor(DamTx, AlphaChange(DamTx.color,alpha));

            _times += Time.deltaTime;
            if (_times >= DelTime) Destroy(gameObject);
        }
        static float Dam_Size(State_StateBase hsta, State_StateBase asta, bool damheal, bool heal= false)
        {
            var size = (float)GetSave_Option.DSizes[0];
            if (hsta == null) size *= GetSave_Option.DSizes[6] * 0.01f;
            else if (damheal)
            {
                if (hsta.CommonValues.Team == MyPlayer.CommonValues.Team)
                    size *= (!heal ? GetSave_Option.DSizes[1] : GetSave_Option.DSizes[3]) * 0.01f;
                else
                    size *= (!heal ? GetSave_Option.DSizes[2] : GetSave_Option.DSizes[4]) * 0.01f;
            }
            else size *= (hsta.CommonValues.Team == MyPlayer.CommonValues.Team ? GetSave_Option.DSizes[5] : GetSave_Option.DSizes[6]) * 0.01f;

            if(asta == null) size *= GetSave_Option.DSizes[7] * 0.01f;
            else if ((int)asta.CommonValues.Team >= (int)State_StateBase.Enum_Team.PLTeamC || (int)asta.CommonValues.Team <= (int)State_StateBase.Enum_Team.PLTeamA)
            {
                var psta = asta.GetComponent<Player_State>();
                if (CanControl(asta.Object))
                {
                    if (psta == null) size *= GetSave_Option.DSizes[13] * 0.01f;
                    else if (psta.BotID < 0) size *= GetSave_Option.DSizes[9] * 0.01f;
                    else size *= GetSave_Option.DSizes[11] * 0.01f;
                }
                else
                {
                    if (psta == null) size *= GetSave_Option.DSizes[14] * 0.01f;
                    else if (psta.BotID < 0) size *= GetSave_Option.DSizes[10] * 0.01f;
                    else size *= GetSave_Option.DSizes[12] * 0.01f;
                }
            }
            else if(asta.CommonValues.Team == State_StateBase.Enum_Team.Collect) size *= GetSave_Option.DSizes[8] * 0.01f;
            else size *= GetSave_Option.DSizes[7] * 0.01f;

            return size;
        }
        public void DamSet(State_StateBase hsta, State_StateBase asta, float val,bool crit,Color colElement,byte RegHit,byte RegEle)
        {
            var size = Dam_Size(hsta,asta,true,val <0);
            var dstr = UIs.UI_System.ValueStrings(Mathf.Abs(val), true);
            var astr = "";
            if (crit)
            {
                dstr = "☆" + dstr;
                size *= 1.2f;
            }
            if (RegHit == 0)
            {
                astr += "↓";
                size *= 0.8f;
            }
            if (RegHit == 2)
            {
                astr += "↑";
                size *= 1.1f;
            }
            if (RegEle == 0)
            {
                astr += "↓";
                size *= 0.8f;
            }
            if (RegEle == 2)
            {
                astr += "↑";
                size *= 1.1f;
            }
            if (astr != "") dstr += "<size=75%><cspace=-60>" + astr + "</cspace></size>";
            DamTx.text = dstr;
            DamTx.transform.localScale = Vector3.one * size * 0.01f;


            var colDown = Color.white;
            if (hsta.CommonValues.Team == MyPlayer.CommonValues.Team)
            {
                colDown = val >= 0 ? Color.red : Color.green;
            }
            else if(val < 0) colDown = Color.magenta;

            var grad = DamTx.colorGradient;

            grad.topLeft = colElement;
            grad.topRight = colElement;

            grad.bottomLeft = colDown;
            grad.bottomRight = colDown;

            DamTx.colorGradient = grad;
            Material mat = DamTx.fontMaterial;
            mat.SetColor("_GradCol1", colElement);
            TeamGet_Str((int)hsta.CommonValues.Team, out var col);
            mat.SetColor("_OutCol2", col);
        }
        public void TextSet(State_StateBase hsta, State_StateBase asta, float sizePer, string str, Color colIn, Color colOut)
        {
            DamTx.text = str;
            DamTx.fontSize = Dam_Size(hsta,asta,false) *sizePer *0.01f;
            var grad = DamTx.colorGradient;
            grad.topLeft = colIn;
            grad.topRight = colIn;
            DamTx.colorGradient = grad;
            Material mat = DamTx.fontMaterial;
            mat.SetColor("_GradCol1", colIn);
            mat.SetColor("_OutCol2", colOut);
        }
        Color AlphaChange(Color col,float alpha)
        {
            col.a = alpha;
            return col;
        }
    }
}
