namespace UIs
{
    using UnityEngine;

    public class UI_MiniMapIcon : MonoBehaviour
    {
        private void LateUpdate()
        {
            var Size = Vector3.one * 10;
            transform.localScale = Size;
        }
    }
}
