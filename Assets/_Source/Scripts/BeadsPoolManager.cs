using System.Collections.Generic;
using UnityEngine;

public class BeadsPoolManager : MonoBehaviour
{
    [SerializeField] private int _beadCount = 98;
    [SerializeField] private GameObject _beadPrefab;

    public static BeadsPoolManager Instance { get; private set; } = null;

    private List<GameObject> _beadsList = new();

    public GameObject GetBeadObject()
    {
        foreach (var bead in _beadsList)
        {
            if (!bead.activeInHierarchy)
            {
                return bead;
            }
        }

        return CreateBeadObject();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Populate(_beadCount);
    }

    private void Populate(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var bead = CreateBeadObject();
            bead.SetActive(false);
        }
    }

    private void Clear(bool shouldDestroy = true)
    {
        if (shouldDestroy)
        {
            _beadsList.ForEach(x => Destroy(x));
        }

        _beadsList.Clear();
    }

    private GameObject CreateBeadObject()
    {
        var beadObj = Instantiate(_beadPrefab, transform);
        _beadsList.Add(beadObj);
        return beadObj;
    }
}
