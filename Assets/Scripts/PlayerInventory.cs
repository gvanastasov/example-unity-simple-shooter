using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int Bullets = 100;

    public static PlayerInventory Instance 
    {
        get;
        private set;
    }

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

        UIManager.Instance.UpdateAmmunitionUI(this.Bullets);
    }

    public int GetBullets(int count)
    {
        var result = count > Bullets ? Bullets : count;
        this.Bullets -= result;
        UIManager.Instance.UpdateAmmunitionUI(this.Bullets);
        return result;
    }
}
