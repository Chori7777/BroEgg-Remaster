using UnityEngine;

public class GameSession : MonoBehaviour // es el que arma el guardado,
                                         // es un script separado porque este script necesita conocer todos los sistemas a la vez
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private PlayerWeaponManager weaponManager;
    //[SerializeField] private InventoryManager inventory;

    //public GameMemento CreateMemento() // Lee el estado del juego y arma el guardado
    //{
    //    return new GameMemento(
    //        LevelManager.Instance.CurrentRound,
    //        playerGold.CurrentGold,
    //        playerStats.CreateSnapshot(),
    //        inventory.GetWeaponIds());
    //}

    public void Restore(GameMemento memento, bool healFull) // Vamos por ornden de guardado 
    {
        playerStats.RestoreSnapshot(memento.Stats);
        if (healFull) playerStats.SetHealth(playerStats.MaxHealth);

        playerGold.SetGold(memento.Gold);

        //inventory.ClearWeapons();
        for (int i = 0; i < memento.WeaponIds.Count; i++)
        {
            if (!string.IsNullOrEmpty(memento.WeaponIds[i]))
                weaponManager.GiveWeapon(memento.WeaponIds[i]);
        }

        playerHealth.Revive(); 
        //LevelManager.Instance.RestoreToRound(memento.Round);
    }
}