// Mapa de costos hacia un destino (el jugador), calculado con Dijkstra.
// Cada celda guarda "cuanto cuesta llegar desde aca hasta el jugador".
// Un enemigo solo tiene que moverse hacia el vecino de menor costo.
/*public class FlowField
{
    public const int Unreachable = int.MaxValue;

    // Costos en enteros porque la cola de prioridad trabaja con int (1.0 -> 10, 1.41 -> 14)
    private const int StraightCost = 10;
    private const int DiagonalCost = 14;

    private readonly Path grid;
    private readonly int[] cost;
    private readonly ISimplePriorityQueue<int> queue = new SimpleArrayPriorityQueue<int>();

    public FlowField(Path grid)
    {
        this.grid = grid;
        cost = new int[grid.Count];
        ResetCosts();
    }

    public int GetCost(int x, int y)
    {
        return grid.InBounds(x, y) ? cost[grid.Index(x, y)] : Unreachable;
    }

    // Dijkstra: se parte del destino con costo 0 y se "derrama" por las celdas caminables.
    public void Compute(int targetX, int targetY)
    {
        ResetCosts();
        queue.Clear();

        if (!grid.IsWalkable(targetX, targetY)) return;

        int target = grid.Index(targetX, targetY);
        cost[target] = 0;
        queue.Enqueue(target, 0);

        while (!queue.IsEmpty)
        {
            int priority = queue.GetHighestPriority();
            int current = queue.Dequeue();    // siempre sale la celda de menor costo

            // Si ya se encontro un camino mas barato a esta celda, esta entrada quedo vieja
            if (priority > cost[current]) continue;

            int cx = current % grid.width;
            int cy = current / grid.width;

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = cx + dx;
                    int ny = cy + dy;
                    if (!CanStep(cx, cy, dx, dy)) continue;

                    bool diagonal = dx != 0 && dy != 0;
                    int newCost = cost[current] + (diagonal ? DiagonalCost : StraightCost) + grid.GetPenalty(nx, ny);

                    int neighbor = grid.Index(nx, ny);
                    if (newCost < cost[neighbor])
                    {
                        cost[neighbor] = newCost;
                        queue.Enqueue(neighbor, newCost);
                    }
                }
            }
        }
    }

    // Devuelve la celda vecina de menor costo (hacia donde hay que caminar).
    // false = no hay un vecino mejor (ya esta en el destino, o la celda no se puede alcanzar).
    public bool TryGetNextStep(int x, int y, out int nextX, out int nextY)
    {
        nextX = x;
        nextY = y;

        int best = GetCost(x, y);
        bool found = false;

        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;
                if (!CanStep(x, y, dx, dy)) continue;

                int neighborCost = cost[grid.Index(x + dx, y + dy)];
                if (neighborCost < best)
                {
                    best = neighborCost;
                    nextX = x + dx;
                    nextY = y + dy;
                    found = true;
                }
            }
        }

        return found;
    }

    // Se puede pasar a la celda vecina? Debe ser caminable y, en diagonal,
    // las dos celdas de al lado tambien (si no, el enemigo se engancharia en la esquina).
    private bool CanStep(int x, int y, int dx, int dy)
    {
        if (!grid.IsWalkable(x + dx, y + dy)) return false;

        bool diagonal = dx != 0 && dy != 0;
        if (diagonal && (!grid.IsWalkable(x + dx, y) || !grid.IsWalkable(x, y + dy))) return false;

        return true;
    }

    private void ResetCosts()
    {
        for (int i = 0; i < cost.Length; i++)
        {
            cost[i] = Unreachable;
        }
    }
}*/