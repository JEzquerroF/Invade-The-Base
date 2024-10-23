using System;
using UnityEngine;

public class ShieldItem : Item
{
    [SerializeField] AudioClip m_ShieldItemSound;
    [SerializeField] private float m_ShieldPoints;

    public static Action<float> OnShieldPicked;

    private void Update()
    {
        if (GameManager.instance.m_Restart)
            Destroy(gameObject);
    }

    public override bool CanPick()
    {
        if (GameManager.instance.CanPickShield())
            return true;
        else
            return false;
    }

    public override void Pick()
    {
        OnShieldPicked?.Invoke(m_ShieldPoints);
        SoundsManager.instance.PlaySoundClip(m_ShieldItemSound, transform, 0.2f);

        base.Pick();
    }
}
