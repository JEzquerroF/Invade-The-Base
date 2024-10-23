using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract bool CanPick();
    public bool m_Destroy = false;

    private void Start()
    {
        m_Destroy = false;
    }
    public virtual void Pick()
    {
        gameObject.SetActive(false);
    }
} 
