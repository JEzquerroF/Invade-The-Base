using System.Collections.Generic;
using UnityEngine;

public class CPoolElemt
{
    public List<GameObject> m_Pool; 
    int m_CurrentElementID = 0; 
    public CPoolElemt(int ElementsCount, GameObject PrefabElement)
    {
        m_Pool = new List<GameObject>();

        for (int i = 0; i < ElementsCount; i++)
        {
            GameObject l_GameObject = GameObject.Instantiate(PrefabElement);
            l_GameObject.SetActive(false);
            m_Pool.Add(l_GameObject);
        }   
    }

    public GameObject GetNextElement()
    {
        GameObject l_GameObject = m_Pool[m_CurrentElementID];
        m_CurrentElementID++;
        if (m_CurrentElementID >= m_Pool.Count)
            m_CurrentElementID = 0;

        return l_GameObject;    
    }

    public void AddBullet(GameObject l_Bullet)
    {
        GameObject l_GameObject = GameObject.Instantiate(l_Bullet);
        l_GameObject.SetActive(false);
        m_Pool.Add(l_GameObject);
    }
}
