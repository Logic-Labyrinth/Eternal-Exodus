using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        if (agent == null) {
            agent = GetComponent<NavMeshAgent>();
        }

        player = GameObject.Find("New Player");
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = player.transform.position;
    }
}
