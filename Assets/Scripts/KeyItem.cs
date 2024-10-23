using UnityEngine;
using System;

public class KeyItem : Item
{
    [SerializeField] private AudioClip m_KeyItemSound;

    public static Action OnKeyPicked;

    public override bool CanPick()
    {
        return true;
    }

    public override void Pick()
    {
        OnKeyPicked?.Invoke();
        SoundsManager.instance.PlaySoundClip(m_KeyItemSound, transform, 0.4f);
        base.Pick();
    }
}

