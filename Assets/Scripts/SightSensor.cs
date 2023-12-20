using System.Collections;
using UnityEngine;

/// <summary>
/// The SightSensor is responsible for detecting the player.
/// </summary>
/// <remarks>
/// This could probably be improved a lot, but for the sake of demo purpose
/// it is good enough.
/// </remarks>
public class SightSensor : MonoBehaviour
{
#region Serializable Fields
    /// <summary>
    /// Fake delay between triggering the sensor in order to give some simple optimization.
    /// </summary>
    [Tooltip("Fake delay between triggering the sensor in order to give some simple optimization.")]
    public float SenseDelay = 0.2f;

    /// <summary>
    /// The distance of the sight.
    /// </summary>
    [Tooltip("The distance of the sight.")]
    public float Distance;

    /// <summary>
    /// The angle of the sight.
    /// </summary>
    /// <remarks>
    /// The angle is split in half, so if the angle is 90 degrees, the sensor will detect
    /// objects in a 45 degree angle.
    /// </remarks>
    [Tooltip("The angle of the sight.")]
    public float Angle;

    /// <summary>
    /// The obstacle layers that will block the sight.
    /// </summary>
    [Tooltip("The obstacle layers that will block the sight.")]
    public LayerMask Obstacles;

    /// <summary>
    /// The object of interest that was detected.
    /// </summary>
    /// <remarks>
    /// This is set to null if nothing was detected.
    /// This is currently only the player.
    /// </remarks>
    [Tooltip("The object of interest that was detected.")]
    public GameObject DetectedObject;
#endregion

#region Private Fields
    /// <summary>
    /// The player object.
    /// </summary>
    private Transform player;
#endregion

#region Unity Callbacks
    void Awake()
    {
        player = FindObjectOfType<PlayerBehaviour>()?.transform;
    }

    void Start()
    {
        StartCoroutine("SenseDelayCoroutine", this.SenseDelay);
    }
#endregion

#region Actions
    /// <summary>
    /// A coroutine that will trigger the logic behind the sight, once every <paramref name="delay"/> seconds.
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator SenseDelayCoroutine(float delay) 
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            Sense();
        }
    }

    /// <summary>
    /// The logic behind the sight. Currently is just a distance check between the sight bearer 
    /// and the player. Raycasting is used to check if there are any obstacles between the 
    /// sight bearer and the player.
    /// </summary>
    void Sense()
    {
        DetectedObject = null;
        if (Vector3.Distance(this.transform.position, player.transform.position) < Distance)
        {
            Vector3 directionToObject = Vector3.Normalize(player.transform.position - transform.position);
            float angleToObject = Vector3.Angle(transform.forward, directionToObject);
            if (angleToObject < Angle / 2)
            {
                if (!Physics.Linecast(transform.position, player.transform.position, Obstacles))
                {
                    DetectedObject = player.gameObject;
                }
            }
        }
    }
#endregion
}
