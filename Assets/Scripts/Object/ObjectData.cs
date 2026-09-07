using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Object", menuName = "Scriptable Objects/Object")]
public class Object : ScriptableObject
{
    [SerializeField] private string idObject;

    [SerializeField] private Image spriteShop;

    [SerializeField] private int baseHealth;
    [SerializeField] private int baseArmor;
    [SerializeField] private int baseDamage;
    [SerializeField] private int baseSpeed;




    //Gets


    public int BaseHealth => baseHealth;
    public int BaseArmor => baseArmor;
    public int BaseDamage => baseDamage;
    public int BaseSpeed => baseSpeed;




}
