using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    public GameObject Bullet = null;
    public Transform GunPoint = null;

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
    }

    public void Shoot()
    {
        if (Bullet != null && GunPoint != null)
        {
            GameObject.Instantiate(Bullet, GunPoint);
        }
    }
}
