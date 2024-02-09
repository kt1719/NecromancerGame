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
            public Dictionary<string, bool> pathsAllowed = new Dictionary<string, bool>();

            public Cell(uint cellId, GameObject cellGameObject, string pathsAllowedString) {
                this.cellId = cellId;
                this.cellGameObject = cellGameObject;
                this.pathsAllowed = new Dictionary<string, bool>() {
                    { "left", pathsAllowedString[0] == '1' },
                    { "right", pathsAllowedString[1] == '1' },
                    { "top", pathsAllowedString[2] == '1' },
                    { "bottom", pathsAllowedString[3] == '1' }
                };
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
        private uint cellId = 0;
        private uint mapId = 0;
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
            mapId = (uint)EditorGUILayout.DelayedIntField("Map Id", (int)mapId);

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Generate Map")) {
                GenerateMap();
            }
		}

        private void GenerateMap()
        {
            Reset();

            Debug.Log("Generating hashmap...");
            GenerateHashMap();

            Debug.Log("Generating map...");
            GenerateMapFromHashMap();
        }

        private void GenerateHashMap()
        {
            // Print the game object of the cell
            SerializedProperty serializedProperty = serializedObject.FindProperty("cellList");
            // Loop through the list of cells
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                GameObject cell = serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue as GameObject;
                // Get the SuperCustomProperties component
                SuperCustomProperties superCustomProperties = cell.GetComponent<SuperCustomProperties>();
                // Get the custom property "paths_allowed"
                CustomProperty cellPathProperty;
                if (superCustomProperties.TryGetCustomProperty("entrance", out cellPathProperty))
                {
                    // Parse the value of the custom property (format is $,$,$,$ where each $ is a boolean - top, bottom, left, right)
                    UpdateEntranceList(cellPathProperty.m_Value, cell, cellId);
                }
                if (superCustomProperties.TryGetCustomProperty("paths_allowed", out cellPathProperty))
                {
                    // Parse the value of the custom property (format is $,$,$,$ where each $ is a boolean - top, bottom, left, right)
                    UpdatePathsHashMap(cellPathProperty.m_Value, cell, cellId);
                }
                cellId += 1;
            }
            // Print the hashmaps generated
            foreach (KeyValuePair<string, List<uint>> entry in compatibleCells)
            {
                Debug.Log(entry.Key + ": " + string.Join(",", entry.Value));
            }
            // print the name of the key and the value of the key
            foreach (KeyValuePair<uint, Cell> entry in cellMappings)
            {
                Debug.Log(entry.Key + ": " + entry.Value.cellGameObject.name);
            }
            // print the entrance cells
            Debug.Log("Entrance cells: " + string.Join(",", entranceCells) + ". Name: " + cellMappings[entranceCells[0]].cellGameObject.name);
        }

        private void UpdateEntranceList(string m_Value, GameObject cell, uint cellId)
        {
            if (m_Value == "true") {
                entranceCells.Add(cellId);
            }
        }

        private void GenerateMapFromHashMap()
        {
            // hashset of cell positions that have been placed
            HashSet<Vector2> placedCells = new HashSet<Vector2>();
            System.Random rnd = new System.Random();
            Vector2 currMapSize = new Vector2(1, 1); // This includes the entrance cell
            Vector2 coordinatePosition = new Vector2(0, 0);
            Vector2 maxPos = new Vector2(0, 0);
            Vector2 minPos = new Vector2(0, 0);

            // Create a game object to hold the map
            GameObject map = new GameObject("Map_" + mapId);
            // Generate the map
            // 1. Pick a random cell from the list of cells
            uint entranceCellId = entranceCells[rnd.Next(entranceCells.Count)];
            Cell entranceCell = cellMappings[entranceCellId];
            Cell currCell = entranceCell;
            PlaceCell(entranceCell, new Vector2(0, 0), map);
            placedCells.Add(new Vector2(0, 0));

            string [] paths = new string[] { "top", "bottom", "left", "right" };
            while (currMapSize.x < mapDimensions.x && currMapSize.y < mapDimensions.y)
            {
                string randomPath = GenerateRandomStringPath(placedCells, rnd, coordinatePosition, currCell, paths);
                // 3. Pick a random cell from the list of compatible cells for the path
                Cell randomCell = GetCompatibleCell(rnd, randomPath);
                // 4. Place the cell
                coordinatePosition = PlaceCell(randomCell, coordinatePosition, map, randomPath);
                placedCells.Add(coordinatePosition);
                currMapSize = CalcualteNewMapSize(coordinatePosition, ref maxPos, ref minPos);
            }
            mapId++;

            /////////// HELPER FUNCTIONS ///////////
            
            Vector2 PlaceCell(Cell cell, Vector2 coordinatePosition, GameObject parent, string pathString = "") {
                switch (pathString)
                {
                    case "top":
                        coordinatePosition.y += 1;
                        break;
                    case "bottom":
                        coordinatePosition.y -= 1;
                        break;
                    case "left":
                        coordinatePosition.x -= 1;
                        break;
                    case "right":
                        coordinatePosition.x += 1;
                        break;
                    // If it's '' then it's the entrance cell
                }

                // Center the cell since the pivot is at the top left
                Vector2 adjustedPos = new Vector2(coordinatePosition.x - (1/2), coordinatePosition.y + (1/2));
                Vector2 position = new Vector2(adjustedPos.x * cellSize.x, adjustedPos.y * cellSize.y);
                GameObject cellInstantiation = Instantiate(cell.cellGameObject, position, Quaternion.identity);
                cellInstantiation.transform.parent = parent ? parent.transform : GameObject.Find("Map").transform;
                return coordinatePosition;
            }

            bool HasBeenPlaced(Vector2 coordinatePosition, string path, HashSet<Vector2> placedCells) {
                switch (path) {
                    case "top":
                        return placedCells.Contains(new Vector2(coordinatePosition.x, coordinatePosition.y + 1));
                    case "bottom":
                        return placedCells.Contains(new Vector2(coordinatePosition.x, coordinatePosition.y - 1));
                    case "left":
                        return placedCells.Contains(new Vector2(coordinatePosition.x - 1, coordinatePosition.y));
                    case "right":
                        return placedCells.Contains(new Vector2(coordinatePosition.x + 1, coordinatePosition.y));
                }
                return false;
            }

            Cell GetCompatibleCell(System.Random rnd, string path)
            {
                // We want to get the opposite path since we are looking for the next cell e.g., the if the exit is on the top, we want the entrance to be on the bottom
                uint randomCellId = 0;
                switch (path)
                {
                    case "top":
                        randomCellId = compatibleCells["bottom"][rnd.Next(compatibleCells["bottom"].Count)];
                        break;
                    case "bottom":
                        randomCellId = compatibleCells["top"][rnd.Next(compatibleCells["top"].Count)];
                        break;
                    case "left":
                        randomCellId = compatibleCells["right"][rnd.Next(compatibleCells["right"].Count)];
                        break;
                    case "right":
                        randomCellId = compatibleCells["left"][rnd.Next(compatibleCells["left"].Count)];
                        break;
                }
                return cellMappings[randomCellId];
            }

            string GenerateRandomStringPath(HashSet<Vector2> placedCells, System.Random rnd, Vector2 currPos, Cell currCell, string[] paths)
            {
                // 2. Pick a random path from the list of paths
                string randomPath = paths[rnd.Next(paths.Length)];
                // Check if this path has been visited if so then regenerate another path
                for (int i = 0; i < paths.Length; i++)
                {
                    if (!HasBeenPlaced(currPos, randomPath, placedCells) && currCell.pathsAllowed[randomPath])
                    {
                        randomPath = paths[rnd.Next(paths.Length)];
                        break;
                    }
                }

                return randomPath;
            }

            Vector2 CalcualteNewMapSize(Vector2 currPos, ref Vector2 maxPos, ref Vector2 minPos)
            {
                Vector2 currMapSize;
                maxPos = new Vector2(Math.Max(maxPos.x, currPos.x), Math.Max(maxPos.y, currPos.y));
                minPos = new Vector2(Math.Min(minPos.x, currPos.x), Math.Min(minPos.y, currPos.y));
                currMapSize = new Vector2(maxPos.x - minPos.x, maxPos.y - minPos.y);
                return currMapSize;
            }
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
    }
}
