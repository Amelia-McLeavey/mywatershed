using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum SeasonState { Summer, Winter }
public enum RedDacePopulationState { High, Average, Low, Extinct }

/// <summary>
/// A script to track the state of the world.
/// </summary>

public class World : MonoBehaviour
{
    public delegate void OnSeasonChangeHandler(SeasonState season);
    public static event OnSeasonChangeHandler OnSeasonChange;

    [SerializeField]
    public float m_summerLengthInSeconds = 30f;

    [SerializeField]
    private int m_currentYear = 1;

    [SerializeField]
    private TMP_Text m_currentYearText;
    [SerializeField]
    private TMP_Text m_redDacePopulationText;
    [SerializeField]
    private Slider m_temperatureSlider;
    [SerializeField]
    private Slider m_turbiditySlider;
    [SerializeField]
    private TMP_Text m_temperatureText;

    [SerializeField]
    private DaceHealthUI m_daceHealthScript;

    [SerializeField]
    private GameObject m_failStateObject;

    private int m_averageTemperature;
    private float m_averageTurbidity;

    [SerializeField]
    private int m_redDaceTotalPopulation;
    [SerializeField]
    private int m_chubTotalPopulation;
    [SerializeField]
    private int m_troutTotalPopulation;
    [SerializeField]
    private int m_insectTotalPopulation;

    private GameManager m_gameManager;
    private WorldGenerator m_worldGenerator;
    private CardDeckHandler m_cardDeckHandler;

    [SerializeField] private GameObject pauseScreen;

    private Heatmap heatmap;
    public EndResultManager m_endResultManager;

    public SeasonState m_seasonState { get; private set; }

    private VolunteerManager volunteerManager;

    private void Start()
    {
        m_gameManager = GameManager.Instance;
        m_gameManager.SetGameState(GameState.Game, null);
        heatmap = GameObject.FindObjectOfType<Heatmap>();
        volunteerManager = GameObject.FindObjectOfType<VolunteerManager>();
        m_endResultManager.CallStart();

        if (m_temperatureSlider == null)
        {
            Debug.LogWarning("m_temperatureSlider is not connected.");
        }
        if (m_redDacePopulationText == null)
        {
            Debug.LogWarning("m_redDacePopulationText is not connected.");
        }
        if (m_currentYearText == null)
        {
            Debug.LogWarning("m_currentYearText is not connected.");
        }

        
        m_worldGenerator = FindObjectOfType<WorldGenerator>();
        m_cardDeckHandler = FindObjectOfType<CardDeckHandler>();
        m_seasonState = SeasonState.Summer;
        ChangeSeason(m_seasonState); 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }      
    }

    public void PauseGame()
    {
        m_gameManager.SetGameState(GameState.Pause, null);
        pauseScreen.SetActive(true);
    }

    public void OnClickReturnToMenu()
    {
        m_gameManager.SetGameState(GameState.MainMenu, "PrototypeMenuScene");
    }

    public void ChangeSeason(SeasonState season)
    {
        // Change to Summer
        if (m_seasonState == SeasonState.Winter && season == SeasonState.Summer)
        {
            m_currentYear++;
            SetYear();
            volunteerManager.AddVolunteers();
            heatmap.GenerateMaps();
        }
        // Change to winter
        else if (m_seasonState == SeasonState.Summer && season == SeasonState.Winter)
        {
            UpdateAllData();
            m_cardDeckHandler.DealCards();

            m_endResultManager.AddDataPoint(m_currentYear, m_redDaceTotalPopulation, m_chubTotalPopulation, m_troutTotalPopulation, m_insectTotalPopulation, m_averageTemperature, m_averageTurbidity);
        }        
        // Change the season state and callback the event
        m_seasonState = season;
        OnSeasonChange?.Invoke(season);
    }

    private void SetYear()
    {
        // TODO: Share UI across scenes
        if (m_currentYearText != null)
        {
            m_currentYearText.text = m_currentYear.ToString();
        }    
    }

    public void UpdateTotalFaunaPopulations(int totalDacePop, int totalChubPop, int totalTroutPop, int totalInsectPop)
    {
        m_redDaceTotalPopulation = totalDacePop;
        m_chubTotalPopulation = totalChubPop;
        m_troutTotalPopulation = totalTroutPop;
        m_insectTotalPopulation = totalInsectPop;
    }

    private void UpdateAllData()
    {
        float totalTemp = 0;
        float totalTurbidity = 0;

        int numberOfTurbidTiles=0;

        for (int x = 0; x < m_worldGenerator.m_rows; x++)
        {
            for (int y = 0; y < m_worldGenerator.m_columns; y++)
            {
                Vector2 tileIndex = new Vector2(x, y);

                if (TileManager.s_TilesDictonary.TryGetValue(tileIndex, out GameObject value))
                {
                    if (value.GetComponent<WaterTemperature>() != null)
                    {
                        totalTemp += value.GetComponent<WaterTemperature>().value;
                    }

                    if (value.GetComponent<Turbidity>() != null)
                    {
                        numberOfTurbidTiles++;
                        totalTurbidity += value.GetComponent<Turbidity>().value;
                    }
                }
            }
        }

        DisplayTotalDacePopulationInUI();
        m_averageTemperature = Mathf.RoundToInt(totalTemp / (m_worldGenerator.m_rows * m_worldGenerator.m_columns));
        DisplayAverageTemperature();

        m_averageTurbidity = totalTurbidity / numberOfTurbidTiles;
        Debug.Log(m_averageTurbidity);
        DisplayAverageTurbidity();
    }

    public void UpdateAverageTemperature()
    {
        float totalTemp=0;
        for (int x = 0; x < m_worldGenerator.m_rows; x++)
        {
            for (int y = 0; y < m_worldGenerator.m_columns; y++)
            {
                Vector2 tileIndex = new Vector2(x, y);

                if (TileManager.s_TilesDictonary.TryGetValue(tileIndex, out GameObject value))
                {
                    if (value.GetComponent<WaterTemperature>() != null)
                    {
                        totalTemp += value.GetComponent<WaterTemperature>().value;
                    }
                }
            }
        }

        m_averageTemperature = Mathf.RoundToInt(totalTemp / (m_worldGenerator.m_rows * m_worldGenerator.m_columns));
        DisplayAverageTemperature();
    }

    private void DisplayTotalDacePopulationInUI()
    {
        //m_daceHealthScript.SetHealth(m_redDaceTotalPopulation);
        // TODO: Share UI across scenes
        if (m_redDacePopulationText != null)
        {
            m_redDacePopulationText.text = m_redDaceTotalPopulation.ToString();
        }

        // TODO: Test that fail state is working accurately
        if (m_redDaceTotalPopulation <= 0)
        {
            m_failStateObject.SetActive(true);
        }
    }

    private void DisplayAverageTemperature()
    {
        // TODO: Share UI across scenes
        if (m_temperatureSlider != null)
        {
            m_temperatureSlider.value = m_averageTemperature;
            m_temperatureText.text = m_averageTemperature.ToString();
        }
        
    }

    private void DisplayAverageTurbidity()
    {
        // TODO: Share UI across scenes
        if (m_turbiditySlider != null)
        {
            m_turbiditySlider.value = m_averageTurbidity;
        }

    }
}
