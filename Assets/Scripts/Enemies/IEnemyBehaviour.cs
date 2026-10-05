public interface IEnemyBehavior : IState
{
    bool IsFinished { get; }

    // cada comportamiento dice que prioridad tiene en ese momento, para poder modificarlo segun las condiciones, como le dijimos al profe una vez
    int GetPriority();
}