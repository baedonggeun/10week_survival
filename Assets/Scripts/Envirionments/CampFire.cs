using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour       //coroutine을 사용하지 않는 데미지 구현 방식
{
    public int damage;      //캠프파이어 데미지
    public float damageRate;        //캠프파이어 데미지율

    private List<IDamagable> thingsToDamage = new List<IDamagable>();       //
    /*
     * 삽입 삭제가 계속 일어나기 때문에 list 로 구현
     * 처리할 코드가 많아지는 경우 HashSet<T> 활용하는 것도 좋음(삽입 삭제가 한 번에 일어나기 때문)
     */


    private void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate);       //일정 시간 후에 데미지 주는 것을 반복
        /*
         * 유니티 내부에서 string으로 동작하는 함수가 생각보다 많다고 함
         * 
         */
    }
    /*
     * 데미지를 주는 방식 종류
     * 1. Update에서 deltaTime에 따라 일정 시간 이상 collision 발생 시, 데미지 함수 실행(반복)
     * 2. coroutine을 활용해 waitForSecond waitForPhysicalSecond 등 시간을 기다리는 코드를 활용해 데미지 함수 실행
     * 3. InvokeRepeating 함수로 지연 실행(반복)하여 데미지 함수 실행
     */

    void DealDamage()       //thingsToDamage list 안에 있는 요소들이 모두 데미지를 주도록 설정
    {
        for (int i = 0; i < thingsToDamage.Count; i++)
        {
            thingsToDamage[i].TakePhysicalDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamagable damagable))     
        {
            thingsToDamage.Add(damagable);
        }
        /*
         * 인터페이스를 쓰는 이유
         * 어떤 애인지는 모르지만 damagable 이라는 인터페이스를 상속 받은 상태이므로 
         * takePhysicalDamage 함수가 있을 것이고, 내가 그 함수를 쓸거니 다른게 뭐가 있든 없든
         * 그 함수만 가져와서 쓰기 위해
         */
        /*
         * try 달린 함수 동작 방식
         * ()안의 값을 제대로 찾아온다면 true를 반환
         * 아닌 경우 false를 반환
         */
    }
    //OnCollisionEnter로 하면 캠프파이어에 다가가면 충돌이 일어나 맞지 않기 때문에 OnTrrigerEnter사용

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            thingsToDamage.Remove(damagable);
        }
    }
    
}