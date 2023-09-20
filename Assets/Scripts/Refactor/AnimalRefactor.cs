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

    //Animal Stats

    private float acceleration;
    private float velocity;
    private float topSpeed;

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
    public float Acceleration { get => acceleration; set => acceleration = value; }
    public float Velocity { get => velocity; set => velocity = value; }
    public float TopSpeed { get => topSpeed; set => topSpeed = value; }

    #endregion

    #region Functions

    public abstract void Initialize();

    public abstract void HandleAnimalBehaviour();

    public abstract IEnumerator HandleMovement(float waitTime, Vector3 targetPosition);

    private void Eat(float energyToAdd)
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

    public virtual void HandleHungry()
    {
        throw new System.Exception("Unimplemented method");
    }

    public abstract void HandleDeath();

    public abstract void EnergyLoss();

    #endregion
}

public enum AnimalRaze { Monkey, Fly, Snake }
public enum AnimalSex { Male, Female }
public enum Behaviour { Carnivorous, Herbivorous, Omnivorous }
public enum AnimalState { Idle, Eating, Moving, Dead, Playing, Running }
