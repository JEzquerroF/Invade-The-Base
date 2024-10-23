using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRobot_Controller : Enemies
{
    [SerializeField] private float m_TimeResting;
    [SerializeField] private GameObject m_Shield;
    [SerializeField] private GameObject m_LaserStartPosition;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private AudioClip m_LaserSound;
    [SerializeField] private float laserDistance = 8f;
    [SerializeField] private LayerMask m_IgnoreMask;
    [SerializeField] private float m_LerpLaser;

    private RaycastHit rayHit;
    private Ray ray;
    private bool laserActive = true;
    private bool m_soundPlayed = false;

    private float m_Timer;
    private bool m_Resting = false;
    private bool m_IsShield;
    float laserProgress = 0f;

    public override void Start()
    {
        base.Start();
        lineRenderer.positionCount = 2;
        m_Timer = 0.0f;
    }

    public override void Update()
    {
        m_PlayerPosition = m_Player.GetPlayerPosition();

        switch (m_CurrentState)
        {
            case StatesEnemy.Idle:
                HandleIdleState();
                break;
            case StatesEnemy.Patrol:
                HandlePatrolState();
                break;
            case StatesEnemy.Alert:
                HandleAlertState();
                break;
            case StatesEnemy.Chase:
                HandleChaseState();
                break;
            case StatesEnemy.Attack:
                HandleAttackState();
                break;
            case StatesEnemy.Rest:
                HandleRestState();
                break;
            case StatesEnemy.Die:
                HandleDieState();
                break;
        }

        if (DistanceToTarget(m_PlayerPosition) > m_MaxDistanceToPatrol)
            m_CurrentState = StatesEnemy.Idle;
        else if (DistanceToTarget(m_PlayerPosition) <= m_MaxDistanceToPatrol && m_Alert == false && m_Resting == false)
            m_CurrentState = StatesEnemy.Patrol;
        else if (m_Alert && SeePlayerConeVision() == false && SeePlayerHit() && m_Resting == false)
            m_CurrentState = StatesEnemy.Alert;
        else if (m_Resting)
            m_CurrentState = StatesEnemy.Rest;
        else if (m_Alert && SeePlayerConeVision() && DistanceToTarget(m_PlayerPosition) >= m_MaxDistanceToAttack && SeePlayerHit() && m_Resting == false)
            m_CurrentState = StatesEnemy.Chase;
        else if (m_Alert && SeePlayerConeVision() && DistanceToTarget(m_PlayerPosition) < m_MaxDistanceToAttack && SeePlayerHit() && m_Resting == false)
            m_CurrentState = StatesEnemy.Attack;


        if (DistanceToTarget(m_PlayerPosition) <= m_MinDistanceToAlert)
            m_Alert = true;
        else
            m_Alert = false;

        if (DistanceToTarget(m_PlayerPosition) >= m_MinDistanceToAttack)
            m_AttackMoving = true;
        else
            m_AttackMoving = false;

        if (m_Alert && SeePlayerHit())
            RotationToTarget(m_PlayerPosition);

        if (m_CurrentState != StatesEnemy.Patrol)
            m_setPatrol = true;

        if (m_CurrentState == StatesEnemy.Alert)
        {
            m_EnemyAnimator.SetBool("Alert", true);
            m_SpeedRotation = 0.75f;
        }
        else
        {
            m_EnemyAnimator.SetBool("Alert", false);
            m_SpeedRotation = Mathf.Lerp(m_SpeedRotation, m_MaxLerpSpeedRotation, 0.5f * Time.deltaTime);
        }

        if (m_SeePlayer && !SeePlayerHit())
        {
            m_SawPlayer = true;
            m_SeePlayer = false;
        }

        if (m_SawPlayer)
        {
            m_CurrentState = StatesEnemy.Chase;
            m_TimeChasing += Time.deltaTime;

            if (m_TimeChasing >= m_MaxTimeChasing || SeePlayerHit() && m_Alert)
            {
                m_SawPlayer = false;
                m_TimeChasing = 0;
            }
        }

        if (m_CurrentState != StatesEnemy.Attack || m_IsDead)
        {
            lineRenderer.enabled = false;
            m_soundPlayed = false;
        }

        if (m_CurrentState == StatesEnemy.Rest || m_CurrentState == StatesEnemy.Attack)
        {
            m_Agent.isStopped = true;
        }

        if (m_Health <= 0)
        {
            m_Shield.SetActive(false);
        }
    }
    public override void HandleChaseState()
    {
        m_EnemyAnimator.SetBool("Attack", false);
        m_EnemyAnimator.SetBool("Idle", false);
        m_Agent.SetDestination(m_PlayerPosition);
        base.HandleChaseState();
    }

    public override void HandleAttackState()
    {
        m_Agent.isStopped = true;
        m_EnemyAnimator.SetBool("Attack", true);

        if (m_Health >= 0 && m_Resting == false)
        {
            if (!m_soundPlayed && m_IsDead == false)
            {
                SoundsManager.instance.PlayLongSound(m_LaserSound, transform, 0.2f, 2.50f);
                m_soundPlayed = true;
            }

            lineRenderer.enabled = true;

            RaycastHit l_hit;
            Vector3 l_direction = m_PlayerPosition - m_LaserStartPosition.transform.position;
            l_direction.Normalize();
            Vector3 l_FinalPoint = m_LaserStartPosition.transform.position + transform.forward * laserDistance;
            lineRenderer.SetPosition(0, m_LaserStartPosition.transform.position);

            //(Physics.Raycast(m_origin, m_direction, out l_hit, l_Distance, ~0, QueryTriggerInteraction.Ignore))

            if (Physics.Raycast(m_LaserStartPosition.transform.position, l_direction, out l_hit, laserDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                if (l_hit.collider.CompareTag("Player"))
                {
                    Life_Player_Controller l_PlayerLife = l_hit.collider.GetComponent<Life_Player_Controller>();
                    l_PlayerLife.UpdateHealth(-m_BulletDamage * Time.deltaTime);
                }
            }

            lineRenderer.SetPosition(1, l_FinalPoint);
        }
    }

    public override void HandleRestState()
    {
        m_Agent.isStopped = true;
        m_Timer += Time.deltaTime;

        if (m_Timer >= m_TimeResting)
        {
            m_Resting = false;
            m_Shield.SetActive(false);
            m_IsShield = false;
            m_EnemyAnimator.SetBool("Idle", false);
            m_Timer = 0;
        }
    }

    public void Resting()
    {
        m_Resting = true;
        m_EnemyAnimator.SetBool("Attack", false);
        m_EnemyAnimator.SetBool("Idle", true);
        m_Shield.SetActive(true);
        m_IsShield = true;
    }

    public override IEnumerator EnemyDie()
    {
        yield return new WaitForSeconds(1.4f);
        SoundsManager.instance.PlaySoundClip(m_ExplosionSound, transform, 0.2f);
        m_ExplosionParticles.Play();

        yield return new WaitForSeconds(0.25f);
        int l_RandomNumber = Random.Range(0, m_ItemsPrefabs.Length);
        Instantiate(m_ItemsPrefabs[l_RandomNumber], transform.position, m_ItemsPrefabs[l_RandomNumber].transform.rotation);
        m_Resting = false;

        gameObject.SetActive(false);
    }

    public override void GetDamage(float damage)
    {
        if (!m_IsShield)
            base.GetDamage(damage);
    }
}
