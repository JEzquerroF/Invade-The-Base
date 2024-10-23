using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IRestartGame
{
    void RestartGame();
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject m_DeathUI;
    [SerializeField] private GameObject m_GameUI;
    [SerializeField] private CharacterController m_CharacterController; 
    [SerializeField] private Player m_Player; 
    [SerializeField] private Player_Controller m_PlayerController;
    [SerializeField] private Weapon_Controller m_WeaponController;
    [SerializeField] private Life_Player_Controller m_LifePlayerController;
    [SerializeField] private GameObject m_Bullet;
    [SerializeField] private GameObject m_BulletDecal;
    [SerializeField] private TMP_Text m_TextCanvas;
    [SerializeField] private TMP_Text m_NewGameText;
    [SerializeField] private FadeController m_FadeController;
    [SerializeField] private int m_MaxBulletEnemyOnScene;

    public List<IRestartGame> m_RestartGames = new List<IRestartGame>();
    private List<Enemies> m_ListEnemies = new List<Enemies>();  

    private CPoolElemt m_CPoolBullet;
    private CPoolElemt m_CPoolBulletDecal;
    public Vector3 m_direction;
    private float m_SpeedPlayer;
    public bool m_Restart;

    private bool m_GameHasEnded = false;
    private List<Item> m_ItemListDestroy = new List<Item>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        m_CPoolBullet = new CPoolElemt(m_MaxBulletEnemyOnScene, m_Bullet);
        m_CPoolBulletDecal = new CPoolElemt(m_MaxBulletEnemyOnScene, m_BulletDecal);
    }
  
    public void SetPlayer(Player l_player)
    {
        m_Player = l_player;    
    }

    public Player GetPlayer()
    {
        return m_Player;
    }

    public void AddRestartGame(IRestartGame l_restart)
    {
        m_RestartGames.Add(l_restart);
    }

    public void AddEnemies(Enemies l_Enemies)
    {
        m_ListEnemies.Add(l_Enemies);
    }


    public void ReStartGame(bool l_EndGame)
    {
        if (l_EndGame)
        {
            m_Player.StopAnimation();   
            m_TextCanvas.text = "You Win";
            m_NewGameText.text = "Menu";
            m_GameHasEnded = true;
        }

        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;

        m_CharacterController.enabled = false; 
        m_PlayerController.enabled = false;
        m_WeaponController.enabled = false;
        m_GameUI.SetActive(false);  
        m_DeathUI.SetActive(true);
    }

    public void NewGame()
    {
        StartCoroutine(NewGameCoroutine());
    }

    public IEnumerator NewGameCoroutine()
    {
        if (m_GameHasEnded == true)
        {
            m_FadeController.StartFade();
            yield return new WaitForSeconds(2.0f);
            m_GameHasEnded = false;
            SceneManager.LoadSceneAsync("MainMenu");
        }
        m_FadeController.StartFade(); 
    }

    public void RestartPosition()
    {
        if (m_GameHasEnded == false)
        {
            m_GameUI.SetActive(true);
            m_DeathUI.SetActive(false);

            m_Restart = true; 

            foreach (Enemies l_enemy in m_ListEnemies) 
            {
                l_enemy.gameObject.SetActive(true);    
            }

            foreach (IRestartGame l_controller in m_RestartGames)
            {
                l_controller.RestartGame();
            }

            Cursor.lockState = CursorLockMode.Locked;
            StartCoroutine(PlayerActive());
        }
    }
    public void AddItemToDestroy(Item l_Item)
    {
        m_ItemListDestroy.Add(l_Item);  
    }

    private IEnumerator PlayerActive()
    {
        yield return new WaitForSeconds(0.1f);
        m_CharacterController.enabled = true;
        m_PlayerController.enabled = true;
        m_PlayerController.SetSpeed(0);
        m_Restart = false;
    }

    public void ActivePlayer()
    {
        m_PlayerController.SetSpeed(m_Player.m_StartSpeed);
        m_WeaponController.enabled = true;
    } 

    public GameObject GetBullet()
    {
        return m_CPoolBullet.GetNextElement();
    }

    public GameObject GetBulletDecal() 
    {
        return m_CPoolBulletDecal.GetNextElement();
    }

    public void BulletDestroy(GameObject l_bullet)
    {
        m_CPoolBulletDecal.m_Pool.Remove(l_bullet);

        if(m_CPoolBulletDecal.m_Pool.Remove(l_bullet))
            m_CPoolBulletDecal.AddBullet(m_BulletDecal);           
    }

    public bool CanPickHealth()
    {
        if (m_LifePlayerController.m_PlayerCanPickHealth)
            return true;
        else
            return false;
    }

    public bool CanPickAmmo()
    {
        if (m_WeaponController.m_CanCollectAmmo)
            return true;
        else 
            return false;
    }

    public bool CanPickShield()
    {
        if (m_LifePlayerController.m_PlayerCanPickShield)
            return true;
        else
            return false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
