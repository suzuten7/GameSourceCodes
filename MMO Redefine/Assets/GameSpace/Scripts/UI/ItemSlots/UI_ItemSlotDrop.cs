namespace UIs
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UI_ItemSlotDrop : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            if (!UI_System.IsDrag) return;
            Debug.Log("入れるよ！");

            var itemdrag = eventData.pointerDrag?.GetComponent<UI_ItemDragDrop>();
            var itemslot = itemdrag != null ? itemdrag.ItemSlot : null;
            if (itemslot == null)
            {
                Debug.LogError("アイテムなんて無い！！");
                return;
            }
            Debug.Log("入れる物" + itemslot.ItemGID);
            GetComponent<UI_ItemSlotBase>().ItemChange(itemslot);
        }
    }
}
