using UnityEngine;

/// <summary>
/// Class responsible for handling behavior of gun.
/// </summary>
/// <remarks>
/// Gun is a weapon that can be used by player or enemy.
/// Gun can shoot bullets.
/// Gun can be reloaded.
/// Gun applies damage to objects that are in range of attack.
/// </remarks>
public class GunBehaviour : MonoBehaviour
{
#region Serializable Fields
    /// <summary>
    /// Object that will be spawned when gun shoots.
    /// </summary>
    [Tooltip("Object that will be spawned when gun shoots.")]
    public GameObject Bullet = null;

    /// <summary>
    /// Object that will be used as a spawn point for bullets.
    /// </summary>
    [Tooltip("Object that will be used as a spawn point for bullets.")]
    public Transform GunPoint = null;

    /// <summary>
    /// Number of bullets that can be loaded into gun.
    /// </summary>
    [Tooltip("Number of bullets that can be loaded into gun.")]
    public int MagazineSize = 9;

    /// <summary>
    /// Should gun reload automatically when magazine is empty.
    /// </summary>
    [Tooltip("Should gun reload automatically when magazine is empty.")]
    public bool AutoReload = true;

    /// <summary>
    /// Damage that gun applies to objects that are in range of attack.
    /// </summary>
    [Tooltip("Damage that gun applies to objects that are in range of attack.")]
    public int AttackDamage = 10;

    /// <summary>
    /// Layer mask that defines what objects can be damaged by gun.
    /// </summary>
    [Tooltip("Layer mask that defines what objects can be damaged by gun.")]
    public LayerMask AttackDamageMask = -1;

    /// <summary>
    /// Range of attack.
    /// </summary>
    [Tooltip("Range of attack.")]
    public int AttackRange = 20;
#endregion

#region Private Fields
    /// <summary>
    /// Audio source component used to play shooting sound.
    /// </summary>
    private AudioSource cmp_audioSource;

    /// <summary>
    /// Particle system component used to play shooting particles.
    /// </summary>
    private ParticleSystem cmp_burstParticles;

    /// <summary>
    /// Number of bullets that are currently loaded into gun.
    /// </summary>
    private int currentMagazineSize = 0;

    /// <summary>
    /// Is this gun used by player
    /// </summary>
    /// <remarks>
    /// This is used to determine if gun should be reloaded automatically.
    /// </remarks>
    private bool isPlayer = true;
#endregion

#region Unity Callbacks
    void Awake()
    {
        if (GunPoint == null)
        {
            Debug.LogWarning("Gun point missing...", this);
        }
        if (Bullet == null) 
        {
            Debug.LogWarning("Bullet prefab reference missing...", this);
        }

        this.cmp_audioSource = GetComponent<AudioSource>();
        this.cmp_burstParticles = GetComponentInChildren<ParticleSystem>();

        this.isPlayer = GetComponentInParent<PlayerBehaviour>() != null;

        EnsureSetup();
        Reload();
    }
#endregion

#region Validation
    /// <summary>
    /// Ensures that gun is properly setup.
    /// </summary>
    private void EnsureSetup()
    {
        if (this.cmp_burstParticles)
        {
            var main = this.cmp_burstParticles.main;
            main.loop = false;
        }
        if (!isPlayer)
        {
            this.AutoReload = true;
        }
    }
#endregion

#region Actions
    /// <summary>
    /// Reloads gun.
    /// </summary>
    /// <remarks>
    /// If gun is used by player, then it will reload gun from player inventory.
    /// Otherwise it will reload gun from magazine size, aka unlimited.
    /// </remarks>
    public void Reload()
    {
        var bullets = this.isPlayer 
            ? PlayerInventory.Instance.GetBullets(MagazineSize)
            : this.MagazineSize;
        this.currentMagazineSize = bullets;

        if (isPlayer)
        {
            UIManager.Instance.Magazine_UpdateText(this.currentMagazineSize, this.MagazineSize);
        }
    }

    /// <summary>
    /// Shoots from gun, by spawning bullet object at the gun point. Target is determined by raycast.
    /// </summary>
    /// <remarks>
    /// If gun has no bullets, then it will reload automatically.
    /// If gun has no gun point, then it will not shoot.
    /// If gun has no bullet, then it will not shoot.
    /// If gun hits object that is damageable, then it will apply damage to it.
    /// </remarks>
    public void Shoot()
    {
        bool hasGun = Bullet != null && GunPoint != null;
        bool hasBullets = this.currentMagazineSize > 0;

        if (hasGun)
        {
            if (hasBullets) 
            {
                this.currentMagazineSize -= 1;
                this.PlaySound();
                this.cmp_burstParticles.Play();

                GameObject.Instantiate(Bullet, GunPoint);
                
                var forward = transform.TransformDirection(Vector3.forward);
                if (Physics.Raycast(transform.position, forward, out RaycastHit hit, this.AttackRange, AttackDamageMask))
                {
                    var target = hit.transform.GetComponent<IDamageable>();
                    if (target != null)
                    {
                        target.Damage(
                            damage: this.AttackDamage,
                            damageOrigin: this.transform.position
                        );
                    }
                }

                if (isPlayer)
                {
                    UIManager.Instance.Magazine_UpdateText(this.currentMagazineSize, this.MagazineSize);
                }
            }
            else
            {
                if (this.AutoReload)
                {
                    Reload();
                }
            }
        }
    }

    /// <summary>
    /// Plays shooting sound.
    /// </summary>
    public void PlaySound()
    {
        this.cmp_audioSource?.Play(0);
    }
#endregion
}
