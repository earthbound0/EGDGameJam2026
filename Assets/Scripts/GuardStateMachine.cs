using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GuardStateMachine : MonoBehaviour
{
    public GuardState currentState;

    private NavMeshAgent _agent;
    public Transform playerTransform;
    private Vector3 _startPos;

    // Wizard model reference (optional)
    // public GameObject wizardModel; // TODO: Assign a wizard model if needed

    [Header("Detection - Patrol")]
    public float patrolViewDistance = 10f;
    public float patrolViewAngle = 45f;

    [Header("Detection - Alert/Chase")]
    public float alertViewDistance = 15f;
    public float alertViewAngle = 60f;

    [Header("Detection - Confused")]
    public float confusedViewDistance = 12f;
    public float confusedViewAngle = 90f;

    [Header("Alert Settings")]
    public float alertDuration = 1.07f;
    private float _alertTimer = 0f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    private int _patrolIndex = 0;

    [Header("Search Transition")]
    public float searchDelay = 1.5f;
    private bool _isAlerting = false;

    [Header("Chase Settings")]
    public float chasePersistenceDuration = 3f;
    private float _loseSightTimer = 0f;
    private bool _hasLostSight = false;
    
    [Header("Debug")]
    public bool showDebugGizmos = false;
    
    // Animator reference removed for portability
    // private Animator _animator; // TODO: Add animator if needed

    // Game over flag removed (no external observer)
    // private bool _isGameOver = false;

    public enum GuardState
    {
        Confused,
        Patrol,
        Alert,
        Chase
    }

    public float CurrentViewDistance
    {
        get
        {
            switch (currentState)
            {
                case GuardState.Patrol: return patrolViewDistance;
                case GuardState.Confused: return confusedViewDistance;
                case GuardState.Alert:
                case GuardState.Chase: return alertViewDistance;
                default: return patrolViewDistance;
            }
        }
    }

    public float CurrentViewAngle
    {
        get
        {
            switch (currentState)
            {
                case GuardState.Patrol: return patrolViewAngle;
                case GuardState.Confused: return confusedViewAngle;
                case GuardState.Alert:
                case GuardState.Chase: return alertViewAngle;
                default: return patrolViewAngle;
            }
        }
    }

    private void Start()
    {
        // _animator = GetComponent<Animator>(); // Add animator if needed
        _agent = GetComponent<NavMeshAgent>();
        _startPos = transform.position;

        // if (wizardModel != null) wizardModel.SetActive(false); // Handle wizard model if used

        ChangeState(GuardState.Patrol);
    }

    private void Update()
    {
        // if (_isGameOver) return; // Game over logic removed

        FindPlayer();
        switch (currentState)
        {
            case GuardState.Confused:
                LookForPlayer();
                break;
            case GuardState.Patrol:
                Patrol();
                LookForPlayer();
                break;
            case GuardState.Alert:
                AlertPlayer();
                break;
            case GuardState.Chase:
                ChasePlayer();
                break;
        }
    }

    private void ChangeState(GuardState newState)
    {
        // if (_isGameOver) return; // Game over logic removed

        currentState = newState;
        // Set animation states here if using an Animator

        // if (wizardModel != null) wizardModel.SetActive(false); // Handle wizard model if used

        switch (newState)
        {
            case GuardState.Patrol:
                _isAlerting = false;
                _agent.isStopped = false;
                _agent.speed = 4f;
                _patrolIndex = GetNearestPatrolPointIndex();
                // Set walking animation
                break;
            case GuardState.Alert:
                _isAlerting = true;
                _alertTimer = 0f;
                _agent.isStopped = true;
                _agent.speed = 0f;
                // Play alert sound here
                // Set alert animation
                break;
            case GuardState.Chase:
                _isAlerting = false;
                _agent.isStopped = false;
                _agent.speed = 8f;
                _loseSightTimer = 0f;
                _hasLostSight = false;
                // TODO: Set running animation
                break;
            case GuardState.Confused:
                _agent.isStopped = true;
                _agent.speed = 0f;
                // Set confused/idle animation
                StartCoroutine(DelayBeforePatrol());
                break;
        }
    }

    void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            // playerTransform = GameObject.FindGameObjectWithTag("Cart").transform; // fallback
            return;
        }
        playerTransform = playerObject.transform;
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;
        _agent.SetDestination(patrolPoints[_patrolIndex].position);
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            _patrolIndex = (_patrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void LookForPlayer()
    {
        // if (_isGameOver) return; // Game over logic removed

        if (playerTransform == null)
        {
            Debug.LogError("Player Object Not Assigned for Guard");
            return;
        }

        if (IsPlayerInView())
        {
            ChangeState(GuardState.Alert);
        }
    }

    private bool IsPlayerInView()
    {
        if (playerTransform == null) return false;

        var playerPos2D = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
        var guardPos2D = new Vector3(transform.position.x, 0, transform.position.z);
        var directionToPlayer2D = playerPos2D - guardPos2D;
        var guardForward2D = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

        float distanceToPlayer = directionToPlayer2D.magnitude;
        if (distanceToPlayer > CurrentViewDistance) return false;

        float angle = Vector3.Angle(guardForward2D, directionToPlayer2D.normalized);
        if (angle > CurrentViewAngle) return false;

        return true;
    }

    private void AlertPlayer()
    {
        // if (_isGameOver) return; // Game over logic removed

        if (playerTransform == null)
        {
            ChangeState(GuardState.Patrol);
            return;
        }

        RotateTowards(playerTransform.position, _agent.angularSpeed);

        if (!_isAlerting) return;
        _alertTimer += Time.deltaTime;
        if (_alertTimer >= alertDuration)
        {
            _alertTimer = 0f;
            _isAlerting = false;
            _agent.isStopped = false;
            ChangeState(GuardState.Chase);
        }
    }

    private void ChasePlayer()
    {
        // if (_isGameOver) return; // Game over logic removed

        if (playerTransform == null)
        {
            ChangeState(GuardState.Patrol);
            return;
        }

        if (_agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_agent.velocity.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _agent.angularSpeed * Time.deltaTime);
        }

        RotateTowards(playerTransform.position, _agent.angularSpeed);
        _agent.SetDestination(playerTransform.position);

        bool canSeePlayer = IsPlayerInView();

        if (canSeePlayer)
        {
            _hasLostSight = false;
            _loseSightTimer = 0f;
        }
        else
        {
            if (!_hasLostSight)
            {
                _hasLostSight = true;
                _loseSightTimer = 0f;
            }

            _loseSightTimer += Time.deltaTime;

            if (_loseSightTimer >= chasePersistenceDuration)
            {
                // Play lost player sound here
                ChangeState(GuardState.Confused);
            }
        }
    }

    private void RotateTowards(Vector3 targetPosition, float rotationSpeed)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private int GetNearestPatrolPointIndex()
    {
        if (patrolPoints.Length == 0) return 0;

        float nearestDistance = float.MaxValue;
        int nearestIndex = 0;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    private IEnumerator DelayBeforePatrol()
    {
        yield return new WaitForSeconds(searchDelay);
        ChangeState(GuardState.Patrol);
    }

    // Called from Animator Event (optional)
    // public void PlayFootstepSound()
    // {
    //     // Play footstep sound here
    // }
    
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        DrawViewConeGizmo(origin, patrolViewDistance, patrolViewAngle, new Color(0.2f, 0.8f, 1f, 0.9f));
        DrawViewConeGizmo(origin, alertViewDistance, alertViewAngle, new Color(1f, 0.35f, 0.25f, 0.9f));
        DrawViewConeGizmo(origin, confusedViewDistance, confusedViewAngle, new Color(1f, 0.9f, 0.2f, 0.9f));

        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            Transform point = patrolPoints[i];
            if (point == null) continue;

            Gizmos.DrawWireSphere(point.position, 0.4f);

            int nextIndex = (i + 1) % patrolPoints.Length;
            Transform nextPoint = patrolPoints[nextIndex];
            if (nextPoint != null)
            {
                Gizmos.DrawLine(point.position, nextPoint.position);
            }
        }
    }

    private void DrawViewConeGizmo(Vector3 origin, float viewDistance, float viewAngle, Color color)
    {
        Gizmos.color = color;

        Vector3 forward = transform.forward.normalized;
        Vector3 leftBoundary = Quaternion.Euler(0f, -viewAngle, 0f) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0f, viewAngle, 0f) * forward;

        Gizmos.DrawLine(origin, origin + forward * viewDistance);
        Gizmos.DrawLine(origin, origin + leftBoundary * viewDistance);
        Gizmos.DrawLine(origin, origin + rightBoundary * viewDistance);
        Gizmos.DrawWireSphere(origin, viewDistance);
    }
}