using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandGenerator : MonoBehaviour
{
    public static float[,] s_LandHeightmap;

    public static Dictionary<Vector2, BaseType> s_LandTiles = new Dictionary<Vector2, BaseType>();

    private int m_rows;
    private int m_columns;

    private List<Vector2> m_definedNeighbourIndexes = new List<Vector2>();

    public void CreateLand(int rows, int columns, int seed)
    {
        // INITIALIZATION
        Random.InitState(seed);
        m_rows = rows;
        m_columns = columns;
        m_definedNeighbourIndexes.Clear();

        FindNeighboursOfWater();
        GetComponent<HeightmapGenerator1>().SetLandHeights(m_definedNeighbourIndexes);
        while (WorldGenerator.s_UndefinedTiles.Count > 0)
        {
            FindNeighboursOfLand();
            GetComponent<HeightmapGenerator1>().SetLandHeights(m_definedNeighbourIndexes);
        }
        HeightmapGenerator1.callCount = 0;
    }

    private void FindNeighboursOfWater()
    {
        for (int x = 0; x < m_rows; x++)
        {
            for (int y = 0; y < m_columns; y++)
            {
                if (WaterGenerator.s_WaterTiles.ContainsKey(new Vector2(x, y)))
                {
                    FindUndefinedNeighbours(new Vector2(x,y));
                }
            }
        }
    }

    private void FindNeighboursOfLand()
    {
        // INITIALIZATION
        Vector2[] definedLandIndexes = new Vector2[m_definedNeighbourIndexes.Count];
        m_definedNeighbourIndexes.CopyTo(definedLandIndexes);
        m_definedNeighbourIndexes.Clear();

        foreach (Vector2 definedLandIndex in definedLandIndexes)
        {
            FindUndefinedNeighbours(definedLandIndex);
        }
    }

    private void FindUndefinedNeighbours(Vector2 index)
    {
        List<Vector2> neighbourIndexes = NeighbourUtility.GetNeighbours(index);

        foreach (Vector2 neighbourIndex in neighbourIndexes)
        {
            // Check if the tile has been assigned a BaseType
            if (!WaterGenerator.s_WaterTiles.ContainsKey(neighbourIndex) && !s_LandTiles.ContainsKey(neighbourIndex))
            {
                m_definedNeighbourIndexes.Add(neighbourIndex);
                s_LandTiles.Add(neighbourIndex, BaseType.Land);
                WorldGenerator.s_UndefinedTiles.Remove(neighbourIndex);
            }
        }
    }
}
