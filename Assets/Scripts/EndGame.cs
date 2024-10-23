using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private AnimationClip m_FadeAnimationClip;
    [SerializeField] private Animator m_FadeAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && SceneManager.GetActiveScene().name == "Level01")
            StartCoroutine(GoToNextScene());
        if (other.CompareTag("Player") && SceneManager.GetActiveScene().name == "Level02")
            GameManager.instance.ReStartGame(true);
    }

    private IEnumerator GoToNextScene()
    {
        m_FadeAnimator.SetTrigger("FadeStarting");
        yield return new WaitForSeconds(m_FadeAnimationClip.length);
        SceneManager.LoadSceneAsync("Level02");
    }
}
