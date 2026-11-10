using ED262C;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.EventSystems.EventTrigger;

// Singleton que le dice a los enemigos hacia donde caminar para esquivar obstaculos.
//
// COMO FUNCIONA (4 pasos):
//  1) Lee los Tilemaps y arma un mapa de celdas: cada celda es "caminable" o "bloqueada".
//  2) Agrupa las celdas en bloques de blockSize x blockSize. Cada bloque totalmente libre es un
//     GraphNode, y los bloques vecinos se conectan con aristas en un ListGraph.
//  3) Cuando el jugador cambia de bloque, DijkstraSolver calcula los caminos desde el nodo del jugador.
//     Devuelve una tabla: para cada nodo, "cuanto cuesta llegar al jugador" y "cual es el nodo anterior
//     del camino" (que es justo el siguiente paso para un enemigo que viene desde ese nodo).
//  4) Cada enemigo pregunta GetDirection(): si no hay nada en el medio va en linea recta,
//     y si hay un obstaculo camina hacia el centro del siguiente nodo del camino.
public class PathfindingManager : MonoBehaviour
{
    //Singleton (igual que LevelManager)
    private static PathfindingManager instance;
    public static PathfindingManager Instance => instance;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorTilemap;      // PapuMap: el piso (donde se puede caminar)
    [SerializeField] private Tilemap obstacleTilemap;   // Objects: paredes y obstaculos

    [Header("Ajustes")]
    [Tooltip("Celdas por lado de cada nodo. Con 2 hay unos 4 veces menos nodos que con 1, y Dijkstra es mucho mas rapido")]
    [SerializeField] private int blockSize = 1;
    [Tooltip("Cada cuantos segundos como maximo se vuelve a correr Dijkstra")]
    [SerializeField] private float recalculateInterval = 0.25f;
    [Tooltip("Mitad del ancho del enemigo: sirve para saber si puede ir en linea recta sin rozar una esquina")]
    [SerializeField] private float bodyRadius = 0.8f;
    [SerializeField] private bool drawGizmos = true;

    //el mapa celda por celda
    private bool[] walkableCells;   // si hay un tile sin nada en la pos 5, la pos [4] = true (esto es un mero ejemplo), sino es [4] = false
    private int cellsWide;
    private int cellsHigh;
    private int originX;            // celda del Tilemap que corresponde a la esquina superior izquierda (0, 0) del mapa, si no entienden esto los mato
    private int originY;

    // el grafo (un nodo por bloque libre)
    private int blocksWide;
    private int blocksHigh;
    private GraphNode<Vector2Int>[] nodes;
    private readonly IGraph<GraphNode<Vector2Int>> graph = new ListGraph<GraphNode<Vector2Int>>(); // acuerdense que el grafo es el mapeado de nodos conectados entre si y poderados
    private readonly IPathFindingSolver<Vector2Int> solver = new DijkstraSolver<Vector2Int>(); //esto es lo que va a operar sobre el grafo para darle la capacidad de a partide un nodo (pos del jugador) los enemigos puedan calcular el mejor camino

    private ISimpleDictionary<GraphNode<Vector2Int>, Edge<GraphNode<Vector2Int>>> table; // resultado de Dijkstra
    private GraphNode<Vector2Int> playerNode;   // nodo desde el que se calculo la tabla

    private Transform playerTransform;
    private float timer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        ReadTilemaps();
        BuildGraph();
    }

    //PASO 1: leemos los Tilemaps. Una celda es caminable si tiene piso y NO tiene obstaculo.
    private void ReadTilemaps()
    {
        BoundsInt bounds = floorTilemap.cellBounds; //representa las bounds de una celda en AABB
        cellsWide = bounds.size.x;
        cellsHigh = bounds.size.y;
        originX = bounds.xMin;
        originY = bounds.yMin;

        walkableCells = new bool[cellsWide * cellsHigh];

        for (int y = 0; y < cellsHigh; y++)
        {
            for (int x = 0; x < cellsWide; x++)
            {
                Vector3Int cell = new Vector3Int(originX + x, originY + y, 0);
                bool hasFloor = floorTilemap.HasTile(cell);
                bool hasObstacle = obstacleTilemap.HasTile(cell);
                walkableCells[y * cellsWide + x] = hasFloor && !hasObstacle; //walkableCell es true si el tile es un floor y no tiene obstaculo
                //COSAS A ACLARAR
                //hacer walkableCells[y * cellsWide + x] es lo mismo que hacer walkableCells[cellsHigh ,cellsWide] si fuera bidimensional, pero de esta forma es mas optimo, mas prolijo de escribir y mas seguro de manejar
            }
        }
    }

    //PASO 2: armamos el grafo. Se hace una sola vez porque los obstaculos no se mueven.
    private void BuildGraph()
    {
        //esto lo dejo asi, pero basicamente lo que hace esto es que la variable blockSize agrupe tiles para que la busqueda de caminos sea menos pasada para el juego
        //eso puede generar problemas pero ahorra memoria, para que funcione bien bien, yo pongo la variable en el inspector = 1, por eso es que esta dividion es inutil
        //pero de momento la voy a dejar por las dudas, OK?
        blocksWide = cellsWide / blockSize;
        blocksHigh = cellsHigh / blockSize;

        nodes = new GraphNode<Vector2Int>[blocksWide * blocksHigh]; //creo X cantidad de espacios dentro del array, por X cantidad de tiles dentro del array

        //2a) un nodo por cada bloque libre. El valor del nodo es la posicion del bloque.
        for (int by = 0; by < blocksHigh; by++)
        {
            for (int bx = 0; bx < blocksWide; bx++)
            {
                if (!IsBlockFree(bx, by)) continue; //si el tile esta libre

                GraphNode<Vector2Int> node = new GraphNode<Vector2Int>(new Vector2Int(bx, by)); //creamos un nodo ahi mismito
                nodes[bx + by * blocksWide] = node;
                graph.AddVertex(node); //añado
            }
        }

        //2b) aristas: cada nodo se conecta con sus 8 vecinos. Costo 10 en recto y 14 en diagonal (1 y 1.4 por 10).
        //Se usan enteros porque el peso de las aristas y de la PriorityQueue es int.
        for (int by = 0; by < blocksHigh; by++)
        {
            for (int bx = 0; bx < blocksWide; bx++)
            {
                GraphNode<Vector2Int> node = GetNode(bx, by); //GetNode(bx, by) me devuelve una posicion del array, ponele nodes[67]
                if (node == null) continue;

                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++) //este doble for me sirve para esto (preparense)
                    {
                        if (dx == 0 && dy == 0) continue;

                        GraphNode<Vector2Int> neighbour = GetNode(bx + dx, by + dy);

                    //    [  ↖  ][  ↑  ][  ↗  ]----------------- (dx = -1, dy = 1)  (dx = 0, dy = 1)  (dx = 1, dy = 1)

                    //    [  ←  ][CENTRO][  →  ]---------------- (dx = -1, dy = 0)  (GetNode(bx, by)  (dx = 1, dy = 0)

                    //    [  ↙  ][  ↓  ][  ↘  ]----------------- (dx = -1, dy = -1) (dx = 0, dy = -1) (dx = 1, dy = -1)

                        if (neighbour == null) continue;

                        //en diagonal, los dos bloques de al lado tambien tienen que estar libres.
                        //Si no, el enemigo estaria cortando la esquina de un obstaculo.
                        bool diagonal = dx != 0 && dy != 0; 
                        if (diagonal && (GetNode(bx + dx, by) == null || GetNode(bx, by + dy) == null)) continue;

                        int weight;
                        if (diagonal == true) weight = 14; //pesa mas ir en diagonal que ir derecho
                        else weight = 10;

                        graph.AddEdge(node, neighbour, weight);
                    }
                }
            }
        }
    }

    //un bloque esta libre si TODAS sus celdas son caminables
    private bool IsBlockFree(int bx, int by)
    {
        for (int dy = 0; dy < blockSize; dy++)
        {
            for (int dx = 0; dx < blockSize; dx++)
            {
                int cellX = bx * blockSize + dx;
                int cellY = by * blockSize + dy;
                if (!walkableCells[cellY * cellsWide + cellX]) return false;
            }
        }
        return true;
    }

    //PASO 3: recalculamos los caminos cuando el jugador cambia de bloque
    private void Update()
    {
        if (nodes == null) return;

        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null) return;
            playerTransform = playerObject.transform;
        }

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        // Incluso si el jugador sigue en el mismo bloque, no necesitamos comprobarlo
        // todos los frames. El intervalo limita solamente el coste de esa comprobación;
        // Dijkstra solo se ejecuta cuando cambia el nodo origen.
        timer = Mathf.Max(0f, recalculateInterval);

        //el nodo del jugador (si esta pisando un bloque bloqueado, usamos el nodo libre mas cercano)
        GraphNode<Vector2Int> origin = FindNodeNear(WorldToBlock(playerTransform.position), false);

        //si no hay nodo, o el jugador sigue en el mismo bloque, la tabla que ya tenemos sigue sirviendo
        if (origin == null || origin == playerNode) return;

        playerNode = origin;
        table = solver.GetAllShortestPath(playerNode, graph);
    }

    //PASO 4: direccion (ya normalizada) para ir de "from" hasta "to" esquivando obstaculos
    public Vector2 GetDirection(Vector3 from, Vector3 to)
    {
        Vector2 straight = ((Vector2)(to - from)).normalized;

        //todavia no hay tabla: vamos en linea recta
        if (table == null) return straight;

        //si no hay nada en el medio, vamos en linea recta (se ve mas natural)
        if (HasClearPath(from, to)) return straight;

        Vector2Int block = WorldToBlock(from);
        GraphNode<Vector2Int> current = GetNode(block.x, block.y);

        //el enemigo esta en un bloque sin nodo (pegado a un obstaculo): va al nodo vecino mas barato
        if (current == null)
        {
            GraphNode<Vector2Int> near = FindNodeNear(block, true);
            if (near == null) return straight;
            return DirectionTo(from, BlockToWorld(near.value));
        }

        //buscamos la entrada del nodo en la tabla de Dijkstra
        if (!table.TryGetValue(current, out Edge<GraphNode<Vector2Int>> data)) return straight;

        //data.to es el nodo anterior del camino jugador -> enemigo, o sea el siguiente paso hacia el jugador.
        //Si es null, este es el nodo del jugador o no hay camino hasta el.
        if (data.to == null) return straight;

        return DirectionTo(from, BlockToWorld(data.to.value));
    }

    private Vector2 DirectionTo(Vector3 from, Vector3 target)
    {
        return ((Vector2)(target - from)).normalized;
    }

    //----- ayudas para pasar de un lado a otro: mundo <-> bloque <-> nodo -----

    private GraphNode<Vector2Int> GetNode(int bx, int by)
    {
        if (bx < 0 || bx >= blocksWide || by < 0 || by >= blocksHigh) return null;
        return nodes[bx + by * blocksWide];
    }

    //posicion del mundo -> bloque donde esta
    private Vector2Int WorldToBlock(Vector3 worldPosition)
    {
        Vector3Int cell = floorTilemap.WorldToCell(worldPosition);
        int x = cell.x - originX;
        int y = cell.y - originY;
        return new Vector2Int(Mathf.FloorToInt((float)x / blockSize), Mathf.FloorToInt((float)y / blockSize));
    }

    //bloque -> posicion del mundo de su centro
    private Vector3 BlockToWorld(Vector2Int block)
    {
        //centro de la primera celda del bloque, corrido medio bloque para quedar en el medio
        Vector3 firstCell = floorTilemap.GetCellCenterWorld(new Vector3Int(originX + block.x * blockSize, originY + block.y * blockSize, 0));
        float shift = (blockSize - 1) * 0.5f;
        return firstCell + new Vector3(shift * floorTilemap.cellSize.x, shift * floorTilemap.cellSize.y, 0f);
    }

    //busca el nodo mas cercano a un bloque, mirando en anillos cada vez mas grandes (radio 0, 1, 2).
    //pickCheapest = true: entre los nodos del anillo elige el de menor costo hacia el jugador.
    private GraphNode<Vector2Int> FindNodeNear(Vector2Int block, bool pickCheapest)
    {
        for (int radius = 0; radius <= 2; radius++)
        {
            GraphNode<Vector2Int> best = null;
            int bestCost = int.MaxValue;

            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    //solo el borde del anillo; el interior ya se reviso con radios menores
                    if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) != radius) continue;

                    GraphNode<Vector2Int> candidate = GetNode(block.x + dx, block.y + dy);
                    if (candidate == null) continue;
                    if (!pickCheapest) return candidate;

                    int cost = CostToPlayer(candidate);
                    if (best == null || cost < bestCost)
                    {
                        best = candidate;
                        bestCost = cost;
                    }
                }
            }

            if (best != null) return best;
        }

        return null;
    }

    private int CostToPlayer(GraphNode<Vector2Int> node)
    {
        if (table != null && table.TryGetValue(node, out Edge<GraphNode<Vector2Int>> data)) return data.weight;
        return int.MaxValue;
    }

    //----- linea recta: hay algo en el medio? (se revisa celda por celda, sin usar fisicas) -----

    //se revisan 3 lineas (el centro y los dos costados del cuerpo) para que el enemigo no roce esquinas
    private bool HasClearPath(Vector2 from, Vector2 to)
    {
        Vector2 side = Vector2.Perpendicular((to - from).normalized) * bodyRadius;
        return LineIsClear(from, to) && LineIsClear(from + side, to + side) && LineIsClear(from - side, to - side);
    }

    private bool LineIsClear(Vector2 from, Vector2 to)
    {
        // Usamos la celda más chica para no saltear obstáculos en tilemaps
        // rectangulares (por ejemplo, cellSize.x != cellSize.y).
        float step = Mathf.Min(floorTilemap.cellSize.x, floorTilemap.cellSize.y) * 0.5f;
        if (step <= 0f) return false;
        int samples = Mathf.CeilToInt(Vector2.Distance(from, to) / step);

        for (int i = 0; i <= samples; i++)
        {
            float t = samples == 0 ? 0f : (float)i / samples;
            if (!IsWalkableAt(Vector2.Lerp(from, to, t))) return false;
        }
        return true;
    }

    private bool IsWalkableAt(Vector2 worldPosition)
    {
        Vector3Int cell = floorTilemap.WorldToCell(worldPosition);
        int x = cell.x - originX;
        int y = cell.y - originY;
        if (x < 0 || x >= cellsWide || y < 0 || y >= cellsHigh) return false;
        return walkableCells[y * cellsWide + x];
    }

    //Seleccionando este objeto en Play: rojo = celda bloqueada, amarillo = nodo del grafo,
    //verde = hacia que nodo va un enemigo que esta en cada nodo
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || nodes == null) return;

        for (int y = 0; y < cellsHigh; y++)
        {
            for (int x = 0; x < cellsWide; x++)
            {
                if (walkableCells[y * cellsWide + x]) continue;
                Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
                Gizmos.DrawCube(floorTilemap.GetCellCenterWorld(new Vector3Int(originX + x, originY + y, 0)), Vector3.one * 0.9f);
            }
        }

        for (int i = 0; i < nodes.Length; i++)
        {
            GraphNode<Vector2Int> node = nodes[i];
            if (node == null) continue;

            Vector3 center = BlockToWorld(node.value);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(center, Vector3.one * blockSize * 0.5f);

            if (table != null && table.TryGetValue(node, out Edge<GraphNode<Vector2Int>> data) && data.to != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(center, BlockToWorld(data.to.value));
            }
        }
    }
}
