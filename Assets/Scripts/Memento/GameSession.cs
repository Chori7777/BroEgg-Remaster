using UnityEngine;

public class GameSession : MonoBehaviour // es el que arma el guardado,
                                         // es un script separado porque este script necesita conocer todos los sistemas a la vez
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private PlayerWeaponManager weaponManager;
    [SerializeField] private InventaryWeapons inventoryWeapons;
    [SerializeField] private InventaryManager inventoryObjects;
   

    public GameMemento CreateMemento() // Lee el estado del juego y arma el guardado
    {
        return new GameMemento(
            LevelManager.Instance.CurrentRound,
            playerGold.CurrentGold,
            playerStats.Health,
            inventoryObjects.GetObjects(),
            inventoryWeapons.GetWeaponIds());
    }

    public void Restore(GameMemento memento, bool healFull) // Vamos por ornden de guardado 
    {
        // sacamos todos los objetos y ponemos los guardados (PlayerStats recalcula los bonus solo)
        foreach (ObjectShopData item in inventoryObjects.GetObjects())
            inventoryObjects.TryRemove(item.id);
        for (int i = 0; i < memento.Objects.Count; i++)
            inventoryObjects.TryAdd(memento.Objects[i]);

        playerStats.SetHealth(healFull ? playerStats.MaxHealth : memento.Health);

        playerGold.SetGold(memento.Gold);

        inventoryWeapons.ClearWeapons();
        for (int i = 0; i < memento.WeaponIds.Count; i++)
        {
            if (!string.IsNullOrEmpty(memento.WeaponIds[i]))
                weaponManager.GiveWeapon(memento.WeaponIds[i]);
        }

        playerHealth.Revive(); 
        LevelManager.Instance.RestoreToRound(memento.Round);
    }
}