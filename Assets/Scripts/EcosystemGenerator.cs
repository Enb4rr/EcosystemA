using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EcosystemGenerator : MonoBehaviour
{
    [Header("Flies")]
    [SerializeField] float flySpeed;
    [SerializeField] float flyTopSpeed;
    [SerializeField] GameObject flyPrefab;
    private Fly[] flies = new Fly[30];
    [SerializeField] Transform fliesFood;
    private Rigidbody fliesFoodBody;

    [Header("Snake")]
    [SerializeField] float snakeSpeed;
    [SerializeField] float snakeTopSpeed;
    [SerializeField] GameObject snakePrefab;
    private Snake[] snakes = new Snake[10];

    [Header("Shark")]
    [SerializeField] float sharkSpeed;
    [SerializeField] float SharkTopSpeed;
    [SerializeField] GameObject sharkPrefab;
    private Shark[] sharks = new Shark[7];

    // Serialize the components required to create the water
    [SerializeField] Transform fluidCornerA;
    [SerializeField] Transform fluidCornerB;
    [SerializeField] Material waterMaterial;
    [SerializeField] float fluidDrag;
    Fluid fluid;
    private void Awake()
    {
        fliesFoodBody = fliesFood.gameObject.GetComponent<Rigidbody>();
    }
    private void Start()
    {
        fluid = new Fluid(fluidCornerA.position, fluidCornerB.position, fluidDrag, waterMaterial);

        if (flyPrefab == null)
        {
            Debug.LogError("flyPrefab is not assigned. Please assign a valid prefab.");
            return;
        }

        for (int i = 0; i < flies.Length; i++)
        {
            flies[i] = new Fly(flySpeed, flyTopSpeed, flyPrefab);
        }
        for (int i = 0; i < snakes.Length; i++)
        {
            snakes[i] = new Snake(snakeSpeed, snakeTopSpeed, snakePrefab);
        }
        for (int i = 0; i < sharks.Length; i++)
        {
            sharks[i] = new Shark(sharkSpeed, SharkTopSpeed, sharkPrefab);
        }
    }

    void FixedUpdate()
    {
        //Get mouse position
        Vector3 mouseScreenPosition = Input.mousePosition;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition = new Vector3 (mouseWorldPosition.x, mouseWorldPosition.y, 0f);
        //Flies
        Vector2 foodPos = new Vector2(fliesFood.position.x, fliesFood.position.y);
        Vector2 dir;
        int randomIndex = 0;
        for (int i = 0; i < flies.Length; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, 10);
            Vector2 deltaPos = foodPos - flies[i].location;
            if (true)
            {
                Vector3 force = flies[i].CalculateAttractionForce(fliesFoodBody);
                dir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
                flies[i].acceleration = dir.normalized * 2;
                flies[i].body.AddForce(force, ForceMode.Force);
                flies[i].CheckEdges();
            }
        }
        //Snakes
        for (int i = 0; i < snakes.Length; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, 2);
            int randomDirection = UnityEngine.Random.Range(-40, 40);

            if ( randomIndex== 0 )
            {
                dir = new Vector2(Mathf.Sin(Time.deltaTime+ randomDirection)*100, Mathf.Cos(Time.deltaTime));
                snakes[i].acceleration = dir.normalized *5;
            }
            else 
            {
                dir = new Vector2(Mathf.Sin(Time.deltaTime+ randomDirection)*100, Mathf.Sin(Time.deltaTime));
                snakes[i].acceleration = dir.normalized *5;
            }
            snakes[i].body.AddForce(snakes[i].acceleration * snakeSpeed, ForceMode.Force);
            if (snakes[i].IsinsideFluid(fluid))
            {
                Debug.Log("isInside");
                snakes[i].body.AddForce(-snakes[i].body.velocity.normalized*fluid.dragCoefficient, ForceMode.Force);
            }


            snakes[i].CheckEdges();
        }
        //Sharks
        for (int i = 0; i < sharks.Length; i++)
        {

            if (Mathf.Abs(mouseWorldPosition.x - sharks[i].location.x) < 30f && Mathf.Abs(mouseWorldPosition.y - sharks[i].location.y) < 30f)
            {
                sharks[i].body.AddForce(sharks[i].CalculateAttractionForce(mouseWorldPosition)*3.5f, ForceMode.Impulse);
            }
            else
            {
                randomIndex = UnityEngine.Random.Range(0, 200);
                int randomDirection = UnityEngine.Random.Range(-1, 1);
                float randomLevyScalar = UnityEngine.Random.Range(20, 40);
                if (randomIndex == 1)
                {
                    dir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
                    sharks[i].body.AddForce(dir.normalized * UnityEngine.Random.Range(10f, 40f) * randomDirection * randomLevyScalar, ForceMode.Impulse);
                }
                else
                {
                    dir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
                    sharks[i].acceleration = dir.normalized * 2 * randomDirection;
                    sharks[i].body.AddForce(sharks[i].acceleration*sharkSpeed*100, ForceMode.Force);
                    
                }
            }
            sharks[i].CheckEdges();
        }
    }
}

public class Mover
{
    public Vector2 location, velocity;
    public Vector2 acceleration;
    protected float topSpeed;

    protected Vector2 maximumPos;

    protected GameObject mover;

    public Rigidbody body;

    public Mover(GameObject moverPrefab)
    {
        FindWindowLimits();
        location = new Vector2(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-4f, 4f));
        velocity = Vector2.zero;
        acceleration = Vector2.zero;
        topSpeed = 1f;
        mover = GameObject.Instantiate(moverPrefab,location,Quaternion.Euler(90,0,180)); // Instantiate the moverPrefab and assign it to the mover variable
        body  = mover.GetComponent<Rigidbody>();
    }

    public virtual void Step()
    {
        location = body.position;
        if (velocity.magnitude <= topSpeed)
        {
            // Speeds up the mover
            velocity += acceleration * Time.deltaTime;

            // Limit Velocity to the top speed
            velocity = Vector2.ClampMagnitude(velocity, topSpeed *1.3f);

            // Moves the mover
            location += velocity * Time.deltaTime;

            // Updates the GameObject of this movement
            mover.transform.position = new Vector3(location.x, location.y, 0);
            //body.velocity = velocity*2;
        }
        else
        {
            velocity += acceleration * Time.deltaTime;
            location += velocity * Time.deltaTime;
            mover.transform.position = new Vector3(location.x, location.y, 0);
            //body.velocity = velocity * 2;
        }
    }
    public Vector3 CalculateAttractionForce(Rigidbody targetBody)
    {
        Vector3 randomoffset = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), 0f);
        Vector3 force = targetBody.transform.position - mover.transform.position+randomoffset;
        float distance = force.magnitude;
        distance = Mathf.Clamp(distance, 8f, 15f);
        int random = UnityEngine.Random.Range(0, 6);
        if (random == 5)
        {
            force = new Vector3(UnityEngine.Random.Range(-100f, 100f), UnityEngine.Random.Range(-100f, 100f), 0f);
        }
        force.Normalize();
        float strength = 60f * (body.mass * targetBody.mass) / (distance * distance);
        force *= strength;

        // Draw the force vector as a line from mover's position in the direction of the force
        Debug.DrawLine(mover.transform.position, mover.transform.position + force *5, Color.red);

        return force;
    }
    public Vector3 CalculateAttractionForce(Vector3 target)
    {
        Vector3 force =target - mover.transform.position;
        float distance = force.magnitude;
        distance = Mathf.Clamp(distance, 8f, 20f);
        force.Normalize();
        float strength = 20f * (body.mass * 1) / (distance * distance);
        force *= strength;
        return force;
    }
    public void CheckEdges()
    {
        Vector2 velocity = body.velocity;
        if (mover.transform.position.x > maximumPos.x || mover.transform.position.x < -maximumPos.x)
        {
            velocity.x *= -0.8f;
        }
        if (mover.transform.position.y > maximumPos.y || mover.transform.position.y < -maximumPos.y)
        {
            velocity.y *= -0.8f;
        }
        body.velocity = velocity;
    }
    public void FindWindowLimits()
    {
        Camera.main.orthographic = true;
        maximumPos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }
}

public class Fly : Mover
{
    public Fly(float flySpeed, float flyTopSpeed, GameObject flyObject) : base(flyObject)
    {
        location = new Vector2(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
    }
}

public class Snake : Mover
{
    public Snake(float SnakeSpeed, float snakeTopSpeed, GameObject SnakeObject) : base(SnakeObject)
    {
        location = new Vector2(UnityEngine.Random.Range(-10f, 20f), UnityEngine.Random.Range(-10f, 10f));
    }
    public bool IsinsideFluid(Fluid fluid)
    {
        if (body.position.x > fluid.minBoundary.x &&
        body.position.x < fluid.maxBoundary.x &&
        body.position.y > fluid.minBoundary.y &&
        body.position.y < fluid.maxBoundary.y &&
        body.position.z > fluid.minBoundary.z &&
        body.position.z < fluid.maxBoundary.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
public class Shark: Mover
{
    public Shark(float sharkSpeed, float sharkTopSpeed, GameObject sharkObject) : base(sharkObject)
    {
        location = new Vector2(UnityEngine.Random.Range(-10f, 20f), UnityEngine.Random.Range(10f, -10f));
        float rotation = sharkObject.transform.rotation.z;
         rotation= 180;
        //sharkObject.transform.eulerAngles.z = rotation;
    }
}
