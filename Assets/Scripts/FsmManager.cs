using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FsmManager : MonoBehaviour
{
    [SerializeField] private CitizenDataSO citizenDataSO;
    [SerializeField] private NpcType npcType;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject playerTarget;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootCooldown;
    [SerializeField] private float bulletSpeed;

    public static event Action onEnemyDied;
    public static event Action onCivilianDied;

    private CitizensSpawner citizenSpawner;
    public Transform[] waypoints;
    private GameObject waypointsHolder;
    private List<StateBase> states = new List<StateBase>();
    private StateBase currentState;
    private float shootCdAux;
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.onDie += HealthSystem_onDie;

        waypointsHolder = GameObject.Find("Waypoints");
        citizenSpawner = GameObject.Find("Ciudadanos").GetComponent<CitizensSpawner>();

        states.Add(new StateIdle());
        states.Add(new StateWalking());
        states.Add(new StateShoot());
        states.Add(new StateDie());

        foreach (StateBase state in states)
            state.Initialize(animator, this, agent);

        currentState = FindState(StateType.Idle);
    }

    private void Start()
    {
        waypoints = new Transform[waypointsHolder.transform.childCount];
        playerTarget = PlayerController.Instance.gameObject;

        for (int i = 0; i < waypointsHolder.transform.childCount; i++)
        {
            waypoints[i] = waypointsHolder.transform.GetChild(i);
        }
        if (npcType == NpcType.Civilian)
        {
            SwitchState(StateType.Walking);
        }
        gun.SetActive(false);
    }

    private void Update()
    {
        CheckPlayerPosition();
        if (currentState!=null)
            currentState.OnUpdate();
    }

    private void OnDestroy()
    {
        healthSystem.onDie -= HealthSystem_onDie;
    }

    private void HealthSystem_onDie()
    {
        switch (npcType)
        {
            case NpcType.Enemy:
                {
                    onEnemyDied?.Invoke();
                    citizenSpawner.ReturnEnemyToPool(gameObject);
                    break;
                }
            case NpcType.Civilian:
                {
                    onCivilianDied?.Invoke();
                    citizenSpawner.ReturnCivilianToPool(gameObject);
                    break;
                }
        }
    }

    public void SwitchState(StateType targetState)
    {
        SwitchState(FindState(targetState));
    }

    public void SwitchState(StateBase targetState)
    {
        if (currentState == targetState)
        {
            return;
        }

        currentState.OnExit();
        currentState = targetState;
        currentState.OnEnter();
    }

    public StateBase FindState(StateType stateToFind)
    {
        foreach (StateBase state in states)
            if (state.stateType == stateToFind)
                return state;
        return null;
    }

    public void CheckPlayerPosition()
    {
        if (npcType == NpcType.Enemy)
        {
            if (Vector3.Distance(playerTarget.transform.position, transform.position) < 30)
            {
                SwitchState(StateType.Shoot);
                gun.SetActive(true);
            }
            else
            {
                gun.SetActive(false);
                SwitchState(StateType.Walking);
            }
        }
    }

    public void Shoot()
    {
        Vector3 dir = playerTarget.transform.position - agent.transform.position;

        dir.y = 0f;

        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            agent.transform.rotation = Quaternion.Slerp(
                agent.transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }

        shootCdAux -= Time.deltaTime;
        if (shootCdAux <= 0)
        {
            GameObject bullet = citizenSpawner.GetBullet();

            bullet.SetActive(true);

            bullet.transform.position = shootPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            Bullet p = bullet.GetComponent<Bullet>();

            p.Init(shootPoint.position, playerTarget.transform.position, bulletSpeed);
            shootCdAux = shootCooldown;
        }
    }
}
