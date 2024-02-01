using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]        //이동과 관련된 값
    public float moveSpeed;     //이동속도
    private Vector2 curMovementInput;       //input system에서 들어온 커서의 값
    public float jumpForce;     //점프 세기
    public LayerMask GroundLayerMask;       //

    [Header("Look")]        //시선처리와 관련된 값
    public Transform cameraContainer;       //main camera를 포함한 cameraContainer 텍스쳐의 transform
    public float minXLook;      //
    public float maxXLook;      //
    private float camCurXRot;      //마우스 2D 좌표 y축 이동 시(위아래), 카메라 x축 회전
    public float lookSensitivity;      //마우스 민감도에 따른 시선 민감도

    private Vector2 mouseDelta;      //

    [HideInInspector]      //
    public bool canLook = true;      //시선 on off 기능
    private Rigidbody _rigidbody;      //
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
        //??
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
     * InputActionPhase(인풋액션 단계) 에는 started performed canceled 등이 있음
     * 이를 통해 키를 어떤 식으로 눌렀는지를 구분할 수 있음
     * 얘네는 언제 값을 가져오는지를 결정함
     * started는 키 입력이 들어왔을 때 맨 처음 프레임에서 1번 값을 가져오고 이후로는 가져오지 않음
     * performed는 키 입력이 눌려지고 있으면 지속적으로 값을 가져옴
     * canceled는 키 입력이 떨어졌을 때 값을 가져옴
     */
}
