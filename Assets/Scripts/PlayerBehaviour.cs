using System;
using UnityEngine;

/// <summary>
/// PlayerBehaviour is the main script for the player. It handles movement, rotation, and shooting.
/// </summary>
/// <remarks>
/// Player is controlled using the mouse and keyboard.
/// Player can be damanged.
/// Player can die.
/// </remarks>
public class PlayerBehaviour : MonoBehaviour, IDamageable
{
#region Constants
    const string xAxis = "Mouse X";
	const string yAxis = "Mouse Y";
#endregion

#region Serializable Fields
    /// <summary>
    /// The speed at which the player moves forward.
    /// </summary>
    [Tooltip("The speed at which the player moves forward.")]
    public float SpeedForward = 10f;

    /// <summary>
    /// The speed at which the player moves backward.
    /// </summary>
    [Tooltip("The speed at which the player moves backward.")]
    public float SpeedBackward = 5f;

    /// <summary>
    /// The speed at which the player moves sideways.
    /// </summary>
    [Tooltip("The speed at which the player moves sideways.")]
    public float SpeedSide = 8f;

    /// <summary>
    /// The sensitivity of the mouse.
    /// </summary>
    [Range(0.1f, 9f)]
    [SerializeField] 
    [Tooltip("The sensitivity of the mouse.")]
    private float sensitivity = 2f;
	
    /// <summary>
    /// Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.
    /// </summary>
    [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
	[Range(0f, 90f)]
    [SerializeField] 
    private float yRotationLimit = 88f;

    /// <summary>
    /// The player's health.
    /// </summary>
    /// <remarks>
    /// Player dies when health reaches 0.
    /// </remarks>
    [SerializeField]
    [Tooltip("The player's health.")]
    private int health = 100;
#endregion

#region Public Properties
    /// <summary>
    /// The player's health.
    /// </summary>
    public int Health
    {
        get
        {
            return this.health;
        }
        set
        {
            this.health = value;
        }
    }

    /// <summary>
    /// The sensitivity of the mouse.
    /// </summary>
    public float Sensitivity 
    {
		get 
        { 
            return sensitivity; 
        }
		set 
        { 
            sensitivity = value; 
        }
	}
#endregion

#region Private Fields
    /// <summary>
    /// The player's maximum health - cached at the start of the game.
    /// </summary>
    private int maxHealth;
    
    /// <summary>
    /// The player's rotation.
    /// </summary>
    private Vector2 rotation = Vector2.zero;

    /// <summary>
    /// The player's gun.
    /// </summary>
    /// <remarks>
    /// Player can only have one gun.
    /// </remarks>
    private GunBehaviour gun = null;

    /// <summary>
    /// The player's camera.
    /// </summary>
    private new Transform camera = null;
#endregion

#region Unity Callbacks
    void Awake()
    {
        this.gun = GetComponentInChildren<GunBehaviour>();
        this.camera = GetComponentInChildren<Camera>()?.transform;
        this.maxHealth = this.health;

        UIManager.Instance.Health_UpdateBar(
            current: this.health,
            total: this.maxHealth
        );
    }

    void Update()
    {
        if (this.camera) 
        {
            Move();
            Rotate();
        }

        // Left
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.Pause();
        }
    }
#endregion

#region Actions
    /// <summary>
    /// Reloads gun.
    /// </summary>
    private void Reload()
    {
        if (gun != null)
        {
            gun.Reload();
        }
    }

    /// <summary>
    /// Shoots gun.
    /// </summary>
    private void Shoot()
    {
        if (gun != null)
        {
            gun.Shoot();
        }
    }

    /// <summary>
    /// Rotates the player.
    /// </summary>
    /// <remarks>
    /// Rotation is based on mouse movement.
    /// Rotation is applied to the camera as child object of the player.
    /// Rotation is limited to prevent flipping.
    /// </remarks>
    private void Rotate()
    {
        rotation.x += Input.GetAxis(xAxis) * sensitivity;
		rotation.y += Input.GetAxis(yAxis) * sensitivity;
		rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
		var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
		var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        this.camera.transform.localRotation = xQuat * yQuat;
    }

    /// <summary>
    /// Moves the player.
    /// </summary>
    /// <remarks>
    /// Movement is based on keyboard input.
    /// Movement is limited to only X and Z axis.
    /// </remarks>
    private void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(
                new Vector3(this.camera.forward.x, 0, this.camera.forward.z) * SpeedForward * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(
                new Vector3(-this.camera.forward.x, 0, -this.camera.forward.z) * SpeedBackward * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(
                new Vector3(-this.camera.right.x, 0, -this.camera.right.z) * SpeedSide * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(
                new Vector3(this.camera.right.x, 0, this.camera.right.z) * SpeedSide * Time.deltaTime, Space.World);
        }
    }

    /// <summary>
    /// Damages the player.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="damageOrigin"></param>
    public void Damage(int damage, Vector3? damageOrigin = null)
    {
        this.Health = Math.Max(0, this.Health - damage);
        UIManager.Instance.Health_UpdateBar(
            current: this.health,
            total: this.maxHealth
        );
        if (this.Health == 0)
        {
            this.Die();
        }
    }

    /// <summary>
    /// Kills the player.
    /// </summary>
    /// <remarks>
    /// Player dies when health reaches 0.
    /// Player loses game when player dies.
    /// </remarks>
    public void Die()
    {
        GameManager.Instance.GameOver();
    }
#endregion
}
