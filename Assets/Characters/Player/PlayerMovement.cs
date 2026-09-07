using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder.MeshOperations;

public class PlayerMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] CinemachineCamera playerCamera;
    [SerializeField] float normalCameraHeight = 1f; // ������ ���� ����
    [SerializeField] float crouchCameraHeight = 0.6f; // ������ ���� � �������

    [SerializeField] float transitionSpeed = 10f;
    [SerializeField] Transform cameraRoot;

    private float _newHeight, _normalHeight;

    [Header("StatementValues")]
    [SerializeField] float jumpHeight = 2.8f;
    public float walkSpeed = 11.6f;
    public float runSpeed = 18f;
    public float crouchSpeed = 8.5f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    private float _velocity;
    public float airSmoothTime = 0.3f;
    public float smoothTime = 0.1f;

    [Header("Slide")]
    [SerializeField] float getToGroundHeight = 3f;
    [SerializeField] float slideStartSpeed = 24.3f;
    [SerializeField] float slideEndSpeed = 8.5f;
    [SerializeField] float slideTime = 1.5f;
    private float _slideEndTime;

    [Header("Grounding")]
    [SerializeField] Transform groundCheckPivot;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float groundCheckRadius = 0.4f;
    RaycastHit _slopeHit;

    [Header("Toggles")]
    [HideInInspector] public bool isJumping, isCrouching, isSliding;
    [HideInInspector] public bool isRunning = true;

    private float _currentSpeed, _bonusSpeed;

    [HideInInspector] public Vector3 moveInput, moveDirection;
    [HideInInspector] public Vector3 currentVelocity;
    private Vector3 _wishDirection;
    private Vector3 _smoothVelocity;

    CharacterController _characterController;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _characterController = GetComponent<CharacterController>();

        _currentSpeed = isRunning ? runSpeed : walkSpeed;
        _normalHeight = _characterController.height;
    }
    void Update()
    {
        GravityApply();
        Move();
        CrouchSlide();
    }
    private Vector3 CameraForward()
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }
    private Vector3 CameraRight()
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }
    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheckPivot.position, groundCheckRadius, groundLayerMask);
    }
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _characterController.height * 0.5f + 0.4f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < _characterController.slopeLimit && angle != 0;
        }
        return false;
    }
    private bool CanGetToGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, getToGroundHeight, groundLayerMask);
    }

    void Move()
    {
        // ������� �����������
        _wishDirection = (CameraForward() * moveInput.y + CameraRight() * moveInput.x);
        Vector3 targetVelocity = _wishDirection * _currentSpeed;

        //�����������
        float smooth = IsGrounded() ? smoothTime : airSmoothTime;
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref _smoothVelocity, smooth);

        if (OnSlope())
        {
            moveDirection = moveDirection = Vector3.ProjectOnPlane(currentVelocity, _slopeHit.normal);
            if (_velocity <= 0)
                moveDirection.y += _velocity - 2f; // ��������� � �����
            else
                moveDirection.y = _velocity; // ���� ��������
        }
        else
        {
            moveDirection = currentVelocity;
            moveDirection.y = _velocity; // � ������� ������ ������/�����
        }

        _characterController.Move(moveDirection * Time.deltaTime);
    }
    public void Jump()
    {
        if (IsGrounded())
            _velocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }
    void CrouchSlide()
    {
        float targetCameraHeight = normalCameraHeight;
        float targetHeight = _normalHeight;

        if (isCrouching || isSliding)
        {
            if (!IsGrounded() && _velocity <= getToGroundHeight)
                GetToGround();

            if (IsGrounded())
            {
                targetCameraHeight = crouchCameraHeight;
                targetHeight = _normalHeight / 2f;

                if (isSliding & IsGrounded())
                {
                    float speedDrop = (slideStartSpeed - slideEndSpeed) / slideTime;
                    _currentSpeed = Mathf.MoveTowards(_currentSpeed, slideEndSpeed, speedDrop * Time.deltaTime);

                    if (_currentSpeed <= slideEndSpeed & Time.time >= _slideEndTime)
                        ExitCrouch();
                }
                else
                    _currentSpeed = crouchSpeed;
            }
            else if (isSliding && _velocity > 0) // ���� ��� ����� �����
            {
                _currentSpeed = isRunning ? runSpeed : walkSpeed;
            }
        }

        float newCameraY = Mathf.Lerp(cameraRoot.localPosition.y, targetCameraHeight, Time.deltaTime * transitionSpeed);
        cameraRoot.localPosition = new Vector3(0, newCameraY, 0); //�������� ������ ������

        _characterController.height = Mathf.Lerp(_characterController.height, targetHeight, Time.deltaTime * transitionSpeed);
    }
    public void EnterCrouch()
    {
        if (isRunning)
        {
            isSliding = true;
            _currentSpeed = slideStartSpeed;
            _slideEndTime = Time.time + slideTime;
            isCrouching = false;
        }
        else
        {
            isCrouching = true;
            isSliding = false;
        }
    }
    public void ExitCrouch()
    {
        _currentSpeed = isRunning ? runSpeed : walkSpeed;
        isCrouching = false; 
        isSliding = false;
    }
    public void EnterWalk()
    {
        isRunning = false;
        _currentSpeed = walkSpeed;
    }
    public void ExitWalk()
    {
        isRunning = true;
        _currentSpeed = runSpeed;
    }
    void GetToGround()
    {
        if (!IsGrounded() && CanGetToGround())
            _velocity = -15f;
    }
    private void GravityApply()
    {
        if (IsGrounded() && _velocity < 0)
            _velocity = -2;
        else
        {
            float gravityMultiplier = (_velocity < 0) ? 4f : 2f;
            _velocity += gravity * gravityMultiplier * Time.deltaTime;
        }
    }
}