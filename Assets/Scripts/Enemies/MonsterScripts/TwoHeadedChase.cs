using UnityEngine;

// Camina hacia el jugador y despues se ralentiza lentamente para disparar con una cabeza,la cabeza que dispara depende de la distancia al jugador
public class TwoHeadedChase : IEnemyBehavior
{
    //Hereda de IEnemyBehaviour para controlar su propia prioridad 
    TwoHeadedEnemy enemy;
    float timer;

    public TwoHeadedChase(TwoHeadedEnemy enemy)
    {
        this.enemy = enemy;
    }

    public bool IsFinished
    {
        get { return timer >= enemy.chaseDuration || enemy.DistanceToPlayer() <= enemy.minChaseDistance; }
    }

    // Obligatorio por la interfaz, pero el Chase no pasa por el cerebro
    public int GetPriority()
    {
        return 0;
    }

    public void Enter()
    {
        timer = 0f;
        //enemy.ShowState("Chase");
        // Aun no tengo animaciones asi que queda comentado xd
    }

    public void Exit() { }

    public void UpdateState()
    {
        //Se mueve al jugador
        timer += Time.deltaTime;
        enemy.Move(enemy.DirToPlayer(), 1f);
    }
}

// Se ralentiza y dispara con una cabeza, la prioridad depende de la distancia al jugador y de si es la cabeza izquierda o derecha
public class TwoHeadedShoot : IEnemyBehavior
{
    TwoHeadedEnemy enemy;
    bool isLeft;
    float timer;
    bool fired;

    public TwoHeadedShoot(TwoHeadedEnemy enemy, bool isLeft)
    {
        this.enemy = enemy;
        this.isLeft = isLeft;
    }

    public bool IsFinished
    {
        get { return fired; }
    }

    // Prioridad 1 sale primero, de lejos gana la izquierda y de cerca gana la derecha
    public int GetPriority()
    {
        bool far = enemy.IsPlayerFar();
        int priority;

        if (isLeft)
        {
            priority = far ? 2 : 1;
        }
        else
        {
            priority = far ? 1 : 2;
        }

       
        return priority;
    }
    public void Enter()
    {
        timer = 0f;
        fired = false;
        //enemy.ShowState(isLeft ? "ShootLeft" : "ShootRight");
        // tampoco tengo hecha esta animacion xd
    }

    public void Exit() { }

    public void UpdateState()
    {
        timer += Time.deltaTime;
        enemy.Move(enemy.DirToPlayer(), enemy.slowFactor);   // sigue caminando, pero lento, para disparar con la cabeza

        if (!fired && timer >= enemy.slowDuration)
        {
            enemy.Shoot(isLeft);
            fired = true;
        }
    }
}