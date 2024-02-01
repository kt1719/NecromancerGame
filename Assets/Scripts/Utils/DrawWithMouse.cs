using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawWithMouse : MonoBehaviour
{
    private LineRenderer line;
    private Vector3 previousPosition;
    public float minDistance = 0.1f;
    public int minPoints = 3;
    public Texture2D defaultMousePointer;
    [SerializeField]
    public static List<UnitCore> selectedPlayers;
    public static GameObject targetObject;
    private void Awake() {
        line = GetComponent<LineRenderer>();
        previousPosition = transform.position;
        selectedPlayers = new List<UnitCore>();
        Cursor.SetCursor(defaultMousePointer, Vector2.zero, CursorMode.Auto);
    }

    private void Update() {
        if (Input.GetMouseButton(1))
        {
            DeselectPlayers();
            DrawSelector();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            SelectRegion();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (targetObject != null)
            {
                // Will need to check in the future for the type of object
                CommandUnitsAttack(targetObject.GetComponent<EnemyCore>());
            }
            else{
                CommandMove();
            }
        }
        else {
            ResetSelector();
        }
    }

    private void CommandMove()
    {
        // Move the selected players to the mouse position
        foreach (UnitCore player in selectedPlayers)
        {
            player.SetTargetPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
    public static void CommandUnitsAttack(EnemyCore enemyCore)
    {
        // Loop through all selected players
        foreach (UnitCore player in selectedPlayers)
        {
            player.CommandUnitsAttack(enemyCore);
        }
    }
    private void DeselectPlayers()
    {
        // Deselct all players
        foreach (UnitCore player in selectedPlayers)
        {
            player.Deselect();
            player.GetComponent<SpriteRenderer>().color = Color.white;
        }
        // Clear the selected players array
        selectedPlayers.Clear();
    }

    private void ResetSelector()
    {
        // Connect the end of the line to the start of the line
        line.positionCount++;
        line.SetPosition(line.positionCount - 1, line.GetPosition(0));


        // reset the previous position when the mouse is released
        previousPosition = transform.position;
        line.positionCount = 0;
    }

    private void DrawSelector()
    {
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPosition.z = 0;

        // Check if the mouse has moved enough for a new point
        if (Vector3.Distance(currentPosition, previousPosition) > minDistance)
        {
            // Add a new point to the line
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, currentPosition);
            previousPosition = currentPosition;
        }
    }

    private void SelectRegion()
    {
        PolygonCollider2D CreatePolyCollider()
        {
            // Create a polygon collider with the line points if there are enough points
            if (line.positionCount < minPoints) return null;
            PolygonCollider2D polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
            // Set the collider to a trigger so it doesn't collide with other objects
            polygonCollider.isTrigger = true;
            // Get a vector2d array of the line points
            Vector2[] linePoints = new Vector2[line.positionCount];
            for (int i = 0; i < line.positionCount; i++)
            {
                linePoints[i] = line.GetPosition(i).ConvertTo<Vector2>();
            }
            // Apply the Monotone Chain Algorithm to the line points so that we have a convex shape
            linePoints = Algorithms.GetConvexHull(linePoints.ConvertTo<List<Vector2>>());
            // Set the collider points to the line points
            polygonCollider.points = linePoints;

            // return the polygon collider created
            return polygonCollider;
        }

        PolygonCollider2D poly = CreatePolyCollider();
        if (poly == null) return;
        SelectObjects(poly);
        // Destroy the polygon collider
        Destroy(poly);
    }

    private void SelectObjects(PolygonCollider2D poly)
    {
        // Loop through all objects with the UnitCore script
        foreach (UnitCore player in FindObjectsOfType<UnitCore>())
        {
            // Check if the player is inside the selector
            if (poly.OverlapPoint(player.transform.position))
            {
                // Select the player
                player.Select();
                // Draw an outline around the player
                player.GetComponent<SpriteRenderer>().color = Color.red;
                // Store the player in the selected players array
                selectedPlayers.Add(player);
            }
            else if (player.selected)
            {
                // Deselect the player
                player.Deselect();
            }
        }
    }
}
