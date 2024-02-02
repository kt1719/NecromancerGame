// Script to define constant mappings between tower types and their respective prefabs
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UIElements;

// Define a static dictionary to map tower names to their respective prefabs 
public class TowerManager
{
    public static Dictionary<string, GameObject> towerMappings;
    public static Dictionary<string, bool> towerUnlockMappings;

    // Hashset of all the tiles that have a tower on them (Each tile is 16x16 and towers are 48x48 so we need to check 4 tiles)
    // Top left, top right, bottom left, bottom right
    public static HashSet<Vector2> tilesWithTowers = new HashSet<Vector2>();

    public static void InstantiateTowerMapping() {
        towerMappings = new Dictionary<string, GameObject>
        {
            { "Tower", Resources.Load<GameObject>("Prefabs/Towers/Tower") },
            { "Summoning_Tower", Resources.Load<GameObject>("Prefabs/Towers/SummoningTower") },
        };

        towerUnlockMappings = new Dictionary<string, bool>
        {
            { "Tower", false },
            { "Summoning_Tower", true },
        };
    }

    public static void AddTowerPosition(Vector2 centerPositon) {
        // Add the 9 tiles to the hashset
        for (int i = -1; i < 2; i++) {
            for (int j = -1; j < 2; j++) {
                tilesWithTowers.Add(new Vector2(centerPositon.x + i, centerPositon.y + j));
            }
        }
    }

    public static bool TileHasTower(Vector2 centerPosition) {
        // Check the 4 corner tiles to see if they have a tower
        Vector2[] tiles = new Vector2[] {
            new Vector2(centerPosition.x - 1, centerPosition.y - 1),
            new Vector2(centerPosition.x + 1, centerPosition.y - 1),
            new Vector2(centerPosition.x - 1, centerPosition.y + 1),
            new Vector2(centerPosition.x + 1, centerPosition.y + 1)
        };

        foreach (Vector2 tile in tiles) {
            if (tilesWithTowers.Contains(tile)) {
                return true;
            }
        }

        return false;
    }
}