using UnityEngine;

public class Player_ViewMode : MonoBehaviour
{
    public Camera mapCam;
    [SerializeField] Rigidbody2D rig;
    [SerializeField] float speedBase;
    [SerializeField] float speedDash;
    public bool dashIn = false;
    private void Start()
    {
        Obj_LocalObjects.ViewObj = this;
    }
    void Update()
    {
        var dashAc = Player_Input.PI.pi.actions["Dash"];
        var dashPress = dashAc.IsPressed();
        var dashTrigger = dashAc.triggered;
        var mvect = Player_Input.PI.pi.actions["Move"].ReadValue<Vector2>();
        if(Player_Moblie.PMoblie != null && Player_Moblie.PMoblie.Active)
        {
            mvect = Player_Moblie.PMoblie.move;
            dashPress = Player_Moblie.PMoblie.dash.press;
            dashTrigger = Player_Moblie.PMoblie.dash.trigger;
        }
        var dash = false;
        if (UI_OptionManager.OptionGetOnOff("GP_Option 01", false))
        {
            if (dashTrigger)   dashIn = !dashIn;
            if (dashIn) dash = true;
        }
        else
        {
            dash = dashPress;
        }
        rig.linearVelocity = mvect.normalized * (!dash ? speedBase : speedDash);
    }
}
