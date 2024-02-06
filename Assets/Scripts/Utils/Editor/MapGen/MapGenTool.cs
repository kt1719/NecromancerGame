using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GameTools {
    public class MapGenTool : EditorWindow {
        private Vector2 mapDimensions = new Vector2(10, 10);
        Editor editor;
        [SerializeField] List<GameObject> cellUpEntranceList = new List<GameObject>();
        [SerializeField] List<GameObject> cellLeftEntranceList = new List<GameObject>();

        [MenuItem("NecromancerGame/Map/MapGenTool")]
        private static void ShowWindow() {
            GetWindow<MapGenTool>("MapGenTool");
        }

        private void OnGUI() {
            GUILayout.Label("Map Generation Tool", EditorStyles.boldLabel);
            mapDimensions = EditorGUILayout.Vector2Field("Map Dimensions", mapDimensions);

            if (!editor) { editor = Editor.CreateEditor(this); }
			if (editor) { editor.OnInspectorGUI(); }
        }
        
        void OnInspectorUpdate() {
            Repaint(); 
        }
    }

	[CustomEditor(typeof(MapGenTool), true)]
	public class MapGenHelper : Editor {
		public override void OnInspectorGUI() {
            // GUI.changed = false;
			var cellListUp = serializedObject.FindProperty("cellUpEntranceList");
            var cellListLeft = serializedObject.FindProperty("cellLeftEntranceList");
			EditorGUILayout.PropertyField(cellListUp, new GUIContent("Cell List With Entrance Starting Up"), true);
            EditorGUILayout.PropertyField(cellListLeft, new GUIContent("Cell List With Entrance Starting Left"), true);

            // if (GUI.changed) {
            //     Debug.Log("Changed");
            // }

            if (GUILayout.Button("Generate Map")) {
                GenerateMap();
            }
		}

        private void GenerateMap() {
            // TODO - Implement map generation
            Debug.Log(serializedObject.FindProperty("cellUpEntranceList").arraySize);
        }

	}
}
