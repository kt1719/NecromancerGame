// Example code
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameTools {
    public class ListTestEditor : EditorWindow {

		[MenuItem(itemName: "TestEditorList", menuItem = "NecromancerGame/Test/TestList")]
		public static void Init() { GetWindow<ListTestEditor>("Haha", true); }

		Editor editor;

		[SerializeField] List<MyClass> ListTest = new List<MyClass>();
        [SerializeField] List<GameObject> GameObjectTest = new List<GameObject>();

		void OnGUI() {
			if (!editor) { editor = Editor.CreateEditor(this); }
			if (editor) { editor.OnInspectorGUI(); }
		}

		void OnInspectorUpdate() { Repaint(); }
	}

	[System.Serializable]
	public class MyClass {
		public List<int> myList;
		public string myString;
		public int myInt;
	}

	[CustomEditor(typeof(ListTestEditor), true)]
	public class ListTestEditorDrawer : Editor {

		public override void OnInspectorGUI() {
			var list = serializedObject.FindProperty("ListTest");
			EditorGUILayout.PropertyField(list, new GUIContent("My List Test"), true);
		}
	}
}
