using System;
using ED262C;
using UnityEngine;

// A*: igual que Dijkstra, pero en vez de explorar en todas las direcciones, prioriza los nodos que parecen
// estar mas cerca del destino. Para eso usa una "heuristica": una funcion que estima cuanto falta para llegar.
//
//   prioridad de un nodo = g + h
//     g = lo que ya costo llegar hasta ese nodo (lo mismo que el "weight" de Dijkstra)
//     h = estimacion de lo que falta hasta el destino
//
// REGLA: la heuristica nunca puede estimar de MAS de lo que falta realmente. Si cumple eso (y ademas es "consistente",
// como la distancia octil en una grilla), A* devuelve el camino mas corto, igual que Dijkstra, pero explorando muchos menos nodos.
public class AStarSolver<T> : IPathFindingSolver<T>
{
    private readonly Func<GraphNode<T>, GraphNode<T>, int> heuristic; // (nodo, destino) -> estimacion de lo que falta
    private readonly int maxExpandedNodes;                           // tope de nodos a explorar

    public AStarSolver(Func<GraphNode<T>, GraphNode<T>, int> heuristic, int maxExpandedNodes = int.MaxValue)
    {
        this.heuristic = heuristic;
        this.maxExpandedNodes = maxExpandedNodes;
    }

    //A* busca el camino hacia UN solo destino, asi que no puede calcular "todos los caminos" como Dijkstra.
    //Para eso hay que usar DijkstraSolver.
    public ISimpleDictionary<GraphNode<T>, Edge<GraphNode<T>>> GetAllShortestPath(GraphNode<T> origin, IGraph<GraphNode<T>> graph)
    {
        //DijkstraSolver para todos los caminos
        return null;
    }

    //devuelve la lista de nodos desde origin hasta destination (los dos incluidos), o null si no hay camino
    public ISimpleList<GraphNode<T>> GetShortestPath(GraphNode<T> origin, GraphNode<T> destination, IGraph<GraphNode<T>> graph)
    {
        //si origen o destino no estan en el grafo, no hay camino
        if (origin == null || destination == null || !graph.ContainsVertex(origin) || !graph.ContainsVertex(destination)) return null;

        //tabla SOLO con los nodos que vamos descubriendo. A diferencia de Dijkstra, no cargamos todo el grafo.
        //weight = g (costo real desde el origen), to = nodo anterior del camino
        ISimpleDictionary<GraphNode<T>, Edge<GraphNode<T>>> table = new SimpleArrayDictionary<GraphNode<T>, Edge<GraphNode<T>>>();

        //PriorityQueue de nodos descubiertos pero todavia no explorados. Sale primero el de menor g + h
        SimpleArrayPriorityQueue<GraphNode<T>> unvisited = new SimpleArrayPriorityQueue<GraphNode<T>>();

        //Set de nodos ya explorados
        SimpleArraySet<GraphNode<T>> visited = new SimpleArraySet<GraphNode<T>>();

        //el origen no tiene costo
        table.Add(origin, new Edge<GraphNode<T>>(null, 0));
        unvisited.Enqueue(origin, heuristic(origin, destination));

        int expanded = 0;

        while (!unvisited.IsEmpty)
        {
            //sacamos el nodo con menor g + h
            GraphNode<T> current = unvisited.Dequeue();

            //si ya lo exploramos, lo saltamos
            if (visited.Contains(current)) continue;

            //si es el destino, listo: armamos el camino con los "anteriores"
            if (current == destination) return GeneratePath(destination, table);

            //si exploramos demasiados nodos sin llegar, abandonamos
            expanded++;
            if (expanded > maxExpandedNodes) return null;

            int currentCost = table[current].weight;

            ISimpleList<Edge<GraphNode<T>>> neighbours = graph.GetNeighbours(current);

            for (int i = 0; i < neighbours.Count; i++)
            {
                GraphNode<T> neighbour = neighbours[i].to;

                //los ya explorados no se tocan (con una heuristica consistente su costo ya es el mejor)
                if (visited.Contains(neighbour)) continue;

                int newCost = currentCost + neighbours[i].weight;

                //buscamos al vecino en la tabla, si es la primera vez que lo vemos, lo agregamos con costo infinito
                Edge<GraphNode<T>> neighbourData;
                if (!table.TryGetValue(neighbour, out neighbourData))
                {
                    neighbourData = new Edge<GraphNode<T>>(null, int.MaxValue);
                    table.Add(neighbour, neighbourData);
                }

                //si encontramos una forma mas barata de llegar al vecino, la guardamos
                if (newCost < neighbourData.weight)
                {
                    neighbourData.weight = newCost;
                    neighbourData.to = current;

                    //ahora la prioridad es el costo real + la estimacion de lo que falta
                    unvisited.Enqueue(neighbour, newCost + heuristic(neighbour, destination));
                }
            }

            visited.Add(current);
        }

        //se acabaron los nodos y nunca llegamos al destino
        return null;
    }

    ISimpleList<GraphNode<T>> GeneratePath(GraphNode<T> destination, ISimpleDictionary<GraphNode<T>, Edge<GraphNode<T>>> table)
    {
        ISimpleStack<GraphNode<T>> reversePath = new SimpleLinkedStack<GraphNode<T>>();
        GraphNode<T> current = destination;

        //vamos del destino hacia atras, siguiendo el "anterior" de cada nodo, hasta llegar al origen (su anterior es null)
        while (current != null)
        {
            reversePath.Push(current);
            current = table[current].to;
        }

        //invertimos: el primer nodo del path es el origen y el ultimo es el destino
        ISimpleList<GraphNode<T>> path = new SimpleArrayList<GraphNode<T>>(reversePath.Count);
        while (!reversePath.IsEmpty)
        {
            path.Add(reversePath.Pop());
        }
        return path;
    }
}
