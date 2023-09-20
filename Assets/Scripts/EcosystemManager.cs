using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UIElements;

public class EcosystemManager : MonoBehaviour
{
    [SerializeField] GameObject MonkeyPrefab;
    [SerializeField] GameObject FlyPrefab;
    [SerializeField] GameObject SnakePrefab;
    [SerializeField] GameObject SquidPrefab;
    [SerializeField] GameObject Frog;
    [SerializeField] GameObject[] trees;
    [SerializeField] GameObject TreesParent;
    [SerializeField] private int amountMonkeys;
    [SerializeField] private int amountTrees;
    [SerializeField] public Material deadMaterial;
    private List<GameObject> Monkeys;

    private void Awake()
    {
        Monkeys = new List<GameObject>();
        for(int i =0; i < amountMonkeys; i++)
        {
            InstantiateAnimal(AnimalSpecie.Monkey);
            InstantiateTrees();
        }
    }
    public void InstantiateAnimal(AnimalSpecie specie)
    {
        int randomIndex = UnityEngine.Random.Range(0, 2);
        Sex sex;
        if (randomIndex == 0)
        {
            sex = Sex.Male;
        }
        else
        {
            sex = Sex.Female;
        }
        if (specie == AnimalSpecie.Monkey)
        {
            Monkey instance = new Monkey(30, sex, MonkeyPrefab, 50f);
            Monkeys.Add(instance._Animal);
        }
    }
    public void InstantiateTrees()
    {
        for(int i =0;i< amountTrees;i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
            Instantiate(trees[Random.Range(0, trees.Length)], randomPosition, Quaternion.identity, TreesParent.transform);
        }
    } 
}
