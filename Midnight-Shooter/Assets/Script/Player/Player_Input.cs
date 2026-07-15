using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Input : MonoBehaviour
{
    public PlayerInput pi;
    static public Player_Input PI;
    private void Start()
    {
        if (PI != null) return;
        PI = this;
    }
    static public Vector2 UIPoint
    {
        get
        {
            if (PI == null) return Vector2.zero;
            if(PI.pi.currentControlScheme == "Controller")
            {
                Mouse vmouse = (Mouse)InputSystem.GetDevice("VirtualMouse");
                if (vmouse != null) return vmouse.position.ReadValue();
            }
            return PI.pi.actions["UI_Point"].ReadValue<Vector2>();
        }
    }
}
