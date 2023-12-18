using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance 
    {
        get;
        private set;
    }

    private PlayerBehaviour cmp_playerBehaviour;

    private bool ingame = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        this.cmp_playerBehaviour = FindFirstObjectByType<PlayerBehaviour>();
        this.Main();
    }

    public void Main()
    {
        this.cmp_playerBehaviour.enabled = false;
        UIManager.Instance.Show(UIManager.Instance.MainMenuGUI);
        Time.timeScale = 0;

        if (ingame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void New() 
    {
        this.cmp_playerBehaviour.enabled = true;
        this.ingame = true;
        LevelManager.Instance.SpawnEnemies();
        UIManager.Instance.Show(UIManager.Instance.IngameGUI);
        Time.timeScale = 1;
    }

    public void Pause()
    {
        this.cmp_playerBehaviour.enabled = false;
        UIManager.Instance.Show(UIManager.Instance.PauseGUI);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        this.cmp_playerBehaviour.enabled = true;
        UIManager.Instance.Show(UIManager.Instance.IngameGUI);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        UIManager.Instance.Show(UIManager.Instance.GameOverGUI);
        Time.timeScale = 0;
    }
}
