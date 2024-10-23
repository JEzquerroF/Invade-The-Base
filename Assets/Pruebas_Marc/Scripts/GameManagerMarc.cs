using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRestartGameElement
{
    void RestartGame();
}

public class GameManagerMarc : MonoBehaviour
{
    public static GameManagerMarc m_GameManager;

    PlayerController m_PlayerController;

    List<IRestartGameElement> m_RestartGameElements = new List<IRestartGameElement>();

    [SerializeField] private GameObject m_DeadUI;

    private void Awake()
    {
        if (m_GameManager == null)
        {
            m_GameManager = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
            GameObject.Destroy(m_GameManager);
    }

    /*private void Start()
    {
        if (GameManager.GetGameManager().GetPlayer() != null)
        {
            Destroy(gameObject);
        }
    }*/

    void InitLevel(PlayerController Player)
    {
        m_PlayerController.enabled = false;
        transform.position = Player.transform.position;
        transform.rotation = Player.transform.rotation;
        m_PlayerController.enabled = true;
    }

    public PlayerController GetPlayer()
    {
        return m_PlayerController;
    }

    public PlayerController SetPlayer(PlayerController Player)
    {
        return m_PlayerController = Player;
    }

    public GameManagerMarc GetGameManager()
    {
        return m_GameManager;
    }

    public void OnMainMenu()
    {
        GameObject.Destroy(gameObject);
        GameObject.Destroy(m_PlayerController.gameObject);
    }

    public void AddRestartGameElement(IRestartGameElement RestartGameElement)
    {
        m_RestartGameElements.Add(RestartGameElement);
    }

    #region LANDA FUNCTION
    delegate void OnDeadUIFinishedFn();

    public void RestartGame()
    {
        StartCoroutine(DeadUICorutine(() =>
        {
            foreach (IRestartGameElement l_RestartGameElement in m_RestartGameElements)
                l_RestartGameElement.RestartGame();
            m_DeadUI.SetActive(false);
        }));
    }

    private IEnumerator DeadUICorutine(OnDeadUIFinishedFn OnDeadUIFinished)
    {
        m_DeadUI.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        OnDeadUIFinished();
    }
    #endregion LANDA FUNCTION


    /*public void RestartGame()
    {
        StartCoroutine(DeadUICorutine(RestartGameAfterDeadUI));
    }

    void RestartGameAfterDeadUI()
    {
        foreach (IRestartGameElement l_RestartGameElement in m_RestartGameElements)
            l_RestartGameElement.RestartGame();
        m_DeadUI.SetActive(false);
    }

    private IEnumerator DeadUICorutine(OnDeadUIFinishedFn OnDeadUIFinished)
    {
        m_DeadUI.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        OnDeadUIFinished();
    }*/
}
