using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: add namespace
// TODO: add base singleton
/// <summary>
/// Responsible for handling GUI display and update handles.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("GUI Refs")]
    public GameObject MainMenuGUI;
    public GameObject PauseGUI;
    public GameObject IngameGUI;
    public GameObject GameOverGUI;
    public GameObject GameWonGUI;

    [Header("Ammunition")]
    public TMP_Text MagazineElement;
    public TMP_Text AmmunitionElement;
    
    [Header("Health Bar")]
    public TMP_Text HealthTextElement;
    public Image HealthImageElement;

    [Header("Enemies Bar")]
    public TMP_Text EnemiesTextElement;
    public Image EnemiesImageElement;
    
    [Header("Score")]
    public TMP_Text ScoreTextElement;
    
    [Header("Level")]
    public TMP_Text LevelTextElement;

    /// <summary>
    /// Cache for GUI screens, from which we can easily toggle one active from, and the
    /// rest off.
    /// </summary>
    private List<GameObject> screens;

    public static UIManager Instance 
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            {
                // TODO: add common logging mechanism
                Debug.Log("UIManager: mutliple instances of UIManager found, destroying this.");
            }
            #endif
            Destroy(this);
        }
        else
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            {
                // TODO: add common logging mechanism
                Debug.Log("UIManager: init as singleton.");
            }
            #endif
            Instance = this;
        }

        // TODO: use reflection or some other smart way to cache properties from this
        this.screens = new List<GameObject> { MainMenuGUI, PauseGUI, IngameGUI, GameOverGUI,GameWonGUI };
    }

    /// <summary>
    /// Activates single-only GUI by reference.
    /// </summary>
    /// <param name="GUI">The target GUI object to activate.</param>
    public void Show(GameObject GUI)
    {
        var targetInstanceId = GUI.GetInstanceID();
        this.screens.ForEach(x => {
            if (x != null)
            {
                #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
                {
                    if (x.GetInstanceID() == targetInstanceId)
                    {
                        // TODO: add common logging mechanism
                        Debug.Log($"UIManager: showing {GUI.name}");
                    }
                }
                #endif
                x.SetActive(x.GetInstanceID() == targetInstanceId);
            }
        });
    }

#region Ammunition
    /// <summary>
    /// Sets the text on the UI element for the magazine info.
    /// </summary>
    /// <param name="currentSize">Amount displayed on the left.</param>
    /// <param name="maxSize">Amount displayed on the right.</param>
    public void Magazine_UpdateText(int currentSize, int maxSize)
    {
        if (this.MagazineElement != null)
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            {
                // TODO: add common logging mechanism
                Debug.Log($"UIManager: magazine text updated.");
            }
            #endif
            MagazineElement.text = $"Clip: {currentSize}/{maxSize}";
        }
    }

    /// <summary>
    /// Sets the text on the UI element for the amount of left ammunition.
    /// </summary>
    /// <param name="count">Amount displayed.</param>
    public void Ammunition_UpdateText(int count)
    {
        if (this.AmmunitionElement != null)
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            {
                // TODO: add common logging mechanism
                Debug.Log($"UIManager: ammunition text updated.");
            }
            #endif
            AmmunitionElement.text = $"Ammu: {count}";
        }
    }
#endregion

#region Health Bar
    /// <summary>
    /// Sets the text and the size of the bar, displaying the amount of health the
    /// player has left.
    /// </summary>
    /// <param name="current">Text and fill percentage divider.</param>
    /// <param name="total">Text and fill divided.</param>
    public void Health_UpdateBar(int current, int total)
    {
        if (this.HealthTextElement != null)
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            {
                // TODO: add common logging mechanism
                Debug.Log($"UIManager: health text updated.");
            }
            #endif
            HealthTextElement.text = $"{current}/{total}";
        }
        if (this.HealthImageElement != null)
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            {
                // TODO: add common logging mechanism
                Debug.Log($"UIManager: health image fill updated - value set to {(float)current/total}.");
            }
            #endif
            this.HealthImageElement.fillAmount = (float)current/total;
        }
    }
#endregion

#region Enemies Bar
    /// <summary>
    /// Sets the text and the size of the bar, displaying the amount of enemies
    /// left for the player to defeat.
    /// </summary>
    /// <param name="current">Text and fill percentage divider.</param>
    /// <param name="total">Text and fill divided.</param>
    public void Enemies_UpdateBar(int current, int total)
    {
        if (this.EnemiesTextElement != null)
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            {
                // TODO: add common logging mechanism
                Debug.Log($"UIManager: enemies text updated.");
            }
            #endif
            this.EnemiesTextElement.text = $"{current}/{total}";
        }

        if (this.EnemiesImageElement != null)
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            {
                // TODO: add common logging mechanism
                Debug.Log($"UIManager: enemies image fill updated - value set to {(float)current/total}.");
            }
            #endif
            this.EnemiesImageElement.fillAmount = (float)current/total;
        }
    }
#endregion

#region Score
    /// <summary>
    /// Sets the text displaying the current amount of score.
    /// </summary>
    /// <param name="value">The value to display.</param>
    public void Score_UpdateText(int value)
    {
        if (this.ScoreTextElement != null)
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            {
                // TODO: add common logging mechanism
                Debug.Log($"UIManager: score text updated - value set to {value}.");
            }
            #endif
            this.ScoreTextElement.text = $"Score: {value}";
        }
    }
#endregion

#region Level
    /// <summary>
    /// Sets the text displaying the current level.
    /// </summary>
    /// <param name="value">The value to display.</param>
    public void Level_UpdateText(int value)
    {
        if (this.LevelTextElement != null)
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            {
                // TODO: add common logging mechanism
                Debug.Log($"UIManager: level text updated - value set to {value}.");
            }
            #endif
            this.LevelTextElement.text = $"Level: {value}";
        }
    }
#endregion
}
