using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]        //이동과 관련된 값
    public float moveSpeed;     //이동속도
    private Vector2 curMovementInput;       //input system에서 들어온 마우스의 이동 값(이동에 사용할 변수)
    public float jumpForce;     //점프 세기
    public LayerMask groundLayerMask;       //

    [Header("Look")]        //시선처리와 관련된 값
    public Transform cameraContainer;       //main camera를 포함한 cameraContainer 텍스쳐의 transform
    public float minXLook;      //
    public float maxXLook;      //
    private float camCurXRot;      //보여지는 화면(2d) y축 값 변경 시(Roll 회전)이므로, 3d 상에 존재하는 카메라의 x축 회전
    public float lookSensitivity;      //마우스 민감도에 따른 시선 민감도
    private Vector2 mouseDelta;      //input system에서 들어온 마우스의 이동 값(시선 처리에 사용할 변수)

    [HideInInspector]      //헤더로 변수를 저장하지만 인스펙터 상에선 나타내지 않는 헤더
    public bool canLook = true;      //시선 on off 기능
    private Rigidbody _rigidbody;      //player의 rigidbody 컴포넌트를 저장할 변수
    public static PlayerController instance;      //싱글톤화

    private void Awake()
    {
        instance = this;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;       //1인칭 fps의 경우 커서 위치가 시점 위치이므로 커서 고정
        //Cursor.visible = false;       //커서 보이지 않게 변경
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if(canLook)
        {
            CameraLook();
        }
    }

    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        //오브젝트의 로컬 방향의 값에 input system 에서 들어온 값을 곱해 방향 설정(단위 벡터)
        dir *= moveSpeed;       //이동 시 벡터의 크기
        dir.y = _rigidbody.velocity.y;      //??왜 z값이 아님?

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);       //최소값 최대값 범위 이외의 값을 갖지 않도록 함
       
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        //카메라를 포함한 카메라 컨테이너의 x값을 마우스 상하 움직임을 각도로 받아옴
        //??왜 x값만 받아옴??
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
        //??
    }

    //event와 연결하여 input system의 입력 처리가 일어난 값을 바로바로 매개변수로 받아와 mouseDelta에 저장
    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    //event와 연결하여 input system의 입력 처리가 일어난 값을 바로바로 매개변수로 받아와 curMovementInput에 저장
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }
    /*
     * InputActionPhase(인풋액션 단계) 에는 started performed canceled 등이 있음 -> 보스 1페이즈 2페이즈 할 때 그 페이즈
     * 이를 통해 키를 어떤 식으로 눌렀는지를 구분할 수 있음
     * 얘네는 언제 값을 가져오는지를 결정함
     * started는 키 입력이 들어왔을 때 맨 처음 프레임에서 1번 값을 가져오고 이후로는 가져오지 않음
     * performed는 키 입력이 눌려지고 있으면 지속적으로 값을 가져옴
     * canceled는 키 입력이 떨어졌을 때 값을 가져옴
     */

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if(IsGrounded())
            {
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
            }
            
        }
    }
    /*
    * AddForce(방향 * 힘, 힘의 종류)
    * 리지드바디에 힘을 전달해주는 함수
    * ForceMode.Impulse : 불연속 + 질량 무시x -> 짧은 순간의 힘, 충돌이나 폭발 점프 같은 것에 많이 쓰임
    * * ForceMode.VelocityChange : 불연속 + 질량 무시o -> 질량이 다른 함대같은 경우 같은 힘으로 같은 속도로 움직이게 함
    * ForceMode.Force : 연속 + 질량 무시x -> 현실적인 물리현상을 나타낼때 주로 사용
    * ForceMode.Acceleration : 연속 + 질량 무시o -> 질량 관계 없이 가속
    * 
    */

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down)
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            { 
                return true; 
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + (transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * 0.2f), Vector3.down);
    }

    public void ToggleCursor(bool toggle)       //토글(인벤 창 등) 여부에 따라 커서와 시야 회전 키고 끄는 함수
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        //toggle 이 true false에 따라 커서 락 키고 끔;
        canLook = !toggle;      //토글이 켜졌을 경우(커서 생김) 시야 못 돌리게, 토글이 꺼졌을 경우(커서 없어짐) 시야 돌리게
    }
}
