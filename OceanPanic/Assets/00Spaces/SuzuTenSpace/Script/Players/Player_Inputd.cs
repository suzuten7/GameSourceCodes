using UnityEngine;
using UnityEngine.InputSystem;
public class Player_Inputd : MonoBehaviour
{
    #region 変数
    PlayerInput PI;
    public Vector2 Move;
    public Vector2 Look;
    public bool Up;
    public bool Down;
    public bool Dash;
    public bool[] ACs = new bool[4];
    #endregion
    private void Start()
    {
        PI = FindFirstObjectByType<PlayerInput>();
    }
    void Update()
    {
        Move = PI.actions["Move"].ReadValue<Vector2>();
        Look = PI.actions["Look"].ReadValue<Vector2>();
        Up = PI.actions["Up"].IsPressed();
        Down = PI.actions["Down"].IsPressed();
        Dash = PI.actions["Dash"].IsPressed();
        for(int i = 0; i < 4; i++)
        {
            ACs[i] = PI.actions["AC" + (i + 1)].IsPressed();
        }
    }
}
