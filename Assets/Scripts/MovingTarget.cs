using System.Collections;
using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    [SerializeField] private ShootingRange m_ShootingRange;
    [SerializeField] private Transform[] m_TargetPositions;
    [SerializeField] private Animator m_MovingTargetAnimator;
    [SerializeField] private AudioClip m_BulletHitSound;
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_WaitTime;

    private int m_CurrentIndex = 0;
    private bool m_Forward = true;
    private int m_HitCounter;

    public void StartMovingTargets()
    {
        StartCoroutine(MovingTargets());
    }

    public void StopMovingTargets()
    {
        m_MovingTargetAnimator.SetBool("MovingTargetFalling", true);
        m_MovingTargetAnimator.SetBool("MovingTargetRising", false);
        StopAllCoroutines();
    }

    private IEnumerator MovingTargets()
    {
        m_MovingTargetAnimator.SetBool("MovingTargetRising", true);
        m_MovingTargetAnimator.SetBool("MovingTargetFalling", false);

        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            Vector3 l_TargetPosition = m_TargetPositions[m_CurrentIndex].position;

            while (Vector3.Distance(transform.position, l_TargetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, l_TargetPosition, m_Speed * Time.deltaTime);
                yield return null;
            }

            if (m_Forward)
            {
                m_CurrentIndex++;
                if (m_CurrentIndex >= m_TargetPositions.Length)
                {
                    m_CurrentIndex = m_TargetPositions.Length - 1;
                    m_Forward = false;
                }
            }
            else
            {
                m_CurrentIndex--;
                if (m_CurrentIndex < 0)
                {
                    m_CurrentIndex = 0;
                    m_Forward = true;
                }
            }

            yield return new WaitForSeconds(m_WaitTime);
        }
    }

    public void MovingRegisterHit()
    {
        SoundsManager.instance.PlaySoundClip(m_BulletHitSound, transform, 0.2f);
        m_HitCounter++;
        m_ShootingRange.m_Score += 100;

        if (m_HitCounter >= 3)
        {
            m_HitCounter = 0;
        }
    }
}

