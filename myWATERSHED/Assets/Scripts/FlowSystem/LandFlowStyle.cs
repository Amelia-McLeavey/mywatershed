using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandFlowStyle : FlowStyle
{
    public override bool CanFlow(GameObject senderTile, GameObject receiverTile, Vector2 tileIndexForDebugging)
    {
        Tile senderTileScript = senderTile.GetComponent<Tile>();
        Tile receiverTileScript = receiverTile.GetComponent<Tile>();

        // Check if any of the neighbouring tiles are able to recieve data

        if (receiverTileScript.m_Basetype == BaseType.Water)
        {
            return true;
        }
        else if (receiverTileScript.m_Basetype == BaseType.Land)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void Flow(GameObject senderTile, GameObject receiverTile, Vector2 tileIndexForDebugging)
    {
        // DEBUGS BE DEFENSIVE 
        Debug.Assert(senderTile != null, $"senderTile is null at index {tileIndexForDebugging}");
        Debug.Assert(receiverTile != null, $"receiverTile is null at index {tileIndexForDebugging}");
        Debug.Assert(senderTile.GetComponent<PollutionLevel>() != null, $"senderTile is missing a PoullutionLevel component at index {tileIndexForDebugging}");
        Debug.Assert(receiverTile.GetComponent<PollutionLevel>() != null, $"receiverTile is missing a PoullutionLevel component at index {tileIndexForDebugging}");
        Debug.Assert(senderTile.GetComponent<SewageLevel>() != null, $"senderTile is missing a SewageLevel component at index {tileIndexForDebugging}");
        Debug.Assert(receiverTile.GetComponent<SewageLevel>() != null, $"receiverTile is missing a SewageLevel component at index {tileIndexForDebugging}");
        Debug.Assert(senderTile.GetComponent<WaterTemperature>() != null, $"senderTile is missing a LandTemperature component at index {tileIndexForDebugging}");
        Debug.Assert(receiverTile.GetComponent<WaterTemperature>() != null, $"recieverTile is missing a LandTemperature component at index {tileIndexForDebugging}");

        // POLLUTION LEVEL
        senderTile.GetComponent<PollutionLevel>().m_PolutionLevel -= 1;
        receiverTile.GetComponent<PollutionLevel>().m_PolutionLevel += 1;
        // SEWAGE LEVEL        
        senderTile.GetComponent<SewageLevel>().m_SewageLevel -= 1;        
        receiverTile.GetComponent<SewageLevel>().m_SewageLevel += 1;
        // WATER TEMPERATURE         
        senderTile.GetComponent<WaterTemperature>().m_waterTemperature -= 1;        
        receiverTile.GetComponent<WaterTemperature>().m_waterTemperature += 1;

    }
}
