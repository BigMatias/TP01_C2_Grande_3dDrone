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

    public static event Action onEnemyDied;
    public static event Action onEnemyShoot;
    public static event Action onCivilianDied;

    private CitizensSpawner citizenSpawner;
    // Suggestion: [Media] - waypoints es público. Debería ser private con property readonly o exponerse por método.
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

        // Warning: [Alta] - Tres GameObject.Find consecutivos en cada NPC spawneado. Con 20 enemigos + civiles, esto recorre la jerarquía completa decenas de veces al iniciar la escena. Pasar referencias o usar un Registry/Singleton.
        // Bug: [Alta] - Todos son magic strings. Cualquier renombre de "Waypoints", "Ciudadanos" o "Racing Drone Merged" rompe el juego con NullReferenceException.
        waypointsHolder = GameObject.Find("Waypoints");
        citizenSpawner = GameObject.Find("Ciudadanos").GetComponent<CitizensSpawner>();
        waypoints = new Transform[waypointsHolder.transform.childCount];
        playerTarget = GameObject.Find("Racing Drone Merged");

        states.Add(new StateIdle());
        states.Add(new StateWalking());
        states.Add(new StateShoot());
        states.Add(new StateDie());

        foreach (StateBase state in states)
            state.Initialize(animator, this, agent);

        // Bug: [Media] - Se asigna currentState pero NUNCA se llama a currentState.OnEnter().
        currentState = FindState(StateType.Idle);
    }

    private void Start()
    {
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
        // Warning: [Alta] - CheckPlayerPosition se llama TODOS los frames y dispara SwitchState constantemente.
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
            // Warning: [Media] - Vector3.Distance hace sqrt() innecesario para una simple comparación de cercanía. Usar (a-b).sqrMagnitude contra 30*30 = 900 es estándar y más barato.
            // Suggestion: [Baja] - "30" magic number. Debería estar en CitizenDataSO como AggroRange y serializado.
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
            onEnemyShoot?.Invoke();
            GameObject bullet = citizenSpawner.GetBullet();

            bullet.SetActive(true);

            bullet.transform.position = shootPoint.position;
            bullet.transform.rotation = Quaternion.identity;

            Bullet p = bullet.GetComponent<Bullet>();

            p.Init(shootPoint.position, playerTarget.transform.position, citizenDataSO.EnemyBulletSpeed);
            shootCdAux = citizenDataSO.EnemyShootCD;
        }
    }
}
