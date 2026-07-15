using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestRayCast_Gabu : MonoBehaviour
{
    public PlayerInput playerInput;
    public InputAction scroll;
    public float lengh = 2;

    private void Start()
    {
        scroll = playerInput.actions["Scroll"];
    }

    private void Update()
    {
        Debug.Log(playerInput.actions["Scroll"].ReadValue<Vector2>());
        lengh += (float)playerInput.actions["Scroll"].ReadValue<Vector2>().y;

        Debug.DrawRay(transform.position, Vector3.right * lengh, Color.yellow, 0.04f);
    }
}
