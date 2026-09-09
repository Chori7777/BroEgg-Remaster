using System;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [SerializeField] private int gold;

    public int CurrentGold => gold;
    public event Action<int> OnGoldChanged; // avisa el nuevo total!

    // En PlayerGold, agregar:
    private void Start()
    {
        OnGoldChanged?.Invoke(gold);
    }

    private void OnEnable()
    {
        Enemy.OnEnemyGoldDrop += AddGold;
    }
    private void OnDisable()
    {
        Enemy.OnEnemyGoldDrop -= AddGold;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
        Debug.Log("Yuan:" + gold);
    }
    // Futura variable para comprar objetos, si el jugador tiene suficiente oro, se gasta y devuelve true, sino devuelve false
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            OnGoldChanged?.Invoke(gold);
            return true;
        }
        return false;
    }
}
