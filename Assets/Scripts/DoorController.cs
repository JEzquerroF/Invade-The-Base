using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private enum DoorType
    {
        Normal,
        Key,
        Score
    }

    [SerializeField] private DoorType m_DoorType;
    [SerializeField] private ShootingRange m_ShootingRange;
    [SerializeField] private Animator m_DoorAnimator;
    [SerializeField] private AudioClip m_DoorSound;
    [SerializeField] private AudioClip m_DoorErrorSound;
    [SerializeField] private GameObject m_DoorTextBox;
    [SerializeField] private TMP_Text m_DoorText;
    [SerializeField] private GameObject m_DoorFence;
    [SerializeField] private int m_DoorKeys;
    [SerializeField] private int m_ScoreToBeat;
    private int m_DoorKeysCount;
    private string m_TextToDisplay;

    private void OnEnable()
    {
        KeyItem.OnKeyPicked += KeyCounter;
    }

    private void OnDisable()
    {
        KeyItem.OnKeyPicked -= KeyCounter;
    }

    private void Start()
    {
        m_DoorKeysCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (m_DoorType == DoorType.Key && m_DoorKeys == m_DoorKeysCount)
            {
                m_DoorAnimator.SetBool("DoorOpening", true);
                m_DoorAnimator.SetBool("DoorClosing", false);
            }
            else if (m_DoorType == DoorType.Key && m_DoorKeys > m_DoorKeysCount)
            {
                m_TextToDisplay = "You need to find the " + m_DoorKeys + " keys to open this door.";
                StartCoroutine(ShowTextCoroutine());
            }
            else if (m_DoorType == DoorType.Score && m_ShootingRange.m_BestScore >= m_ScoreToBeat)
            {
                m_DoorAnimator.SetTrigger("FenceAnimation");
            }
            else if (m_DoorType == DoorType.Score && m_ShootingRange.m_BestScore < m_ScoreToBeat)
            {
                m_TextToDisplay = "You need to have at least " + m_ScoreToBeat + " points in the shooting range.";
                StartCoroutine(ShowTextCoroutine());
            }
            else if (m_DoorType == DoorType.Normal)
            {
                m_DoorAnimator.SetBool("DoorOpening", true);
                m_DoorAnimator.SetBool("DoorClosing", false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && m_DoorAnimator != null)
        {
            m_DoorAnimator.SetBool("DoorClosing", true);
            m_DoorAnimator.SetBool("DoorOpening", false);
        }
    }

    private IEnumerator ShowTextCoroutine()
    {
        m_DoorTextBox.SetActive(true);
        m_DoorText.text = m_TextToDisplay;
        SoundsManager.instance.PlaySoundClip(m_DoorErrorSound, transform, 0.2f);
        yield return new WaitForSeconds(5.0f);
        m_DoorTextBox.SetActive(false);
    }
    public void PlayDoorSound()
    {
        SoundsManager.instance.PlaySoundClip(m_DoorSound, transform, 0.2f);
    }

    private void KeyCounter()
    {
        m_DoorKeysCount++;
    }

}
