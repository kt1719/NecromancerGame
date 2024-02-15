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
        // Mapping of cell id to cell object
        private Dictionary<uint, Cell> cellMappings = new Dictionary<uint, Cell>();
        private Dictionary<string, List<uint>> edgeCells = new Dictionary<string, List<uint>>()
        {
            { "left", new List<uint>() },
            { "right", new List<uint>() },
            { "top", new List<uint>() },
            { "bottom", new List<uint>() }
        };
        private uint mapId = 0;
        private Vector2 mapDimensions = new Vector2(10, 10);
        private Vector2 cellSize = new Vector2(30, 20);
        // private int seed = 0;

		public override void OnInspectorGUI() {
            serializedObject.Update();
            GUI.changed = false;
			var cellList = serializedObject.FindProperty("cellList");
            // var cellListLeft = serializedObject.FindProperty("cellLeftEntranceList");
			EditorGUILayout.PropertyField(cellList, new GUIContent("List of Cells"), true);
            mapDimensions = EditorGUILayout.Vector2Field("Map Dimensions", mapDimensions);
            cellSize = EditorGUILayout.Vector2Field("Cell Size", cellSize);
            mapId = (uint)EditorGUILayout.DelayedIntField("Map Id", (int)mapId);
            // seed = (int)EditorGUILayout.DelayedIntField("Seed", seed);

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Generate Map")) {
                GenerateMap();
            }
		}

        private void GenerateMap()
        {
            // Reset();

            Debug.Log("Generating hashmap...");
            // GenerateHashMap();

            Debug.Log("Generating map...");
            // GenerateMapFromHashMap();
        }

    //     private void GenerateHashMap()
    //     {
    //         // Print the game object of the cell
    //         SerializedProperty serializedProperty = serializedObject.FindProperty("cellList");
    //         // Loop through the list of cells
    //         for (int i = 0; i < serializedProperty.arraySize; i++)
    //         {
    //             GameObject cell = serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue as GameObject;
    //             // Get the SuperCustomProperties component
    //             SuperCustomProperties superCustomProperties = cell.GetComponent<SuperCustomProperties>();
    //             // Get the custom property "paths_allowed"
    //             CustomProperty cellPathProperty;
    //             if (superCustomProperties.TryGetCustomProperty("entrance", out cellPathProperty))
    //             {
    //                 // Parse the value of the custom property (format is $,$,$,$ where each $ is a boolean - top, bottom, left, right)
    //                 UpdateEntranceList(cellPathProperty.m_Value, cell, cellId);
    //             }

    //             superCustomProperties.TryGetCustomProperty("max_quantity", out cellPathProperty);
    //             maxNumberOfCells[cellId] =  (cellPathProperty == null || cellPathProperty.m_Value == null) ? -1 : int.Parse(cellPathProperty.m_Value);

    //             if (superCustomProperties.TryGetCustomProperty("paths_allowed", out cellPathProperty))
    //             {
    //                 CustomProperty edgeCellProperty;
    //                 bool edgeCell = false;
    //                 if (superCustomProperties.TryGetCustomProperty("edge", out edgeCellProperty))
    //                 {
    //                     edgeCell = edgeCellProperty.m_Value == "true";
    //                 }
    //                 // Parse the value of the custom property (format is $,$,$,$ where each $ is a boolean - top, bottom, left, right)
    //                 UpdatePathsHashMap(cellPathProperty.m_Value, cell, cellId, edgeCell);
    //             }
    //             cellId += 1;
    //         }
    //         // foreach (KeyValuePair<string, List<uint>> entry in compatibleCells)
    //         // {
    //         //     Debug.Log("Compable Cells is: " + entry.Key + ": " + string.Join(",", entry.Value));
    //         // }
    //         // foreach (KeyValuePair<uint, Cell> entry in cellMappings)
    //         // {
    //         //     Debug.Log("Cell mappings is: " +entry.Key + ": " + entry.Value.cellGameObject.name);
    //         // }
    //         // foreach (KeyValuePair<uint, int> entry in maxNumberOfCells)
    //         // {
    //         //     Debug.Log("Max num of cells is: " + entry.Key + ": " + entry.Value);
    //         // }
    //         // foreach (KeyValuePair<string, List<uint>> entry in edgeCells)
    //         // {
    //         //     Debug.Log("Edge cells is: " + entry.Key + ": " + string.Join(",", entry.Value));
    //         // }
    //         // foreach (uint entry in entranceCells)
    //         // {
    //         //     Debug.Log("Entrance cells is: " + entry);
    //         // }
    //     }
        
    //     private void UpdatePathsHashMap(string pathsAllowedString, GameObject cellGameObject, uint cellId, bool edgeCell = false)
    //     {
    //         var hashMap = compatibleCells;
    //         if (edgeCell)
    //         {
    //             hashMap = edgeCells;
    //         }
    //         UpdateHashMap(pathsAllowedString, cellId, hashMap);
    //         cellMappings[cellId] = new Cell(cellId, cellGameObject, pathsAllowedString);

    //         static void UpdateHashMap(string pathsAllowedString, uint cellId, Dictionary<string, List<uint>> hashMap)
    //         {
    //             for (int i = 0; i < pathsAllowedString.Length; i++)
    //             {
    //                 if (pathsAllowedString[i] == 't' || pathsAllowedString[i] == '1')
    //                 {
    //                     switch (i)
    //                     {
    //                         case 0:
    //                             hashMap["top"].Add(cellId);
    //                             break;
    //                         case 1:
    //                             hashMap["right"].Add(cellId);
    //                             break;
    //                         case 2:
    //                             hashMap["bottom"].Add(cellId);
    //                             break;
    //                         case 3:
    //                             hashMap["left"].Add(cellId);
    //                             break;
    //                     }
    //                 }
    //             }
    //         }
    //     }

    //     private void UpdateEntranceList(string m_Value, GameObject cell, uint cellId)
    //     {
    //         if (m_Value == "true") {
    //             entranceCells.Add(cellId);
    //         }
    //     }

    //     private void Reset() {
    //         compatibleCells["top"].Clear();
    //         compatibleCells["bottom"].Clear();
    //         compatibleCells["left"].Clear();
    //         compatibleCells["right"].Clear();
    //         cellMappings.Clear();
    //         entranceCells.Clear();
    //         cellId = 0;
    //     }
    }
}
