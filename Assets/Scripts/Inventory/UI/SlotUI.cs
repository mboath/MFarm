using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("组件获取")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        public Image slotHighlight;
        [SerializeField] private Button button;

        [Header("格子类型")]
        public SlotType slotType;

        //是否选中
        public bool isSelected;

        //格子序号
        public int slotIndex;

        //物品信息和数量
        public ItemDetails itemDetails;
        public int itemAmount;

        //父物体InventoryUI组件
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>(); 
        // private InventoryUI inventoryUI { get { GetComponentInParent<InventoryUI>(); } }

        private void Start()
        {
            isSelected = false;
            // public变量会初始化，不能用itemDetails == null来判断
            if (itemDetails.itemID == 0)
            {
                UpdateEmptySlot();
            }
        }

        /// <summary>
        /// 更新格子UI和信息
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="amount">数量</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            //保存数据
            itemDetails = item;
            itemAmount = amount;

            //更新格子UI
            slotImage.enabled = true;
            slotImage.sprite = item.itemIcon;
            amountText.text = amount.ToString();
            button.interactable = true;
        }

        /// <summary>
        /// 更新格子为空
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
            }

            itemAmount = 0;

            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemAmount == 0) return;

            isSelected = !isSelected;
            inventoryUI.UpdateSlotHightlight(slotIndex);

            if (slotType == SlotType.Player)
            {
                //通知物品被选中的状态
                EventHandler.CallItemSlectedEvent(itemDetails, isSelected);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemAmount != 0)
            {
                inventoryUI.dragItemImage.enabled = true;
                inventoryUI.dragItemImage.sprite = slotImage.sprite;
                inventoryUI.dragItemImage.SetNativeSize();

                isSelected = true;
                inventoryUI.UpdateSlotHightlight(slotIndex);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItemImage.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItemImage.enabled = false;

            //Debug.Log(eventData.pointerCurrentRaycast);
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                    return;

                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;

                //Player背包范围内交换
                if (slotType == SlotType.Player && targetSlot.slotType == SlotType.Player)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }

                //清空所有高亮显示
                inventoryUI.UpdateSlotHightlight(-1);
            }
/*            else //测试物品扔在地上
            {
                if (itemDetails.canDrop)
                {
                    //鼠标对应世界坐标
                    var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

                    EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
                }
            }*/
        }      
    }
}
