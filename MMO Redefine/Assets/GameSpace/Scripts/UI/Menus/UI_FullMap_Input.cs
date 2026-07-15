namespace UIs
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using static Player.Player_Controle;
    public class UI_FullMap_Input : MonoBehaviour, IDragHandler
    {
        [SerializeField] UI_FullMap_Base FMapBase;

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            FMapBase.Move(-eventData.delta * FMapBase.DragSpeed * 0.01f);
        }
        private void LateUpdate()
        {
            FMapBase.Move(PCont.V2_Move * FMapBase.MoveSpeed * 0.01f);
        }
    }
}

