using UnityEngine;

// Calcula hacia donde hay que disparar para que la bala y el enemigo lleguen al mismo punto.
// Es static y no guarda estado, igual que DamageCalculator: solo recibe datos y devuelve un resultado.
public static class AimPredictor
{
    // Devuelve el punto del mundo donde la bala (velocidad constante) se cruza con el enemigo (velocidad constante)
    // Si no se puede predecir, devuelve la posicion actual del enemigo (disparo directo)
    public static Vector2 GetInterceptPoint(Vector2 shooterPosition, Vector2 targetPosition, Vector2 targetVelocity, float bulletSpeed)
    {
        // si la bala no tiene velocidad o el enemigo esta quieto, apuntamos directo y listo
        if (bulletSpeed <= 0f || targetVelocity.sqrMagnitude < 0.0001f) return targetPosition;

        Vector2 toTarget = targetPosition - shooterPosition;

        // Buscamos el tiempo t en el que la distancia que recorre la bala (bulletSpeed * t)
        // es igual a la distancia al enemigo ya movido: |toTarget + targetVelocity * t| = bulletSpeed * t
        // Elevando al cuadrado queda una ecuacion de segundo grado: a*t^2 + b*t + c = 0
        float a = Vector2.Dot(targetVelocity, targetVelocity) - bulletSpeed * bulletSpeed;
        float b = 2f * Vector2.Dot(toTarget, targetVelocity);
        float c = Vector2.Dot(toTarget, toTarget);

        float time;

        if (Mathf.Abs(a) < 0.0001f)
        {
            // la bala y el enemigo van igual de rapido, la ecuacion deja de ser cuadratica y queda lineal
            if (Mathf.Abs(b) < 0.0001f) return targetPosition;
            time = -c / b;
        }
        else
        {
            float discriminant = b * b - 4f * a * c;

            // si el discriminante es negativo no hay solucion: la bala nunca lo alcanzaria
            if (discriminant < 0f) return targetPosition;

            float root = Mathf.Sqrt(discriminant);
            float t1 = (-b - root) / (2f * a);
            float t2 = (-b + root) / (2f * a);
            time = SmallestPositive(t1, t2);
        }

        // un tiempo negativo significa que el punto esta "en el pasado", no sirve
        if (time <= 0f) return targetPosition;

        // la posicion del enemigo despues de "time" segundos
        return targetPosition + targetVelocity * time;
    }

    // De las dos soluciones nos quedamos con la menor que sea positiva (la que llega primero)
    private static float SmallestPositive(float t1, float t2)
    {
        if (t1 > 0f && t2 > 0f) return Mathf.Min(t1, t2);
        if (t1 > 0f) return t1;
        if (t2 > 0f) return t2;
        return -1f;
    }
}