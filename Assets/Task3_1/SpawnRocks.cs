using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRocks : MonoBehaviour
{
    private Transform spawn;
    public GameObject rock;
    // Start is called before the first frame update
    void Start()
    {
        spawn = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            SpawnRock(rock);
        }
    }

    public void SpawnRock(GameObject rock)
    {
        if (rock == null) return;
        Transform pos = rock.transform;
        pos.position = new Vector3(
                spawn.position.x,
                spawn.position.y,
                spawn.position.z
        );

        GameObject newR = Instantiate(rock);
    }
}
