using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel; // leftWheel
    public WheelCollider rightWheel; // rightWheel
    public bool motor;
    public bool steering;	 
}
