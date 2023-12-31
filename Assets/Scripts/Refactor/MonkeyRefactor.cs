using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditorInternal.VR;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class MonkeyRefactor : AnimalRefactor
{
    //This code goes in a prefab, it must have the components that are marked as required in base class

    private BoxCollider boxCollider;
    private SphereCollider sphereCollider;
    private Rigidbody body;
    private Animator animator;

    //Animal variables
    [SerializeField] public AnimalState state;
    public GameObject target;
    [SerializeField] Material deathMaterial;

    //Swing Variables 
    [SerializeField] private float aVelocity;
    [SerializeField] private float aAcceleration;
    [SerializeField] private float damping;
    [SerializeField] private float radious;
    [SerializeField] private float angle;
    //Timers
    private float randomDirectionTimer = 0;
    private float randomDirectionTime =1;
    //Generate monkey
    public MonkeyRefactor(AnimalRaze raze, AnimalSex sex, Behaviour foodType, AnimalRaze predator, float energy) : base(raze, sex, foodType, predator, energy)
    {
    }

    private void Awake()
    {
        Initialize();

        //Get required components from GameObject
        boxCollider = GetComponent<BoxCollider>();
        sphereCollider = GetComponent<SphereCollider>();
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        state = AnimalState.Idle;
    }

    private void FixedUpdate()
    {
        //Check if dead
        if (state == AnimalState.Dead) return;

        //Check if eating
        if (state == AnimalState.Eating) return;

        //Handle other states
        if (state == AnimalState.Idle) HandleAnimalBehaviour();
        else if (state == AnimalState.Playing) HandleSwinging();
        else if (state == AnimalState.Running) HandleAttack();

        //Energy loss
        EnergyLoss();
    }
    private void OnTriggerStay(Collider other)
    {
        //Check if dead
        if (state == AnimalState.Dead) return;
        var animalComponent = other.GetComponent<AnimalRefactor>();

        if (animalComponent == null) 
        {
            if (other.CompareTag("Tree"))
            {
                if (Energy > 80) 
                {
                    target = other.gameObject;
                    state = AnimalState.Playing;
                }
                else if (Energy <= 40)
                {
                    target = other.gameObject;
                    state = AnimalState.Eating;
                    HandleHunger();
                }
            }
        }
        else if(animalComponent != null)
        {
            Debug.Log("Runnig from a target");
            if (animalComponent.Raze == Predator)
            {
                target = other.gameObject;
                state = AnimalState.Running;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //Check if dead
        if (state == AnimalState.Dead) return;
        var animalComponent = other.GetComponent<AnimalRefactor>();
        if (animalComponent == null)
        {
            if (other.CompareTag("Tree"))
            {
                if(state == AnimalState.Playing)
                {
                    //Reset the animal position y
                    state = AnimalState.Idle;
                    Vector3 outPosition = new Vector3(transform.position.x, 0, transform.position.z);
                    transform.position = outPosition;
                    target = null;
                }
                else if (state == AnimalState.Eating)
                {
                    state = AnimalState.Idle;
                    target = null;
                }
            }
        }
        if (animalComponent != null)
        {
            if (animalComponent.Raze == Predator)
            {
                target = null;
                state = AnimalState.Idle;
            }
        }
    }
    public override void Initialize()
    {
        Raze = AnimalRaze.Monkey;
        int randomIndex = Random.Range(0, 1);
        if (randomIndex == 0) Sex = AnimalSex.Male;
        else Sex = AnimalSex.Female;
        FoodType = Behaviour.Herbivorous;
        Predator = AnimalRaze.Snake;
        Energy = 100;
    }

    //Handle monkey loop behaviour
    public override void HandleAnimalBehaviour()
    {
        //Start a movement where the monkey moves to random positions with basic movement, react to collisions
        HandleMovement(Vector3.zero);
        //After each interaction with collisions monkey must return to HandleAnimalBehaviour() state
    }

    //Handle move coroutine
    public override void HandleMovement(Vector3 targetPosition)
    {
        randomDirectionTimer += Time.deltaTime;
        if ((gameObject.transform.position - targetPosition).magnitude <= 1)
        {
            return;
        }
        if (targetPosition != Vector3.zero)
        {
            Acceleration = CalculateDirection(targetPosition);
            if (state == AnimalState.Running) Acceleration *= -1;
        }
        else if(randomDirectionTimer>= randomDirectionTime)// calculate random direction 
        {
            randomDirectionTime = Random.Range(1, 3);
            randomDirectionTimer = 0;
            Vector3 randomDirection = new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f));
            Acceleration = randomDirection.normalized;
        }
        if (Velocity.magnitude >= 3.5)
        {
            Velocity *= 0.6f;
        }
        Velocity += Acceleration;
        //CheckEdges();

        transform.position += Velocity * Time.deltaTime/4;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        float angle = Mathf.Atan2(Velocity.x, Velocity.z);
        Turn(angle);
        Debug.DrawRay(gameObject.transform.position, Acceleration * 4);
    }

    //Handle swinging coroutine
    public override void HandleSwinging()
    {
        Vector3 swingOrigin = target.transform.position;
        swingOrigin.y += radious;
        aAcceleration = (-4f / radious) * Mathf.Sin(angle);
        aVelocity += aAcceleration * Time.deltaTime;
        aVelocity *= damping;

        angle += aVelocity*Time.deltaTime;

        float y = swingOrigin.y + radious * Mathf.Sin(angle - 90 * Mathf.Deg2Rad);
        float z = swingOrigin.z + radious * Mathf.Cos(angle - 90 * Mathf.Deg2Rad);

        transform.localPosition = new Vector3(transform.position.x, y, z);
       
    }
    //Handle being attacked coroutine, use base movement to move to new direction
    public override void HandleAttack()
    {
        Debug.Log("Handle attacked");
        HandleMovement(target.transform.position);
    }

    //Handle hungry state, look for nearest tree, move to that direction using base movement
    public override void HandleHunger()
    {
        Eat(3);
        Debug.Log("food Eat + energy"+ Energy);
        HandleMovement(target.transform.position);
        if (Energy >= 100)
        {
            Energy = 100;
            target = null;
            state = AnimalState.Idle;
        }
    }

    //Handle death
    public override void HandleDeath()
    {
        Debug.Log("Animal is dead");
        state = AnimalState.Dead;
        target = null;
        target = null;
        GetComponentInChildren<SkinnedMeshRenderer>().material = deathMaterial;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        body.constraints = RigidbodyConstraints.FreezeAll;
        Destroy(gameObject, 20f);
    }

    public override void EnergyLoss()
    {
        //Lose 1 energy each second
        Energy -= Time.deltaTime;
        if (Energy <= 0) 
        {
            StopAllCoroutines();
            HandleDeath();
        }
    }
}
