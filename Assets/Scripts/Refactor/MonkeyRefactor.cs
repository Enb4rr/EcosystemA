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

    //Generate variables that are not set in constructor
    public MonkeyRefactor(AnimalRaze raze, AnimalSex sex, Behaviour foodType, float energy) : base(raze, sex, foodType, energy)
    {
        Acceleration = 0;
        Velocity = 0;
        TopSpeed = 0;
    }

    private void Awake()
    {
        //Get required components from GameObject
        boxCollider = GetComponent<BoxCollider>();
        sphereCollider = GetComponent<SphereCollider>();
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(HandleAnimalBehaviour());
    }

    //Handle monkey loop behaviour
    public override IEnumerator HandleAnimalBehaviour()
    {
        //Start a loop movement where the monkey moves to random positions with basic movement, react to collisions
        //After each interaction with collisions monkey must return to HandleAnimalBehaviour() state

        while(true)
        {
            Debug.Log("Handling behaviour");
            EnergyLoss();
            yield return new WaitForSeconds(1);
            //yield return null;
        }
    }

    //Handle move coroutine
    public override IEnumerator HandleMovement(float waitTime, Vector3 targetPosition)
    {
        throw new System.NotImplementedException();
    }

    //Handle swinging coroutine
    public override IEnumerator HandleSwinging(float waitTime)
    {
        return base.HandleSwinging(waitTime);
    }

    //Handle being attacked coroutine, use base movement to move to new direction
    public override IEnumerator HandleAttack(float waitTime)
    {
        return base.HandleAttack(waitTime);
    }

    //Handle hungry state, look for nearest tree, move to that direction using base movement
    public override IEnumerator HandleHungry(float waitTime)
    {
        return base.HandleHungry(waitTime);
    }

    //Handle death
    public override void HandleDeath()
    {
        throw new System.NotImplementedException();
    }

    public override void EnergyLoss()
    {
        //Lose 1 energy each second
        Energy--;
        if(Energy <= 0) HandleDeath();
    }
}
