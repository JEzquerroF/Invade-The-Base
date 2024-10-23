using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class Weapon_Controller : MonoBehaviour, IRestartGame
{
    [SerializeField] private Camera m_Camera;
    [SerializeField] private TMP_Text m_textAmmo;
    [SerializeField] private GameObject m_Rifle;
    [SerializeField] private GameObject m_Bullet;
    [SerializeField] private GameObject m_BulletDecal;
    [SerializeField] private float m_targetShoot;
    [SerializeField] private int m_maxAmmoMagazine;
    [SerializeField] private int m_totalAmmo;
    [SerializeField] private CanvasGroup m_crosshairSniper;
    [SerializeField] float m_CameraAim;
    [SerializeField] float m_AimLerpSpeed;

    [Header("Particles")]
    [SerializeField] ParticleSystem m_MuzzleParticle;

    [Header("Sounds")]
    [SerializeField] private AudioClip m_ShootSound;
    [SerializeField] private AudioClip m_ReloadSound;
    [SerializeField] private AudioClip m_NoBulletsSound;

    [SerializeField] float m_BulletSpeed;
    [SerializeField] float m_BulletDamage;

    private Animator m_Animator;
    private int m_ammoCount;
    private int m_MaxTotalAmmo;
    private bool m_isReloading = false;
    private bool m_canShoot = true;
    private bool m_firstShoot = true;
    private bool m_Aiming = false;
    private bool m_SetToAim = false;
    private float m_timeShoot = 0;

    public bool m_CanCollectAmmo = false;

    private void OnEnable()
    {
        AmmoItem.OnAmmoPicked += UpdateAmmo;
    }

    private void OnDisable()
    {
        AmmoItem.OnAmmoPicked -= UpdateAmmo;
    }

    void Start()
    {
        m_ammoCount = m_maxAmmoMagazine;
        m_Animator = GetComponent<Animator>();
        m_crosshairSniper.alpha = 0.0f;
        m_MaxTotalAmmo = m_totalAmmo;
        GameManager.instance.AddRestartGame(this);  
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && m_ammoCount > 0 && m_isReloading == false)
            Shoot();

        if (Input.GetMouseButtonUp(0))
            m_firstShoot = true;

        if (Input.GetMouseButtonDown(1) && m_Aiming == false)
        {
            m_Animator.SetBool("Aim", true);
        }

        if (Input.GetMouseButton(1) == false)
        {
            m_SetToAim = false;
            m_Rifle.SetActive(true);

            if (m_Aiming)
            {
                m_Camera.fieldOfView = 60;
                m_Aiming = false;
            }
            else
            {
                m_Animator.SetBool("Aim", false);
            }

            m_crosshairSniper.alpha = Mathf.Lerp(m_crosshairSniper.alpha, 0.0f, m_AimLerpSpeed * Time.deltaTime);
            m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, 60, m_AimLerpSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.R) && m_ammoCount < m_maxAmmoMagazine && m_isReloading == false && m_totalAmmo > 0)
        {
            m_isReloading = true;
            m_canShoot = false;
            m_Animator.SetBool("Reloading", true);
            SoundsManager.instance.PlaySoundClip(m_ReloadSound, transform, 0.2f);
        }

        if (Input.GetMouseButtonUp(0) || m_ammoCount <= 0)
            m_Animator.SetBool("Shooting", false);


        if (Input.GetMouseButtonDown(0) && m_ammoCount <= 0)
            SoundsManager.instance.PlaySoundClip(m_NoBulletsSound, transform, 0.2f);

        if (m_SetToAim && Input.GetMouseButton(1))
        {
            m_crosshairSniper.alpha = Mathf.Lerp(m_crosshairSniper.alpha, 1.0f, m_AimLerpSpeed * Time.deltaTime);
            m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_CameraAim, m_AimLerpSpeed * Time.deltaTime);

            if (m_crosshairSniper.alpha == 1.0f && m_Camera.fieldOfView == m_CameraAim)
                m_SetToAim = false;
        }

        if (m_totalAmmo < m_MaxTotalAmmo)
            m_CanCollectAmmo = true;
        else
            m_CanCollectAmmo = false;

        UpdateText();
    }

    private void Shoot()
    {
        if (m_canShoot || m_firstShoot)
        {
            m_Animator.SetBool("Shooting", true);
            SoundsManager.instance.PlaySoundClip(m_ShootSound, transform, 0.2f);
            m_MuzzleParticle.Play();

            Ray shootRay = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            GameObject l_bullet = GameManager.instance.GetBullet();
            GameObject l_bulletDecal = GameManager.instance.GetBulletDecal();

            l_bullet.SetActive(true);
            l_bullet.transform.position = shootRay.origin;
            l_bullet.transform.rotation = Quaternion.LookRotation(shootRay.direction);

            BulletController l_bulletController = l_bullet.GetComponent<BulletController>();
            l_bulletController.Shoot(shootRay.origin, shootRay.direction, l_bulletDecal, m_BulletSpeed, m_BulletDamage);

            ParticleSystem parentParticleSystem = l_bulletDecal.GetComponentInChildren<ParticleSystem>();
            parentParticleSystem.Play();

            m_ammoCount--;

            m_timeShoot = 0.0f;
            m_canShoot = false;
            m_firstShoot = false;
        }

        m_timeShoot += Time.deltaTime;

        if (m_timeShoot >= m_targetShoot)
            m_canShoot = true;
    }

    private void UpdateText()
    {
        m_textAmmo.text = m_ammoCount.ToString("D2") + "  " + m_totalAmmo.ToString("D3");
    }

    private void Reloading()
    {
        m_Animator.SetBool("Reloading", false);
        int l_bulletsCapacity;
        int l_bulletsReloaded;

        l_bulletsCapacity = m_maxAmmoMagazine - m_ammoCount;

        if (l_bulletsCapacity >= m_totalAmmo)
            l_bulletsReloaded = m_totalAmmo;
        else
            l_bulletsReloaded = l_bulletsCapacity;

        m_totalAmmo -= l_bulletsReloaded;
        m_ammoCount += l_bulletsReloaded;

        m_canShoot = true;
        m_isReloading = false;
    }

    private void Aim()
    {
        m_Aiming = true;
        m_Rifle.SetActive(false);
        m_Animator.SetBool("Aim", false);
    }

    private void SetToAim()
    {
        m_SetToAim = true;
    }

    private void UpdateAmmo(int value)
    {
        int l_BulletsUsed;

        l_BulletsUsed = m_MaxTotalAmmo - m_totalAmmo;

        if (l_BulletsUsed < 30)
        {
            for (int i = 0; i < l_BulletsUsed; i++)
            {
                m_totalAmmo++;
            }
        }
        else
        {
            m_totalAmmo += value;
        }
    }

    public void RestartGame()
    {
        m_ammoCount = m_maxAmmoMagazine;
        m_totalAmmo = m_MaxTotalAmmo;
    }
}