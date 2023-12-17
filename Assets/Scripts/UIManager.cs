using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance 
    {
        get;
        private set;
    }

    public TMP_Text magazineElement;
    public TMP_Text ammunitionElement;

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
    }

    public void UpdateMagazineUI(int currentSize, int maxSize)
    {
        if (this.magazineElement != null)
        {
            magazineElement.text = $"Clip: {currentSize}/{maxSize}";
        }
    }

    public void UpdateAmmunitionUI(int count)
    {
        if (this.ammunitionElement != null)
        {
            ammunitionElement.text = $"Ammu: {count}";
        }
    }
}
