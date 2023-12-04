using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BranchesScript : MonoBehaviour {
    [SerializeField] GameObject branch;
    [SerializeField] float spawnTime;
    [SerializeField] float spawnHeight;
    [SerializeField] float spawnRange;
    [SerializeField] float branchLifeTime;
    [SerializeField] float spawnDistance;

    private float timer;
    private Dictionary<GameObject, float> branches;

    void Start() {
        branches = new Dictionary<GameObject, float>();
    }

    void Update() {
        timer += Time.deltaTime;

        foreach (var item in branches.Keys.ToList()) {
            branches[item] += Time.deltaTime;
            if (branches[item] > branchLifeTime) {
                branches.Remove(item);
                Destroy(item);
            }
        }

        if (timer > spawnTime) {
            timer = 0f;
            Vector3 position = transform.position;
            position.y = spawnHeight;

            //position.x += Random.Range(-spawnRange, spawnRange);
            //position.z += Random.Range(-spawnRange, spawnRange) + spawnDistance;

            GameObject newBranch = Instantiate(branch, position, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            branches.Add(newBranch, 0);
        }
    }
}
