using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class ItemSlot       //아이템 슬롯
{
    public ItemData item;       //아이템
    public int quantity;        //수량
}

public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlots;        //아이템 슬롯 키고 끄기 위한 UI 여러 개 배열로(개수 정해져 있기 때문)
    public ItemSlot[] slots;        //아이템 슬롯

    public GameObject inventoryWindow;      //인벤토리 창 오브젝트
    public Transform dropPosition;      //드랍 위치

    [Header("Selected Item")]
    private ItemSlot selectedItem;      //선택된 아이템 오브젝트
    private int selectedItemIndex;      //선택된 아이템의 번호
    public TextMeshProUGUI selectedItemName;        //선택된 아이템의 이름
    public TextMeshProUGUI selectedItemDescription;     //선택된 아이템 설명
    public TextMeshProUGUI selectedItemStatNames;       //선택된 아이템 스탯 이름
    public TextMeshProUGUI selectedItemStatValues;      //선택된 아이템 스탯 값
    public GameObject useButton;        //사용 버튼
    public GameObject equipButton;      //장비 버튼
    public GameObject unEquipButton;        //장비 해제 버튼
    public GameObject dropButton;       //드랍 버튼

    private int curEquipIndex;      //현재 장비하고 있는 아이템의 번호

    private PlayerController controller;
    private PlayerConditions condition;

    [Header("Events")]
    public UnityEvent onOpenInventory;      //인벤토리 창 활성화 이벤트
    public UnityEvent onCloseInventory;     //인벤토리 창 비활성화 이벤트

    public static Inventory instance;       //싱글톤
    void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerConditions>();
    }
    private void Start()
    {
        inventoryWindow.SetActive(false);       //인벤토리 메뉴 비활성화한 상태로 시작
        slots = new ItemSlot[uiSlots.Length];
        //prefabs로 만들어 놓은 itemslot을 UI밑에 개수를 지정해서 넣었으므로 길이가 정해져 있음

        for (int i = 0; i < slots.Length; i++)      //slots와 uiSlots 초기화
        {
            slots[i] = new ItemSlot();
            uiSlots[i].index = i;
            uiSlots[i].Clear();
        }

        ClearSeletecItemWindow();
    }

    public void OnInventoryButton(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Started)
        {
            Toggle();
        }
    }


    public void Toggle()        //인벤토리 창 키고 끄는 함수
    {
        if (inventoryWindow.activeInHierarchy)      //하이라키 상에서 인벤토리 창이 켜져있으면 
        {
            inventoryWindow.SetActive(false);       //인벤토리 창 끔
            onCloseInventory?.Invoke();
            //controller.ToggleCursor(false);
        }
        else        //하이라키 상에서 인벤토리 창이 꺼져있으면
        {
            inventoryWindow.SetActive(true);        //인벤토리 창 킴
            onOpenInventory?.Invoke();
            //controller.ToggleCursor(true);
        }
    }

    public bool IsOpen()        //인벤토리 창이 켜져있는지 꺼져있는지 확인하는 함수
    {
        return inventoryWindow.activeInHierarchy;       //active값(true / false)를 반환
    }

    public void AddItem(ItemData item)      //아이템 추가 함수
    {
        if (item.canStack)      //아이템이 스택 가능하다면(중첩)
        {
            ItemSlot slotToStackTo = GetItemStack(item);        
            //추가하려는 아이템이 기존에 중첩된 아이템이 있는지 확인하기 위해 
            //가져온 아이템의 중첩값을 새로운 아이템 슬롯에 넣음???
            if (slotToStackTo != null)      //중첩된 아이템이 있다면
            {
                slotToStackTo.quantity++;       //수량 추가
                UpdateUI();     //UI 업데이트
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();        
        //위 코드에서 return이 안 됐다면 스택가능 중첩아이템x or 스택 불가능인 아이템이므로 새로운 슬롯에 값 추가

        if (emptySlot != null)      //빈 슬롯이 있다면
        {
            emptySlot.item = item;      //빈 슬롯에 아이템 넣어줌
            emptySlot.quantity = 1;     //수량 1개로 설정
            UpdateUI();     //UI 업데이트
            return;
        }

        ThrowItem(item);
        //위 코드에서 return이 안 됐다면 빈 슬롯도 없는 것이므로 드랍
    }

    void ThrowItem(ItemData item)       //아이템 드랍 함수
    {
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360f));
        //랜덤한 회전을 갖고 지정한 위치에 매개변수로 들어온 ItemData 생성
    }

    void UpdateUI()     //슬롯의 UI 데이터 최신화
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)      //슬롯 배열의 해당 요소에 아이템이 있다면
                uiSlots[i].Set(slots[i]);       //슬롯에 있는 데이터로 uiSlots의 데이터 최신화
            else
                uiSlots[i].Clear();     //아이템이 없다면 ui초기화
        }
    }

    ItemSlot GetItemStack(ItemData item)        //아이템 중첩 개수 설정(최대값을 넘지 못하게) 함수
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item && slots[i].quantity < item.maxStackAmount)
                return slots[i];
        }

        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
                return slots[i];
        }

        return null;
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null)
            return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDescription.text = selectedItem.item.description;

        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        for (int i = 0; i < selectedItem.item.consumables.Length; i++)
        {
            selectedItemStatNames.text += selectedItem.item.consumables[i].type.ToString() + "\n";
            selectedItemStatValues.text += selectedItem.item.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped);
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].equipped);
        dropButton.SetActive(true);
    }

    private void ClearSeletecItemWindow()
    {
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;

        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void OnUseButton()       //아이템 사용 버튼 동작 시 작동해야할 함수
    {
        if (selectedItem.item.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.item.consumables.Length; i++)
            {
                switch (selectedItem.item.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.item.consumables[i].value); break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.item.consumables[i].value); break;
                }
            }
        }
        RemoveSelectedItem();
    }

    public void OnEquipButton()     //장착 버튼 동작 시 작동해야할 함수
    {

    }

    void UnEquip(int index)     //장착 해제 함수
    {

    }

    public void OnUnEquipButton()       //장착 해제 버튼 동작 시 작동해야할 함수
    {

    }
    /*
     *아이템 드랍 버튼
     *TrowItem 함수가 있지만 해당 함수는 드랍 기능만을 나타내고
     *OnDropButton에서 드랍기능, 인벤토리에서 제거 등
     * 드랍 시 작동할 함수 및 기능들을 버튼에 달아주기 위한 함수
     */
    public void OnDropButton()      //드랍 버튼 동작 시 작동해야할 함수
    {
        ThrowItem(selectedItem.item);       //선택된 아이템 드랍
        RemoveSelectedItem();       //인벤토리에서 해당 아이템 제거
    }

    private void RemoveSelectedItem()       //인벤토리에서 선택된 아이템 제거 함수
    {
        selectedItem.quantity--;

        if (selectedItem.quantity <= 0)
        {
            if (uiSlots[selectedItemIndex].equipped)
            {
                UnEquip(selectedItemIndex);
            }

            selectedItem.item = null;
            ClearSeletecItemWindow();
        }

        UpdateUI();
    }

    public void RemoveItem(ItemData item)       //아이템 제거 함수
    {

    }

    public bool HasItems(ItemData item, int quantity)
    {
        return false;
    }
}