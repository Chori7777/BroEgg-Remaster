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
    }
    // Futura variable para comprar objetos, si el jugador tiene suficiente oro, se gasta y devuelve true, sino devuelve false
    public void SetGold(int amount)
    {
        gold = Mathf.Max(0, amount);
        OnGoldChanged?.Invoke(gold);
    }
    public bool SpendGold(int amount)
    {
        if (amount < 0)
        {
            return false;
        }
        if (gold >= amount)
        {
            gold -= amount;
            OnGoldChanged?.Invoke(gold);
            return true;
        }
        return false;
    }
}
