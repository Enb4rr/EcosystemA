using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;

public abstract class Animal
{
    public  float _energy;
    public Sex _sex;
    protected Vector3 _acceleration;
    protected Vector3 _velocity;
    protected Vector3 _location;
    protected float _topSpeed;
    protected GameObject _animal;
    public Rigidbody _body;
    // variable to calculate stuff
    protected float dirTimer;
    protected Vector3 maximumPos = new Vector3(80, 0, 80);
    protected float dirLimit = 3f;

    public Pendulum pendulum;
    public bool isEating;
    public GameObject _Animal { get => _animal; }
    public float Energy { get => _energy; }
    public Sex Sex { get => _sex; }

    public Animal(int energy, Sex sex, GameObject animal, float topSpeed)
    {
        _energy = energy;
        _sex = sex;
        _animal = animal;
        _topSpeed = topSpeed;
        pendulum= new Pendulum();
    }
    public virtual void Move(Vector3 desiredDirection)
    {

    }
    public virtual void Eat()
    {

    }
    public virtual void Turn(float radiansAngle)
    {
        float eulerAngle = (radiansAngle * Mathf.Rad2Deg) + 180;
        float toSpin = eulerAngle - ((_animal.transform.eulerAngles.y + 180) % 360);
        
        if (toSpin > 180 || toSpin < -180)
        {
            toSpin %= 180;
            toSpin *= -1;
        }
        toSpin = Mathf.Clamp(toSpin, -6, 6);
        _animal.transform.Rotate(new Vector3(0, toSpin, 0));
    }
    public void CheckEdges()
    {
        if (_location.x > maximumPos.x)
        {
            _location.x= -maximumPos.x;
        }
        else if (_location.x < -maximumPos.x)
        {
            _location.x = maximumPos.x;
        }
        if (_location.z > maximumPos.z)
        {
            _location.z = -maximumPos.z;
        }
        else if (_location.z < -maximumPos.z)
        {
            _location.z = maximumPos.z;
        }
    }
    public virtual void Swing(Vector3 origin)
    {

    }
}
public class Monkey : Animal
{
    //Pendulum fields
    Vector3 alocation;
    public Vector3 origin = new Vector3(0, 0, 0);
    public float radio =3;
    public float angle = 60*Mathf.Deg2Rad;
    public float aVelocity = 0f;
    public float aAcceleration;
    public float damping = 0.9995f;

    public Monkey(int energy, Sex sex, GameObject animal, float topSpeed) : base(energy, sex, animal, topSpeed)
    {
        Vector3 spawnLocation = new Vector3(Random.Range(-15f,15f), 0f,Random.Range(-15f,15f));
        _location = spawnLocation;
        _acceleration = Vector2.zero;
        _topSpeed = topSpeed;
        Vector3 randomRotation = new Vector3(0, UnityEngine.Random.Range(0f, 360f), 0);
        _animal = GameObject.Instantiate(animal, spawnLocation, Quaternion.identity);
        _animal.transform.Rotate(randomRotation);
        _body = _animal.AddComponent<Rigidbody>();
        _body.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX ;
        _body.useGravity = false;
        AnimalController animalController = _animal.AddComponent<AnimalController>();
        animalController.setAnimal("Tree","Snake",AnimalSpecie.Monkey);
        animalController.Animal = this;

    }
    public override void Move(Vector3 desiredDirection)
    {
        dirTimer += Time.deltaTime;
        
        if (desiredDirection != Vector3.zero)
        {
            _acceleration= desiredDirection*1;
        }
        else if(dirTimer >= dirLimit)
        {
            Vector3 dir = new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f));
            _acceleration = dir.normalized;
            dirLimit = Random.Range(1f, 6f);
            dirTimer = 0;
        }
        if(_velocity.magnitude >= _topSpeed)
        {
            _velocity *= 0.6f;
        }
        _velocity += _acceleration;
        _location += _velocity * Time.deltaTime/100;
        CheckEdges();
        _animal.transform.position = new Vector3(_location.x,0,_location.z);
        float angle = Mathf.Atan2(_velocity.x, _velocity.z);
        Turn(angle);
        Debug.DrawRay(_animal.transform.position, _acceleration * 10);
    }
    public override void Eat()
    {
        _acceleration = Vector3.zero;
        _velocity = Vector3.zero;
        _energy += 5;
        Debug.Log("Animal"+ _energy);
        //_energy = Mathf.Clamp(_energy, 0, 100);
    }

    public override void Swing(Vector3 swingOrigin)
    {
        swingOrigin.y +=5f;
        aAcceleration = (-4f / radio) * Mathf.Sin(angle);
        aVelocity += aAcceleration * Time.deltaTime;
        aVelocity *= damping;

        angle += aVelocity * Time.deltaTime;

        float y = swingOrigin.y + radio * Mathf.Sin(angle- 90 * Mathf.Deg2Rad);
        float z = swingOrigin.z + radio * Mathf.Cos(angle- 90 * Mathf.Deg2Rad);

        _location = new Vector3(_location.x, y, z);
        _animal.transform.localPosition = _location;
    }
}
public enum AnimalBehaviour
{
    Carnivorous, Herbivorous, Omnivorous
}
public enum Sex
{
    Male, Female
}
public enum state
{
    Eating, Searchingfood, Dead, Moving
}
