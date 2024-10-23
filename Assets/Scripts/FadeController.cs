using UnityEngine;

public class FadeController : MonoBehaviour
{
    private Animator m_Animator;

    void Start()
    {
        m_Animator = GetComponent<Animator>();  
    }

    public void StartFade()
    {
        m_Animator.SetTrigger("FadeStarting");
    }

    public void RestartPositions()
    {
        GameManager.instance.RestartPosition(); 
    }

    public void ActivePlayer()
    {
        GameManager.instance.ActivePlayer();
    }
}
