using System;
using UnityEditor;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    public GameObject Bullet = null;
    public Transform GunPoint = null;
    public int MagazineSize = 9;
    public bool AutoReload = true;
    public int AttackDamage = 10;
    public LayerMask AttackDamageMask = -1;
    public int AttackRange = 20;
    
    private AudioSource cmp_audioSource;
    private ParticleSystem cmp_burstParticles;

    private int currentMagazineSize = 0;
    private bool isPlayer = true;

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

    public void Reload()
    {
        var bullets = this.isPlayer 
            ? PlayerInventory.Instance.GetBullets(MagazineSize)
            : this.MagazineSize;
        this.currentMagazineSize = bullets;

        if (isPlayer)
        {
            UIManager.Instance.UpdateMagazineUI(this.currentMagazineSize, this.MagazineSize);
        }
    }

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
                        target.Damage(this.AttackDamage);
                    }
                }

                if (isPlayer)
                {
                    UIManager.Instance.UpdateMagazineUI(this.currentMagazineSize, this.MagazineSize);
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



    public void PlaySound()
    {
        this.cmp_audioSource?.Play(0);
    }
}
