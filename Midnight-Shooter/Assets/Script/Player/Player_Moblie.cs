using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
public class Player_Moblie : MonoBehaviour
{
    static public Player_Moblie PMoblie;
    public bool Active;
    [SerializeField] CanvasGroup UI;
    [SerializeField] GameObject targetStick;
    [SerializeField] Image adsUI;
    [SerializeField] Image dashUI;
    [SerializeField] Image walkUI;
    [SerializeField] GameObject[] viewNoUIs;
    public Vector2 move;
    public Vector2 target;
    public MInput shot;
    public MInput ads;
    public MInput reload;
    public MInput dash;
    public MInput walk;
    public MInput melee;
    public MInput gadget;
    public MInput ult;
    public MInput marker;
    [System.Serializable]
    public class MInput
    {
        public bool press;
        public bool trigger;
        bool bpress;
        public void TriggerSet()
        {
            trigger = press && !bpress;
            bpress = press;
        }
    }
    private void Start()
    {
        PMoblie = this;
        bool actives = false;
        if(Application.platform == RuntimePlatform.Android)actives = true;
        if (Application.platform == RuntimePlatform.IPhonePlayer) actives = true;
        if(actives)Active = true;
    }
    private void Update()
    {
        UI.gameObject.SetActive(Active);
        if (!Active) return;
        UI.alpha = UI_OptionManager.OptionGetFloat("To_Option 00", 50)*0.01f;
        TouchTarget();
        shot.TriggerSet();
        ads.TriggerSet();
        reload.TriggerSet();
        dash.TriggerSet();
        walk.TriggerSet();
        melee.TriggerSet();
        gadget.TriggerSet();
        ult.TriggerSet();
        marker.TriggerSet();
        var adsIn = false;
        var dashIn= false;
        var walkIn= false;
        if (Obj_LocalObjects.MyPlayer != null)
        {
            var con = Obj_LocalObjects.MyPlayer.controlle;
            adsIn = con.ads.press;
            dashIn = con.dash.press;
            walkIn = con.walk.press;
        }
        else if(Obj_LocalObjects.ViewObj != null)
        {
            dashIn = dash.press || Obj_LocalObjects.ViewObj.dashIn;
        }
        var adsCol = adsIn ? Color.yellow : Color.white;
        adsCol.a = adsUI.color.a;
        adsUI.color = adsCol;
        var dashCol = dashIn ? Color.yellow : Color.white;
        dashCol.a = dashUI.color.a;
        dashUI.color = dashCol;
        var walkCol = walkIn ? Color.yellow : Color.white;
        walkCol.a = walkUI.color.a;
        walkUI.color = walkCol;

        for(int i = 0; i < viewNoUIs.Length; i++)
        {
            var vnui = viewNoUIs[i];
            if (vnui == null) continue;
            vnui.SetActive(Obj_LocalObjects.MyPlayer != null);
        }
    }
    public void InputSetMove(Vector2 vect)
    {
        move = vect;
    }
    public void InputSetTarget(Vector2 vect)
    {
        target = vect;
    }
    public void TouchTarget()
    {
        var mode = UI_OptionManager.OptionGetInt("To_Option 02", 0);
        targetStick.SetActive(mode == 0);
        if (mode == 0)
        {
            return;
        }
        TouchState? ts1 = null;
        for (int i = 0; i <= 4; i++)
        {
            TouchState ts = Player_Input.PI.pi.actions["Touch_" + i].ReadValue<TouchState>();
            if (ts.isInProgress == true)
            {
                var results = new List<RaycastResult>();
                var pointer = new PointerEventData(EventSystem.current)
                {
                    position = ts.startPosition
                };
                EventSystem.current.RaycastAll(pointer, results);
                bool CamUI = true;
                foreach (RaycastResult target in results)
                {
                    if (target.gameObject.CompareTag("NoTouch"))
                    {
                        CamUI = false;
                        break;
                    }
                }
                if (CamUI == true)
                {
                    if (ts1 == null) ts1 = ts;
                }
            }
        }
        if (ts1 != null)
        {
            switch (mode)
            {
                case 1:
                    target = ts1.Value.delta;
                    break;
                case 2:
                    target = ts1.Value.position;
                    break;
            }
        }
        else
        {
            switch (mode)
            {
                case 1:
                    target = Vector2.zero;
                    break;
            }
        }
    }
    public void InputSetShot(bool input)
    {
        shot.press = input;
    }
    public void InputSetAds(bool input)
    {
        ads.press = input;
    }
    public void InputSetReload(bool input)
    {
        reload.press = input;
    }
    public void InputSetDash(bool input)
    {
        dash.press = input;
    }
    public void InputSetWalk(bool input)
    {
        walk.press = input;
    }
    public void InputSetMelee(bool input)
    {
        melee.press = input;
    }
    public void InputSetGadget(bool input)
    {
        gadget.press = input;
    }
    public void InputSetUlt(bool input)
    {
        ult.press = input;
    }
    public void InputSetMarker(bool input)
    {
        marker.press = input;
    }



}
