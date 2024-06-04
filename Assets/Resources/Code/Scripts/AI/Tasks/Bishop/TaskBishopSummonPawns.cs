using System.Collections;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskBishopSummonPawns : AINode {
    readonly NavMeshAgent agent;
    readonly Animator animator;
    readonly SpawnManager spawnManager;
    readonly float summonCooldown;
    readonly float summonCount;
    readonly float summonRange;

    SummonState summonState = SummonState.Idle;
    enum SummonState { Summoning, Cooldown, Idle }

    public TaskBishopSummonPawns(Animator animator, NavMeshAgent agent, float summonCount, float summonCooldown, float summonRange) {
        this.agent = agent;
        this.animator = animator;
        this.summonCount = summonCount;
        this.summonCooldown = summonCooldown;
        this.summonRange = summonRange;
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    public override NodeState Evaluate() {
        if (!spawnManager) {
            Debug.LogError("SpawnManager is null");
            return NodeState.FAILURE;
        }

        switch (summonState) {
            case SummonState.Summoning:
                return NodeState.SUCCESS;

            case SummonState.Cooldown:
                return NodeState.FAILURE;

            case SummonState.Idle:
                agent.isStopped = true;
                animator.SetTrigger("SummonPawns");
                summonState = SummonState.Summoning;
                GameManager.Instance.StartCoroutine(SummonPawns());
                return NodeState.RUNNING;
        }

        return NodeState.FAILURE;
    }

    IEnumerator SummonPawns() {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < summonCount; i++) {
            Vector3 randomPoint = agent.transform.position + Random.insideUnitSphere * summonRange;
            randomPoint.y = agent.transform.position.y;

            if (!NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas)) {
                Debug.LogError("Could not find a valid position to summon pawns");
                yield break;
            }

            spawnManager.SpawnEnemy(EnemyType.Pawn, hit.position);
            yield return new WaitForSeconds(0.2f);
        }

        agent.isStopped = false;
        summonState = SummonState.Cooldown;

        yield return new WaitForSeconds(summonCooldown + Random.Range(0, -summonCooldown));

        summonState = SummonState.Idle;
    }
}
