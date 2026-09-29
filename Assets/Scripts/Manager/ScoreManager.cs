using ED262C;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int killCount = 0;
    public int waveCount = 1;
    private static ScoreManager instance;
    public static ScoreManager Instance=> instance;
    private Achievements logros;
    private ISimpleSet<Achievements> achievementsSet = new SimpleArraySet<Achievements>();


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (killCount == 5)
        {
            achievementsSet.Add(Achievements.Ultrakill);
            Leedor();
        }


        if(killCount == 10)
        {
            achievementsSet.Add(Achievements.Bloodthirsty);
            Leedor();
        }

        if(waveCount == 5)
        {
            achievementsSet.Add(Achievements.LastSurvivor);
            Leedor();
        }

        if (waveCount == 10)
        {
            achievementsSet.Add(Achievements.LastMan);
            Leedor();
        }

    }

    void Leedor()
    {
        foreach (var item in achievementsSet.ToArray())
        {
            Debug.Log("<color=red>¡LOGRO DESBLOQUEADO!</color> " + item);
        }
    }
}
