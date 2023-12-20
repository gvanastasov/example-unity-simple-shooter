using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SightSensor))]
public class SightSensorEditor : Editor
{
    void OnSceneGUI() 
    {
        var ss = (SightSensor)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(ss.transform.position, Vector3.up, Vector3.forward, 360, ss.Distance);

        Vector3 viewAngleA = GetSightPoint(ss.transform, ss.Distance, -ss.Angle / 2);
		Vector3 viewAngleB = GetSightPoint(ss.transform, ss.Distance, ss.Angle / 2);

        Handles.DrawLine(ss.transform.position, ss.transform.position + viewAngleA);
        Handles.DrawLine(ss.transform.position, ss.transform.position + viewAngleB);

		Handles.color = Color.red;
        if (ss.DetectedObject != null)
        {
			Handles.DrawLine(ss.transform.position, ss.DetectedObject.transform.position);
		}
    }

    private Vector3 GetSightPoint(Transform origin, float radius, float deg)
    {
        float rad = Mathf.Deg2Rad * (deg - origin.eulerAngles.y + 90);

        float x = radius * Mathf.Cos(rad);
        float z = radius * Mathf.Sin(rad);

        return new Vector3(x, 0, z);
    }
}
