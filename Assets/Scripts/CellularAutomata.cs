using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

// 0 is water
// 1 is grass

public class CellularAutomata : MonoBehaviour
{

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    [SerializeField] private string seed;
    [SerializeField] private bool useRandomSeed = true;

    [Range(0, 1)]
    [SerializeField] private float threshold = 0.5f;
    [Range(0, 9)]
    [SerializeField] private int neighborThreshold = 5;

    [Range(0, 100)]
    [SerializeField] private int iterations = 5;

    [SerializeField] Tilemap tileMap;
    [SerializeField] Tile dirt;
    [SerializeField] Tile grass;
    [SerializeField] Tile rock;
    [SerializeField] Tile water;

    private float[,] noiseGrid;
    private float[,] unprocessedGrid;
    private float[,] processedGrid;

    private void Awake()
    {
        noiseGrid = new float[width, height];
        generate_initial_noise();
        // smooth_grid();
    }


    private void Start()
    {
        // draw_tilemap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            smooth_grid();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            generate_initial_noise();
        }
    }

    private void generate_initial_noise()
    {

        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudo_random_number_generator = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noiseGrid[x, y] = pseudo_random_number_generator.Next(0, 100) / 100f;
            }
        }

        processedGrid = (float[,])noiseGrid.Clone();

        draw_tilemap();
    }

    private void smooth_grid_loop()
    {
        for (int i = 0; i < iterations; i++)
        {
            smooth_grid();
        }
    }

    private void smooth_grid()
    {
        unprocessedGrid = (float[,])processedGrid.Clone();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int number_of_neighbors = get_neighboring_walls_count(x, y);
                if (number_of_neighbors > neighborThreshold)
                {
                    processedGrid[x, y] = 1f;
                }
                else if (number_of_neighbors < neighborThreshold)
                {
                    processedGrid[x, y] = 0f;
                }
            }
        }

        draw_tilemap();
    }

    private int get_neighboring_walls_count(int x, int y)
    {
        int neighboring_walls = 0;

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < width && j >= 0 && j < height)
                {
                    if (i != x || j != y) // Skip the center position as that is the current cell
                    {
                        if (unprocessedGrid[i, j] >= threshold)
                        {
                            neighboring_walls++;
                        }
                    }
                }
                else
                {
                    neighboring_walls++;
                }
            }
        }

        return neighboring_walls;
    }

    private void draw_tilemap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                set_tile(x, y);
            }
        }
    }

    private void set_tile(int x, int y)
    {
        Vector3Int position = new Vector3Int(x, y, 0);
        if (processedGrid[x, y] < threshold)
        {
            tileMap.SetTile(position, grass);
        }
        else
        {
            tileMap.SetTile(position, water);
        }
    }

}
