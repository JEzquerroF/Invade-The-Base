using System.Collections;
using TMPro;
using UnityEngine;

public class ShootingRange : MonoBehaviour
{
    [SerializeField] private MovingTarget[] m_MovingTarget;
    [SerializeField] private Animator[] m_StaticTargetAnimator;
    [SerializeField] private TMP_Text[] m_ScoreTextMonitor;
    [SerializeField] private TMP_Text m_ScoreText;
    [SerializeField] private float m_Round1Time;
    [SerializeField] private float m_Round2Time;

    private float m_Timer;
    private int m_CurrentRound;

    public bool m_IsRangeActive;
    public int m_Score;
    public int m_BestScore;

    private void Start()
    {
        m_ScoreText.enabled = false;
        m_IsRangeActive = false;
        m_CurrentRound = 0;
    }

    private void Update()
    {
        if (m_ScoreText.isActiveAndEnabled)
        {
            m_ScoreText.text = "Score: " + m_Score;
        }

        if (m_IsRangeActive)
        {
            m_Timer += Time.deltaTime;

            if ((m_CurrentRound == 1 && m_Timer > m_Round1Time) || (m_CurrentRound == 2 && m_Timer > m_Round2Time))
            {
                if (m_CurrentRound == 1)
                {
                    StartRound2();
                }
                else
                {
                    EndShootingRange();
                }
            }
        }
    }

    public void StartRangeCoroutine()
    {
        StartCoroutine(StartShootingRange());
    }

    public void StopRangeCoroutine()
    {
        StopAllCoroutines();
        StartCoroutine(StopShootingRange());
    }

    private IEnumerator StartShootingRange()
    {
        m_IsRangeActive = true;
        m_ScoreText.enabled = true;
        m_Score = 0;

        m_CurrentRound = 1;
        ActivateStaticTargets(true);

        yield return null;
    }

    private void StartRound2()
    {
        m_CurrentRound = 2;
        m_Timer = 0;

        ActivateStaticTargets(true, true);
        ActivateMovingTargets(true);
    }

    private void EndShootingRange()
    {
        m_IsRangeActive = false;
        m_Timer = 0;
        BulletController.m_RangeRunning = false;

        if (m_Score >= m_BestScore)
            m_BestScore = m_Score;

        for (int i = 0; i < m_ScoreTextMonitor.Length; i++)
        {
            m_ScoreTextMonitor[i].text = m_BestScore.ToString();
        }

        StopRangeCoroutine();
    }

    private IEnumerator StopShootingRange()
    {
        m_ScoreText.enabled = false;
        ActivateStaticTargets(false);
        ActivateMovingTargets(false);
        yield return null;
    }

    private void ActivateStaticTargets(bool l_Activate, bool l_ActivateAllAtOnce = false)
    {
        if (l_Activate)
        {
            if (l_ActivateAllAtOnce)
            {
                for (int i = 0; i < m_StaticTargetAnimator.Length; i++)
                {
                    m_StaticTargetAnimator[i].SetBool("TargetRise", true);
                    m_StaticTargetAnimator[i].SetBool("TargetFall", false);
                }
            }
            else
            {
                StartCoroutine(ActivateStaticTargetsCoroutine(l_Activate));
            }
        }
        else
        {
            for (int i = 0; i < m_StaticTargetAnimator.Length; i++)
            {
                m_StaticTargetAnimator[i].SetBool("TargetRise", false);
                m_StaticTargetAnimator[i].SetBool("TargetFall", true);
            }
        }
    }

    private IEnumerator ActivateStaticTargetsCoroutine(bool l_Activate)
    {
        for (int i = 0; i < m_StaticTargetAnimator.Length; i++)
        {
            m_StaticTargetAnimator[i].SetBool("TargetRise", l_Activate);
            m_StaticTargetAnimator[i].SetBool("TargetFall", !l_Activate);

            yield return new WaitForSeconds(3.0f);
        }
    }

    private void ActivateMovingTargets(bool l_Activate)
    {
        for (int j = 0; j < m_MovingTarget.Length; j++)
        {
            if (l_Activate)
            {
                m_MovingTarget[j].StartMovingTargets();
            }
            else
            {
                m_MovingTarget[j].StopMovingTargets();
            }
        }
    }
}
