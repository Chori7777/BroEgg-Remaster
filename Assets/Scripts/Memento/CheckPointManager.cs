using System.Collections;
using UnityEngine;

public class CheckpointManager : MonoBehaviour // Guarda una variable last que seria el ultimo guardado 
{
    private static CheckpointManager instance;
    public static CheckpointManager Instance => instance;

    [SerializeField] private GameSession session;
    [SerializeField] private bool healFullOnRetry = true;

    private GameMemento lastCheckpoint;

    public bool HasCheckpoint => lastCheckpoint != null;
    public int CheckpointRound => lastCheckpoint != null ? lastCheckpoint.Round : 0;

    private void Awake()
    {
        if (instance == null) instance = this; // Lo hago singleton para que gameplaystate lo encuentre sin referencia de inspector ya que gameplay state es un componente de unity
        else Destroy(gameObject);
    }

    public void SaveCheckpoint()
    {
        StartCoroutine(SaveNextFrame());
    }

    private IEnumerator SaveNextFrame()
    {
        yield return null; // espera 1 frame para que ya existan las armas iniciales, esto es solo porque el start de cuando inicia la ronda entra antes de que se creen las armas por eso espero un 1 segundos antes de guardar
        //lastCheckpoint = session.CreateMemento();
        Debug.Log("[Checkpoint] Guardado: ronda " + lastCheckpoint.Round + " | oro " + lastCheckpoint.Gold);
    }

    public bool RetryFromCheckpoint() // esto lo usa el GameOverUi
    {
        if (lastCheckpoint == null) return false; // Proteccion

        foreach (Enemy e in FindObjectsByType<Enemy>(FindObjectsSortMode.None))  // Borra todos los objetos que tengan el script Enemy y los devueve como lista
                                                                                 // Uso el sortMode porque no me improta el orden solo es mas rapido porque unity no tiene que ordenarlos
            Destroy(e.gameObject); // Destruyo el GO del enemy 

        foreach (Bullet b in FindObjectsByType<Bullet>(FindObjectsSortMode.None))  // Encuentro la sbalas activas y las reciclo con la funcion del pool recycle
        {
            if (b.pool != null) b.pool.Recycle(b);
            else b.gameObject.SetActive(false);
        }

        session.Restore(lastCheckpoint, healFullOnRetry);// Le paso el guardado a GameSession que devuelve el juego a ese estado 
        return true;
    }
}