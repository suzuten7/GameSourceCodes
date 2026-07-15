using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
public class VMouseSettings : MonoBehaviour
{
    static public VMouseSettings settings;
    PlayerInput PI;
    [SerializeField] VirtualMouseInput VMouse;
    [SerializeField] SoftwareCursorPositionAdjuster vsoft;
    [SerializeField] GameObject mouseObj;
    float speedBase;
    private void Start()
    {
        speedBase = VMouse.cursorSpeed;
        if (settings == null) settings = this;
    }
    void Update()
    {
        if (settings != this) return;
        var active = false;
        if (SceneManager.GetActiveScene().buildIndex == 0) active = true;
        if(Obj_LocalObjects.UIOpenCheck)active = true;
        VMouse.enabled = active;
        vsoft.enabled = active;
        if (!active)
        {
            mouseObj.SetActive(false);
            return;
        }
        if (PI == null) PI = FindAnyObjectByType<PlayerInput>();
        if (PI.actions["VMouseMove"].ReadValue<Vector2>().magnitude <= UI_OptionManager.OptionGetFloat("C_Option 01", 1f) * 0.01f)
        {
            VMouse.cursorSpeed = 0;
        }
        else
        {
            VMouse.cursorSpeed = speedBase * UI_OptionManager.OptionGetFloat("C_Option 02", 100) * 0.01f;
        }
    }
}
