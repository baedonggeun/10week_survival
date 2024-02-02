using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image image;
    public float flashSpeed;        //플래시 터지는 속도

    private Coroutine coroutine;        

    public void Flash()
    {
        if (coroutine != null)      //이전에 coroutine을 돌린 적이 있다면
        {
            StopCoroutine(coroutine);       //coroutine을 정지
        }

        image.enabled = true;       //이미지 보이게
        image.color = Color.red;        //이미지 색상 변경
        coroutine = StartCoroutine(FadeAway());     //함수를 startcoroutine으로 동작
    }

    private IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while (a > 0.0f)        //coroutine이므로 반복문 실행
        {
            a -= (startAlpha / flashSpeed) * Time.deltaTime;        //fadeaway 효과를 위해 값을 시간에 따라 감소시킴
            image.color = new Color(1.0f, 0.0f, 0.0f, a);       //붉은 색으로 설정
            yield return null;
        }

        image.enabled = false;      //이미지 안 보이게
    }
}