using System.Collections.Generic;
using UnityEngine;

public class InfluenceMap : MonoBehaviour
{
    // Singleton member
    public static InfluenceMap Instance = null;

    // Constants for readability - no magic numbers
    private const int RED = 1;
    private const int GREEN = -1;
    private const float NO_INFLUENCE = 0.0f;
    private const float FULL_INFLUENCE = 1.0f;
    private const char EMPTY = ' ';
    private const int NO_DIRECTION = 0;
    private const int NORTH = -1;
    private const int SOUTH = 1;
    private const int WEST = -1;
    private const int EAST = 1;

    // The relevant influence data
    public int size;
    public float[,] map;
    public char[,] placements;

    // Correctly handles singleton behavior
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GenerateArrays();
    }

    private void GenerateArrays()
    {
        map = new float[size, size];
        placements = new char[size, size];

        // Fill both arrays with blank slots
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                map[i, j] = NO_INFLUENCE;
                placements[i, j] = EMPTY;
            }
        }
    }

    // Creates influence for a unit on the map
    private void CreateInfluence(int startX, int startZ, int color, int strength)
    {
        // Iterate through influence map
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                // Generate the influence value from distance to influencer
                int distance = Distance(x, z, startX, startZ);
                float influence = ModInfluence(x, z, distance, color, strength);
            }
        }
    }

    // Creates influence in a specific direction for a unit
    private void CreateInfluence(int startX, int startZ, int xMod, int zMod, int color)
    {
        // Variables for influence generation
        int x = startX;
        int z = startZ;
        int xPre = x;
        int zPre = z;

        // Iterate until we have either 0 influence or run out of space on the map
        while (true)
        {
            x += xMod;
            z += zMod;

            if (SquareExists(x, z))
            {
                // Generate the influence value from distance to influencer
                int distance = Distance(x, z, xPre, zPre);
                float influence = ModInfluence(x, z, distance, color);

                // Break out of the loop
                if (influence == 0)
                {
                    return;
                }
            }
            else return;
        }
    }

    // Determines if a square exists on the map
    private bool SquareExists(int x, int z)
    {
        return x >= 0 && x < size && z >= 0 && z < size;
    }

    // Modifies the influence of a location in the grid based on distance and color
    private float ModInfluence(int x, int z, int distance, int color, int strength)
    {
        // Generate influence, add it to current influence, and then return the value
        // Some units are stronger than others. strengthMod extends their influence.
        float strengthMod = strength * 0.1f;
        float influence = Mathf.Clamp(FULL_INFLUENCE - (0.1f * distance), 0.0f, 1.0f);
        influence += strengthMod;
        influence *= color;
        map[x, z] += influence;
        return influence;
    }

    // Finds the distance between two indeces
    private int Distance(int x1, int z1, int x2, int z2)
    {
        return Mathf.Abs(x2 - x1) + Mathf.Abs(z2 - z1);
    }

    public void AddRedUnit(GameObject unit)
    {
        // Place the red unit
        int x = (int)Mathf.Floor(unit.transform.position.x);
        int z = (int)Mathf.Floor(unit.transform.position.z);
        Instance.placements[x, z] = 'r';
        Instance.map[x, z] = FULL_INFLUENCE * RED;

        // Modify influence map
        Instance.CreateInfluence(x, z, RED);
    }

    public void AddGreenUnit(GameObject unit)
    {
        // Place the red unit
        int x = (int)Mathf.Floor(unit.transform.position.x);
        int z = (int)Mathf.Floor(unit.transform.position.z);
        Instance.placements[x, z] = 'g';
        Instance.map[x, z] = FULL_INFLUENCE * GREEN;

        // Modify influence map
        Instance.CreateInfluence(x, z, GREEN);
    }
}
