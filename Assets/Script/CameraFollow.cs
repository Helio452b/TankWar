using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public float distance;
    public float rot = 0;
    private float roll = 30f * Mathf.PI * 2 / 360;
    public GameObject target;
    private GameObject cameralPoint;

    // roll camera
    private float rotSpeed = 0.2f;

    // rotate camera
    private const float maxRoll = 70f * Mathf.PI * 2 / 360;
    private const float minRoll = -10f * Mathf.PI * 2 / 360;

    public float roolSpeed = 0.1f;

	// Use this for initialization
	void Start () {
        if (GameObject.Find("Tank") != null)
            target = GameObject.Find("Tank");
        cameralPoint = GameObject.Find("CameraPoint");

        distance =  Vector3.Distance(target.transform.position, transform.position);        
	}

    // zoom
    public float maxDistance = 22f;
    public float minDistance = 5f;
    public float m_zoomSpeed = 2f;

	// Update is called once per frame
	void LateUpdate () {
        if (target == null)                    
            return;
         
        Vector3 targetPos = target.transform.position;

        Vector3 cameraPos;
        float d = distance * Mathf.Sin(roll);
        float height = distance * Mathf.Sin(roll);

        cameraPos.x = targetPos.x + d * Mathf.Cos(rot);
        cameraPos.z = targetPos.z + d * Mathf.Sin(rot);
        cameraPos.y = targetPos.y + height;

        Camera.main.transform.position = cameraPos;

        Camera.main.transform.LookAt(cameralPoint.transform);

        Rotate();
        Roll();
        Zoom();
	}

    private void Rotate()
    {
        if (Input.GetMouseButton(1) == true)
        { 
            float w = Input.GetAxis("Mouse X")  * rotSpeed;
            rot -= w;
        }
    }

    private void Roll()
    {        
        float w = Input.GetAxis("Mouse Y") * roolSpeed * 0.5f;

        roll -= w;
        if (roll > maxRoll)
            roll = maxRoll;
        if (roll < minRoll)
            roll = minRoll;        
    }

    private void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (distance > minDistance)
                distance -= m_zoomSpeed;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (distance < maxDistance)
                distance += m_zoomSpeed;
        }
    }
}
