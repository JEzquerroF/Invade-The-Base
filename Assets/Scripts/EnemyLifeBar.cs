using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLifeBar : MonoBehaviour
{
    [SerializeField] private Enemies m_Enemy; 
    [SerializeField] private Slider m_HealthScrollBar;
    [SerializeField] private Slider m_LerpHealthBar;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private GameObject m_Life;

    [SerializeField] float m_Offset; 

    private Camera m_Camera;
    private float m_Health; 

    void Start()
    {
        m_Enemy.GetComponent<Enemies>(); 
        m_Camera = Camera.main; 
    }

    void Update()
    {
        m_Health = m_Enemy.GetHealth(); 
        m_HealthScrollBar.value = m_Health;

        if(m_HealthScrollBar.value != m_LerpHealthBar.value)
            m_LerpHealthBar.value = Mathf.Lerp(m_LerpHealthBar.value, m_HealthScrollBar.value, lerpSpeed * Time.deltaTime); 

       Vector3 l_ScreenSpace = m_Camera.WorldToScreenPoint(m_Enemy.transform.position +  Vector3.up * m_Offset);
       transform.position = l_ScreenSpace; 

        if(m_Enemy.SeePlayerConeVision() && m_Enemy.SeePlayerHit() && l_ScreenSpace.z > 0)
            m_Life.SetActive(true);
        else
            m_Life.SetActive(false);
    }
}
