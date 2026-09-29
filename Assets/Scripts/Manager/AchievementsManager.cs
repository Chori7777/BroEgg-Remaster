using ED262C;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public int killCount = 0;
    public int waveCount = 1;
    private static AchievementsManager instance;
    public static AchievementsManager Instance=> instance;
    private Achievements thophies;
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
            Reader();
        }


        if(killCount == 10)
        {
            achievementsSet.Add(Achievements.Bloodthirsty);
            Reader();
        }

        if(waveCount == 5)
        {
            achievementsSet.Add(Achievements.LastSurvivor);
            Reader();
        }

        if (waveCount == 10)
        {
            achievementsSet.Add(Achievements.LastMan);
            Reader();
        }

    }

    void Reader()
    {
        foreach (var item in achievementsSet.ToArray())
        {
            Debug.Log("<color=red>¡LOGRO DESBLOQUEADO!</color> " + item);
        }
    }
}
