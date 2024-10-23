using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemies : MonoBehaviour, IRestartGame
{
    [SerializeField] protected List<Transform> m_PatrolPoints;
    [SerializeField] protected Transform m_BulletShootPosition;
    [SerializeField] protected float m_MaxHealth;
    [SerializeField] protected float m_SpeedRotation;
    [SerializeField] protected float m_BulletSpeed;
    [SerializeField] protected float m_BulletDamage;
    [SerializeField] protected float m_MaxDistanceToAttack;
    [SerializeField] protected float m_MinDistanceToAttack;
    [SerializeField] protected float m_MaxDistanceToPatrol;
    [SerializeField] protected float m_MinDistanceToAlert;
    [SerializeField] protected float m_ConeAngle;
    [SerializeField] protected float m_TimeCadenceShoot;
    [SerializeField] protected GameObject[] m_ItemsPrefabs;
    [SerializeField] protected float m_MaxTimeChasing;

    protected Player m_Player;
    protected Vector3 m_PlayerPosition;
    protected Vector3 m_StartPosition;
    protected Animator m_EnemyAnimator;
    protected NavMeshAgent m_Agent;
    protected StatesEnemy m_CurrentState;
    protected bool m_Idle;
    protected int m_CurrentPatrol;
    protected bool m_FirstPatrol;
    protected bool m_AttackMoving;
    protected bool m_setPatrol;
    protected bool m_Alert;
    protected bool m_CanShoot;
    protected bool m_FirstShoot;
    protected bool m_IsDead;
    protected bool m_SeePlayer;
    protected bool m_SawPlayer;
    protected float m_Health;
    protected float m_TimeChasing;
    protected float m_TimeShoot;
    protected float m_MaxLerpSpeedRotation = 20;
    protected Vector3 m_LastValidPosition;

    [Header("Particles&Sound")]
    [SerializeField] protected ParticleSystem m_DronMuzzleParticles;
    [SerializeField] protected ParticleSystem m_ExplosionParticles;
    [SerializeField] protected AudioClip m_ExplosionSound;

    protected enum StatesEnemy
    {
        Idle,
        Patrol,
        Alert,
        Chase,
        Attack,
        Rest,
        Die
    }

    public virtual void Start()
    {
        m_CurrentState = StatesEnemy.Idle;
        m_EnemyAnimator = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.autoBraking = false;
        m_Health = m_MaxHealth;
        m_FirstPatrol = true;
        m_setPatrol = true;
        m_FirstShoot = true;
        m_TimeChasing = 0;
        m_StartPosition = transform.position;
        m_Player = GameManager.instance.GetPlayer();
        GameManager.instance.AddRestartGame(this);
        GameManager.instance.AddEnemies(this);
    }

    public virtual void Update()
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
            case StatesEnemy.Die:
                HandleDieState();
                break;
        }

        if (DistanceToTarget(m_PlayerPosition) > m_MaxDistanceToPatrol || m_Idle)
            m_CurrentState = StatesEnemy.Idle;
        else if (DistanceToTarget(m_PlayerPosition) <= m_MaxDistanceToPatrol && m_Alert == false)
            m_CurrentState = StatesEnemy.Patrol;
        else if (m_Alert && SeePlayerConeVision() == false && SeePlayerHit())
            m_CurrentState = StatesEnemy.Alert;
        else if (m_Alert && SeePlayerConeVision() && DistanceToTarget(m_PlayerPosition) >= m_MaxDistanceToAttack && SeePlayerHit())
            m_CurrentState = StatesEnemy.Chase;
        else if (m_Alert && SeePlayerConeVision() && DistanceToTarget(m_PlayerPosition) < m_MaxDistanceToAttack && SeePlayerHit())
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

        if (m_SawPlayer && m_Alert)
        {
            m_CurrentState = StatesEnemy.Chase;
            m_TimeChasing += Time.deltaTime;

            if (m_TimeChasing >= m_MaxTimeChasing || SeePlayerHit() && m_Alert)
            {
                m_SawPlayer = false;
                m_TimeChasing = 0;
            }
        }
    }

    public virtual void GetDamage(float damage)
    {
        m_Health -= damage;
        m_Health = Mathf.Clamp(m_Health, 0, m_MaxHealth);

        if (m_Health <= 0)
        {
            HandleDieState();
        }
    }

    public virtual void HandleIdleState()
    {
    }

    public virtual void HandlePatrolState()
    {
        m_Agent.isStopped = false;
        Transform l_PatrolPoint;

        if (m_setPatrol)
        {
            l_PatrolPoint = m_PatrolPoints[SetPatrol()];
            m_setPatrol = false;
        }

        l_PatrolPoint = m_PatrolPoints[m_CurrentPatrol];
        m_Agent.SetDestination(l_PatrolPoint.position);

        if (Vector3.Distance(l_PatrolPoint.position, transform.position) <= 4f)
            m_setPatrol = true;
    }

    public virtual void HandleAlertState()
    {
        m_Agent.isStopped = true;
        RotationToTarget(m_PlayerPosition);
    }

    public virtual void HandleChaseState()
    {
        m_Agent.isStopped = false;
        m_Agent.SetDestination(m_PlayerPosition);
    }

    public void Shoot()
    {
        m_TimeShoot += Time.deltaTime;

        if (m_CanShoot || m_FirstShoot)
        {
            GameObject l_bullet = GameManager.instance.GetBullet();
            GameObject l_bulletDecal = GameManager.instance.GetBulletDecal();

            l_bullet.SetActive(true);
            l_bullet.transform.position = m_BulletShootPosition.position;
            l_bullet.transform.rotation = Quaternion.LookRotation(GetDirectionPlayer());

            BulletController l_bulletController = l_bullet.GetComponent<BulletController>();
            l_bulletController.Shoot(m_BulletShootPosition.position, GetDirectionPlayer(), l_bulletDecal, m_BulletSpeed, m_BulletDamage);
            m_DronMuzzleParticles.Play();

            m_CanShoot = false;
            m_FirstShoot = false;
            m_TimeShoot = 0.0f;
        }

        if (m_TimeShoot > m_TimeCadenceShoot)
            m_CanShoot = true;
    }

    public virtual void HandleAttackState()
    {
        if (m_AttackMoving)
        {
            m_Agent.isStopped = false;
            m_Agent.SetDestination(m_PlayerPosition);
        }
        else
        {
            m_Agent.isStopped = true;
        }

        if (m_Health > 0)
            Shoot();
    }

    public virtual void HandleRestState()
    {

    }

    public virtual void HandleDieState()
    {
        if (m_IsDead) return;

        m_IsDead = true;

        m_EnemyAnimator.SetBool("Die", true);
        StartCoroutine(EnemyDie());
    }

    public virtual IEnumerator EnemyDie()
    {
        yield return new WaitForSeconds(1.4f);
        SoundsManager.instance.PlaySoundClip(m_ExplosionSound, transform, 0.2f);
        m_ExplosionParticles.Play();

        yield return new WaitForSeconds(0.25f);
        int l_RandomNumber = Random.Range(0, m_ItemsPrefabs.Length);
        Instantiate(m_ItemsPrefabs[l_RandomNumber], new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), m_ItemsPrefabs[l_RandomNumber].transform.rotation);
        m_IsDead = false;
        gameObject.SetActive(false);
    }

    public virtual bool SeePlayerConeVision()
    {
        Vector3 l_directionToPlayer = (m_PlayerPosition - m_BulletShootPosition.position); // REVISAR LAYER
        l_directionToPlayer.Normalize();

        float l_angleToPlayer = Vector3.Angle(transform.forward, l_directionToPlayer);

        Debug.DrawLine(m_BulletShootPosition.position, m_PlayerPosition, Color.blue);

        if (l_angleToPlayer < m_ConeAngle / 2)
        {
            return true;
        }

        return false;
    }

    public virtual bool SeePlayerHit()
    {
        Vector3 l_directionToPlayer = (m_PlayerPosition - m_BulletShootPosition.position);
        l_directionToPlayer.Normalize();
        RaycastHit l_hit;
        Debug.DrawRay(m_PlayerPosition, l_directionToPlayer, Color.blue);
        if (Physics.Raycast(m_BulletShootPosition.position, l_directionToPlayer, out l_hit, (DistanceToTarget(m_PlayerPosition) + 10.0f), ~0, QueryTriggerInteraction.Ignore))
        {
            if (l_hit.collider.CompareTag("Player"))
            {
                m_SeePlayer = true;
                return true;
            }
        }
        return false;
    }

    public virtual void RotationToTarget(Vector3 l_target)
    {
        Vector3 l_direction = (l_target - transform.position).normalized;
        Quaternion l_rotation = Quaternion.LookRotation(l_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, l_rotation, Time.deltaTime * m_SpeedRotation);
    }

    public virtual float DistanceToTarget(Vector3 l_target)
    {
        return Vector3.Distance(l_target, transform.position);
    }

    private Vector3 GetDirectionPlayer()
    {
        Vector3 l_direction = m_PlayerPosition - transform.position;

        return l_direction.normalized;
    }

    private int SetPatrol()
    {
        if (m_FirstPatrol)
        {
            float l_minDistance = Vector3.Distance(transform.position, m_PatrolPoints[0].transform.position);
            int l_nextPatrolPosition = 0;

            for (int i = 1; i < m_PatrolPoints.Count; i++)
            {
                if (Vector3.Distance(transform.position, m_PatrolPoints[i].transform.position) < l_minDistance)
                {
                    l_minDistance = Vector3.Distance(transform.position, m_PatrolPoints[i].transform.position);
                    l_nextPatrolPosition = i;
                }
            }
            m_FirstPatrol = false;
            m_CurrentPatrol = l_nextPatrolPosition;

            return m_CurrentPatrol;
        }
        else
        {
            m_CurrentPatrol++;

            if (m_CurrentPatrol >= m_PatrolPoints.Count)
            {
                m_CurrentPatrol = 0;
            }
        }
        return m_CurrentPatrol;
    }

    private void OnDrawGizmos()
    {
        Vector3 l_forward = m_BulletShootPosition.forward;
        float l_ConeAngle = m_ConeAngle / 2;

        float l_MaxDistance = m_MinDistanceToAlert;

        Vector3 l_leftDirection = Quaternion.Euler(0, -l_ConeAngle, 0) * l_forward;
        Vector3 l_RightDirection = Quaternion.Euler(0, l_ConeAngle, 0) * l_forward;

        Debug.DrawRay(m_BulletShootPosition.position, l_forward * l_MaxDistance, Color.green);
        Debug.DrawRay(m_BulletShootPosition.position, l_leftDirection * l_MaxDistance, Color.red);
        Debug.DrawRay(m_BulletShootPosition.position, l_RightDirection * l_MaxDistance, Color.red);

        int l_NumRays = 10;

        for (int i = 1; i < l_NumRays; ++i)
        {
            float t = i / (float)(l_NumRays);
            Vector3 interpolatedDirection = Vector3.Slerp(l_leftDirection, l_RightDirection, t);
            Debug.DrawRay(m_BulletShootPosition.position, interpolatedDirection * l_MaxDistance, Color.yellow);
        }
    }

    public float GetHealth()
    {
        return m_Health;
    }

    public void RestartGame()
    {
        Debug.Log("RESTART");
        m_CurrentState = StatesEnemy.Idle;
        m_Agent.velocity = Vector3.zero;
        m_Alert = false;
        m_SeePlayer = false;
        m_setPatrol = true;
        m_FirstShoot = true;
        m_FirstPatrol = true;
        m_IsDead = false;
        m_Health = m_MaxHealth;
        transform.position = m_StartPosition;
    }
}
