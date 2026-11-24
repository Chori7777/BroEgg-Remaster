using ED262C;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;


public class PathfindingManager : MonoBehaviour
{
    //Singleton (igual que LevelManager)
    private static PathfindingManager instance;
    public static PathfindingManager Instance => instance;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorTilemap;      // PapuMap: el piso (donde se puede caminar)
    [SerializeField] private Tilemap obstacleTilemap;   // Objects: paredes y obstaculos

    [Header("Ajustes")]
    [Tooltip("Dejar en 1: IsBlockFree solo mira la primera celda del bloque")]
    [SerializeField] private int blockSize = 1;
    [Tooltip("Tope de nodos que A* explora por camino. Si lo supera, no hay camino y el enemigo va derecho (evita tirones si el jugador es inalcanzable)")]
    [SerializeField] private int maxExpandedNodes = 1000;
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
    private ISimpleDictionary<GraphNode<Vector2Int>, Edge<GraphNode<Vector2Int>>> table;
    private IPathFindingSolver<Vector2Int> AStarSolver; //esto es lo que va a operar sobre el grafo: A* busca el mejor camino entre dos nodos
    private IPathFindingSolver<Vector2Int> DijkstraSolver;

    private GraphNode<Vector2Int> playerNode;   // nodo desde el que se calculo la tabla
    private Transform playerTransform;
    private float timer;
    [SerializeField] private float recalculateInterval = 0.25f;
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

        //A* necesita una heuristica: una funcion que estime cuanto falta entre dos nodos (OctileDistance, mas abajo)
        AStarSolver = new AStarSolver<Vector2Int>(OctileDistance, maxExpandedNodes);
        DijkstraSolver = new DijkstraSolver<Vector2Int>();
    }

    // leemos los Tilemaps. Una celda es caminable si tiene piso y NO tiene obstaculo.
    private void ReadTilemaps()
    {
        BoundsInt bounds = floorTilemap.cellBounds; //representa las bounds de una celda en AABB
        cellsWide = bounds.size.x; //tamaño total de los tiles en x e y
        cellsHigh = bounds.size.y;
        originX = bounds.xMin; // origenes x e y de la celda superior izquierda del mapa
        originY = bounds.yMin;

        walkableCells = new bool[cellsWide * cellsHigh];

        for (int y = 0; y < cellsHigh; y++)
        {
            for (int x = 0; x < cellsWide; x++)
            {
                // Traduce el índice local (0.3, 0.2) a coordenada real del Tilemap
                Vector3Int cell = new Vector3Int(originX + x, originY + y, 0);
                // Pregunta a los dos Tilemaps de Unity qué hay en esa coordenada
                bool hasFloor = floorTilemap.HasTile(cell);
                bool hasObstacle = obstacleTilemap.HasTile(cell);
                // Guarda true solo si hay piso Y NO hay obstáculo, en el cajón único

                walkableCells[y * cellsWide + x] = hasFloor && !hasObstacle; //walkableCell es true si el tile es un floor y no tiene obstaculo
                
                //hacer walkableCells[y * cellsWide + x] es lo mismo que hacer walkableCells[cellsHigh ,cellsWide] si fuera bidimensional, pero de esta forma es mas optimo, mas prolijo de escribir y mas seguro de manejar
            }
        }
    }

    // armamos el grafo. Se hace una sola vez porque los obstaculos no se mueven.
    private void BuildGraph()
    {
        //esto lo dejo asi, pero basicamente lo que hace esto es que la variable blockSize agrupe tiles para que la busqueda de caminos sea menos pesada para el juego
        //eso puede generar problemas pero ahorra memoria, para que funcione bien bien, yo pongo la variable en el inspector = 1, por eso es que esta dividion es inutil
        //pero de momento la voy a dejar por las dudas, OK?
        blocksWide = cellsWide / blockSize;
        blocksHigh = cellsHigh / blockSize;

        nodes = new GraphNode<Vector2Int>[walkableCells.Length]; //creo X cantidad de espacios dentro del array, por X cantidad de tiles dentro del array

        // un nodo por cada bloque libre. El valor del nodo es la posicion del bloque.
        for (int by = 0; by < blocksHigh; by++)
        {
            for (int bx = 0; bx < blocksWide; bx++)
            {
                if (!IsBlockFree(bx, by)) continue; //si el tile esta libre, que haga lo de abajo, sino que salta al proximo bucle

                GraphNode<Vector2Int> node = new GraphNode<Vector2Int>(new Vector2Int(bx, by)); //creamos un nodo 
                nodes[bx + by * blocksWide] = node; //lo agregamos a sus posiciones dentro del array de nodos
                graph.AddVertex(node); //y por ultimo lo añado al grafo 
            }
        }

        // aristas: cada nodo se conecta con sus 8 vecinos. Costo 10 en recto y 14 en diagonal (1 y 1.4 por 10).
        //Se usan enteros porque el peso de las aristas y de la PriorityQueue es int.
        //se hacen 2 for con indices de grilla hasta el ancho y alto del mapa
        for (int by = 0; by < blocksHigh; by++)
        {
            for (int bx = 0; bx < blocksWide; bx++)
            {
                GraphNode<Vector2Int> node = GetNode(bx, by); //GetNode(bx, by) me devuelve una posicion del array, ponele nodes[67]
                if (node == null) continue;

                //los dos nodos interiores buscan vecinos que ya existen para conectarlos mediante aristas
                // si no existen, no hacen nada. Por eso es que primero se crean todos los nodos y despues se conectan entre si.

                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++) //este doble for me sirve para esto ver sus 8 vecinos
                    {
                        if (dx == 0 && dy == 0) continue; //aca se asegura que no este en el centro

                        GraphNode<Vector2Int> neighbour = GetNode(bx + dx, by + dy);

                        //    [  x  ][  x  ][  x  ]----------------- (dx = -1, dy = 1)  (dx = 0, dy = 1)  (dx = 1, dy = 1)

                        //    [  x  ][CENTRO][ x  ]---------------- (dx = -1, dy = 0)  (GetNode(bx, by)  (dx = 1, dy = 0)

                        //    [  x  ][  x  ][  x  ]----------------- (dx = -1, dy = -1) (dx = 0, dy = -1) (dx = 1, dy = -1)

                        if (neighbour == null) continue;

                        bool diagonal = dx != 0 && dy != 0; //si dx o dy es 0, significa que esta derecha, arriba, abajo ou izquierda

                        if (diagonal && (GetNode(bx + dx, by) == null || GetNode(bx, by + dy) == null)) continue;

                        int weight;
                        if (diagonal == true) weight = 14; //pesa mas ir en diagonal que ir derecho
                        else weight = 10;

                        graph.AddEdge(node, neighbour, weight); //añado arista
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (nodes == null) return;

        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null) return;
            playerTransform = playerObject.transform;
        }

        timer -= Time.deltaTime; //todo el tiempo va bajando timer y en el mistras ese timer sea mayor a 0 todo se detiene, no existe dijkstra

        if (timer > 0f) return;

        timer = Mathf.Max(0f, recalculateInterval); //cuando llegue aca, timer vuelve a ser mayor a 0, pero hasta que vuelva al chequeo de arriba, tiene que ejecutar el dijkstra de abajo

        Vector2Int playerBlock = WorldToBlock(playerTransform.position); // guardamos en un Vector2int donde esta el Player (origen del dijkstra)
        GraphNode<Vector2Int> origin = GetNode(playerBlock.x, playerBlock.y); //pasamos ese Vector2int a una variable GraphNode

        playerNode = origin;
        table = DijkstraSolver.GetAllShortestPath(playerNode, graph); //El algoritmo devuelve una tabla gigante que contiene, para cada nodo del mapa, dos datos cruciales
                                                              //1 - Cuánto cuesta llegar al jugador desde ese nodo.
                                                              //2 - Cuál es el nodo siguiente al que hay que moverse(el "próximo paso").
    }

    public Vector2 GetDirection(Vector3 from, Vector3 to) //Dijkstra
    {
        Vector2 straight = ((Vector2)(to - from)).normalized; //un movimiento sin pathfinding, solo ir para adelante como un wachin

        if (table == null) return straight;

        Vector2Int currentEnemyBlock = WorldToBlock(from);
        GraphNode<Vector2Int> currentEnemyPosition = GetNode(currentEnemyBlock.x, currentEnemyBlock.y);

        // Si el enemigo esta en un bloque nulo (pared), va directo al jugador a lo bruto
        if (currentEnemyPosition == null) return straight;

        if (!table.TryGetValue(currentEnemyPosition, out Edge<GraphNode<Vector2Int>> data)) return straight; //si no obtenemos el valor porque la key currentEnemyPosition no existe, vamos derechito

        if (data.to == null) return straight;//si no hay un camino posible en la tabla, que vaya para adelante

        // Retorna la direccion hacia el centro del siguiente nodo del grafo
        return DirectionTo(from, BlockToWorld(data.to.value));
    }

    private Vector2 DirectionTo(Vector3 from, Vector3 target)
    {
        return ((Vector2)(target - from)).normalized;
    }


    private bool IsBlockFree(int bx, int by) //devuelve true si una celda es true
    {
        if (!walkableCells[by * cellsWide + bx]) return false;

        return true;
    }


    //posicion del mundo del centro de un nodo (para saber hacia donde caminar)
    public Vector3 NodeToWorld(GraphNode<Vector2Int> node)
    {
        return BlockToWorld(node.value);
    }


    // formula de la heuristica de movimiento octil 
    //10 es cardinal
    //14 es en diagonal
    //abs para que no sean negativos los parametros
    private int OctileDistance(GraphNode<Vector2Int> a, GraphNode<Vector2Int> b) // movimiento en 8 direcciones
    {
        int dx = Mathf.Abs(a.value.x - b.value.x);
        int dy = Mathf.Abs(a.value.y - b.value.y);

        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);
        return 14 * min + 10 * (max - min);
    }

    //----- ayudas para pasar de un lado a otro: mundo <-> bloque <-> nodo -----


    //posicion del mundo -> bloque donde esta
    private Vector2Int WorldToBlock(Vector3 worldPosition) // Convierte esa posición mundial en coordenadas lógicas de bloques del tilemap
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


    private GraphNode<Vector2Int> GetNode(int bx, int by) //devuelve un nodo en una posicion del array
    {
        if (bx < 0 || bx >= blocksWide || by < 0 || by >= blocksHigh) return null;
        return nodes[bx + by * blocksWide];
    }








    // camino A* entre dos posiciones del mundo. Devuelve la lista de nodos (el primero es donde esta el enemigo,
    //el ultimo es donde esta el jugador), o null si no hay camino.
    public ISimpleList<GraphNode<Vector2Int>> FindPath(Vector3 from, Vector3 to)
    {
        if (nodes == null) return null;

        //nodo de origen y de destino. Si una posicion cae en una pared, usamos el nodo libre mas cercano
        GraphNode<Vector2Int> origin = FindNode(WorldToBlock(from));
        GraphNode<Vector2Int> destination = FindNode(WorldToBlock(to));

        if (origin == null || destination == null) return null;

        return AStarSolver.GetShortestPath(origin, destination, graph);
    }


    //si el bloque tiene nodo lo devuelve; si es una pared, busca el nodo mas cercano mirando en anillos cada vez mas grandes (radio 1 y 2)
    private GraphNode<Vector2Int> FindNode(Vector2Int block)
    {
 
        GraphNode<Vector2Int> candidate = GetNode(block.x, block.y);
        if (candidate != null) return candidate;

        return null;
    }










    //Seleccionando este objeto en Play: rojo = celda bloqueada, amarillo = nodo del grafo
    //(el camino de cada Suicide se dibuja en verde al seleccionar al enemigo)
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || nodes == null) return;

        for (int y = 0; y < cellsHigh; y++)
        {
            for (int x = 0; x < cellsWide; x++)
            {
                if (walkableCells[y * cellsWide + x]) continue; //solo dibujo si walkableCells = false
                Gizmos.color = new Color(1f, 0f, 0f, 0.25f); //dibujo de color rojo
                Gizmos.DrawCube(floorTilemap.GetCellCenterWorld(new Vector3Int(originX + x, originY + y, 0)), Vector3.one * 0.9f);
            }
        }

        for (int i = 0; i < nodes.Length; i++)
        {
            GraphNode<Vector2Int> node = nodes[i];
            if (node == null) continue;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(BlockToWorld(node.value), Vector3.one * blockSize * 0.5f);
        }
    }
}
