using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ShowItemToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private SlotUI slotUI;
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.itemAmount != 0)
            {
                inventoryUI.itemToolTipUI.gameObject.SetActive(true);
                inventoryUI.itemToolTipUI.SetupItemToolTip(slotUI.itemDetails, slotUI.slotType);

                inventoryUI.itemToolTipUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                inventoryUI.itemToolTipUI.transform.position = transform.position + Vector3.up * 30;
            }
            else
            {
                inventoryUI.itemToolTipUI.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemToolTipUI.gameObject.SetActive(false);
        }
    }
}
