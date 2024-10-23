using System;
using UnityEngine;

public class AmmoItem : Item
{
    [SerializeField] private AudioClip m_AmmoItemSound;
    [SerializeField] private int m_BulletsToRestore;

    public static Action<int> OnAmmoPicked;
    private void Update()
    {
        if (GameManager.instance.m_Restart)
            Destroy(gameObject);
    }

    public override bool CanPick()
    {
        if (GameManager.instance.CanPickAmmo())
            return true;
        else
            return false;
    }

    public override void Pick()
    {
        OnAmmoPicked?.Invoke(m_BulletsToRestore);
        SoundsManager.instance.PlaySoundClip(m_AmmoItemSound, transform, 0.5f);
        base.Pick();
    }
}
