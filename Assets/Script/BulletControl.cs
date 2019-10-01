using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour {

    public float speed = 100f;
    public GameObject explode;
    public float maxLifeTime = 2f;
    public float instantiateTime = 0f;

	// Use this for initialization
	void Start () {
        instantiateTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (Time.time - instantiateTime > maxLifeTime)
            Destroy(gameObject);
	}

    void OnCollisionEnter(Collision collisionInfo)
    {
        Instantiate(explode, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
