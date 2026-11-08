// Grilla de celdas caminables.
// No usa nada de Unity: solo guarda datos y hace cuentas con enteros (asi se puede probar facil).
public class Path
{
    // Costo extra para las celdas pegadas a una pared: hace que los enemigos se alejen de las esquinas
    public const int WallPenalty = 20;

    public readonly int width;
    public readonly int height;
    public readonly int originX;   // celda del Tilemap que corresponde a la columna 0 de la grilla
    public readonly int originY;   // celda del Tilemap que corresponde a la fila 0 de la grilla

    private readonly bool[] walkable;
    private readonly int[] penalty;

    public Path(int width, int height, int originX, int originY, bool[] walkable)
    {
        this.width = width;
        this.height = height;
        this.originX = originX;
        this.originY = originY;
        this.walkable = walkable;

        penalty = new int[walkable.Length];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (IsWalkable(x, y) && TouchesWall(x, y))
                {
                    penalty[Index(x, y)] = WallPenalty;
                }
            }
        }
    }

    public int Count => walkable.Length;

    // Cada celda (x, y) tiene un numero unico: asi guardamos todo en arrays simples
    public int Index(int x, int y) => y * width + x;

    public bool InBounds(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;

    // Fuera de la grilla cuenta como pared
    public bool IsWalkable(int x, int y) => InBounds(x, y) && walkable[Index(x, y)];

    public int GetPenalty(int x, int y) => penalty[Index(x, y)];

    // Celda del Tilemap -> coordenadas de la grilla
    public int ToGridX(int cellX) => cellX - originX;
    public int ToGridY(int cellY) => cellY - originY;

    // Busca la celda caminable mas cercana, mirando en anillos cada vez mas grandes.
    // Sirve cuando el jugador esta pisando el borde de una pared.
    public bool FindNearestWalkable(int x, int y, int maxRadius, out int foundX, out int foundY)
    {
        for (int radius = 0; radius <= maxRadius; radius++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    // Solo el borde del anillo; el interior ya se reviso en radios menores
                    if (System.Math.Max(System.Math.Abs(dx), System.Math.Abs(dy)) != radius) continue;

                    if (IsWalkable(x + dx, y + dy))
                    {
                        foundX = x + dx;
                        foundY = y + dy;
                        return true;
                    }
                }
            }
        }

        foundX = x;
        foundY = y;
        return false;
    }

    private bool TouchesWall(int x, int y)
    {
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;
                if (!IsWalkable(x + dx, y + dy)) return true;
            }
        }
        return false;
    }
}