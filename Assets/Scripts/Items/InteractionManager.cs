using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable        //상호작용 인터페이스
{
    string GetInteractPrompt();
    void OnInteract();      //얘 정의가 왜 itemObject에 있는데 작동함?
}

public class InteractionManager : MonoBehaviour
{
    public float checkRate = 0.05f;        //시간 체크하는데 드는 최소 시간
    private float lastCheckTime;        //가장 최근의 시간
    public float maxCheckDistance;        //ray 발사했을 때 충돌한 최대 거리
    public LayerMask layerMask;        //레이어 조정 변수

    private GameObject curInteractGameobject;        //카메라 중앙에 온 오브젝트(상호작용을 하고 있는)
    private IInteractable curInteractable;        //카메라 중앙에 온 오브젝트의 인터페이스(상호작용을 하고 있는)

    public TextMeshProUGUI promptText;        //사용자의 명령을 받을 준비가 됐다는 표시를 나타내는 텍스트
    private Camera camera;        //카메라


    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;        //main으로 되어있는 카메라 하나만 잡힘 싱글톤 쓰는 것과 유사
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)        //checkRate 보다 최신 시간이 업데이트가 안 되어 있으면
        {
            lastCheckTime = Time.time;      //최신 시간을 현재 시간으로 변경

            /*
             * raycast를 쏠 때는 ray를 어떻게 쏠건지 먼저 정하고(1) 
             * ray를 쏴봤을 때(2) 어떻게 충돌이 일어나는지 정보를 받아와서(3)
             * 그 정보를 처리(4)
             * 
             */
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));        //화면의 중앙에서 ray를 발사
            RaycastHit hit;     //ray와 충돌한 object의 정보를 저장하는 변수

            /*
             * out : 해당 값을 무조건 채워서 반환
             * ref : 해당 값을 채워서 반환할 수도 있고 아닐 수도 있음
             * out으로 hit을 받아오기 때문에 무조건 값이 채워져 있을거라 생각하고 진행하면 됨
             */
            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                /*
                 * ray를 쐈을 때 ray에 맞은 오브젝트가 hit에 저장되고 hit과
                 * 현재 상호작용 중인 오브젝트값과 다를 경우 hit을 현재 상호작용 중인 오브젝트에 저장
                 * (update에 들어있으므로 카메라 중앙에 온 오브젝트(hit)와 
                 * 이전에 curInteractGameobject에 저장된 object가 있을 수 있음)
                 */
                if (hit.collider.gameObject != curInteractGameobject)
                {
                    curInteractGameobject = hit.collider.gameObject;        
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }
            else        //ray에 부딫힌 오브젝트가 없을 경우(화면 중앙에 오브젝트가 없을 경우) 현재 상호작용 중인 오브젝트 null처리
            {
                curInteractGameobject = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
            
        }
    }

    private void SetPromptText()        //interactive 오브젝트와 근접했을 때 프롬프트텍스트 active 시키는 함수
    {
        promptText.gameObject.SetActive(true);
        promptText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt());     //프롬프트 텍스트 내용
    }

    public void OnInteractInput(InputAction.CallbackContext callbackContext)
    {
        //상호작용 키가 눌러졌고 그 시점에 바라보고 있는 오브젝트가 있다면(화면 중앙에 오브젝트가 있다면)
        //아이템과 상호작용 진행 후 정보 초기화
        if (callbackContext.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractGameobject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}