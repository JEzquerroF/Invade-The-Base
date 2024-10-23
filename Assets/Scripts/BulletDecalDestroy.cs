using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDecalDestroy : MonoBehaviour
{
    public void Bullet()
    {
        if(this.gameObject != null && GameManager.instance != null)
        { 
            GameManager.instance.BulletDestroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        Bullet();
    }
}
