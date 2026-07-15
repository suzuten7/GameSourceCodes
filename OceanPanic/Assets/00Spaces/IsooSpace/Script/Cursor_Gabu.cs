using UnityEngine;

public class Cursor_Gabu : MonoBehaviour
{
    public bool isLock = true;
    private void Start()
    {
        Cursor.lockState = isLock ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
