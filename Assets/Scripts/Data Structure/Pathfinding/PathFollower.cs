using ED262C;
using UnityEngine;

// Sigue un camino A* hacia el jugador. Es una clase comun (no un MonoBehaviour): cada enemigo suicida tiene la suya.
//
// COMO FUNCIONA:
//  - Cada "repathInterval" segundos le pide a PathfindingManager un camino nuevo (lista de nodos hasta el jugador).
//  - Mientras tanto va de nodo en nodo: se mueve hacia el centro del siguiente y, cuando llega, pasa al que sigue.
//  - Al llegar al ultimo nodo, va directo al jugador.
//  - Si no hay camino (o el manager no existe), va en linea recta.
public class PathFollower
{
    private readonly float repathInterval;
    private readonly float reachDistance;

    private ISimpleList<GraphNode<Vector2Int>> path;
    private int nextIndex;
    private float timer;

    public ISimpleList<GraphNode<Vector2Int>> CurrentPath => path; //para dibujarlo con Gizmos

    public PathFollower(float repathInterval = 0.5f, float reachDistance = 0.4f)
    {
        this.repathInterval = repathInterval;
        this.reachDistance = reachDistance;

        //el timer arranca en un valor al azar: si aparecen 10 enemigos juntos no calculan todos en el mismo frame
        timer = Random.Range(0f, repathInterval);
    }

    public Vector2 GetDirection(Vector3 from, Vector3 to)
    {
        Vector2 straight = ((Vector2)(to - from)).normalized; //sin pathfinding: derecho al jugador

        PathfindingManager manager = PathfindingManager.Instance;
        if (manager == null) return straight;

        //cada cierto tiempo pedimos un camino nuevo (el jugador se mueve y el camino viejo se desactualiza)
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = repathInterval;
            path = manager.FindPath(from, to);
            nextIndex = 1; //el nodo 0 es donde el enemigo ya esta
        }

        //sin camino, o con un solo nodo (ya estamos en el nodo del jugador): derecho al jugador
        if (path == null || path.Count < 2) return straight;

        //si llegamos al nodo al que ibamos, pasamos al siguiente
        while (nextIndex < path.Count && Vector2.Distance(from, manager.NodeToWorld(path[nextIndex])) < reachDistance)
        {
            nextIndex++;
        }

        //ya pasamos el ultimo nodo: derecho al jugador
        if (nextIndex >= path.Count) return straight;

        return ((Vector2)(manager.NodeToWorld(path[nextIndex]) - from)).normalized;
    }
}
