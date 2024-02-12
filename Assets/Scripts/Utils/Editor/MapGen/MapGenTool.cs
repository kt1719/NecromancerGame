using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SuperTiled2Unity;
using System;
using System.Linq;
using SuperTiled2Unity.Editor.ClipperLib;
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
                    { "top", pathsAllowedString[0] == 't' },
                    { "right", pathsAllowedString[1] == 't' },
                    { "bottom", pathsAllowedString[2] == 't' },
                    { "left", pathsAllowedString[3] == 't' }
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
        private Dictionary<uint, int> maxNumberOfCells = new Dictionary<uint, int>(); // Dictionary for the maximum quantity we can have for each cell (-1 means no limit)
        private Dictionary<string, List<uint>> edgeCells = new Dictionary<string, List<uint>>()
        {
            { "left", new List<uint>() },
            { "right", new List<uint>() },
            { "top", new List<uint>() },
            { "bottom", new List<uint>() }
        };
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

        private void GenerateMapFromHashMap()
        {
            // hashset of cell positions that have been placed
            Dictionary<Vector2, List<string>> placedCells = new Dictionary<Vector2, List<string>>();
            // queue of coordinate and direction to go
            Queue<(Vector2, string)> queue = new Queue<(Vector2, string)>();
            System.Random rnd = new System.Random();
            Vector2 currMapSize = new Vector2(1, 1); // This includes the entrance cell
            Vector2 coordinatePosition = new Vector2(0, 0);
            // Create a game object to hold the map
            GameObject map = new GameObject("Map_" + mapId);
            bool forceRightPath = true;
            bool forceUpPath = true;

            // Generate the map
            // 1. Pick a random cell from the list of cells
            uint entranceCellId = entranceCells[rnd.Next(entranceCells.Count)];
            Cell entranceCell = cellMappings[entranceCellId];
            PlaceCell(entranceCell, coordinatePosition, map);

            while (queue.Count > 0)
            {
                // pop the queue
                string path;
                (coordinatePosition, path) = queue.Dequeue();
                Cell randomCell = GetRandomCell(placedCells, rnd, coordinatePosition, path, forceRightPath, forceUpPath);
                // 4. Place the cell
                PlaceCell(randomCell, coordinatePosition, map);
                currMapSize = CalculateNewMapSize(coordinatePosition, currMapSize);
                if (forceRightPath) {
                    forceRightPath = !(currMapSize.x == mapDimensions.x);
                }
                if (forceUpPath) {
                    forceUpPath = !(currMapSize.y == mapDimensions.y);
                }
            }
            mapId++;

            /////////// HELPER FUNCTIONS ///////////
            
            void PlaceCell(Cell cell, Vector2 coordinatePosition, GameObject parent) {
                // loop through the open paths the cell has and add the new cells to the visited cells
                foreach (KeyValuePair<string, bool> entry in cell.pathsAllowed)
                {
                    if (entry.Value)
                    {
                        if (!placedCells.ContainsKey(coordinatePosition)) {
                            placedCells[coordinatePosition] = new List<string>();
                        }
                        placedCells[coordinatePosition].Add(entry.Key);
                    }
                }
                Vector2 adjustedPos = new Vector2(coordinatePosition.x - (1/2f), coordinatePosition.y + (1/2f));
                Vector2 position = new Vector2(adjustedPos.x * cellSize.x, adjustedPos.y * cellSize.y);
                GameObject cellInstantiation = Instantiate(cell.cellGameObject, position, Quaternion.identity);
                cellInstantiation.transform.parent = parent ? parent.transform : GameObject.Find("Map").transform;

                // loop through the open paths the cell has and add the new cells to the queue
                foreach (KeyValuePair<string, bool> entry in cell.pathsAllowed)
                {
                    if (entry.Value && !placedCells.ContainsKey(UpdateCoordPosition(coordinatePosition, entry.Key)))
                    {
                        Vector2 nextPos = UpdateCoordPosition(coordinatePosition, entry.Key);
                        queue.Enqueue((nextPos, entry.Key));
                    }
                }
            }

            Cell GetRandomCell(Dictionary<Vector2, List<string>> placedCells, System.Random rnd, Vector2 currPos, string path, bool forceRightPath, bool forceUpPath)
            {
                // Case 1 path is left and pos x is 0
                // Case 2 path is right and pos x is max
                // Case 3 path is top and pos y is max
                // Case 4 path is bottom and pos y is 0
                // Case 5 path is else
                // Case 1-4 are all edge cells
                // Case 5 is a normal compatible cell

                bool isEdgeCell = (path == "left" && currPos.x == 0)
                    || (path == "right" && currPos.x == mapDimensions.x-1)
                    || (path == "top" && currPos.y == mapDimensions.y-1)
                    || (path == "bottom" && currPos.y == 0);

                Dictionary<string, List<uint>> cellDictionary;
                if (isEdgeCell) {
                    cellDictionary = edgeCells;
                }
                else {
                    cellDictionary = compatibleCells;
                }

                if (cellDictionary.Count == 0) {
                    throw new Exception("No compatible cell found");
                }

                return GetCompatibleCell(currPos, isEdgeCell, rnd, path, placedCells, cellDictionary);
            }

            Cell GetCompatibleCell(Vector2 currPos, bool isEdgeCell, System.Random rnd, string path, Dictionary<Vector2, List<string>> placedCells, Dictionary<string, List<uint>> cellDictionary)
            {   
                // We want to look at adjacent cells and see if they also have open paths
                List<List<uint>> compatibleCellsList = new List<List<uint>>() {};
                // If they do, we want to pick a cell that has a path that is also compatible with the adjacent cell
                List<Vector2> adjacentCells = new List<Vector2>() {
                    new Vector2(currPos.x, currPos.y + 1),
                    new Vector2(currPos.x, currPos.y - 1),
                    new Vector2(currPos.x - 1, currPos.y),
                    new Vector2(currPos.x + 1, currPos.y)
                };
                for (int i = 0; i < adjacentCells.Count; i++) {
                    if (placedCells.ContainsKey(adjacentCells[i])) {
                        if (i == 0 && placedCells[adjacentCells[i]].Contains("bottom")) {
                            compatibleCellsList.Add(cellDictionary["top"]);
                        }
                        else if (i == 1 && placedCells[adjacentCells[i]].Contains("top")) {
                            compatibleCellsList.Add(cellDictionary["bottom"]);
                        }
                        else if (i == 2 && placedCells[adjacentCells[i]].Contains("right")) {
                            compatibleCellsList.Add(cellDictionary["left"]);
                        }
                        else if (i == 3 && placedCells[adjacentCells[i]].Contains("left")) {
                            compatibleCellsList.Add(cellDictionary["right"]);
                        }
                    }
                }
                // We want to get the opposite path since we are looking for the next cell e.g., the if the exit is on the top, we want the entrance to be on the bottom
                uint randomCellId;
                string opposite_path = "";
                switch (path)
                {
                    case "top":
                        opposite_path = "bottom";
                        break;
                    case "bottom":
                        opposite_path = "top";
                        break;
                    case "left":
                        opposite_path = "right";
                        break;
                    case "right":
                        opposite_path = "left";
                        break;
                }
                List<uint> oppositePathCells = cellDictionary[opposite_path];
                List<uint> compatibleOpposingCells = new List<uint>();
                // Loop through each of the cells and see if they are compatible with the adjacent cells
                for (int i = 0; i < oppositePathCells.Count; i++) {
                    // If we are on the edge we want to make sure that the cell we are picking does not have an open path on the edge
                    // If force right path is true, we want to make sure that the cell that has a path to the right
                    Cell currCell = cellMappings[oppositePathCells[i]];
                    if (currPos.x == 0 && currCell.pathsAllowed["left"]
                        || currPos.y == 0 && currCell.pathsAllowed["bottom"]
                        || currPos.x == mapDimensions.x && currCell.pathsAllowed["right"]
                        || currPos.y == mapDimensions.y && currCell.pathsAllowed["top"]
                        || (forceRightPath && !currCell.pathsAllowed["right"]
                        && forceUpPath && !currCell.pathsAllowed["top"] && !isEdgeCell)
                        )
                    {
                        continue;
                    }
                    compatibleOpposingCells.Add(oppositePathCells[i]);
                }

                compatibleCellsList.Add(compatibleOpposingCells);

                // We want to get the intersection of all the compatible cells
                List<uint> intersection = new List<uint>();
                if (compatibleCellsList.Count > 0) {
                    intersection = compatibleCellsList[0];
                    for (int i = 1; i < compatibleCellsList.Count; i++) {
                        intersection = new List<uint>(new HashSet<uint>(intersection).Intersect(new HashSet<uint>(compatibleCellsList[i])));
                    }
                }

                Debug.Log(intersection.Count);
                if (intersection.Count == 0) {
                    Debug.Log(path);
                    Debug.Log(currPos);
                }
                randomCellId = intersection[rnd.Next(intersection.Count)];
                if (maxNumberOfCells[randomCellId] != -1 && maxNumberOfCells[randomCellId] <= 0) {
                    throw new Exception("Max number of cells reached");
                }
                else if (maxNumberOfCells[randomCellId] != -1) {
                    maxNumberOfCells[randomCellId] -= 1;
                    if (maxNumberOfCells[randomCellId] == 0) {
                        // loop through the keys of the cell dictionary and remove the cell from the list
                        foreach (string key in cellDictionary.Keys) {
                            cellDictionary[key].Remove(randomCellId);
                        }
                    }
                }

                return cellMappings[randomCellId];
            }

            Vector2 UpdateCoordPosition(Vector2 pos, string path) {
                switch (path)
                {
                    case "top":
                        return new Vector2(pos.x, pos.y + 1);
                    case "bottom":
                        return new Vector2(pos.x, pos.y - 1);
                    case "left":
                        return new Vector2(pos.x - 1, pos.y);
                    case "right":
                        return new Vector2(pos.x + 1, pos.y);
                    default:
                        throw new Exception("Invalid path");
                }

            }
            
            Vector2 CalculateNewMapSize(Vector2 currPos, Vector2 prevMapSize)
            {
                return new Vector2(Math.Max(currPos.x, prevMapSize.x), Math.Max(currPos.y, prevMapSize.y));
            }
        }

        /// <summary>
        /// Helper functions for modifying the hashmaps
        /// </summary>
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

                superCustomProperties.TryGetCustomProperty("max_quantity", out cellPathProperty);
                maxNumberOfCells[cellId] =  (cellPathProperty == null || cellPathProperty.m_Value == null) ? -1 : int.Parse(cellPathProperty.m_Value);

                if (superCustomProperties.TryGetCustomProperty("paths_allowed", out cellPathProperty))
                {
                    CustomProperty edgeCellProperty;
                    bool edgeCell = false;
                    if (superCustomProperties.TryGetCustomProperty("edge", out edgeCellProperty))
                    {
                        edgeCell = edgeCellProperty.m_Value == "true";
                    }
                    // Parse the value of the custom property (format is $,$,$,$ where each $ is a boolean - top, bottom, left, right)
                    UpdatePathsHashMap(cellPathProperty.m_Value, cell, cellId, edgeCell);
                }
                cellId += 1;
            }
            // foreach (KeyValuePair<string, List<uint>> entry in compatibleCells)
            // {
            //     Debug.Log("Compable Cells is: " + entry.Key + ": " + string.Join(",", entry.Value));
            // }
            // foreach (KeyValuePair<uint, Cell> entry in cellMappings)
            // {
            //     Debug.Log("Cell mappings is: " +entry.Key + ": " + entry.Value.cellGameObject.name);
            // }
            // foreach (KeyValuePair<uint, int> entry in maxNumberOfCells)
            // {
            //     Debug.Log("Max num of cells is: " + entry.Key + ": " + entry.Value);
            // }
            // foreach (KeyValuePair<string, List<uint>> entry in edgeCells)
            // {
            //     Debug.Log("Edge cells is: " + entry.Key + ": " + string.Join(",", entry.Value));
            // }
            // foreach (uint entry in entranceCells)
            // {
            //     Debug.Log("Entrance cells is: " + entry);
            // }
        }
        
        private void UpdatePathsHashMap(string pathsAllowedString, GameObject cellGameObject, uint cellId, bool edgeCell = false) {
            var hashMap = compatibleCells;
            if (edgeCell) {
                hashMap = edgeCells;
            }
            for (int i = 0; i < pathsAllowedString.Length; i++) {
                if (pathsAllowedString[i] == 't' || pathsAllowedString[i] == '1') {
                    switch (i) {
                        case 0:
                            hashMap["top"].Add(cellId);
                            break;
                        case 1:
                            hashMap["right"].Add(cellId);
                            break;
                        case 2:
                            hashMap["bottom"].Add(cellId);
                            break;
                        case 3:
                            hashMap["left"].Add(cellId);
                            break;
                    }
                }
            }
            cellMappings[cellId] = new Cell(cellId, cellGameObject, pathsAllowedString);
        }

        private void UpdateEntranceList(string m_Value, GameObject cell, uint cellId)
        {
            if (m_Value == "true") {
                entranceCells.Add(cellId);
            }
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
