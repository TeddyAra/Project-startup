using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BranchesScript : MonoBehaviour {
    [SerializeField] GameObject branch; // Branch prefab
    [SerializeField] float spawnTime; // After how much time should a branch be spawned
    [SerializeField] float spawnSpeedUp; // With how much should the timer decrease every time a branch spawns
    [SerializeField] float minimumTime; // The absolute minimum time it should take for a branch to spawn
    [SerializeField] float spawnHeight; // How high up should the branch spawn
    [SerializeField] float spawnRange; // How far away can the branch spawn
    [SerializeField] float branchLifeTime; // For how long should the branch exist
    [SerializeField] float spawnDistance; // How far forward should the branch spawn
    [SerializeField] float maxDistance; // The max distance a branch can be before being removed

    private float timer; // The timer that keeps track of when to spawn a branch
    private List<GameObject> branches; // All branches
    private bool firstHit; 
    void Start() {
        branches = new List<GameObject>();
    }

    void Update() {

        // Adds time to timer and branch lifetime
        timer += Time.deltaTime;

        // Spawn a new branch
        if (timer > spawnTime) {
            // Decreases spawnTime
            spawnTime -= spawnSpeedUp;
            if (spawnTime < minimumTime) 
                spawnTime = minimumTime;

            // Reset timer and get the base position
            timer = 0f;
            Vector3 position = transform.position;
            position.y = spawnHeight;

            // Randomize position of branch and move it forward
            position.x += Random.Range(-spawnRange, spawnRange);
            position.z += Random.Range(-spawnRange, spawnRange) + spawnDistance;

            // Create the branch and add it to the dictionary
            GameObject newBranch = Instantiate(branch, position, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            branches.Add(newBranch);

            // Check which branches can be removed
            foreach (GameObject item in branches.ToList()) {
                if ((item.transform.position - transform.position).magnitude > maxDistance) {
                    branches.Remove(item);
                    Destroy(item);
                }
            }
        }
    }

}
