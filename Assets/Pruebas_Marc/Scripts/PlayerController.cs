using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour, IRestartGameElement
{
    public Transform m_PitchController;
    private CharacterController m_CharacterController;
    private float m_Yaw;
    private float m_Pitch;

    [SerializeField] private float m_YawSpeed;
    [SerializeField] private float m_PitchSpeed;

    [SerializeField] private float m_MinPitch;
    [SerializeField] private float m_MaxPitch;
    
    [SerializeField] private float m_MovementSpeed;
    [SerializeField] private float m_VerticalSpeed;
    [SerializeField] private float m_RunSpeedMultiplier;
    [SerializeField] private float m_JumpSpeed;

    private Vector3 m_StartPosition;
    private Quaternion m_StartRotation;

    [Header("Keys")]
    private KeyCode m_LeftKeyCode = KeyCode.A;
    private KeyCode m_RightKeyCode = KeyCode.D;
    private KeyCode m_UpKeyCode = KeyCode.W;
    private KeyCode m_DownKeyCode = KeyCode.S;
    private KeyCode m_RunKeyCode = KeyCode.LeftShift;
    private KeyCode m_JumpKeyCode = KeyCode.Space;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        GameManagerMarc l_GameManager = GameManagerMarc.m_GameManager.GetGameManager();

        m_Yaw = transform.eulerAngles.y;
        m_Pitch = m_PitchController.localRotation.eulerAngles.x;


        l_GameManager.AddRestartGameElement(this);
        l_GameManager.SetPlayer(this);

        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
    }

    private void Update()
    {
        #region PlayerCam
        float l_HorizontalValue = Input.GetAxis("Mouse X");
        float l_VerticalValue = -Input.GetAxis("Mouse Y");
        //m_Pitch -= l_VerticalValue;

        m_Yaw = m_Yaw + l_HorizontalValue * m_YawSpeed * Time.deltaTime;
        m_Pitch = m_Pitch + l_VerticalValue * m_PitchSpeed * Time.deltaTime;
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        transform.rotation = Quaternion.Euler(0, m_Yaw, 0);

        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0, 0);
        #endregion PlayerCam

        #region PlayerMovement
        float l_ForwardAngleRadians = m_Yaw * Mathf.Deg2Rad;
        float l_RightAngleRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;

        Vector3 l_Forward = new Vector3(Mathf.Sin(l_ForwardAngleRadians), 0.0f, Mathf.Cos(l_ForwardAngleRadians));
        Vector3 l_Right = new Vector3(Mathf.Sin(l_RightAngleRadians), 0.0f, Mathf.Cos(l_RightAngleRadians));

        Vector3 l_Movement = Vector3.zero;

        if (Input.GetKey(m_RightKeyCode))
            l_Movement = l_Right;
        else if(Input.GetKey(m_LeftKeyCode))
            l_Movement = -l_Right;

        if (Input.GetKey(m_UpKeyCode))
            l_Movement += l_Forward;
        else if (Input.GetKey(m_DownKeyCode))
            l_Movement -= l_Forward;

        l_Movement.Normalize();

        if (m_CharacterController.isGrounded && Input.GetKeyDown(m_JumpKeyCode))
            m_VerticalSpeed = m_JumpSpeed;

        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;

        float l_SpeedMultiplier = 1.0f;

        if (Input.GetKey(m_RunKeyCode))
            l_SpeedMultiplier = m_MovementSpeed * m_RunSpeedMultiplier;

        l_Movement *= m_MovementSpeed * l_SpeedMultiplier * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);

        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
            m_VerticalSpeed = 0.0f;

        if((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
            m_VerticalSpeed = 0.0f;
        #endregion PlayerMovement
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Item l_Item = other.GetComponent<Item>();

            if (l_Item.CanPick())
            {
                l_Item.Pick();
            }
        }
    }

    public void RestartGame()
    {
        
    }
}
