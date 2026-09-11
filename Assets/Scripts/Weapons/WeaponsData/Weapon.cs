using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private string id;

    public Sprite sprite;
    public string Id => id;
}
