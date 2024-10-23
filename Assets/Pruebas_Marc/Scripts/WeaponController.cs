using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Camera m_MainCamera;

    [SerializeField] private int m_MaxAmmo;
    private int m_CurrentAmmo;
    [SerializeField] private GameObject m_DecalPrefab;

    private float m_MaxBulletDistance;
    private float m_Timer;
    private float m_MaxTime = 0.4f;

    [SerializeField] private TMP_Text m_AmmoText;

    private LayerMask m_ShootLayerMask;

    void Start()
    {
        m_CurrentAmmo = m_MaxAmmo;
    }

    void Update()
    {
        m_AmmoText.text = "Ammo: " + m_CurrentAmmo;

        if (Input.GetMouseButton(0))
            ShootGun();
        if (Input.GetKeyDown(KeyCode.R))
            ReloadGun();
    }

    private void ShootGun()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer >= m_MaxTime && m_CurrentAmmo > 0)
        {
            Ray l_Ray = m_MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit l_Hit;

            if (Physics.Raycast(l_Ray, out l_Hit, m_MaxBulletDistance, m_ShootLayerMask.value))
            {
                m_CurrentAmmo--;

                Instantiate(m_DecalPrefab, new Vector3(l_Hit.point.x, l_Hit.point.y, l_Hit.point.z - 0.01f), Quaternion.LookRotation(-l_Hit.normal));
            }

            m_Timer = 0;
        }
    }

    private void ReloadGun()
    {
        m_CurrentAmmo = m_MaxAmmo;
    }

    private void CreateShootParticles(Vector3 Position, Vector3 Normal)
    {
        GameObject l_HitParticles = GameObject.Instantiate(m_DecalPrefab, null);
        l_HitParticles.transform.position = Position;
    }
}
