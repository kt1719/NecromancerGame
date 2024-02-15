using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameTools {
    // Serializable struct to store the maze cell data //
    [Serializable] struct MazeCellSerializable
    {
        public bool top;
        public bool right;
        public bool bottom;
        public bool left;
    }

    [Serializable] struct MinimumSpanningTreeNode
    {
        public Vector2Int src;
        public Vector2Int dest;
    }

    [CreateAssetMenu(fileName = "MapMetaData", menuName = "ScriptableObjects/MapMetaData")]
    [Serializable]
    public class MapMetaData : ScriptableObject
    {
        [SerializeField] private List<MazeCellSerializable> mazeCells = new List<MazeCellSerializable>();
        [SerializeField] private List<MinimumSpanningTreeNode> minimumSpanningTreeNodes = new List<MinimumSpanningTreeNode>();

        /////////////////////////////////////////////////////
        public Vector2 dimensions;

        public void SetMaze(MazeCell[,] maze, Vector2 dimensions, List<((uint, uint), (uint, uint))> minimumSpanningTree)
        {
            // Copy the maze to mazeCells serializable list
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    MazeCellSerializable cell = new MazeCellSerializable();
                    cell.top = maze[i, j].top;
                    cell.right = maze[i, j].right;
                    cell.bottom = maze[i, j].bottom;
                    cell.left = maze[i, j].left;
                    mazeCells.Add(cell);
                }
            }

            // Copy the dimensions
            this.dimensions = new Vector2(dimensions.x, dimensions.y);

            // Copy the minimum spanning tree
            foreach (var edge in minimumSpanningTree)
            {
                MinimumSpanningTreeNode node = new MinimumSpanningTreeNode();
                node.src = new Vector2Int((int)edge.Item1.Item1, (int)edge.Item1.Item2);
                node.dest = new Vector2Int((int)edge.Item2.Item1, (int)edge.Item2.Item2);
                minimumSpanningTreeNodes.Add(node);
            }

        }

        //	Implement this method to receive a callback before Unity serializes your object.    
        public MazeCell[,] ReturnMaze()
        {
            MazeCell[,] maze = new MazeCell[(int)dimensions.x, (int)dimensions.y];
            for (int i = 0; i < mazeCells.Count; i++)
            {
                MazeCell cell = new MazeCell();
                cell.top = mazeCells[i].top;
                cell.right = mazeCells[i].right;
                cell.bottom = mazeCells[i].bottom;
                cell.left = mazeCells[i].left;
                maze[i / (int)dimensions.y, i % (int)dimensions.y] = cell;
            }
            return maze;
        }
        
        public List<((uint, uint), (uint, uint))> ReturnMinimumSpanningTree()
        {
            List<((uint, uint), (uint, uint))> minimumSpanningTree = new List<((uint, uint), (uint, uint))>();
            foreach (var node in minimumSpanningTreeNodes)
            {
                ((uint, uint), (uint, uint)) edge = (((uint)node.src.x, (uint)node.src.y), ((uint)node.dest.x, (uint)node.dest.y));
                minimumSpanningTree.Add(edge);
            }
            return minimumSpanningTree;
        }

        public Vector2 ReturnDimensions()
        {
            return dimensions;
        }
    }
}
