using System;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour, IDamageable
{
    const string xAxis = "Mouse X";
	const string yAxis = "Mouse Y";

    public float SpeedForward = 10f;
    public float SpeedBackward = 5f;
    public float SpeedSide = 8f;

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

    [SerializeField]
    private int health = 100;
    private int maxHealth;
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

    [Range(0.1f, 9f)]
    [SerializeField] 
    private float sensitivity = 2f;
	
    [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
	[Range(0f, 90f)]
    [SerializeField] 
    private float yRotationLimit = 88f;

    private Vector2 rotation = Vector2.zero;

    private GunBehaviour gun = null;

    private new Transform camera = null;

    void Awake()
    {
        this.gun = GetComponentInChildren<GunBehaviour>();
        this.camera = GetComponentInChildren<Camera>()?.transform;
        this.maxHealth = this.health;
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

    private void Reload()
    {
        if (gun != null)
        {
            gun.Reload();
        }
    }

    private void Shoot()
    {
        if (gun != null)
        {
            gun.Shoot();
        }
    }

    private void Rotate()
    {
        rotation.x += Input.GetAxis(xAxis) * sensitivity;
		rotation.y += Input.GetAxis(yAxis) * sensitivity;
		rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
		var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
		var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        this.camera.transform.localRotation = xQuat * yQuat;
    }

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

    public void Damage(int damage, Vector3? damageOrigin = null)
    {
        this.Health = Math.Max(0, this.Health - damage);
        UIManager.Instance.UpdateHealthText(
            currentHealth: this.health,
            maxHealth: this.maxHealth
        );
        if (this.Health == 0)
        {
            this.Die();
        }
    }

    public void Die()
    {
        GameManager.Instance.GameOver();
    }
}
