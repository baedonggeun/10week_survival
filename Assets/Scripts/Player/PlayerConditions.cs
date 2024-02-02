using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IDamagable
{
    void TakePhysicalDamage(int damageAmount);
}

[System.Serializable]       //Condition 클래스를 unity에서 인스펙터 창에 나타낼 수 있도록 Serialize
public class Condition
{
    [HideInInspector]
    public float curValue;      //현재 값
    public float maxValue;      //최대 값
    public float startValue;        //시작 값
    public float regenRate;     //회복률
    public float decayRate;     //감소율
    public Image uiBar;     //ui의 이미지

    public void Add(float amount)       //현재값 변화량과 최대값 중 작은 값 사용(최대값 안 넘도록)
    {
        curValue = Mathf.Min(curValue + amount, maxValue);
    }

    public void Subtract(float amount)      //현재값 변화량과 0 중 큰 값 사용(0보다 낮아지지 않도록)
    {
        curValue = Mathf.Max(curValue - amount, 0.0f);
    }

    public float GetPercentage()        //유니티에선 대부분 0~1의 범위값을 사용하기 때문에 비율값 생성
    {
        return curValue / maxValue;
    }

}


public class PlayerConditions : MonoBehaviour, IDamagable
//c# 에선 다중 클래스 상속은 지원하지 않지만, 인터페이스 다중 상속은 지원함
{
    public Condition health;        //체력
    public Condition hunger;        //공복도
    public Condition stamina;       //스테미나

    public float noHungerHealthDecay;       //공복도가 0일 경우 체력 감소수치

    public UnityEvent onTakeDamage;     //데미지 받을 때 처리할 이벤트

    void Start()        //시작 시, curValue를 시작값으로 초기화
    {
        health.curValue = health.startValue;
        hunger.curValue = hunger.startValue;
        stamina.curValue = stamina.startValue;
    }

    // Update is called once per frame
    void Update()
    {
        hunger.Subtract(hunger.decayRate * Time.deltaTime);     //시간에 따른 공복도 감소
        stamina.Add(stamina.regenRate * Time.deltaTime);        //시간에 따른 스테미나 회복

        if (hunger.curValue == 0.0f)        //공복도가 0일 경우, 시간에 따른 체력 감소
            health.Subtract(noHungerHealthDecay * Time.deltaTime);

        if (health.curValue == 0.0f)        //체력이 0일 경우, 죽음
            Die();

        health.uiBar.fillAmount = health.GetPercentage();       //체력 UI bar 값 처리
        hunger.uiBar.fillAmount = hunger.GetPercentage();       //공복도 UI bar 값 처리
        stamina.uiBar.fillAmount = stamina.GetPercentage();       //스테미나 UI bar 값 처리
        /*
         * 인스펙터의 값을 코드편집기에서 수정하고 싶으면 인스펙터 창에서 해당 이름 확인하기!
         * UI에서 fillAmount 라고 unity 인스펙터 창에 적혀 있음
         * 얘네들은 컴포넌트에 Header로 저장된 변수들이라 위와 같이 바로 접근해서 사용할 수 있음
         */
    }

    public void Heal(float amount)      //힐
    {
        health.Add(amount);
    }

    public void Eat(float amount)       //공복도 증가
    {
        hunger.Add(amount);
    }

    public bool UseStamina(float amount)        //스테미나 사용 여부
    {
        if (stamina.curValue - amount < 0)      //스테미나 0보다 작으면 사용x(스테미나 쓸 수 있는지 체크)
            return false;

        stamina.Subtract(amount);       //스테미나 사용 후, 사용했다는 bool값(true) 반환
        return true;
    }

    public void Die()       //죽었을 경우, 로그 생성
    {
        Debug.Log("플레이어가 죽었다.");
    }

    public void TakePhysicalDamage(int damageAmount)        //데미지를 받았을 경우, 이벤트 확인 후 실행
    {
        health.Subtract(damageAmount);
        onTakeDamage?.Invoke();
    }
}