using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Animal master class, all animals are children of this class
/// </summary>
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public abstract class AnimalRefactor : MonoBehaviour
{
    #region Variables and Constructor

    //Animal Variables

    private AnimalRaze raze;
    private AnimalSex sex;
    private Behaviour foodType;
    private AnimalRaze predator;
    private float energy;
    //Animal Movement
    private Vector3 acceleration;
    private Vector3 velocity;
    //Animal Stats
    protected AnimalRefactor(AnimalRaze raze, AnimalSex sex, Behaviour foodType, AnimalRaze predator, float energy)
    {
        Raze = raze;
        Sex = sex;
        FoodType = foodType;
        Energy = energy;
        Predator = predator;
    }

    #endregion

    #region Getters and Setters
    public AnimalRaze Raze { get => raze; set => raze = value; }
    public AnimalSex Sex { get => sex; set => sex = value; }
    public Behaviour FoodType { get => foodType; set => foodType = value; }
    public AnimalRaze Predator { get => predator; set => predator = value; }
    public float Energy { get => energy; set => energy = value; }
    protected Vector3 Acceleration { get => acceleration; set => acceleration = value; }
    protected Vector3 Velocity { get => velocity; set => velocity = value; }

    #endregion

    #region Functions

    public abstract void Initialize();

    public abstract void HandleAnimalBehaviour();

    public abstract void HandleMovement(Vector3 targetPosition);

    protected void Eat(float energyToAdd)
    {
        Energy += energyToAdd;
        if(Energy > 100) Energy = 100;
    }

    private IEnumerator HandleEating(float waitTime, float energy)
    {
        while(Energy <= 100)
        {
            Eat(energy);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public virtual void HandleSwinging()
    {
        throw new System.Exception("Unimplemented method");
    }

    public virtual void HandleAttack()
    {
        throw new System.Exception("Unimplemented method");
    }

    public virtual void HandleHunger()
    {
        throw new System.Exception("Unimplemented method");
    }

    public abstract void HandleDeath();

    public abstract void EnergyLoss();

    protected void Turn(float radiansAngle)
    {
        float eulerAngle = (radiansAngle * Mathf.Rad2Deg) + 180;
        float toSpin = eulerAngle - ((gameObject.transform.eulerAngles.y + 180) % 360);

        if (toSpin > 180 || toSpin < -180)
        {
            toSpin %= 180;
            toSpin *= -1;
        }
        toSpin = Mathf.Clamp(toSpin, -6, 6);
        gameObject.transform.Rotate(new Vector3(0, toSpin, 0));
    }
    protected Vector3 CalculateDirection(Vector3 target)
    {
        if(target == null ) return Vector3.zero;
        Vector3 direction = target - gameObject.transform.position;
        direction.Normalize();
        return direction;
    }
    #endregion
}

public enum AnimalRaze { Monkey, Fly, Snake, None }
public enum AnimalSex { Male, Female }
public enum Behaviour { Carnivorous, Herbivorous, Omnivorous }
public enum AnimalState { Idle, Eating, Moving, Dead, Playing, Running }
