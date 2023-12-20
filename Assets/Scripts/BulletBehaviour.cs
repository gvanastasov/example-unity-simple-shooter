using UnityEngine;

/// <summary>
/// Bullet behaviour.
/// </summary>
public class BulletBehaviour : MonoBehaviour
{
#region Serializable Fields
    /// <summary>
    /// Speed of the bullet.
    /// </summary>
    public float Speed = 100f;

    /// <summary>
    /// Lifespan of the bullet.
    /// </summary>
    public int Lifespan = 1;
#endregion

#region Unity Callbacks
    /// <summary>
    /// Destroy the bullet after a certain amount of time.
    /// </summary>
    void Start()
    {
        Destroy(this.gameObject, this.Lifespan);
    }

    /// <summary>
    /// Move the bullet forward.
    /// </summary>
    void Update()
    {
        this.transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }
#endregion
}
