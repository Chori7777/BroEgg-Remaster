using UnityEngine;
using System.Collections.Generic;
using ED262C;

public class LevelManager : MonoBehaviour
{
    //  SINGLETON 
    private static LevelManager instance;
    public static LevelManager Instance => instance;

    // DATOS EDITABLES EN EL INSPECTOR 
    public List<WaveData> waveList = new List<WaveData>();

    //  COLA
    private ISimpleQueue<WaveData> waveQueue = new SimpleArrayQueue<WaveData>();

    private WaveData currentWave;

    public WaveData CurrentWave => currentWave;

    private float timeRemaining;
    public float TimeRemaining
    {
        get => timeRemaining;
        set => timeRemaining = value;
    }

    private int currentRound;

    public int CurrentRound => currentRound;

    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        

        // Cargamos todas las waves del Inspector a la cola, en orden
        for (int i = 0; i < waveList.Count; i++)
        {
            waveQueue.Enqueue(waveList[i]);
        }

        // Arrancamos con la primera wave
        currentWave = waveQueue.Dequeue();

        
    }

    void Update()
    {
        
    }
}