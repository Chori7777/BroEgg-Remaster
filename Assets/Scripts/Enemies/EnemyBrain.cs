using System.Collections.Generic;
using UnityEngine;

// El cerebro decide QUE comportamiento sigue, mas no lo ejecuta, de eso lo hace la StateMachine
public class EnemyBrain
{
    // la lista guarda solo los comportamientos ahora la prioridad la da cada comportamiento con GetPriority().
    // La lista no se consume, pero la cola si, la lista sirve para resetear la cola y recargarla con los mismos comportamientos.
    // En el enemigo que hice podriamos obviar el enemyBrain, pero lo estoy forzando para decir que lo tenemos implementado y que funciona. En enemigos mas complejos es util tenerlo.

    private List<IEnemyBehavior> behaviors = new List<IEnemyBehavior>();
    private ISimplePriorityQueue<IEnemyBehavior> queue = new SimpleArrayPriorityQueue<IEnemyBehavior>();

    // false: carga la cola y la vacia en orden antes de recargar (secuencia fija).
    // true: recarga la cola en cada decision (la prioridad depende de la situacion).
    private bool reevaluateEachTime;
    public EnemyBrain(bool reevaluateEachTime)
    {
        this.reevaluateEachTime = reevaluateEachTime;
    }

    //recibe  el comportamiento.
    public void Add(IEnemyBehavior behavior)
    {
        behaviors.Add(behavior);
    }

    public IEnemyBehavior Next()
    {
     

        if (reevaluateEachTime || queue.IsEmpty)
        {
         
            LoadQueue();
        }

        IEnemyBehavior next = queue.Dequeue();

 

        return next;
    }

    // Vacia la cola pero conserva los comportamientos sirve en caso de que queramos reciclar al enemigo y que vuelva a empezar desde el primer comportamiento.
    public void Reset()
    {
        queue.Clear();
    }

    private void LoadQueue()
    {
        queue.Clear();
        foreach (IEnemyBehavior behavior in behaviors)
        {
            // la prioridad se pide al comportamiento en este momento,
            // asi un comportamiento dinamico su prioridad actual,puede depender de la situacion del enemigo o del jugador.
            queue.Enqueue(behavior, behavior.GetPriority());
        }
    }
}