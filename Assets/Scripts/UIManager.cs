using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject MainMenuGUI;
    public GameObject PauseGUI;
    public GameObject IngameGUI;

    public static UIManager Instance 
    {
        get;
        private set;
    }

    public TMP_Text magazineElement;
    public TMP_Text ammunitionElement;

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

        this.GUIs = new List<GameObject> { MainMenuGUI, PauseGUI, IngameGUI };
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
