using System.Collections;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    [SerializeField] private GameObject m_SavedGameIcon;

    private bool isChecked = false;

    private void OnEnable()
    {
        Player.OnCheckpointEntered += ShowIcon;
    }

    private void OnDisable()
    {
        Player.OnCheckpointEntered -= ShowIcon;
    }

    private void Start()
    {
        m_SavedGameIcon.SetActive(false);
    }

    public Vector3 CheckPointPosition()
    {
        return transform.position;
    }

    public bool IsChecked()
    {
        return isChecked;
    }

    public void SetChecked(bool value)
    {
        isChecked = value;
    }

    private void ShowIcon(bool l_Checkpoint)
    {
        if (l_Checkpoint)
            StartCoroutine(ShowIconCoruotine());
    }

    private IEnumerator ShowIconCoruotine()
    {
        m_SavedGameIcon.SetActive(true);
        yield return new WaitForSeconds(4.0f);
        m_SavedGameIcon.SetActive(false);
    }
}
