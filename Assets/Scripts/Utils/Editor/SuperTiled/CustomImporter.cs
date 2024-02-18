using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
public class CustomImporter : CustomTmxImporter
{
    private static BindingFlags accessFlagsPrivate = BindingFlags.NonPublic | BindingFlags.Instance;
    private static FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", accessFlagsPrivate);
    public override void TmxAssetImported(TmxAssetImportedArgs args)
    {
        // Just log the name of the map
        var map = args.ImportedSuperMap;
        Debug.LogFormat("Map '{0}' has been imported.", map.name);
        SuperLayer[] layers = map.GetComponentsInChildren<SuperLayer>();
        // Loop through the GameObjects layer and log their names
        foreach (var layer in layers)
        {
            Debug.LogFormat("Wall layer '{0}' has been imported.", layer.m_TiledName);
            GameObject gameObject = layer.gameObject;
            if (layer.m_TiledName == "GameObjects")
            {
                AddShadowToGameObjects(layer);
            }
            if (layer.m_TiledName == "wall_layer")
            {
                AddShadowToWalls(layer);
            }
        }
    }

    private void AddShadowToWalls(SuperLayer layer)
    {
        // Add shadow to the walls
        GameObject gameObject = layer.gameObject;
        ShadowCaster2D shadowCaster2D = gameObject.AddComponent<ShadowCaster2D>();
        shadowCaster2D.selfShadows = true;
        CompositeShadowCaster2D compositeShadowCaster2D = gameObject.AddComponent<CompositeShadowCaster2D>();

        // Get the collider in the layer below
        if (gameObject.transform.childCount > 0) {
            GameObject child = gameObject.transform.GetChild(0).gameObject;
            if (child.GetComponent<PolygonCollider2D>()) {
                PolygonCollider2D polygonCollider2D = child.GetComponent<PolygonCollider2D>();
                Vector2[] points = polygonCollider2D.points;
                Vector3[] points3D = new Vector3[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    points3D[i] = new Vector3(points[i].x, points[i].y, 0);
                }
                shapePathField.SetValue(shadowCaster2D, points3D);
            }
        }
    }

    private static void AddShadowToGameObjects(SuperLayer layer)
    {
        // Check for super custom properties on the GameObject
        SuperCustomProperties[] superCustomProperties = layer.GetComponentsInChildren<SuperCustomProperties>();
        foreach (var superCustomProperty in superCustomProperties)
        {
            Debug.Log("superCustomProperties: " + superCustomProperty.name);
            CustomProperty customProperty;
            if (superCustomProperty.TryGetCustomProperty("shadow", out customProperty))
            {
                bool shadow = customProperty.GetValueAsBool();
                if (shadow)
                {
                    Debug.LogFormat("Shadow: {0} ", shadow);
                    Debug.LogFormat("Shadow: {0}", superCustomProperty.gameObject.name);
                    // Loop until we find the deepest child
                    GameObject child = superCustomProperty.gameObject;
                    while (!child.GetComponent<SpriteRenderer>())
                    {
                        child = child.transform.GetChild(0).gameObject;
                    }
                    ShadowCaster2D shadowCaster = child.AddComponent<ShadowCaster2D>();
                    CompositeShadowCaster2D compositeShadowCaster2D = child.AddComponent<CompositeShadowCaster2D>();

                    // Get the collider in the layer below 
                    child = child.transform.GetChild(0).gameObject;
                    if (child.GetComponent<BoxCollider2D>())
                    {
                        // We need to update m_ShapePath to be the same as the points in the box
                        BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D>();
                        Vector3[] points = new Vector3[4];
                        float x = boxCollider2D.size.x;
                        float y = boxCollider2D.size.y;
                        float startingX = child.transform.localPosition.x;
                        float startingY = child.transform.localPosition.y;
                        points[0] = new Vector3(startingX, startingY, 0);
                        points[1] = new Vector3(x + startingX, startingY, 0);
                        points[2] = new Vector3(x + startingX, -y + startingY, 0);
                        points[3] = new Vector3(startingX, -y + startingY, 0);
                        shapePathField.SetValue(shadowCaster, points);
                    }
                    if (child.GetComponent<TilemapCollider2D>())
                    {
                    }
                    if (child.GetComponent<PolygonCollider2D>())
                    {
                    }
                }
            }
        }
    }
}

// Use DisplayNameAttribute to control how class appears in the list
// [DisplayName("My Other Importer")]
// public class MyOtherTmxImporter : CustomTmxImporter
// {
//     public override void TmxAssetImported(TmxAssetImportedArgs args)
//     {
//         // Just log the number of layers in our tiled map
//         var map = args.ImportedSuperMap;
//         SuperLayer[] layers = map.GetComponentsInChildren<SuperLayer>();
//         Debug.LogFormat("Map '{0}' has {1} layers.", map.name, layers.Length);
//         Debug.Log("Map layer names:  " + string.Join(", ", layers[0].m_TiledName));
//     }
// }

// [AutoCustomTmxImporter()]
// public class MyOrderedTmxImporter : CustomTmxImporter
// {
//     public override void TmxAssetImported(TmxAssetImportedArgs args)
//     {
//         Debug.Log("MyOrderedTmxImporter importer");
//     }
// }

// [AutoCustomTmxImporter(1)]
// public class MyOrderedTmxImporter1 : CustomTmxImporter
// {
//     public override void TmxAssetImported(TmxAssetImportedArgs args)
//     {
//         Debug.Log("MyOrderedTmxImporter1 importer");
//     }
// }

// [AutoCustomTmxImporter(2)]
// public class MyThrowingCustomImporter : CustomTmxImporter
// {
//     public override void TmxAssetImported(TmxAssetImportedArgs args)
//     {
//         throw new CustomImporterException("This is my custom importer exception message.");
//     }
// }