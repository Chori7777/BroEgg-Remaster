using UnityEngine;

// Espera quieto hasta ver al jugador, hace la alerta
public class DasherIdle : IEnemyBehavior
{
    DasherEnemy enemy;
    bool alerted;
    float timer;

    public DasherIdle(DasherEnemy enemy)
    {
        this.enemy = enemy;
    }

    // Termina cuando ya dio la alerta Y esta termino de reproducirse.
    public bool IsFinished
    {
        get { return alerted && timer >= enemy.alertDuration; }
    }

    // Obligatorio por la interfaz, pero Idle no pasa por el cerebro.
    public int GetPriority()
    {
        return 0;
    }

    public void Enter()
    {
        alerted = false;
        timer = 0f;
        enemy.Stop();
        enemy.ShowState("ChasingAnim");
    }

    public void Exit() { }

    public void UpdateState()
    {
        if (!alerted)
        {
            if (enemy.DistanceToPlayer() <= enemy.detectionRange)
            {
                alerted = true;
                enemy.ShowState("PlayerFound");
            }
        }
        else
        {
            timer += Time.deltaTime;   // cuenta lo que lleva la alerta
        }
    }
}

// Embestida en linea recta. Prioridad 1: sale primero en el ciclo.
public class DasherDash : IEnemyBehavior
{
    DasherEnemy enemy;
    Vector2 direction;
    float timer;

    public DasherDash(DasherEnemy enemy)
    {
        this.enemy = enemy;
    }

    public bool IsFinished
    {
        get { return timer >= enemy.dashDuration; }
    }

    public int GetPriority()
    {
        return 1;
    }

    public void Enter()
    {
        timer = 0f;
        // La direccion se toma UNA vez, al empezar. Si siguiera al jugador durante el dash seria imposible esquivarlo.
        direction = enemy.DirToPlayer();
        enemy.ShowState("ChaserChasing");
    }

    public void Exit() { }

    public void UpdateState()
    {
        timer += Time.deltaTime;
        enemy.Move(direction, enemy.dashSpeedMultiplier);
    }
}

// Pausa despues del dash. Prioridad 2. Es la ventana para castigarlo.
public class DasherRest : IEnemyBehavior
{
    DasherEnemy enemy;
    float timer;

    public DasherRest(DasherEnemy enemy)
    {
        this.enemy = enemy;
    }

    public bool IsFinished
    {
        get { return timer >= enemy.restDuration; }
    }

    public int GetPriority()
    {
        return 2;
    }

    public void Enter()
    {
        timer = 0f;
        enemy.Stop();   // frena la velocidad que traia del dash
        enemy.ShowState("Resting");
    }

    public void Exit() { }

    public void UpdateState()
    {
        timer += Time.deltaTime;
    }
}

// Corre hacia el jugador hasta la distancia optima con una prioridad de 3.
public class DasherChase : IEnemyBehavior
{
    DasherEnemy enemy;
    float timer;

    public DasherChase(DasherEnemy enemy)
    {
        this.enemy = enemy;
    }

    // Termina al llegar a la distancia optima, o por tiempo si no llega nunca.
    public bool IsFinished
    {
        get { return enemy.DistanceToPlayer() <= enemy.optimalDistance || timer >= enemy.chaseTimeout; }
    }

    public int GetPriority()
    {
        return 3;
    }

    public void Enter()
    {
        timer = 0f;
        enemy.ShowState("ChaseAnim");
    }

    public void Exit() 
    
    {
    
    }

    public void UpdateState()
    {
        timer += Time.deltaTime;
        enemy.Move(enemy.DirToPlayer(), enemy.chaseSpeedMultiplier);
    }
}