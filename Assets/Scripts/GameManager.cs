using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _maps;

    private void Awake()
    {
        GenerateRandomMap();
    }

    private void GenerateRandomMap()
    {
        var index = UnityEngine.Random.Range(0, _maps.Count);

        _maps[index].SetActive(true);
    }
}