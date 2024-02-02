using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType        //아이템 타입
{
    Resource,       //리소스
    Equipable,      //장착
    Consumable      //소비
}

public enum ConsumableType      //소비 아이템 타입
{
    Hunger,     //공복도 관련
    Health      //체력 관련
}

[System.Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]     //에셋 메뉴 설정 -> 우클릭 create 에 menu 추가
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;      //오브젝트 표시 이름
    public string description;      //설명
    public ItemType type;       //해당 아이템의 타입 설정
    public Sprite icon;     //아이콘 값
    public GameObject dropPrefab;       //프리펩으로 설정

    [Header("Stacking")]
    public bool canStack;       //중첩 가능 여부
    public int maxStackAmount;      //최대 중첩량

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;        //소비템인지 설정

}