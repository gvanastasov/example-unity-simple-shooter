using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject MainMenuGUI;
    public GameObject PauseGUI;
    public GameObject IngameGUI;
    public GameObject GameOverGUI;
    public GameObject GameWonGUI;

    public static UIManager Instance 
    {
        get;
        private set;
    }

    public TMP_Text magazineElement;
    public TMP_Text ammunitionElement;
    public TMP_Text HealthTextElement;
    public Image HealthImageElement;
    public TMP_Text EnemiesTextElement;
    public Image EnemiesImageElement;

    private List<GameObject> GUIs;

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

        // todo: use reflection or some other smart way to cache properties from this
        this.GUIs = new List<GameObject> { MainMenuGUI, PauseGUI, IngameGUI, GameOverGUI,GameWonGUI };
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

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (this.HealthTextElement != null)
        {
            HealthTextElement.text = $"{currentHealth}/{maxHealth}";
        }

        if (this.HealthImageElement != null)
        {
            this.HealthImageElement.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void UpdateEnemiesBar(int currentEnemies, int totalEnemies)
    {
        if (this.EnemiesTextElement != null)
        {
            this.EnemiesTextElement.text = $"{currentEnemies}/{totalEnemies}";
        }

        if (this.EnemiesImageElement != null)
        {
            this.EnemiesImageElement.fillAmount = (float)currentEnemies/totalEnemies;
        }
    }

    public void Show(GameObject GUI)
    {
        var targetInstanceId = GUI.GetInstanceID();
        this.GUIs.ForEach(x => {
            if (x != null)
            {
                x.SetActive(x.GetInstanceID() == targetInstanceId);
            }
        });
    }
}
