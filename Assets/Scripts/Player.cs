using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour, IRestartGame
{
    [SerializeField] private GameObject m_PlayerPosition;
    private Life_Player_Controller m_LifePlayerController;
    private CheckpointController m_CheckpointController;
    private Player_Controller m_PlayerController;
    private Quaternion m_StartRotation;
    private Vector3 m_StartPosition;
    private Animator m_Animator;
    public float m_StartSpeed;

    public static Action<bool> OnCheckpointEntered;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        GameManager.instance.SetPlayer(this);
        GameManager.instance.AddRestartGame(this);
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_Animator = GetComponent<Animator>();
        m_LifePlayerController = GetComponent<Life_Player_Controller>();
        m_PlayerController = GetComponent<Player_Controller>();
        m_StartSpeed = m_PlayerController.GetSpeed();
    }

    public Vector3 GetPlayerPosition()
    {
        return m_PlayerPosition.transform.position;
    }

    public void StopAnimation()
    {
        m_Animator.SetBool("Aim", false);
        m_Animator.SetBool("Shooting", false);
        m_Animator.SetBool("Reloading", false);
        m_Animator.SetBool("Walking", false);
    }

    public void RestartGame()
    {
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_Animator.SetBool("Death", false);
    }

    private void SetStartPosition(Vector3 l_position)
    {
        m_StartPosition = l_position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            m_CheckpointController = other.GetComponent<CheckpointController>();

            if (m_CheckpointController.IsChecked() == false)
            {
                OnCheckpointEntered?.Invoke(true);
                SetStartPosition(m_CheckpointController.CheckPointPosition());
                m_CheckpointController.SetChecked(true);
            }
        }
    }
}
