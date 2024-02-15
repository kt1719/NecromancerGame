using System;
using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using UnityEditor;
using UnityEngine;

namespace GameTools {
    public class MazeGen : EditorWindow {
        class MazeCell {
            public bool top = false;
            public bool right = false;
            public bool bottom = false;
            public bool left = false;
            public void UpdateCoord(Vector2 src, Vector2 dest) {
                if (src.x == dest.x) {
                    if (src.y < dest.y) {
                        top = true;
                    } else if (src.y > dest.y) {
                        bottom = true;
                    }
                } else if (src.y == dest.y) {
                    if (src.x < dest.x) {
                        right = true;
                    } else if (src.x > dest.x) {
                        left = true;
                    }
                }
            }

            public MazeCell() {}
        }

        private enum MazeAlgorithm {
            Kruskal,
            test
        }
        public Vector2 dimensions = new Vector2(10, 10);
        private float lineThickness = 0.05f;
        MazeAlgorithm algorithm = MazeAlgorithm.Kruskal;

        [MenuItem("NecromancerGame/Map/MazeGen")]
        private static void ShowWindow() {
            GetWindow<MazeGen>("MazeGen");
        }

        private void OnGUI() {

            lineThickness = EditorGUILayout.Slider("Line Thickness", lineThickness, 0.01f, 0.1f);
            GUILayout.Label("MazeGen", EditorStyles.boldLabel);
            dimensions = EditorGUILayout.Vector2Field("Dimensions", dimensions);
            algorithm = (MazeAlgorithm)EditorGUILayout.EnumPopup("Algorithm", algorithm);

            if (GUILayout.Button("Generate")) {
                GenerateMaze(dimensions);
            }
        }

        private void GenerateMaze(Vector2 dimensions)
        {
            List<((uint, uint), (uint, uint))> minimumSpanningTree = new List<((uint, uint), (uint, uint))>();
            // Create the maze mathematically using algorithm
            switch (algorithm)
            {
                case MazeAlgorithm.Kruskal:
                    minimumSpanningTree = MapGenAlgorithms.Kruskal(dimensions);
                    break;
                case MazeAlgorithm.test:
                    break;
            }

            // Make a grid of MazeCells
            MazeCell[,] maze = new MazeCell[(int)dimensions.x, (int)dimensions.y];
            for (int i = 0; i < dimensions.x; i++)
            {
                for (int j = 0; j < dimensions.y; j++)
                {
                    maze[i, j] = new MazeCell();
                }
            }
            // Convert the minimum spanning tree to a grid of MazeCells
            foreach (var edge in minimumSpanningTree)
            {
                // Draw a line between the two cells
                Vector2 cell1 = new Vector2((int)edge.Item1.Item1, (int)edge.Item1.Item2);
                Vector2 cell2 = new Vector2((int)edge.Item2.Item1, (int)edge.Item2.Item2);
                maze[(int)cell1.x, (int)cell1.y].UpdateCoord(cell1, cell2);
                maze[(int)cell2.x, (int)cell2.y].UpdateCoord(cell2, cell1);
            }

            // Visualisation of the maze
            VisualiseMazeEdge(minimumSpanningTree);
            VisualiseMaze(maze);
        }

        private void VisualiseMaze(MazeCell[,] maze)
        {
            void DrawLine(Vector2 start, Vector2 end, Color color)
            {
                Debug.DrawLine(start, end, color, 40f);
            }

            void DrawCell(Vector2 coordinatePosition, MazeCell cell) {
                Vector2 cellPos = new Vector2(coordinatePosition.x * 10, coordinatePosition.y * 10);
                if (!cell.top) {
                    DrawLine(new Vector2(cellPos.x - 5, cellPos.y + 5), new Vector2(cellPos.x + 5, cellPos.y + 5), Color.blue);
                }
                if (!cell.right) {
                    DrawLine(new Vector2(cellPos.x + 5, cellPos.y - 5), new Vector2(cellPos.x + 5, cellPos.y + 5), Color.blue);
                }
                if (!cell.bottom) {
                    DrawLine(new Vector2(cellPos.x - 5, cellPos.y - 5), new Vector2(cellPos.x + 5, cellPos.y - 5), Color.blue);
                }
                if (!cell.left) {
                    DrawLine(new Vector2(cellPos.x - 5, cellPos.y - 5), new Vector2(cellPos.x - 5, cellPos.y + 5), Color.blue);
                }
            }

            // loop through the maze and draw the walls
            for (int i = 0; i < dimensions.x; i++)
            {
                for (int j = 0; j < dimensions.y; j++)
                {
                    DrawCell(new Vector2(i, j), maze[i, j]);
                }
            }
        }

        private void VisualiseMazeEdge(List<((uint, uint), (uint, uint))> minimumSpanningTree)
        {
            // Draw the maze
            foreach (var edge in minimumSpanningTree)
            {
                // Draw a line between the two cells
                Vector2 cell1 = new Vector2(edge.Item1.Item1, edge.Item1.Item2);
                Vector2 cell2 = new Vector2(edge.Item2.Item1, edge.Item2.Item2);
                Vector2 cell1Pos = new Vector2(cell1.x * 10, cell1.y * 10);
                Vector2 cell2Pos = new Vector2(cell2.x * 10, cell2.y * 10);
                Debug.DrawLine(cell1Pos, cell2Pos, Color.red, 40f);
            }
        }
    }
}