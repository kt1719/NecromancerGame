using UnityEngine;
using UnityEditor;

namespace GameTools {
    public class BasicObjectSpawner : EditorWindow
    {
        string objectBaseName = "";
        int objectId = 1;
        GameObject objectToSpawn;
        float objectScale = 1;
        float spawnRadius = 5f;
        public Vector2 startingPosition = new Vector2(0, 0);

        [MenuItem("NecromancerGame/Test/Basic Object Spawner")]
        public static void ShowWindow()
        {
            GetWindow<BasicObjectSpawner>("Basic Object Spawner");
        }

        private void OnGUI()
        {
            GUILayout.Label("Spawn New Object", EditorStyles.boldLabel);
            objectBaseName = EditorGUILayout.TextField("Object Base Name", objectBaseName);
            objectId = EditorGUILayout.IntField("Object ID", objectId);
            objectToSpawn = (GameObject)EditorGUILayout.ObjectField("Object To Spawn", objectToSpawn, typeof(GameObject), false);
            objectScale = EditorGUILayout.FloatField("Object Scale", objectScale);
            spawnRadius = EditorGUILayout.FloatField("Spawn Radius", spawnRadius);
            startingPosition = EditorGUILayout.Vector2Field("Starting Position", startingPosition);

            if (GUILayout.Button("Spawn Object"))
            {
                SpawnObject();
            }
        }

        private void SpawnObject()
        {
            if (objectToSpawn == null)
            {
                Debug.LogError("Object to spawn is null!");
                return;
            }
            if (objectBaseName == string.Empty)
            {
                Debug.LogError("Object base name is empty!");
                return;
            }

            Vector2 spawnCircle = Random.insideUnitCircle * spawnRadius;
            Vector2 spawnPosition = new Vector2(startingPosition.x + spawnCircle.x, startingPosition.y + spawnCircle.y);
            GameObject newObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
            newObject.name = objectBaseName + objectId;
            newObject.transform.localScale = new Vector3(objectScale, objectScale, newObject.transform.localScale.z);

            objectId++;
        }
    }
}