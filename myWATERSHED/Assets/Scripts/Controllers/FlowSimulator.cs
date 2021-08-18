using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowSimulator : MonoBehaviour
{
    [SerializeField]
    private float m_pulseSpeed;

    private int m_rows;
    private int m_columns;

    private void OnEnable()
    {
        WorldGenerator.OnWorldGenerationComplete += InitializeFlow;
    }

    private void OnDisable()
    {
        WorldGenerator.OnWorldGenerationComplete -= InitializeFlow;
    }

    public void FlowPulse()
    {
        for (int x = 0; x < m_rows; x++)
        {
            for (int y = 0; y < m_columns; y++)
            {
                if (WorldGenerator.s_TilesDictonary.TryGetValue(new Vector2(x, y), out GameObject value))
                {
                    // FLOW FOR WATER TILES
                    Tile tileScript = value.GetComponent<Tile>();
                    if (tileScript.m_Basetype == BaseType.Water)
                    {
                        tileScript.SendInformationFlow();
                    }
                }
            }
        }
    }

    private void InitializeFlow(int rows, int columns)
    {
        m_rows = rows;
        m_columns = columns;

        for (int x = 0; x < m_rows; x++)
        { 
            for (int y = 0; y < m_columns; y++)
            {
                if (WorldGenerator.s_TilesDictonary.TryGetValue(new Vector2(x,y), out GameObject value)) 
                {
                    // FLOW FOR WATER TILES
                    Tile tileScript = value.GetComponent<Tile>();
                    if (tileScript.m_Basetype == BaseType.Water)
                    {
                        tileScript.FindNeighbours();
                    }
                }
            }
        }
    }

}
