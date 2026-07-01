using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public Status playerstatus;
    public State playerstate;
    
    
    void Start()
    {
        playerstate = GetComponent<State>();
        playerstatus.Hp = GetComponent<int>();
        playerstatus.Mp = GetComponent<int>();
        playerstate = State.Idle;
        playerstatus.Hp = 0;
        playerstatus.Mp = 0;

    }

    public void Idle()
    {
        playerstate = State.Idle ;
    }
    public void Fight()
    {
        playerstate = State.Fight;
    }
   public void Die()
    {
        if (playerstatus.Hp <= 0)
        {
            playerstatus.Hp = 0;
            playerstate = State.Dead;
        }   
    }

    void Update()
    {
        
    }
}
