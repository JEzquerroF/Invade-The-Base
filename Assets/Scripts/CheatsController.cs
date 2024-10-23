using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatsController : MonoBehaviour
{
#if UNITY_EDITOR    

    [SerializeField] private TMP_Text m_CheatsText; 
    private Life_Player_Controller m_PlayerLife;
    private bool m_GodMode;
    private bool m_Cheats; 
    void Start()
    {
        m_PlayerLife = GetComponent<Life_Player_Controller>();  
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            m_CheatsText.gameObject.SetActive(true);
            m_Cheats = true;    
        }

        if (m_Cheats)
        {
            if (Input.GetKey(KeyCode.LeftControl))
                Time.timeScale = 3.0f;
            else Time.timeScale = 1.0f;

            if(Input.GetKeyDown(KeyCode.H))
               m_PlayerLife.UpdateHealth(100);

            if(Input.GetKeyDown(KeyCode.J))
                GameManager.instance.NewGame(); 


            if(Input.GetKeyDown(KeyCode.G))
                m_GodMode = true;

            if(m_GodMode)
               m_PlayerLife.UpdateHealth(100);

            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                m_GodMode = false;
                m_CheatsText.gameObject.SetActive(false);
                m_Cheats = false;
            }
        }
    }

#endif
}
