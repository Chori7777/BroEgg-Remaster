using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyAction
{
    public string name;
    // 0% a 100% 
    [Range(0f, 1f)] public float probability;

    public EnemyAction(string name, float probability)
    {
        this.name = name;
        this.probability = probability;
    }
}

public class EnemyBrain : MonoBehaviour
{
    public List<EnemyAction> possibleActions = new List<EnemyAction>();

    ISimplePriorityQueue<EnemyAction> queue = new SimpleArrayPriorityQueue<EnemyAction>();

    void Start()
    {
        possibleActions.Add(new EnemyAction("Attack", 0.5f));
        possibleActions.Add(new EnemyAction("Defend", 0.3f));
        possibleActions.Add(new EnemyAction("Flee", 0.2f));

        LoadQueue();
    }

    void LoadQueue()
    {
        queue.Clear();
        Debug.Log("Queue cleared, reloading actions...");

        foreach (var action in possibleActions)
        {
            int priority = 100 - Mathf.RoundToInt(action.probability * 100f);
            queue.Enqueue(action, priority);
            Debug.Log($"Enqueue {action.name} (priority: {priority})");
        }
    }

    public void ExecuteNextAction()
    {
        if (queue.IsEmpty)
        {
            Debug.Log("Queue empty, reloading...");
            LoadQueue();
        }

        EnemyAction current = queue.Dequeue();
        Debug.Log($"Dequeue -> {current.name}");

        switch (current.name)
        {
            case "Attack": Debug.Log("Enemy attacks!"); break;
            case "Defend": Debug.Log("Enemy defends!"); break;
            case "Flee": Debug.Log("Enemy flees!"); break;
        }

        Debug.Log($"Actions left in queue: {queue.Count}");
    }
}