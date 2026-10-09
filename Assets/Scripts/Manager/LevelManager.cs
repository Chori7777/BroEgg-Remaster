using UnityEngine;
using System.Collections.Generic;
using ED262C;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
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
    public Image shopPanel;
    public Button[] botones;
    public ObjectsInventory inventory;
    public ShopManager shopManager;
    [SerializeField] private GameObject inventoryPanel;
    private int pauseChangedFrame = -1;

    public GameObject InventoryPanel => inventoryPanel;
    public bool IsPaused => levelStateMachine != null && levelStateMachine.IsPaused;
    public bool ControlsBlocked => IsPaused || Time.timeScale == 0f || pauseChangedFrame == Time.frameCount;

    public LevelStateMachine levelStateMachine;

    public EnemySpawnerManager enemySpawnerManager;


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
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        

        // Cargamos todas las waves del Inspector a la cola, en orden
        for (int i = 0; i < waveList.Count; i++)
        {
            waveQueue.Enqueue(waveList[i]);
        }

        levelStateMachine = new LevelStateMachine(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleInventory();
            return;
        }
        levelStateMachine.UpdateMachine();
    }

    public void ToggleInventory()
    {
        if (levelStateMachine == null || inventoryPanel == null) return;
        if (IsPaused)
            levelStateMachine.Resume();
        else
        {
            // No superponer el inventario con la pausa existente de Escape.
            if (Time.timeScale == 0f && levelStateMachine.CurrentState != levelStateMachine.Shop) return;
            levelStateMachine.Pause();
        }
        pauseChangedFrame = Time.frameCount;
    }

    private void OnDestroy()
    {
        if (instance != this) return;
        if (IsPaused) levelStateMachine.Resume();
        instance = null;
    }

    public void AdvanceToNextWave()
    {
        currentWave = waveQueue.Dequeue();
        currentRound++;
        AchievementsManager.Instance.waveCount++;
        Debug.Log("Ronda Actual" + currentRound);
    }
    public void ChangeTime(float time)
    {
        Time.timeScale = time;
    }



    public void NextWave()
    {
        if (IsPaused) return;
        shopPanel.gameObject.SetActive(false);
        levelStateMachine.ChangeState(levelStateMachine.Gameplay);
    }
}
