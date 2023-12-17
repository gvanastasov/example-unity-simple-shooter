using System;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    public GameObject Bullet = null;
    public Transform GunPoint = null;
    
    private AudioSource cmp_audioSource;
    private ParticleSystem cmp_burstParticles;

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
    }

    private void EnsureSetup()
    {
        if (this.cmp_burstParticles)
        {
            var main = this.cmp_burstParticles.main;
            main.loop = false;
        }
    }

    public void Shoot()
    {
        if (Bullet != null && GunPoint != null)
        {
            GameObject.Instantiate(Bullet, GunPoint);
            this.PlaySound();
            this.cmp_burstParticles.Play();
        }
    }

    public void PlaySound()
    {
        this.cmp_audioSource?.Play(0);
    }
}
