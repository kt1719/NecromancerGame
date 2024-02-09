using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SuperTiled2Unity;
using System;
// ASSUMES ALL THE CELLS ARE THE SAME SIZE!!!!!!!!!!!!!
namespace GameTools {
    public class MapGenTool : EditorWindow {
        Editor editor;
        [SerializeField] List<GameObject> cellList = new List<GameObject>();
        // [SerializeField] List<GameObject> cellLeftEntranceList = new List<GameObject>();

        [MenuItem("NecromancerGame/Map/MapGenTool")]
        private static void ShowWindow() {
            GetWindow<MapGenTool>("MapGenTool");
        }

        private void OnGUI() {
            GUILayout.Label("Map Generation Tool", EditorStyles.boldLabel);

            if (!editor) { editor = Editor.CreateEditor(this); }
			if (editor) { editor.OnInspectorGUI(); }
        }
        
        void OnInspectorUpdate() {
            Repaint(); 
        }
    }

	[CustomEditor(typeof(MapGenTool), true)]
	public class MapGenHelper : Editor {
        private class Cell {
            public uint cellId;
            public GameObject cellGameObject;
            public bool leftPath;
            public bool rightPath;
            public bool topPath;
            public bool bottomPath;

            public Cell(uint cellId, GameObject cellGameObject, string pathsAllowedString) {
                this.cellId = cellId;
                this.cellGameObject = cellGameObject;
                this.leftPath = pathsAllowedString[0] == '1';
                this.rightPath = pathsAllowedString[1] == '1';
                this.topPath = pathsAllowedString[2] == '1';
                this.bottomPath = pathsAllowedString[3] == '1';
            }
        }
        // Hashmap of compatible cells for each path
        private Dictionary<string, List<uint>> compatibleCells = new Dictionary<string, List<uint>>() 
        {
            { "left", new List<uint>() },
            { "right", new List<uint>() },
            { "top", new List<uint>() },
            { "bottom", new List<uint>() }
        };
        // Mapping of cell id to cell object
        private Dictionary<uint, Cell> cellMappings = new Dictionary<uint, Cell>();
        private List<uint> entranceCells = new List<uint>();
        uint cellId = 0;
        private Vector2 mapDimensions = new Vector2(10, 10);
        private Vector2 cellSize = new Vector2(30, 20);

		public override void OnInspectorGUI() {
            serializedObject.Update();
            GUI.changed = false;
			var cellList = serializedObject.FindProperty("cellList");
            // var cellListLeft = serializedObject.FindProperty("cellLeftEntranceList");
			EditorGUILayout.PropertyField(cellList, new GUIContent("List of Cells"), true);
            mapDimensions = EditorGUILayout.Vector2Field("Map Dimensions", mapDimensions);
            cellSize = EditorGUILayout.Vector2Field("Cell Size", cellSize);

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Generate Map")) {
                GenerateMap();
            }
		}

        private void GenerateMap() {
            Reset();
            Debug.Log(serializedObject.FindProperty("cellList").arraySize);

            Debug.Log("Generating map...");
            // Print the game object of the cell
            SerializedProperty serializedProperty = serializedObject.FindProperty("cellList");
            // Loop through the list of cells
            for (int i = 0; i < serializedProperty.arraySize; i++) {
                GameObject cell = serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue as GameObject;
                // Get the SuperCustomProperties component
                SuperCustomProperties superCustomProperties = cell.GetComponent<SuperCustomProperties>();
                // Get the custom property "paths_allowed"
                CustomProperty cellPathProperty;
                if (superCustomProperties.TryGetCustomProperty("entrance", out cellPathProperty)) {
                    // Parse the value of the custom property (format is $,$,$,$ where each $ is a boolean - top, bottom, left, right)
                    UpdateEntranceList(cellPathProperty.m_Value, cell, cellId);
                }
                if (superCustomProperties.TryGetCustomProperty("paths_allowed", out cellPathProperty)) {
                    // Parse the value of the custom property (format is $,$,$,$ where each $ is a boolean - top, bottom, left, right)
                    UpdatePathsHashMap(cellPathProperty.m_Value, cell, cellId);
                }
                cellId += 1;
            }
            // Print the hashmaps generated
            foreach (KeyValuePair<string, List<uint>> entry in compatibleCells) {
                Debug.Log(entry.Key + ": " + string.Join(",", entry.Value));
            }
            // print the name of the key and the value of the key
            foreach (KeyValuePair<uint, Cell> entry in cellMappings) {
                Debug.Log(entry.Key + ": " + entry.Value.cellGameObject.name);
            }
            // print the entrance cells
            Debug.Log("Entrance cells: " + string.Join(",", entranceCells) + ". Name: " + cellMappings[entranceCells[0]].cellGameObject.name);

            // Generate the map
            GenerateMapFromHashMap();
        }

        private void UpdateEntranceList(string m_Value, GameObject cell, uint cellId)
        {
            if (m_Value == "true") {
                entranceCells.Add(cellId);
            }
        }

        private void GenerateMapFromHashMap()
        {
            bool HasBeenPlaced(Vector2 currPosition, string path, HashSet<Vector2> placedCells) {
                switch (path) {
                    case "top":
                        return placedCells.Contains(new Vector2(currPosition.x, currPosition.y + cellSize.y));
                    case "bottom":
                        return placedCells.Contains(new Vector2(currPosition.x, currPosition.y - cellSize.y));
                    case "left":
                        return placedCells.Contains(new Vector2(currPosition.x - cellSize.x, currPosition.y));
                    case "right":
                        return placedCells.Contains(new Vector2(currPosition.x + cellSize.x, currPosition.y));
                }
                return false;
            }
            // hashset of cell positions that have been placed
            HashSet<Vector2> placedCells = new HashSet<Vector2>();
            System.Random rnd = new System.Random();
            Vector2 currMapSize = new Vector2(1, 1); // This includes the entrance cell
            Vector2 currPos = new Vector2(0, 0);
            Vector2 maxPos = new Vector2(0, 0);
            Vector2 minPos = new Vector2(0, 0);

            // Generate the map
            // 1. Pick a random cell from the list of cells
            uint entranceCellId = entranceCells[rnd.Next(entranceCells.Count)];
            Cell entranceCell = cellMappings[entranceCellId];
            PlaceCell(entranceCell, new Vector2(0, 0));
            placedCells.Add(new Vector2(0, 0));

            string [] paths = new string[] { "top", "bottom", "left", "right" };
            int x = 0;
            while (currMapSize.x < mapDimensions.x && currMapSize.y < mapDimensions.y) {
                // 2. Pick a random path from the list of paths
                string randomPath = paths[rnd.Next(paths.Length)];
                // Check if this path has been visited if so then regenerate another path
                while (HasBeenPlaced(currPos, randomPath, placedCells)) {
                    randomPath = paths[rnd.Next(paths.Length)];
                }
                // 3. Pick a random cell from the list of compatible cells for the path
                List<uint> compatibleCellsList = compatibleCells[randomPath];
                uint randomCellId = compatibleCellsList[rnd.Next(compatibleCellsList.Count)];
                Cell randomCell = cellMappings[randomCellId];
                // 4. Place the cell
                switch (randomPath) {
                    case "top":
                        currPos.y += 1;
                        break;
                    case "bottom":
                        currPos.y -= 1;
                        break;
                    case "left":
                        currPos.x -= 1;
                        break;
                    case "right":
                        currPos.x += 1;
                        break;
                }
                if (!placedCells.Contains(currPos)) {
                    PlaceCell(randomCell, currPos);
                    placedCells.Add(currPos);
                    maxPos = new Vector2(Math.Max(maxPos.x, currPos.x), Math.Max(maxPos.y, currPos.y));
                    minPos = new Vector2(Math.Min(minPos.x, currPos.x), Math.Min(minPos.y, currPos.y));
                    currMapSize = new Vector2(maxPos.x - minPos.x, maxPos.y - minPos.y);
                }
                // 5. If no compatible cells are found, restart the process (NO)
                if (x > 100) {
                    Debug.Log("Break");
                    break;
                }
                x += 1;
            }
            // 6. If the map is generated, return the map (NO)

            // 7. If the map is not generated, return null (NO)
        }

        private void UpdatePathsHashMap(string pathsAllowedString, GameObject cellGameObject, uint cellId) {
            for (int i = 0; i < pathsAllowedString.Length; i++) {
                if (pathsAllowedString[i] == 't' || pathsAllowedString[i] == '1') {
                    switch (i) {
                        case 0:
                            compatibleCells["top"].Add(cellId);
                            break;
                        case 1:
                            compatibleCells["bottom"].Add(cellId);
                            break;
                        case 2:
                            compatibleCells["left"].Add(cellId);
                            break;
                        case 3:
                            compatibleCells["right"].Add(cellId);
                            break;
                    }
                }
            }
            cellMappings[cellId] = new Cell(cellId, cellGameObject, pathsAllowedString);
        }

        private void Reset() {
            compatibleCells["top"].Clear();
            compatibleCells["bottom"].Clear();
            compatibleCells["left"].Clear();
            compatibleCells["right"].Clear();
            cellMappings.Clear();
            entranceCells.Clear();
            cellId = 0;
        }
	
        private void PlaceCell(Cell cell, Vector2 coordinatePosition) {
            // Center the cell since the pivot is at the top left
            coordinatePosition.x -= 1 / 2;
            coordinatePosition.y += 1 / 2;
            Vector2 position = new Vector2(coordinatePosition.x * cellSize.x, coordinatePosition.y * cellSize.y);
            Instantiate(cell.cellGameObject, position, Quaternion.identity);
        }

    }
}
