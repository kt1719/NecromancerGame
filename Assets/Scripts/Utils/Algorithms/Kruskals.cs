using UnityEngine;
using System.Collections.Generic;

public class MapGenAlgorithms
{    
    public static List<((uint, uint), (uint, uint))> Kruskal(Vector2 dimensions)
    {
        // Create a grid of sets (Node)
        Dictionary<(uint, uint), uint> grid = new Dictionary<(uint, uint), uint>();
        for (int i = 0; i < dimensions.x; i++)
        {
            for (int j = 0; j < dimensions.y; j++)
            {
                uint x = (uint)i;
                uint y = (uint)j;
                uint v = (uint)(i * dimensions.y + j);
                grid.Add((x, y), v);
            }
        }
        // Create a list of all the edges in the graph
        List<((uint, uint), (uint, uint))> edges = new List<((uint, uint), (uint, uint))>();
        for (int i = 0; i < dimensions.x; i++)
        {
            for (int j = 0; j < dimensions.y; j++)
            {
                if (i < dimensions.x - 1)
                {
                    edges.Add((((uint, uint), (uint, uint)))((i, j), (i + 1, j)));
                }
                if (j < dimensions.y - 1)
                {
                    edges.Add((((uint, uint), (uint, uint)))((i, j), (i, j + 1)));
                }
            }
        }

        // Randomize the edges
        for (int i = 0; i < edges.Count; i++)
        {
            int randomIndex = Random.Range(i, edges.Count);
            ((uint, uint), (uint, uint)) temp = edges[i];
            edges[i] = edges[randomIndex];
            edges[randomIndex] = temp;
        }

        // Create a list of edges in the minimum spanning tree
        List<((uint, uint), (uint, uint))> minimumSpanningTree = new List<((uint, uint), (uint, uint))>();
        foreach (((uint, uint), (uint, uint)) edge in edges)
        {
            uint v1 = grid[edge.Item1];
            uint v2 = grid[edge.Item2];
            if (v1 != v2)
            {
                minimumSpanningTree.Add(edge);
            }
            // loop and find all the keys that have v2 
            List<(uint, uint)> keys = new List<(uint, uint)>();
            foreach (var key in grid.Keys)
            {
                if (grid[key] == v2)
                {
                    keys.Add(key);
                }
            }
            foreach (var key in keys)
            {
                grid[key] = v1;
            }
        }
        return minimumSpanningTree;
    }
}