using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI quatityText;
    private ItemSlot curSlot;
    private Outline outline;        //아이템 외곽 선(선택 시 활성화되며 아이템이 선택됐다는 표시를 함)

    public int index;
    public bool equipped;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()     //equipped 값이 true면 아웃라인을 켜줌
    {
        outline.enabled = equipped;
    }

    public void Set(ItemSlot slot)      //아이템 슬롯을 전달해주면 그 슬롯으로 값을 세팅
    {
        curSlot = slot;     //현재 슬롯에 가져온 슬롯 입력
        icon.gameObject.SetActive(true);        //아이콘 활성화
        icon.sprite = slot.item.icon;       //아이콘 스프라이트를 가져온 슬롯의 아이템 아이콘으로 설정
        quatityText.text = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty;
        //슬롯의 수량 텍스트가 가져온 슬롯의 수가 1보다 큰 경우 그 숫자를 1보다 작거나 같은 경우 Empty로 설정

        if (outline != null)        //아웃라인이 존재한다면 equipped 값을 넣고 아웃라인을 켜줌
        {
            outline.enabled = equipped;
        }
    }

    public void Clear()     //아이템 슬롯 제거
    {
        curSlot = null;     //현재 아이템 슬롯 비우기
        icon.gameObject.SetActive(false);       //아이콘 비활성화
        quatityText.text = string.Empty;        //수량 텍스트 empty로 설정
    }

    public void OnButtonClick()     //슬롯 클릭시 인벤토리 클래스의 SelectItem 함수에 index를 넘겨주고 실행
    {
        Inventory.instance.SelectItem(index);
    }
}