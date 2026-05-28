using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class magic : MonoBehaviour
{
    public Transform magical;
    public void Summon()
    {
        Vector3 localpoint = new Vector3(2f, 0, 0);
        
        localpoint = transform.TransformPoint(2,0,2);
        magical.position = localpoint;

        
    }
    void Start()
    {

        magical.InverseTransformDirection(magical.position);
        Summon();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
class Monster
{
    public string name;
    public string hp;
    public Monster(string name, string hp)
    {
        this.name = name;
        this.hp = hp;
    }
}