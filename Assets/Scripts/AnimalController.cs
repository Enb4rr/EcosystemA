using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditorInternal;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public class AnimalController : MonoBehaviour
{
    private Animal animal;
    
    [SerializeField] public AnimalSpecie animalSpecie;
    private string food;
    private string predator;
    public GameObject target;

    [SerializeField] Scrollbar energyScrollbar;
    [SerializeField] TMP_Text title;
    [SerializeField] Image[] sexImgs;
    [SerializeField] animalState currentState;
    public Animal Animal { get => animal; set => animal = value; }

    private bool ableToMate = true;
    private float mateTiming;

    private Vector3 currentDirection;
    private Vector3 swingOrigin;

    private bool hasReachedTarget;
    public void setAnimal(string food, string predator,AnimalSpecie specie)
    {
        animalSpecie = specie;
        this.food = food;
        this.predator = predator;
    }
    private void Start()
    {
        currentState = animalState.random;

        energyScrollbar = GetComponentInChildren<Scrollbar>();
        title = GetComponentInChildren<TMP_Text>();
        sexImgs = GetComponentsInChildren<Image>();

        title.text = animalSpecie.ToString();
        energyScrollbar.size = animal.Energy / 100;
        if(animal.Sex == Sex.Female)
        {
            sexImgs[1].enabled = false;
        }
        else if (animal.Sex == Sex.Male)
        {
            sexImgs[2].enabled = false;
        }
    }
    void FixedUpdate()
    {
        if (animal._energy <= 0 && currentState != animalState.dead)
        {
            HandleDeath();
            return;
        }
        if (currentState == animalState.random && target == null)
        {
            animal.Move(Vector3.zero);
        }
        else if (currentState == animalState.swinging)
        {
            animal.Swing(swingOrigin);
        }
        else if (target != null)
        {
            currentDirection = target.gameObject.transform.position - gameObject.transform.position;
            currentDirection.Normalize();
            if (currentState == animalState.running) currentDirection *= -1;
            animal.Move(currentDirection);
            if((target.transform.position - gameObject.transform.position).magnitude<= 0.2f)
            {
                hasReachedTarget= true;
            }
        }
        if (!ableToMate) mateTiming += Time.deltaTime;
        if (mateTiming >= 30)
        {
            ableToMate = true;
            mateTiming = 0;
        }
        animal._energy -= Time.deltaTime;
        energyScrollbar.size = animal._energy / 100;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (currentState == animalState.dead) return;
        if (other.CompareTag(predator))
        {
            target = other.gameObject;
            currentState = animalState.running;
            Debug.Log("collided with "+ predator);
        }
        else if(animal.Energy >= 50 && other.CompareTag(animalSpecie.ToString()) && currentState != animalState.eating && currentState != animalState.running && animal.Sex != other.gameObject.GetComponent<AnimalController>().Animal.Sex)
        {
            target = other.gameObject;
            currentState = animalState.mating;
            ableToMate = false;
        }
        else if (animalSpecie == AnimalSpecie.Monkey &&  animal.Energy >= 50 && other.CompareTag("Tree") && currentState != animalState.eating && currentState != animalState.running)// Only for monkeys
        {
            swingOrigin = other.gameObject.transform.position;
            Debug.Log("its swinging");
            currentState = animalState.swinging;
            
        }
        else if (other.CompareTag(food)){
            if (animal._energy < 50 || currentState == animalState.eating)
            {
               HandleEating(other);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == string.Empty) return;
        if (other.CompareTag(food) && animal is Monkey)
        {
            target = null;
        }
        else if (other.CompareTag(predator))
        {
            currentState = animalState.random;
        }
    }
    private void HandleDeath()
    {
        Debug.Log(animalSpecie.ToString()+" is dead");
        currentState = animalState.dead;
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        EcosystemManager manager = FindObjectOfType<EcosystemManager>();
        renderer.material = manager.deadMaterial;
        gameObject.transform.Rotate(new Vector3(180, 0, 0));
        Vector3 tempPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y+0.5f, gameObject.transform.position.z);
        gameObject.transform.position = tempPosition;
    }
    private void HandleEating(Collider other)
    {
        
        if (animal.isEating)
        {
            Debug.Log("Eating cancelled");
            return;
        }
        target = other.gameObject;
        if (target == null) target = other.gameObject;
        animal.isEating = true;
        Debug.Log("Following a tree");
        currentState = animalState.eating;
        Debug.Log("eating");
        StartCoroutine(Eating());
    }
    public IEnumerator Eating()
    {
        yield return new WaitUntil(() => hasReachedTarget == true);
        while (animal._energy <= 100)
        {
            animal.Eat();
            yield return new WaitForSeconds(1f);
        }
        animal.isEating = false;
        currentState = animalState.random;
        hasReachedTarget = false;
        Debug.Log("finished eating");
        animal._energy = 100;
        energyScrollbar.size = 1;
        yield break;
    }
}
public enum AnimalSpecie
{
    Monkey,Squid,Frog,Snake,Fly
}
public enum animalState
{
    eating, swinging, running, random, mating, dead

}