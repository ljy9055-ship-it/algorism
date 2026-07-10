using UnityEngine;
using UnityEngine.Playables;

public class MonsterStatus : MonoBehaviour
{
    public Status monsterstatus = new Status();
    public State monsterstate = new State();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        monsterstate = State.Idle;
    }
    public void Idle()
    {
        monsterstate = State.Idle;
    }
    public void Fight()
    {
        monsterstate = State.Fight;
    }
    public void Die()
    {
        if (monsterstatus.Hp <= 0)
        {
            monsterstatus.Hp = 0;
            monsterstate = State.Dead;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
