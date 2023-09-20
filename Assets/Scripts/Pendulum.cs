using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum
{
    Vector3 location;
    public Vector3 origin = new Vector3 (0,0,0);
    public float r;
    public float angle;
    public float aVelocity = 10f;
    public float aAcceleration = 10f;
    public float damping = 0.995f;
    public Vector3 Swing()
    {
        float gravity = .4f;
        aAcceleration = -1 * gravity * Mathf.Sin(angle);
        aVelocity += aAcceleration;
        angle += aVelocity;
        location = new Vector3(r * Mathf.Sin(angle), r * Mathf.Cos(angle), 0f);
        location += origin;
        return location;
    }
}
