using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fluid 
{
    public Vector3 minBoundary;
    public Vector3 maxBoundary;
    public float dragCoefficient;

    public Fluid(Vector3 corner1, Vector3 corner2, float dragCoefficient, Material material)
    {
        // Get the minimum and maximum corners of the rectangular prism
        // This code allows the designer to place the volume corners at
        // any of the eight possible diagonals of a rectangular prism.
        minBoundary = new Vector3(
            Mathf.Min(corner1.x, corner2.x),
            Mathf.Min(corner1.y, corner2.y),
            Mathf.Min(corner1.z, corner2.z)
        );
        maxBoundary = new Vector3(
            Mathf.Max(corner1.x, corner2.x),
            Mathf.Max(corner1.y, corner2.y),
            Mathf.Max(corner1.z, corner2.z)
        );
        this.dragCoefficient = dragCoefficient;

        // Create the presence of the object in 3D space
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.GetComponent<Renderer>().material = material;

        // Remove undesired components that come with the primitive
        obj.GetComponent<BoxCollider>().enabled = false;
        Object.Destroy(obj.GetComponent<BoxCollider>());

        // Position and scale the new cube to match the boundaries.
        obj.transform.position = (corner1 + corner2) / 2;
        obj.transform.localScale = new Vector3(
            Mathf.Abs(corner2.x - corner1.x),
            Mathf.Abs(corner2.y - corner1.y),
            Mathf.Abs(corner2.z - corner1.z)
        );
    }
}
