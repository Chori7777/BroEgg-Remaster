using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Zombie : Enemy
{
   
    // Update is called once per frame
    void Update()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;

        rb.linearVelocity = direction * stats.Speed;
    }
}
