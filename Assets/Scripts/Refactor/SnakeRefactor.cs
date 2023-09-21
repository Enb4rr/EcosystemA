using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class SnakeRefactor : AnimalRefactor
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

    //Timers
    private float randomDirectionTimer = 0;
    private float randomDirectionTime = 3;
    //Wave based movement
    private float currentAngle = 0.0f;
    //Generate Snake
    public SnakeRefactor(AnimalRaze raze, AnimalSex sex, Behaviour foodType, AnimalRaze predator, float energy) : base(raze, sex, foodType, predator, energy)
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
        //Manage collisions with trees and snakes
        var animalComponent = other.GetComponent<AnimalRefactor>();

        if (animalComponent != null)
        {
            if (other.CompareTag("Monkey"))
            {
                if (Energy < 80 || state == AnimalState.Eating)
                {
                    target = other.gameObject;
                    state = AnimalState.Eating;
                    HandleHunger();
                    float distance;
                    if (target != null) {
                        distance = Vector3.Distance(target.transform.position, transform.position);
                        if (distance < 2)
                        {
                            animalComponent.HandleDeath();
                        }
                    }
                }
            }
            else if (animalComponent.Raze == Predator)
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

        }
        if (animalComponent != null)
        {
            if (animalComponent.Raze == Predator)
            {
                target = null;
                state = AnimalState.Idle;
            }
            else if (other.CompareTag("Monkey"))
            {
                if (state == AnimalState.Eating)
                {
                    state = AnimalState.Idle;
                    target = null;
                }
            }
        }
    }
    public override void Initialize()
    {
        Raze = AnimalRaze.Snake;
        int randomIndex = Random.Range(0, 1);
        if (randomIndex == 0) Sex = AnimalSex.Male;
        else Sex = AnimalSex.Female;
        FoodType = Behaviour.Carnivorous;
        Predator = AnimalRaze.None;
        Energy = 60;
    }

    //Handle snake loop behaviour
    public override void HandleAnimalBehaviour()
    {
        HandleMovement(Vector3.zero);
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
        }
        else if (randomDirectionTimer >= randomDirectionTime)// calculate random direction 
        {
            randomDirectionTime = Random.Range(1, 3);
            randomDirectionTimer = 0;
            Vector3 randomDirection = new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f));
            Acceleration = randomDirection.normalized;
        }
        if (Velocity.magnitude >= 5.5f)
        {
            Velocity *= 0.6f;
        }
        currentAngle += Time.deltaTime;
        float zOffset = Mathf.Sin(currentAngle*7) / 18;
        Vector3 offset = new Vector3(0, 0, zOffset);
        Acceleration += offset;
        Velocity += Acceleration * 1.5f;
        //CheckEdges();
        //add the sinosodial offset
        gameObject.transform.position += Velocity * Time.deltaTime / 4;
        transform.position += offset;
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

    //Handle hungry state, look for nearest monkey, move to that direction using base movement
    public override void HandleHunger()
    {
        HandleMovement(target.transform.position);
        if(target.GetComponent<MonkeyRefactor>().state == AnimalState.Dead)
        {
            Eat(3);
            if (Energy >= 100)
            {
                Energy = 100;
                target = null;
                state = AnimalState.Idle;
            }
        }
    }

    //Handle death
    public override void HandleDeath()
    {
        Debug.Log("Animal is dead");
        state = AnimalState.Dead;
        target = null;
        GetComponentInChildren<SkinnedMeshRenderer>().material = deathMaterial;
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
