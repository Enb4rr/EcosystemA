using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MonkeyRefactor : AnimalRefactor
{
    //This code goes in a prefab, it must have the components that are marked as required in base class

    private BoxCollider boxCollider;
    private SphereCollider sphereCollider;
    private Rigidbody body;
    private Animator animator;

    //Animal variables
    [SerializeField] AnimalState state;
    public GameObject target;

    //Generate monkey
    public MonkeyRefactor(AnimalRaze raze, AnimalSex sex, Behaviour foodType, float energy) : base(raze, sex, foodType, energy)
    {
        Acceleration = 0;
        Velocity = 0;
        TopSpeed = 0;
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
        if (state == AnimalState.Dead) return;
       
        Debug.Log("Animal energy: " + Energy);

        //Check energy
        if (Energy <= 50)
        {
            state = AnimalState.SearchingFood;
            HandleHungry();
        }

        //Handle other states
        if (state == AnimalState.Idle) HandleAnimalBehaviour();
        else if (state == AnimalState.Playing) HandleSwinging();
        else if (state == AnimalState.Running) HandleAttack();

        //Energy loss
        EnergyLoss();
    }

    public override void Initialize()
    {
        Raze = AnimalRaze.Monkey;
        int randomIndex = Random.Range(0, 1);
        if (randomIndex == 0) Sex = AnimalSex.Male;
        else Sex = AnimalSex.Female;
        FoodType = Behaviour.Herbivorous;
        Energy = 100;
    }

    //Handle monkey loop behaviour
    public override void HandleAnimalBehaviour()
    {
        //Start a movement where the monkey moves to random positions with basic movement, react to collisions
        //After each interaction with collisions monkey must return to HandleAnimalBehaviour() state
        Debug.Log("Handling behaviour");
    }

    //Handle move coroutine
    public override IEnumerator HandleMovement(float waitTime, Vector3 targetPosition)
    {
        throw new System.NotImplementedException();
    }

    //Handle swinging coroutine
    public override void HandleSwinging()
    {
    }

    //Handle being attacked coroutine, use base movement to move to new direction
    public override void HandleAttack()
    {
    }

    //Handle hungry state, look for nearest tree, move to that direction using base movement
    public override void HandleHungry()
    {
    }

    //Handle death
    public override void HandleDeath()
    {
        Debug.Log("Animal is dead");
        state = AnimalState.Dead;
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
