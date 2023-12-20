using UnityEngine;

/// <summary>
/// The LevelManager is responsible for managing the level state.
/// </summary>
public class LevelManager : MonoBehaviour
{
#region Static Properties
    /// <summary>
    /// The singleton instance of the LevelManager.
    /// </summary>
    public static LevelManager Instance 
    {
        get;
        private set;
    }
#endregion

#region Serializable Fields
    /// <summary>
    /// Reference to the enemy prefab.
    /// </summary>
    [Tooltip("Reference to the enemy prefab.")]
    public GameObject Enemy;

    /// <summary>
    /// Reference to a mesh used for spawing enemies at random positions.
    /// </summary>
    [Tooltip("Reference to a mesh used for spawing enemies at random positions.")]
    public MeshFilter SpawnMeshFilter;

    /// <summary>
    /// The current level.
    /// </summary>
    [Tooltip("The current level.")]
    public int currentLevel = 1;

    /// <summary>
    /// The maximum level.
    /// </summary>
    [Tooltip("The maximum level.")]
    public int maxLevel = 5;
#endregion

#region Public Properties
    /// <summary>
    /// A flag indicating whether the game is currently running.
    /// </summary>
    public bool HasNextLevel
    {
        get
        {
            return this.currentLevel + 1 <= maxLevel;
        }
    }

    /// <summary>
    /// The number of enemies to spawn in current level.
    /// </summary>
    public int EnemyLevelCount
    {
        get
        {
            return currentLevel * 2;
        }
    }
#endregion

#region Private Fields
    /// <summary>
    /// The current number of enemies.
    /// </summary>
    private int enemyCurrentCount;
#endregion

#region Unity Callbacks
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        SpawnMeshFilter.gameObject.SetActive(false);
    }
#endregion

#region Actions
    /// <summary>
    /// Increase the current level.
    /// </summary>
    public void LevelIncrease()
    {
        this.currentLevel += 1;
    }
    
    /// <summary>
    /// Spawns enemies at random positions over a geometry defined by a mesh.
    /// </summary>
    public void SpawnEnemies()
    {
        if (this.Enemy == null)
        {
            return;
        }

        var renderer = Enemy.GetComponent<Renderer>();
        for (int i = 0; i < this.EnemyLevelCount; i++)
        {
            var pos = MeshHelpers.GetRandomPosition(SpawnMeshFilter.mesh);
            GameObject.Instantiate(Enemy, new Vector3(pos.x, pos.y + renderer.bounds.extents.y, pos.z), Quaternion.identity);
        }
        
        this.enemyCurrentCount = EnemyLevelCount;

        UIManager.Instance.Enemies_UpdateBar(
            current: this.enemyCurrentCount,
            total: this.EnemyLevelCount 
        );
    }
#endregion

#region Callbacks
    /// <summary>
    /// Handles the event when an enemy is destroyed.
    /// </summary>
    /// <remarks>
    /// Updates the score and checks whether the level is won, when all enemies are destroyed.
    /// </remarks>
    public void EnemyDestroyed()
    {
        this.enemyCurrentCount = Mathf.Max(0, this.enemyCurrentCount - 1);

        UIManager.Instance.Enemies_UpdateBar(
            current: this.enemyCurrentCount,
            total: this.EnemyLevelCount
        );

        GameManager.Instance.IncreaseScore(this.currentLevel);
        if (this.enemyCurrentCount == 0)
        {
            GameManager.Instance.Win();
        }
    }
#endregion
}
