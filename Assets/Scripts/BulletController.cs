using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private AudioClip M_PlayerHitSound;

    private GameObject m_BulletDecal;
    private bool m_collision;
    private Vector3 m_EndPointFrame;
    public Vector3 m_origin;
    public Vector3 m_direction;
    private float m_BulletSpeed;
    private float m_bulletDamage;
    public static bool m_RangeRunning;

    private void Start()
    {
        m_collision = false;
    }
    public void Shoot(Vector3 Position, Vector3 Direction, GameObject bullet, float bulletSpeed, float bulletDamage)
    {
        transform.position = Position;
        transform.rotation = Quaternion.LookRotation(Direction);
        m_origin = Position;
        m_direction = Direction;
        m_collision = false;
        m_BulletDecal = bullet;
        m_BulletSpeed = bulletSpeed;
        m_bulletDamage = bulletDamage;
    }

    void Update()
    {
        if (m_collision == false)
        {
            m_EndPointFrame = m_origin + m_direction * m_BulletSpeed * Time.deltaTime;
            float l_Distance = Vector3.Distance(m_origin, m_EndPointFrame);
            RaycastHit l_hit;

            if (Physics.Raycast(m_origin, m_direction, out l_hit, l_Distance, ~0, QueryTriggerInteraction.Ignore))
            {
                //Bullet Decal

                if (l_hit.collider.tag != "Player")
                {
                    m_BulletDecal.SetActive(true);
                    m_BulletDecal.transform.SetParent(null);
                    m_BulletDecal.transform.rotation = Quaternion.LookRotation(l_hit.normal);
                    m_BulletDecal.transform.position = l_hit.point;
                    m_BulletDecal.transform.localScale = Vector3.one / 100;
                    m_BulletDecal.transform.SetParent(l_hit.collider.transform);
                }

                float l_headshoot = 1.5f;

                if (l_hit.collider.CompareTag("StaticTarget"))
                {
                    StaticTarget l_Staticarget = l_hit.collider.GetComponent<StaticTarget>();

                    l_Staticarget.StaticRegisterHit();
                }

                if (l_hit.collider.CompareTag("MovingTarget"))
                {
                    MovingTarget l_MovingTarget = l_hit.collider.GetComponent<MovingTarget>();

                    l_MovingTarget.MovingRegisterHit();
                }

                if (l_hit.collider.CompareTag("ShootingRange") && m_RangeRunning == false)
                {
                    ShootingRange l_ShootingRange = l_hit.collider.GetComponentInParent<ShootingRange>();

                    l_ShootingRange.StartRangeCoroutine();
                    m_RangeRunning = true;
                }

                if (l_hit.collider.CompareTag("EnemyHead"))
                {
                    Enemies enemy = l_hit.collider.GetComponentInParent<Enemies>();
                    enemy.GetDamage(l_headshoot * m_bulletDamage);
                }
                else if (l_hit.collider.CompareTag("EnemyBody"))
                {
                    Enemies enemy = l_hit.collider.GetComponentInParent<Enemies>();
                    enemy.GetDamage(m_bulletDamage);
                }
                else if (l_hit.collider.CompareTag("Player"))
                {
                    Life_Player_Controller l_playerlife = l_hit.collider.GetComponent<Life_Player_Controller>();
                    SoundsManager.instance.PlaySoundClip(M_PlayerHitSound, transform, 0.4f);
                    l_playerlife.UpdateHealth(-m_bulletDamage);
                }
                m_collision = true;
            }
            m_origin = m_EndPointFrame;
        }
    }
}
