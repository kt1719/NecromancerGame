using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using SuperTiled2Unity;

// ASSUMES ALL THE CELLS ARE THE SAME SIZE!!!!!!!!!!!!!
namespace GameTools {
    public class MapGen : EditorWindow {
        Editor editor;
        
        [SerializeField] List<GameObject> cellList = new List<GameObject>();

        [MenuItem("NecromancerGame/Map/MapGen")]
        private static void ShowWindow() {
            GetWindow<MapGen>("MapGen");
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

	[CustomEditor(typeof(MapGen), true)]
	public class MapGenHelper : Editor {
        // Mapping of cell id to cell object
        [Serializable] struct KeyValuePair {
            public string key;
            public GameObject value;
        }
        MapMetaData mapMetaData;
        [SerializeField] List<KeyValuePair> cellKeyValuePairList = new List<KeyValuePair>(); // Serializable representation of the dictionary
        private Dictionary<string, GameObject> cellMappings = new Dictionary<string, GameObject>();

        private uint mapId = 0;
        private Vector2 cellSize = new Vector2(30, 20);

		public override void OnInspectorGUI() {
            serializedObject.Update();
            GUI.changed = false;
            mapId = (uint)EditorGUILayout.DelayedIntField("Map Id", (int)mapId);
            cellSize = EditorGUILayout.Vector2Field("Cell Size", cellSize);
            mapMetaData = (MapMetaData)EditorGUILayout.ObjectField("Map Meta Data", mapMetaData, typeof(MapMetaData), false);
            var cellList = serializedObject.FindProperty("cellList");
            EditorGUILayout.PropertyField(cellList, new GUIContent("List of Cells"), true);
            

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Generate Map")) {
                GenerateMap(cellList, cellMappings, cellKeyValuePairList, mapMetaData ,mapId++);
            }

            if (GUILayout.Button("Reset")) {
                Reset();
            }
		}

        private void GenerateMap(
            SerializedProperty cellList, 
            Dictionary<string, GameObject> cellMappings, 
            List<KeyValuePair> cellKeyValuePairList, 
            MapMetaData mapMetaData,
            uint mapId)
        {
            // Reset();

            Debug.Log("Generating hashmap...");
            GenerateHashMap(cellList, cellMappings, cellKeyValuePairList);

            Debug.Log("Generating map...");
            GenerateMapGameObject(mapId, mapMetaData, cellMappings, cellSize);
        }

        private void GenerateMapGameObject(uint mapId, MapMetaData mapMetaData, Dictionary<string, GameObject> cellMappings, Vector2 cellSize)
        {
            string GenerateCellKey(MazeCell mazeCell)
            {
                // If false append "f" to the key, if true append "t"
                string key = "";
                key += mazeCell.top ? "t" : "f";
                key += mazeCell.right ? "t" : "f";
                key += mazeCell.bottom ? "t" : "f";
                key += mazeCell.left ? "t" : "f";
                return key;
            }

            GameObject map = new GameObject("Map" + mapId);
            MazeCell[,] maze = mapMetaData.ReturnMaze();
            Vector2 mazeDim = mapMetaData.ReturnDimensions();
            for (int i = 0; i < mazeDim.x; i++)
            {
                for (int j = 0; j < mazeDim.y; j++)
                {
                    string key = GenerateCellKey(maze[i, j]);
                    GameObject cell = cellMappings[key];
                    GameObject cellInstance = Instantiate(cell, new Vector3(i * cellSize.x, j * cellSize.y, 0), Quaternion.identity);
                    cellInstance.transform.parent = map.transform;
                }
            }
        }

        private void GenerateHashMap(
            SerializedProperty cellList, 
            Dictionary<string, GameObject> cellMappings, 
            List<KeyValuePair> cellKeyValuePairList)
        {
            for (int i = 0; i < cellList.arraySize; i++) {
                GameObject cell = cellList.GetArrayElementAtIndex(i).objectReferenceValue as GameObject;
                SuperCustomProperties superCustomProperties = cell.GetComponent<SuperCustomProperties>();
                // Get the custom property "paths_allowed"
                CustomProperty cellPathProperty;
                if (superCustomProperties.TryGetCustomProperty("paths_allowed", out cellPathProperty))
                {
                    string pathsAllowed = cellPathProperty.GetValueAsString();
                    cellMappings[pathsAllowed] = cell;
                    cellKeyValuePairList.Add(new KeyValuePair { key = pathsAllowed, value = cell });
                }
                else {
                    Debug.LogError("Cell " + cell.name + " does not have a custom property 'paths_allowed'");
                }
            }

            // foreach (var key in cellMappings.Keys) {
            //     Debug.Log("Key: " + key + " Value: " + cellMappings[key]);
            // }
        }

        private void Reset() {
            cellMappings.Clear();
            cellKeyValuePairList.Clear();
            mapId = 0;
        }
    }
}
