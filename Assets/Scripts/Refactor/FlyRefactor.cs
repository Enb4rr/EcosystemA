using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VR;
using UnityEngine;

public class FlyRefactor : AnimalRefactor
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

    //Spring Variables 
    float anchorPosition = 5;
    float springConstantK = 3.5f;
    float restLength = 0.5f;
    //Timers
    private float randomDirectionTimer = 0;
    private float randomDirectionTime = 1;
    //Generate monkey
    public FlyRefactor(AnimalRaze raze, AnimalSex sex, Behaviour foodType, AnimalRaze predator, float energy) : base(raze, sex, foodType, predator, energy)
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
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 newPos = transform.position;
            newPos.y -= 1.5f;
            transform.position = newPos;    
        }
    }
    private void FixedUpdate()
    {
        //Check if dead
        if (state == AnimalState.Dead) return;

        //Check if eating
        if (state == AnimalState.Eating) return;

        //Handle other states
        if (state == AnimalState.Idle) HandleAnimalBehaviour();
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

        }
        else if (animalComponent != null)
        {
            if (other.CompareTag("Monkey") )
            {
                if (Energy <= 60 && other.GetComponent<MonkeyRefactor>().state == AnimalState.Dead)
                {
                    target = other.gameObject;
                    state = AnimalState.Eating;
                    HandleHunger();
                }
            }
            else if (other.CompareTag("Snake"))
            {
                if (Energy <= 60 && other.GetComponent<SnakeRefactor>().state == AnimalState.Dead)
                {
                    target = other.gameObject;
                    state = AnimalState.Eating;
                    HandleHunger();
                }
            }
            if (animalComponent.Raze == Predator)
            {
                Debug.Log("Runnig from a target");
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
            if (other.CompareTag("Monkey") || other.CompareTag("Snake"))
            {
                if (state == AnimalState.Eating)
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
        Raze = AnimalRaze.Fly;
        int randomIndex = Random.Range(0, 1);
        if (randomIndex == 0) Sex = AnimalSex.Male;
        else Sex = AnimalSex.Female;
        FoodType = Behaviour.Carnivorous;
        Predator = AnimalRaze.None;
        Energy = 100;
    }

    //Handle Fly loop behaviour
    public override void HandleAnimalBehaviour()
    {
        //Start a movement where the fly moves to random positions with basic movement, react to collisions
        HandleMovement(Vector3.zero);
        //After each interaction with collisions fly must return to HandleAnimalBehaviour() state
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
        else if (randomDirectionTimer >= randomDirectionTime)// calculate random direction 
        {
            randomDirectionTime = Random.Range(1, 3);
            randomDirectionTimer = 0;
            Vector3 randomDirection = new Vector3(Random.Range(-100f, 100f), 0,  Random.Range(-100f, 100f));
            Acceleration = randomDirection.normalized;
        }
        if (Velocity.magnitude >= 3.5)
        {
            Velocity *= 0.6f;
        }


        //CheckEdges();
        //Spring Behaviour;
        Vector3 force = new Vector3(0, transform.position.y - 5,0);
        Debug.Log(force);
        float currentLength = force.magnitude;
        float stretchLength = currentLength - restLength;
        force = -force * 0.1f * stretchLength;
        Acceleration += force;
        Acceleration *= 0.9f;
        Velocity += Acceleration;
        //body.AddForce(Acceleration,ForceMode.Acceleration);
        //body.AddForce(force, ForceMode.Impulse);
        if(Velocity.magnitude > 100)
        {
            Velocity.Normalize();
            Velocity *= 100;
        }
        body.position += Velocity * Time.deltaTime / 4;
        float angle = Mathf.Atan2(Velocity.x, Velocity.z);
        Turn(angle);
        Debug.DrawRay(gameObject.transform.position, Acceleration * 4);
    }

    //Handle being attacked coroutine, use base movement to move to new direction
    public override void HandleAttack()
    {
        Debug.Log("Handle attacked");
        HandleMovement(target.transform.position);
    }

    //Handle hungry state, look for nearest dead body, move to that direction using base movement
    public override void HandleHunger()
    {
        Eat(3);
        Debug.Log("food Eat + energy" + Energy);
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
