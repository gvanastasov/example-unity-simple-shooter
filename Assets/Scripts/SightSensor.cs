using System;
using System.Collections;
using UnityEngine;

public class SightSensor : MonoBehaviour
{
    public float SenseDelay = 0.2f;
    public float distance;
    public float angle;
    public LayerMask obstacles;
    public GameObject detectedObject;

    private Transform player;

    void Awake()
    {
        player = FindObjectOfType<PlayerBehaviour>()?.transform;
    }

    void Start()
    {
        StartCoroutine("SenseDelayCoroutine", this.SenseDelay);
    }

    IEnumerator SenseDelayCoroutine(float delay) 
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            Sense();
        }
    }

    void Sense()
    {
        detectedObject = null;
        if (Vector3.Distance(this.transform.position, player.transform.position) < distance)
        {
            Vector3 directionToObject = Vector3.Normalize(player.transform.position - transform.position);
            float angleToObject = Vector3.Angle(transform.forward, directionToObject);
            if (angleToObject < angle / 2)
            {
                if (!Physics.Linecast(transform.position, player.transform.position, obstacles))
                {
                    detectedObject = player.gameObject;
                }
            }
        }
    }
}
