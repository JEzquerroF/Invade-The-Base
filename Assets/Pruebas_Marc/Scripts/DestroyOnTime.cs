using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    [SerializeField] private float m_DestroyOnTime;

    private void Start()
    {
        StartCoroutine(DestroyOnTimeFn());
    }

    IEnumerator DestroyOnTimeFn()
    {
        yield return new WaitForSeconds(m_DestroyOnTime);
        GameObject.Destroy(gameObject);
    }
}
