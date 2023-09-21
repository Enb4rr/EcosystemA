using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EcosystemManagerRefactor : MonoBehaviour
{
    [SerializeField] GameObject MonkeyPrefab;
    [SerializeField] GameObject SnakePrefab;
    [SerializeField] GameObject FlyPrefab;
    [SerializeField] GameObject[] trees;
    [SerializeField] GameObject TreesParent;
    [SerializeField] private int amountMonkeys;
    [SerializeField] private int amountSnakes;
    [SerializeField] private int amountFlies;
    [SerializeField] private int amountTrees;
    private List<GameObject> Monkeys = new List<GameObject>();
    private List<GameObject> Snakes = new List<GameObject>();
    private List<GameObject> Flies = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < amountMonkeys; i++) InstantiateAnimal(AnimalRaze.Monkey);
        for (int i = 0; i < amountSnakes; i++) InstantiateAnimal(AnimalRaze.Snake);
        for (int i = 0; i < amountFlies; i++) InstantiateAnimal(AnimalRaze.Fly);
        for (int i = 0; i < amountTrees; i++) InstantiateTrees();
    }

    public void InstantiateAnimal(AnimalRaze specie)
    {
        if (specie == AnimalRaze.Monkey)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
            var instace = Instantiate(MonkeyPrefab, randomPosition, Quaternion.identity);
            Monkeys.Add(instace);
        }
        else if (specie == AnimalRaze.Snake)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
            var instace = Instantiate(SnakePrefab, randomPosition, Quaternion.identity);
            Snakes.Add(instace);
        }
        else if (specie == AnimalRaze.Fly)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-80, 20), 0, Random.Range(-80, 20));
            var instace = Instantiate(FlyPrefab, randomPosition, Quaternion.identity);
            Flies.Add(instace);
        }
    }

    public void InstantiateTrees()
    {
        for (int i = 0; i < amountTrees; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
            Instantiate(trees[Random.Range(0, trees.Length)], randomPosition, Quaternion.identity, TreesParent.transform);
        }
    }
}
