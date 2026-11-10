using ED262C;
using UnityEngine;

/*public class DijkstraSolver<T> : IPathFindingSolver<T>
{
    //funcion de Dijkstra que devuelve los caminos mas cercanos
    public ISimpleDictionary<GraphNode<T>, Edge<GraphNode<T>>> GetAllShortestPath(GraphNode<T> origin, IGraph<GraphNode<T>> graph)
    {

        //tabla con todos los nodos del grafo
        ISimpleDictionary<GraphNode<T>, Edge<GraphNode<T>>> result = new SimpleArrayDictionary<GraphNode<T>, Edge<GraphNode<T>>>();

        //PriorityQueue que guarda los nodos no seleccionados   
        SimpleArrayPriorityQueue<GraphNode<T>> unvisited = new SimpleArrayPriorityQueue<GraphNode<T>>();

        //Set que guarda todos los nodos ya visitados
        SimpleArraySet<GraphNode<T>> visited = new SimpleArraySet<GraphNode<T>>();

        //Array de vertices/nodos
        GraphNode<T>[] vertices = graph.vertices;

        //inicializamos la tabla con el primero teniendo prev = null y su peso es infinito
        for (int i = 0; i < vertices.Length; i++)
        {
            result.Add(vertices[i], new Edge<GraphNode<T>>(null, int.MaxValue)); //se le agrega al diccionario todos los vertices y una arista con(to == null y un peso maximo) 
        }

        //el primer nodo no tiene costo
        result[origin].weight = 0;

        //encolamos al primero con su peso 0
        unvisited.Enqueue(origin, result[origin].weight); //cuanto menos sea el peso, antes va a salir en Dequeue

        while (!unvisited.IsEmpty)
        {
            //sacamos el primero de la queue
            GraphNode<T> current = unvisited.Dequeue();

            //si ya lo visitamos, lo saltemaos
            if (visited.Contains(current)) continue; //si fuera hash seria O(1)

            //guardamos los vecinos en una lista para evaluar
            ISimpleList<Edge<GraphNode<T>>> neighbours = graph.GetNeighbours(current);

            for (int i = 0; i < neighbours.Count; i++)
            {
                //suamamos el costo total hasta el nodo actual
                int totalWeight = neighbours[i].weight + result[current].weight;

                //Si el costo total es menor al que esta en la tabla para llegar al vecino
                //actualizamos ese costo con el que calculamos (buscamos siempre el menor costo)
                if (totalWeight < result[neighbours[i].to].weight)
                    result[neighbours[i].to].weight = totalWeight;

                //Agregamos a la Queue el vecino si es que no fue visitado antes con el peso
                if (!visited.Contains(neighbours[i].to))
                    unvisited.Enqueue(neighbours[i].to, totalWeight);
            }

            //marcamos el nodo como visitado
            visited.Add(current);
        }

        return result;
    }

    //igual a la de arriba, pero devuelve una lista y corta si encuentra destination
    public ISimpleList<GraphNode<T>> GetShortestPath(GraphNode<T> origin, GraphNode<T> destination, IGraph<GraphNode<T>> graph)
    {
        //tabla con toda la info
        ISimpleDictionary<GraphNode<T>, Edge<GraphNode<T>>> table = new SimpleArrayDictionary<GraphNode<T>, Edge<GraphNode<T>>>();

        SimpleArrayPriorityQueue<GraphNode<T>> unvisited = new SimpleArrayPriorityQueue<GraphNode<T>>();
        SimpleArraySet<GraphNode<T>> visited = new SimpleArraySet<GraphNode<T>>();

        //inicializar valores principales
        GraphNode<T>[] vertices = graph.vertices;

        //inicializamos la tabla con el primero teniendo prev = null y su peso es infinito
        for (int i = 0; i < vertices.Length;)
        {
            table.Add(vertices[i], new Edge<GraphNode<T>>(null, int.MaxValue));
        }

        //el primer nodo no tiene costo
        table[origin].weight = 0;

        //encolamos al primero con su peso 0
        unvisited.Enqueue(origin, table[origin].weight);

        while (!unvisited.IsEmpty)
        {
            //sacamos el primero de la queue
            GraphNode<T> current = unvisited.Dequeue();

            //si justo es el que buscamos, terminamos el bucle
            if (current == destination)
            {
                break;
            }

            //si ya lo visitamos, lo saltemaos
            if (visited.Contains(current)) continue;

            //guardamos los vecinos en una lista para evaluar
            ISimpleList<Edge<GraphNode<T>>> neighbours = graph.GetNeighbours(current);

            for (int i = 0; i < neighbours.Count; i++)
            {
                //suamamos el costo total hasta el nodo actual
                int totalWeight = neighbours[i].weight + table[current].weight;

                //Si el costo total es menor al que esta en la tabla para...
                //actualizamos ese costo con el que calculamos (buscamos siempre el menor costo)
                if (totalWeight < table[neighbours[i].to].weight)
                    table[neighbours[i].to].weight = totalWeight;
            }

            //marcamos el nodo como visitado
            visited.Add(current);
        }

        //generamos un unico path a partir de la tabla
        return GeneratePath(destination, table);
    }

    ISimpleList<GraphNode<T>> GeneratePath(GraphNode<T> destination, ISimpleDictionary<GraphNode<T>, Edge<GraphNode<T>>> table)
    {
        ISimpleStack<GraphNode<T>> reversePath = new SimpleLinkedStack<GraphNode<T>>();
        reversePath.Push(destination);
        GraphNode<T> current = destination;

        //mientras haya un nodo para evaluar...
        while (current != null)
        {
            //lo agregamos al camino
            reversePath.Push(current);

            //pasamos al nodo anterior
            current = table[current].to;
        }

        //si el ultimo elemento no es alcanzable, no hay camino posible y devolvemos null
        if (table[destination].to == null) return null;

        //creamos el path definitivo
        ISimpleList<GraphNode<T>> path = new SimpleArrayList<GraphNode<T>>(reversePath.Count);

        //Invertimos la lista
        while (!reversePath.IsEmpty)
        {
            //el primer indice del path es el ultimo 
            path.Add(reversePath.Pop());
        }
        return path;
    }
}*/




public class DijkstraSolver<T> : IPathFindingSolver<T>
{
    //funcion de Dijkstra que devuelve los caminos mas cercanos desde "origin" hasta TODOS los nodos
    //
    //la tabla que devuelve tiene una entrada por nodo:
    //  weight = costo total para llegar desde origin hasta ese nodo (int.MaxValue = no se puede llegar)
    //  to     = el nodo ANTERIOR del camino (el vecino que esta un paso mas cerca de origin)
    public ISimpleDictionary<GraphNode<T>, Edge<GraphNode<T>>> GetAllShortestPath(GraphNode<T> origin, IGraph<GraphNode<T>> graph)
    {

        //tabla con todos los nodos del grafo
        ISimpleDictionary<GraphNode<T>, Edge<GraphNode<T>>> result = new SimpleArrayDictionary<GraphNode<T>, Edge<GraphNode<T>>>();

        //PriorityQueue que guarda los nodos no seleccionados   
        SimpleArrayPriorityQueue<GraphNode<T>> unvisited = new SimpleArrayPriorityQueue<GraphNode<T>>();

        //Set que guarda todos los nodos ya visitados
        SimpleArraySet<GraphNode<T>> visited = new SimpleArraySet<GraphNode<T>>();

        //Array de vertices/nodos
        GraphNode<T>[] vertices = graph.vertices;

        //inicializamos la tabla con el primero teniendo prev = null y su peso es infinito
        for (int i = 0; i < vertices.Length; i++)
        {
            result.Add(vertices[i], new Edge<GraphNode<T>>(null, int.MaxValue)); //se le agrega al diccionario todos los vertices y una arista con(to == null y un peso maximo) 
        }

        //CAMBIO: si el origen no esta en el grafo no hay nada que calcular (antes tiraba KeyNotFoundException)
        if (origin == null || !result.ContainsKey(origin)) return result;

        //el primer nodo no tiene costo
        result[origin].weight = 0;

        //encolamos al primero con su peso 0
        unvisited.Enqueue(origin, result[origin].weight); //cuanto menos sea el peso, antes va a salir en Dequeue

        while (!unvisited.IsEmpty)
        {
            //sacamos el primero de la queue
            GraphNode<T> current = unvisited.Dequeue();

            //si ya lo visitamos, lo saltemaos
            if (visited.Contains(current)) continue; //si fuera hash seria O(1)

            //CAMBIO: guardamos el costo del nodo actual una sola vez (antes se buscaba en el diccionario en cada vecino)
            int currentWeight = result[current].weight;

            //guardamos los vecinos en una lista para evaluar
            ISimpleList<Edge<GraphNode<T>>> neighbours = graph.GetNeighbours(current);

            for (int i = 0; i < neighbours.Count; i++)
            {
                GraphNode<T> neighbour = neighbours[i].to;

                //suamamos el costo total hasta el nodo actual
                int totalWeight = neighbours[i].weight + currentWeight;

                //CAMBIO: buscamos al vecino en la tabla una sola vez y trabajamos con esa entrada
                Edge<GraphNode<T>> neighbourData = result[neighbour];

                //Si el costo total es menor al que esta en la tabla para llegar al vecino
                //actualizamos ese costo con el que calculamos (buscamos siempre el menor costo)
                if (totalWeight < neighbourData.weight)
                {
                    neighbourData.weight = totalWeight;

                    //CAMBIO: antes faltaba esta linea. Guardamos de que nodo venimos (el "anterior"),
                    //sin esto la tabla tiene costos pero no se puede reconstruir ningun camino
                    neighbourData.to = current;

                    //CAMBIO: encolamos al vecino solo si mejoramos su costo (antes se encolaba siempre)
                    unvisited.Enqueue(neighbour, totalWeight);
                }
            }

            //marcamos el nodo como visitado
            visited.Add(current);
        }

        return result;
    }

    //igual a la de arriba, pero devuelve una lista y corta si encuentra destination
    public ISimpleList<GraphNode<T>> GetShortestPath(GraphNode<T> origin, GraphNode<T> destination, IGraph<GraphNode<T>> graph)
    {
        //tabla con toda la info
        ISimpleDictionary<GraphNode<T>, Edge<GraphNode<T>>> table = new SimpleArrayDictionary<GraphNode<T>, Edge<GraphNode<T>>>();

        SimpleArrayPriorityQueue<GraphNode<T>> unvisited = new SimpleArrayPriorityQueue<GraphNode<T>>();
        SimpleArraySet<GraphNode<T>> visited = new SimpleArraySet<GraphNode<T>>();

        //inicializar valores principales
        GraphNode<T>[] vertices = graph.vertices;

        //inicializamos la tabla con el primero teniendo prev = null y su peso es infinito
        //CAMBIO: faltaba el i++ en este for, asi que se quedaba en un bucle infinito
        for (int i = 0; i < vertices.Length; i++)
        {
            table.Add(vertices[i], new Edge<GraphNode<T>>(null, int.MaxValue));
        }

        //CAMBIO: si origen o destino no estan en el grafo, no hay camino
        if (origin == null || destination == null || !table.ContainsKey(origin) || !table.ContainsKey(destination)) return null;

        //el primer nodo no tiene costo
        table[origin].weight = 0;

        //encolamos al primero con su peso 0
        unvisited.Enqueue(origin, table[origin].weight);

        while (!unvisited.IsEmpty)
        {
            //sacamos el primero de la queue
            GraphNode<T> current = unvisited.Dequeue();

            //si ya lo visitamos, lo saltemaos
            if (visited.Contains(current)) continue;

            //si justo es el que buscamos, terminamos el bucle
            //(CAMBIO: ahora esta despues del chequeo de visitados; en este punto su costo ya es el definitivo)
            if (current == destination)
            {
                break;
            }

            int currentWeight = table[current].weight;

            //guardamos los vecinos en una lista para evaluar
            ISimpleList<Edge<GraphNode<T>>> neighbours = graph.GetNeighbours(current);

            for (int i = 0; i < neighbours.Count; i++)
            {
                GraphNode<T> neighbour = neighbours[i].to;

                //suamamos el costo total hasta el nodo actual
                int totalWeight = neighbours[i].weight + currentWeight;

                Edge<GraphNode<T>> neighbourData = table[neighbour];

                //Si el costo total es menor al que esta en la tabla para...
                //actualizamos ese costo con el que calculamos (buscamos siempre el menor costo)
                if (totalWeight < neighbourData.weight)
                {
                    neighbourData.weight = totalWeight;
                    neighbourData.to = current;                //CAMBIO: guardamos el nodo anterior
                    unvisited.Enqueue(neighbour, totalWeight); //CAMBIO: antes nunca se encolaban los vecinos
                }
            }

            //marcamos el nodo como visitado
            visited.Add(current);
        }

        //generamos un unico path a partir de la tabla
        return GeneratePath(destination, table);
    }

    ISimpleList<GraphNode<T>> GeneratePath(GraphNode<T> destination, ISimpleDictionary<GraphNode<T>, Edge<GraphNode<T>>> table)
    {
        //CAMBIO: esta comprobacion ahora va primero. Si el destino nunca recibio un costo, no hay camino posible y devolvemos null
        //(antes se miraba "to == null", pero el origen tambien tiene to == null)
        if (table[destination].weight == int.MaxValue) return null;

        ISimpleStack<GraphNode<T>> reversePath = new SimpleLinkedStack<GraphNode<T>>();
        GraphNode<T> current = destination;

        //mientras haya un nodo para evaluar...
        while (current != null)
        {
            //lo agregamos al camino
            reversePath.Push(current);

            //pasamos al nodo anterior
            current = table[current].to;
        }
        //CAMBIO: antes se hacia un Push(destination) extra antes del while y el destino quedaba repetido

        //creamos el path definitivo
        ISimpleList<GraphNode<T>> path = new SimpleArrayList<GraphNode<T>>(reversePath.Count);

        //Invertimos la lista
        while (!reversePath.IsEmpty)
        {
            //el primer indice del path es el ultimo 
            path.Add(reversePath.Pop());
        }
        return path;
    }
}
