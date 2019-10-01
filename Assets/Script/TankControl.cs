using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankControl : MonoBehaviour {

    // tank
    private Transform gun;
    private Transform tank;
    public float rotateSpeed = 20;
    public float moveSpeed = 20;

    // 轮轴
    public List<AxleInfo> axleInfos;

    // 电机扭矩
    [SerializeField]private float m_curMortorTorque;
    public float m_maxMotroTorque = 120;

    // 旋转角度
    private float m_curSteerAngle;
    public float m_maxSteerAngle = 40;

    // 制动扭矩
    private float m_curBrakeTorque;
    public float m_maxBrakeTorque = 100;

    // 炮台
    private Transform turret;
    private float turretRotateTarget = 0;
    private float turretRotateSpeed = 0.5f;

    // 轮子
    private Transform wheels;

    // 轨道
    private Transform tracks;

    // 音效
    private AudioSource motorAudioSource;
    // public AudioClip motorAudioClip;

    // 炮弹
    public GameObject bullet;
    public float lastShootTime = 0;
    private float shootInterval = 0.5f;

	void Start () {
        turret = transform.Find("Turret");

        wheels = transform.Find("Wheels");

        tracks = transform.Find("Tracks");
        
        gun = turret.Find("Gun");
 
        motorAudioSource = gameObject.GetComponent<AudioSource>();
        motorAudioSource.spatialBlend = 1;
	}
	
	void Update () {
        // 获取输入
        m_curMortorTorque = Input.GetAxis("Vertical") * m_maxMotroTorque;
        m_curSteerAngle = Input.GetAxis("Horizontal") * m_maxSteerAngle;

        TankeMove();
        TankBrake();
        if (axleInfos[1].leftWheel != null)
            WheelRotation(axleInfos[1].leftWheel);
 
        // get the camera eulerAngle
        turretRotateTarget = Camera.main.transform.eulerAngles.y;
        
        TurretRotate();

        if (axleInfos[1] != null)
            TrackMove();

        MotorSound();

        // 发射炮弹
        if (Input.GetMouseButton(0))
            Shoot();
    }

    /// <summary>
    /// 坦克移动
    /// </summary>
    private void TankeMove()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = m_curMortorTorque;
                axleInfo.rightWheel.motorTorque = m_curMortorTorque;                
            }

            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = m_curSteerAngle;
                axleInfo.rightWheel.steerAngle = m_curSteerAngle;
            }
        }
    }

    /// <summary>
    /// 制动
    /// </summary>
    private void TankBrake()
    {
        m_curBrakeTorque = 0;

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.leftWheel.rpm > 5 && m_curMortorTorque < 0)
                m_curBrakeTorque = m_maxBrakeTorque;
            else if (axleInfo.leftWheel.rpm < -5 && m_curBrakeTorque > 0)
                m_curBrakeTorque = m_maxBrakeTorque;
        }
    }

     
    /// <summary>
    /// 左右旋转炮塔
    /// </summary>
    private void TurretRotate()
    {
        float rotateAngle = turret.eulerAngles.y - turretRotateTarget;

        if (rotateAngle < 0)
            rotateAngle += 360;

        if (rotateAngle > turretRotateSpeed && rotateAngle < 180)
            turret.Rotate(0f, -turretRotateSpeed, 0f);
        else if (rotateAngle > 180 && rotateAngle < 360 - turretRotateSpeed)
            turret.Rotate(0f, turretRotateSpeed, 0f);
    }

    /// <summary>
    /// 旋转轮子
    /// </summary>
    /// <param name="collider"></param>
    private void WheelRotation(WheelCollider collider)
    {
        if (collider == null)
            return;

        Vector3 positon = new Vector3();
        Quaternion rotation;
        collider.GetWorldPose(out positon, out rotation);

        foreach (Transform wheel in wheels)
        {
            wheel.rotation = rotation;
        }
    }

    /// <summary>
    /// 移动坦克的链条
    /// </summary>
    private void TrackMove()
    {
        if (tracks == null)
            return;

        float offset = 0;
        if (wheels.GetChild(0) != null)
            offset = wheels.GetChild(0).localEulerAngles.x / 90f;

        foreach (Transform track in tracks)
        {
            MeshRenderer mr = track.gameObject.GetComponent<MeshRenderer>();
            if (mr == null) continue;
            Material mtl = mr.material;
            mtl.mainTextureOffset = new Vector2(0, offset);
        }
    }

    /// <summary>
    /// 坦克的声音控制
    /// </summary>
    private void MotorSound()
    {
        if (Mathf.Abs(axleInfos[1].leftWheel.rpm) > 5 && !motorAudioSource.isPlaying)
        {
            motorAudioSource.loop = true;
            // motorAudioSource.clip = motorAudioClip;
            motorAudioSource.Play();
        }
        else if (Mathf.Abs(axleInfos[1].leftWheel.rpm) < 5)
            motorAudioSource.Pause();
    }

    private void Shoot()
    {
        if (bullet == null)
            return;

        if (Time.time - lastShootTime > shootInterval)
        {  
            Vector3 bulletPos = gun.position + gun.forward * 5;
            Instantiate(bullet, bulletPos, gun.rotation);
            lastShootTime = Time.time;
        }
    }
}
