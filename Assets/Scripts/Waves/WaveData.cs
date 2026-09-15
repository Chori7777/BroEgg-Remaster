using System.Collections.Generic;

[System.Serializable]
public class EnemyProbability  //Creo una mini clase para juntar los dos datos que necesito el id del enmigo y la proba
{
    public string EnemyId;   //HACER UN STRUCT Y LOS ID HACERLOS ENUMS sugerencia del profe
    public float Probability;
}

[System.Serializable] //Hace serializable todas las variables de esta clase
public class WaveData
{
    public int EnemiesPerWave;
    public float Time;
    public float WaveRate;
    public List<EnemyProbability> enemyProbabilities; // Aca junto esos dos datos para guardarlos en una lista asi no creo un string y float 
}