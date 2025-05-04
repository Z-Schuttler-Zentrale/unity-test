using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public int itemsToSpawn;
    public Item item;

    void Start()
    {

        Debug.Log(transform.localScale);
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        
        Vector3 meshSize = meshFilter.sharedMesh.bounds.size;
        Vector3 worldSize = Vector3.Scale(meshSize, transform.localScale);

        for (int i = 0; i < itemsToSpawn; i++)
        {
            float x = Random.Range(-worldSize.x / 2, worldSize.x / 2);
            float z = Random.Range(-worldSize.z / 2, worldSize.z / 2);

            Instantiate(item.prefab, new Vector3(x, transform.position.y, z), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
