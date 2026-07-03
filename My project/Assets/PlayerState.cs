using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public Status playerstatus = new Status();
    public State playerstate = new State();
    
    
    void Start()
    {
        playerstate = State.Idle;
     
        

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
