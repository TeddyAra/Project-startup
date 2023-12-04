using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BranchesScript : MonoBehaviour {
    [SerializeField] GameObject branch; // Branch prefab
    [SerializeField] float spawnTime; // After how much time should a branch be spawned
    [SerializeField] float spawnHeight; // How high up should the branch spawn
    [SerializeField] float spawnRange; // How far away can the branch spawn
    [SerializeField] float branchLifeTime; // For how long should the branch exist
    [SerializeField] float spawnDistance; // How far forward should the branch spawn

    private float timer;
    private Dictionary<GameObject, float> branches;

    void Start() {
        branches = new Dictionary<GameObject, float>();
    }

    void Update() {
        // Adds time to timer and branch lifetime
        timer += Time.deltaTime;

        foreach (var item in branches.Keys.ToList()) {
            branches[item] += Time.deltaTime;
            // Remove branch
            if (branches[item] > branchLifeTime) {
                branches.Remove(item);
                Destroy(item);
            }
        }

        // Spawn a new branch
        if (timer > spawnTime) {
            // Reset timer and get the base position
            timer = 0f;
            Vector3 position = transform.position;
            position.y = spawnHeight;

            // Randomize position of branch and move it forward
            position.x += Random.Range(-spawnRange, spawnRange);
            position.z += Random.Range(-spawnRange, spawnRange) + spawnDistance;

            // Create the branch and add it to the dictionary
            GameObject newBranch = Instantiate(branch, position, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            branches.Add(newBranch, 0);
        }
    }
}
