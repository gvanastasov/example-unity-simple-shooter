using UnityEngine;

/// <summary>
/// The PlayerInventory is responsible for managing the player's inventory.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
#region Static Properties
    /// <summary>
    /// The singleton instance of the PlayerInventory.
    /// </summary>
    public static PlayerInventory Instance 
    {
        get;
        private set;
    }
#endregion

#region Serializable Fields
    /// <summary>
    /// The number of bullets the player has at the start of each level.
    /// </summary>
    [Tooltip("The number of bullets the player has at the start of each level.")]
    public int Bullets = 100;
#endregion

#region Unity Callbacks
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

        UIManager.Instance.Ammunition_UpdateText(this.Bullets);
    }
#endregion

#region Actions
    /// <summary>
    /// Remove bullets from the player's inventory.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public int GetBullets(int count)
    {
        var result = count > Bullets ? Bullets : count;
        this.Bullets -= result;
        UIManager.Instance.Ammunition_UpdateText(this.Bullets);
        return result;
    }
#endregion
}
