using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowSimulator : MonoBehaviour
{
    private int m_rows;
    private int m_columns;

    private void OnEnable()
    {
        SystemGenerator.OnSystemGenerationComplete += InitializeFlow;
        FlowTimer.Instance().OnTimerTick += SendLandFlowPulse;
        FlowTimer.Instance().OnTimerTick += SendWaterFlowPulse;
        FlowTimer.Instance().OnTimerTick += SendFishFlowPulse;
    }

    private void OnDisable()
    {
        SystemGenerator.OnSystemGenerationComplete -= InitializeFlow;
        FlowTimer.Instance().OnTimerTick -= SendLandFlowPulse;
        FlowTimer.Instance().OnTimerTick -= SendWaterFlowPulse;
        FlowTimer.Instance().OnTimerTick -= SendFishFlowPulse;
    }

    public void SendLandFlowPulse()
    {
        //Debug.Log("SEND LAND FLOW");

        FlowStyle flowStyle = new LandFlowStyle();

        for (int x = m_rows; x > 0; x--)
        {
            for (int y = m_columns; y > 0; y--)
            {
                Vector2 tileIndex = new Vector2(x, y);

                if (TileManager.s_TilesDictonary.TryGetValue(tileIndex, out GameObject value))
                {
                    if (value.GetComponent<Tile>().m_Basetype == BaseType.Land)
                    {
                        FlowPulse(value, flowStyle, tileIndex);
                    }         
                }
            }
        }
    }

    public void SendWaterFlowPulse()
    {
        //Debug.Log("SEND WATER FLOW");

        FlowStyle flowStyle = new WaterFlowStyle();

        for (int x = m_rows; x > 0; x--)
        {
            for (int y = m_columns; y > 0 ; y--)
            {
                Vector2 tileIndex = new Vector2(x, y);

                if (TileManager.s_TilesDictonary.TryGetValue(tileIndex, out GameObject value))
                {
                    if (value.GetComponent<Tile>().m_Basetype == BaseType.Water)
                    {
                        FlowPulse(value, flowStyle, tileIndex);
                    }
                }
            }
        }
    }

    public void SendFishFlowPulse()
    {
        //Debug.Log("SEND LAND FLOW");

        FlowStyle flowStyle = new FishFlowStyle();

        for (int x = m_rows; x > 0; x--)
        {
            for (int y = m_columns; y > 0; y--)
            {
                Vector2 tileIndex = new Vector2(x, y);

                if (TileManager.s_TilesDictonary.TryGetValue(tileIndex, out GameObject value))
                {
                    if (value.GetComponent<Tile>().m_Basetype == BaseType.Water)
                    {
                        FlowPulse(value, flowStyle, tileIndex);
                    }
                }
            }
        }
    }

    private void FlowPulse(GameObject senderTile, FlowStyle flowStyle, Vector2 indexForDebugging)
    {
        List<GameObject> receiverTiles = GetRequiredNeighbours(senderTile);

        foreach (GameObject receiverTile in receiverTiles)
        {
            if (flowStyle.CanFlow(senderTile, receiverTile, indexForDebugging))
            {
                flowStyle.Flow(senderTile, receiverTile, indexForDebugging);
            }
            else
            {

            }
        }
    }

    private List<GameObject> GetRequiredNeighbours (GameObject senderTile)
    {
        return senderTile.GetComponent<Tile>().m_receiverNeighbours;
    }
     
    // Initializes tiles by finding the nieghbours of each tile that will receive information
    private void InitializeFlow(int rows, int columns)
    {
        //Debug.Log("ITIALIZE FLOW");

        m_rows = rows;
        m_columns = columns;

        for (int x = 0; x < m_rows; x++)
        { 
            for (int y = 0; y < m_columns; y++)
            {
                Vector2 tileIndex = new Vector2(x, y);

                if (TileManager.s_TilesDictonary.TryGetValue(tileIndex, out GameObject value)) 
                {
                    Tile tileScript = value.GetComponent<Tile>();
                    List<Vector2> neighbourIndices = NeighbourUtility.FindAllNeighbours(tileIndex);

                    if (tileScript.m_Basetype == BaseType.Water)
                    {
                        tileScript.m_receiverNeighbours = NeighbourUtility.FindLowestNeighbours(neighbourIndices);
                    }

                    if (tileScript.m_Basetype == BaseType.Land)
                    {
                        tileScript.m_receiverNeighbours = NeighbourUtility.FindLowestNeighbour(neighbourIndices, value);
                    }
                }
            }
        }
    }
}
