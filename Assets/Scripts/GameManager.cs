using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance 
    {
        get;
        private set;
    }

    private PlayerBehaviour player;

    private bool ingame = false;

    private int currentScore = 0;

    private Vector3 playerStartPos;
    private Quaternion playerStartRot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            this.player = FindFirstObjectByType<PlayerBehaviour>();
            this.Main();
        }
    }

    public void Main()
    {
        this.player.enabled = false;
        UIManager.Instance.Show(UIManager.Instance.MainMenuGUI);
        Time.timeScale = 0;

        if (ingame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void New()
    {
        SavePlayerSpawnPoint();

        this.player.enabled = true;
        this.ingame = true;
        this.currentScore = 0;
        LevelManager.Instance.SpawnEnemies();
        UIManager.Instance.Show(UIManager.Instance.IngameGUI);
        UIManager.Instance.Score_UpdateText(0);
        UIManager.Instance.Level_UpdateText(LevelManager.Instance.currentLevel);
        Time.timeScale = 1;
    }

    public void Next()
    {
        ResetPlayerPosition();

        this.player.enabled = true;
        LevelManager.Instance.IncreaseLevel();
        LevelManager.Instance.SpawnEnemies();
        UIManager.Instance.Show(UIManager.Instance.IngameGUI);
        UIManager.Instance.Level_UpdateText(LevelManager.Instance.currentLevel);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        ResetPlayerPosition();

        this.currentScore = 0;
        LevelManager.Instance.SpawnEnemies();
        UIManager.Instance.Show(UIManager.Instance.IngameGUI);
        UIManager.Instance.Score_UpdateText(0);
        Time.timeScale = 1;
    }

    public void Pause()
    {
        this.player.enabled = false;
        UIManager.Instance.Show(UIManager.Instance.PauseGUI);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        this.player.enabled = true;
        UIManager.Instance.Show(UIManager.Instance.IngameGUI);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        this.player.enabled = false;
        UIManager.Instance.Show(UIManager.Instance.GameOverGUI);
        Time.timeScale = 0;
    }

    public void Win()
    {
        this.player.enabled = false;
        if (LevelManager.Instance.HasNextLevel)
        {
            this.Next();
        }
        else
        {
            UIManager.Instance.Show(UIManager.Instance.GameWonGUI);
            Time.timeScale = 0;
        }
    }

    public void IncreaseScore(int value)
    {
        this.currentScore += value;
        UIManager.Instance.Score_UpdateText(this.currentScore);
    }

    private void SavePlayerSpawnPoint()
    {
        this.playerStartPos = this.player.transform.position;
        this.playerStartRot = this.player.transform.rotation;
    }

    private void ResetPlayerPosition()
    {
        this.player.transform.position = this.playerStartPos;
        this.player.transform.rotation = this.playerStartRot;
    }
}
