using UnityEngine;

public class EnemyBulletPool : PoolGeneric<EnemyBullet>
{
    public void RecycleByTimer(EnemyBullet bullet, float timer)
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            base.Recycle(bullet);
        }
    }
}
