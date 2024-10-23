using System;
using UnityEngine;

public class LifeItem : Item
{
    [SerializeField] private AudioClip m_HealthItemSound;
    [SerializeField] private float m_LifePoints;

    public static Action<float> OnHealthPicked;

    private void Update()
    {
        if (GameManager.instance.m_Restart)
            Destroy(gameObject);
    }

    public override bool CanPick()
    {
        if (GameManager.instance.CanPickHealth())
            return true;
        else
            return false;
    }

    public override void Pick()
    {
        OnHealthPicked?.Invoke(m_LifePoints);
        SoundsManager.instance.PlaySoundClip(m_HealthItemSound, transform, 0.2f);
        base.Pick();
    }
}
