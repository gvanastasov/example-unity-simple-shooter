using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float Speed = 100f;
    public int Lifespan = 1;

    void Start()
    {
        Destroy(this.gameObject, this.Lifespan);
    }

    void Update()
    {
        this.transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }
}
