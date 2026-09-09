using UnityEngine;

public class PlateSpawner : MonoBehaviour
{
    public static PlateSpawner Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameObject platePrefab;
    [SerializeField] private Transform plateSpawnPoint;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SpawnNewPlate();
    }

    public void SpawnNewPlate()
    {
        if (platePrefab != null && plateSpawnPoint != null)
        {
            Instantiate(platePrefab, plateSpawnPoint);
        }
    }
}
