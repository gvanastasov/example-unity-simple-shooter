using System;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    public GameObject Bullet = null;
    public Transform GunPoint = null;
    public int MagazineSize = 9;
    public bool AutoReload = true;
    
    private AudioSource cmp_audioSource;
    private ParticleSystem cmp_burstParticles;

    private int currentMagazineSize = 0;

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
    }

    public void Reload()
    {
        var bullets = PlayerInventory.Instance.GetBullets(MagazineSize);
        this.currentMagazineSize = bullets;
        UIManager.Instance.UpdateMagazineUI(this.currentMagazineSize, this.MagazineSize);
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
                UIManager.Instance.UpdateMagazineUI(this.currentMagazineSize, this.MagazineSize);
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
