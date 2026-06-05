using UnityEngine;

public class Playerscript : MonoBehaviour
{
    public Transform Monster;
    public float sightangle = 60f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dirtomonster = (Monster.position - transform.position).normalized;
        float dot =Vector3.Dot(transform.forward, dirtomonster);
        float angle=Mathf.Acos(dot)*Mathf.Rad2Deg;

        if (angle < sightangle)
        {
            Debug.Log("몬스터 발견.");
            Vector3 cross = Vector3.Cross(transform.forward, dirtomonster);
            if (cross.y > 0)
            {
                Debug.Log("몬스터는 오른쪽에 있음");

            }
            else if(cross.y < 0)
            {
                Debug.Log("몬스터는 왼쪽에 있음");
            }
        }
    }
}
