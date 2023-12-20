using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The GameManager is responsible for managing the game state.
/// </summary>
public class GameManager : MonoBehaviour
{
#region Static Properties
    /// <summary>
    /// The singleton instance of the GameManager.
    /// </summary>
    public static GameManager Instance 
    {
        get;
        private set;
    }
#endregion

#region Private Fields
    /// <summary>
    /// The player object.
    /// </summary>
    private PlayerBehaviour player;

    /// <summary>
    /// A flag indicating whether the game is currently running.
    /// </summary>
    private bool ingame = false;

    /// <summary>
    /// The current score.
    /// </summary>
    private int currentScore = 0;

    /// <summary>
    /// The player's start position. Used to reset the player's position when restarting the game.
    /// </summary>
    private Vector3 playerStartPos;

    /// <summary>
    /// The player's start rotation. Used to reset the player's rotation when restarting the game.
    /// </summary>
    private Quaternion playerStartRot;
#endregion

#region Unity Callbacks
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
#endregion

#region Actions
    /// <summary>
    /// The main menu.
    /// </summary>
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

    /// <summary>
    /// Starts a new game.
    /// </summary>
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

    /// <summary>
    /// Starts the next level.
    /// </summary>
    public void Next()
    {
        ResetPlayerPosition();

        this.player.enabled = true;
        LevelManager.Instance.LevelIncrease();
        LevelManager.Instance.SpawnEnemies();
        UIManager.Instance.Show(UIManager.Instance.IngameGUI);
        UIManager.Instance.Level_UpdateText(LevelManager.Instance.currentLevel);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Restarts the current level.
    /// </summary>
    public void Restart()
    {
        ResetPlayerPosition();

        this.currentScore = 0;
        LevelManager.Instance.SpawnEnemies();
        UIManager.Instance.Show(UIManager.Instance.IngameGUI);
        UIManager.Instance.Score_UpdateText(0);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void Pause()
    {
        this.player.enabled = false;
        UIManager.Instance.Show(UIManager.Instance.PauseGUI);
        Time.timeScale = 0;
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    public void Resume()
    {
        this.player.enabled = true;
        UIManager.Instance.Show(UIManager.Instance.IngameGUI);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Ends the game.
    /// </summary>
    public void GameOver()
    {
        this.player.enabled = false;
        UIManager.Instance.Show(UIManager.Instance.GameOverGUI);
        Time.timeScale = 0;
    }

    /// <summary>
    /// Wins the game.
    /// </summary>
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

    /// <summary>
    /// Increases the score by the given value.
    /// </summary>
    /// <param name="value"></param>
    public void IncreaseScore(int value)
    {
        this.currentScore += value;
        UIManager.Instance.Score_UpdateText(this.currentScore);
    }
#endregion

#region Helpers
    /// <summary>
    /// Saves the player's spawn position and rotation.
    /// </summary>
    private void SavePlayerSpawnPoint()
    {
        this.playerStartPos = this.player.transform.position;
        this.playerStartRot = this.player.transform.rotation;
    }

    /// <summary>
    /// Resets the player's position and rotation to the spawn position and rotation.
    /// </summary>
    private void ResetPlayerPosition()
    {
        this.player.transform.position = this.playerStartPos;
        this.player.transform.rotation = this.playerStartRot;
    }
#endregion
}
