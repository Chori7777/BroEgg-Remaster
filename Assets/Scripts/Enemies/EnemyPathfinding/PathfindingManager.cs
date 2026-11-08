using ED262C;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PathfindingManager : MonoBehaviour
{
    [SerializeField] private Tilemap obstacleTilemap;

    //Contiene el cada tile y sus vecinos
    private Dictionary<GraphNode<Vector2Int>, List<Edge<GraphNode<Vector2Int>>>> adjacencyList;

    //cada tile del tilem
    private GraphNode<Vector2Int>[] vertices;


}