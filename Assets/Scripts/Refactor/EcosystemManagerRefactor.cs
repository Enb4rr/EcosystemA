using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EcosystemManagerRefactor : MonoBehaviour
{
    [SerializeField] GameObject MonkeyPrefab;
    [SerializeField] GameObject[] trees;
    [SerializeField] GameObject TreesParent;
    [SerializeField] private int amountMonkeys;
    [SerializeField] private int amountTrees;
    private List<GameObject> Monkeys;

    private void Awake()
    {
        Monkeys = new List<GameObject>();
        for (int i = 0; i < amountMonkeys; i++) InstantiateAnimal(AnimalRaze.Monkey);
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
