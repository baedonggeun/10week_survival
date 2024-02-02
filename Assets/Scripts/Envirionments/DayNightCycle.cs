using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]     //인스펙터에서 해당 값을 0~1사이 값으로 스크롤로 조정시키는 헤더
    public float time;      //시간
    public float fullDayLength;     //하루의 전체 길이(시간초로 지정) ex)30초를 하루로
    public float startTime = 0.4f;      //하루 시작 시간
    private float timeRate;     //시간에 따른 해, 달의 이동량??
    public Vector3 noon;        //정오의 해의 각도 x : 90 y : 0 z : 0
    //z만 90이 아니면 됨 -> Yaw회전은 수평 회전이기 때문(이러면 북극에 있는 해의 움직임이 됨) ???y가 북극이고 z는 안보이는데 머징
    
    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;     //그래프를 만들 수 있는 변수

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;        //시간에 따른 달빛의 강도

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;       //??

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
        /*
         * rendersettings에 접근해 시간에 따른 전체적인 빛 강도 조절
         * ambientIntensity : 환경광 강도
         * reflectionIntensity : 반사광 강도
         */

    }

    void UpdateLighting(Light lightSource, Gradient colorGradiant, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);        //animationcurve에 시간값(x)을 주면 해당하는 그래프 값(y 빛의 강도)을 가져옴

        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4.0f;
        /*
         * ???
         * 
         */
        lightSource.color = colorGradiant.Evaluate(time);       //시간에 따른 색상값을 가져옴
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }
}