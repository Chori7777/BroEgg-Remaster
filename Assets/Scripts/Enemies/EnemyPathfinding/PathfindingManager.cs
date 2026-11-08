using UnityEngine;
using UnityEngine.Tilemaps;

// Singleton que arma la grilla a partir de los Tilemaps, recalcula el FlowField cuando el jugador
// cambia de celda y le dice a los enemigos hacia donde caminar.
[DefaultExecutionOrder(-50)]
public class PathfindingManager : MonoBehaviour
{
    private static PathfindingManager instance;
    public static PathfindingManager Instance => instance;

    [SerializeField] private Tilemap floorTilemap;      // PapuMap: el piso (donde se puede caminar)
    [SerializeField] private Tilemap obstacleTilemap;   // Objects: paredes y obstaculos

    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private float bodyRadius = 0.8f;
    [SerializeField] private bool drawGizmos = true;

    private Path grid;
    private FlowField field;
    private Transform playerTransform;
    private float timer;
    private int targetX = int.MinValue;
    private int targetY = int.MinValue;
    private bool ready;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        BuildGrid();
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    // Una celda es caminable si tiene piso y NO tiene obstaculo.
    // Los obstaculos no se mueven, asi que esto se hace una sola vez.
    private void BuildGrid()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        bool[] walkable = new bool[bounds.size.x * bounds.size.y];

        for (int y = 0; y < bounds.size.y; y++)
        {
            for (int x = 0; x < bounds.size.x; x++)
            {
                Vector3Int cell = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                bool hasFloor = floorTilemap.HasTile(cell);
                bool hasObstacle = obstacleTilemap.HasTile(cell);
                walkable[y * bounds.size.x + x] = hasFloor && !hasObstacle;
            }
        }

        grid = new Path(bounds.size.x, bounds.size.y, bounds.xMin, bounds.yMin, walkable);
        field = new FlowField(grid);
    }

    private void Update()
    {
        if (grid == null) return;

        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); //esto es como un cast de Unreal
            if (playerObject == null) return;
            playerTransform = playerObject.transform;
        }

        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = checkInterval;

        // Celda donde esta el jugador. Si pisa una pared, usamos la celda libre mas cercana.
        Vector3Int playerCell = floorTilemap.WorldToCell(playerTransform.position);
        if (!grid.FindNearestWalkable(grid.ToGridX(playerCell.x), grid.ToGridY(playerCell.y), 3, out int x, out int y)) return;

        // Si sigue en la misma celda, el campo ya esta bien: no hace falta recalcular
        if (x == targetX && y == targetY) return;

        targetX = x;
        targetY = y;
        field.Compute(x, y);
        ready = true;
    }

    // Direccion (ya normalizada) para ir de "from" hasta "to" esquivando obstaculos.
    public Vector2 GetDirection(Vector3 from, Vector3 to)
    {
        Vector2 straight = ((Vector2)(to - from)).normalized;

        if (!ready) return straight;

        // Si no hay nada en el medio, vamos en linea recta (se ve mas natural)
        if (HasClearPath(from, to)) return straight;

        // Si hay obstaculo, seguimos el campo: vamos al centro de la celda vecina de menor costo
        Vector3Int cell = floorTilemap.WorldToCell(from);
        if (!field.TryGetNextStep(grid.ToGridX(cell.x), grid.ToGridY(cell.y), out int nextX, out int nextY))
        {
            return straight;   // ya esta en el destino, o su celda no se puede alcanzar
        }

        Vector3Int nextCell = new Vector3Int(grid.originX + nextX, grid.originY + nextY, 0);
        Vector3 nextCenter = floorTilemap.GetCellCenterWorld(nextCell);
        return ((Vector2)(nextCenter - from)).normalized;
    }

    // Revisa 3 lineas (centro y los dos costados del cuerpo) para que el enemigo no roce esquinas
    private bool HasClearPath(Vector2 from, Vector2 to)
    {
        Vector2 side = Vector2.Perpendicular((to - from).normalized) * bodyRadius;
        return LineIsClear(from, to) && LineIsClear(from + side, to + side) && LineIsClear(from - side, to - side);
    }

    private bool LineIsClear(Vector2 from, Vector2 to)
    {
        float step = floorTilemap.cellSize.x * 0.5f;
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
        return grid.IsWalkable(grid.ToGridX(cell.x), grid.ToGridY(cell.y));
    }
}