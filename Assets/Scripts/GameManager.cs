
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameManager sınıfı şimdilik sadece oyun başlangıcında random bir haritanın seçilmesini sağlıyor.
/// İleride Zenject ile bind edilip oyuunun state'lerini yönetebilir. Play Pause MainMenu gibi
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _maps;

    private void Awake()
    {
        GenerateRandomMap();
    }

    private void GenerateRandomMap()
    {
        var index = Random.Range(0, _maps.Count);

        _maps[index].SetActive(true);
    }
}