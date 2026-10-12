using System.Collections.Generic;


public class GameMemento // Guarda ronda, oro stats y los ids de las armas
{
    public readonly int Round; // readonly significa solo lectura
    public readonly int Gold;
    public readonly int Health;
    private readonly ObjectShopData[] objects;
    private readonly string[] weaponIds; // Si guardara la referencia al objeto se destruiran al morir por eso guardo las ids para al volver crear las armas 

    public IReadOnlyList<string> WeaponIds => weaponIds;

    public IReadOnlyList<ObjectShopData> Objects => objects;
    public GameMemento(int round, int gold, int health, ObjectShopData[] objects, string[] weaponIds)
    {
        Round = round;
        Gold = gold;
        Health = health;
        this.objects = (ObjectShopData[])objects.Clone();
        this.weaponIds = (string[])weaponIds.Clone();  // Copia del array, si guardo el array original y despues se modifica, el guarado cambiaria 
    }


}
 
