using System.ComponentModel;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CustomImporter : CustomTmxImporter
{
    public override void TmxAssetImported(TmxAssetImportedArgs args)
    {
        // Just log the name of the map
        var map = args.ImportedSuperMap;
        Debug.LogFormat("Map '{0}' has been imported.", map.name);
        SuperLayer[] layers = map.GetComponentsInChildren<SuperLayer>();
        // Loop through the GameObjects layer and log their names
        foreach (var layer in layers)
        {
            if (layer.m_TiledName == "wall_layer") {
                Debug.LogFormat("Wall layer '{0}' has been imported.", layer.m_TiledName);
                layer.gameObject.AddComponent<TilemapCollider2D>();
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