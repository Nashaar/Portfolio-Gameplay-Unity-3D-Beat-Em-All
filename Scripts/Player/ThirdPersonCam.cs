using Unity.Cinemachine;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    #region Declarations
    [Header("References")]
    [SerializeField] private Rigidbody _playerRb;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _playerBodyTransform;
    [SerializeField] private Transform _playerOrientationTransform;
    [SerializeField] private Transform _fightingLookAtTransform;
    [SerializeField] private InputController _inputController;
    [SerializeField] private Transform _lockingCamera;

    [Space]

    [Header("Movement Variables")]
    public CameraStyle cameraStyle;
    public GameObject lockOnEnemy;
    public enum CameraStyle
    {
        Walking,
        Fighting,
        Locking
    }
    [SerializeField] private float _rotationSpeed;
    #endregion

    #region Unity Functions
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        lockOnEnemy = null;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        RotateOrientation();
    }
    #endregion

    #region Camera
    private void RotateOrientation()
    {
        Vector2 cameraInputDirection = _inputController.MoveInput();

               

        if(cameraStyle == CameraStyle.Walking)
        {
            Vector3 viewDirection = _playerTransform.position - new Vector3(transform.position.x, _playerTransform.position.y, transform.position.z);
            _playerOrientationTransform.forward = viewDirection.normalized; 

            Vector3 moveDirection = _playerOrientationTransform.forward * cameraInputDirection.y + _playerOrientationTransform.right * cameraInputDirection.x;

            if(cameraInputDirection != Vector2.zero)
            {
                _playerBodyTransform.forward = Vector3.Slerp(_playerBodyTransform.forward, moveDirection.normalized, Time.fixedDeltaTime * _rotationSpeed);
            } 
        }
        else if(cameraStyle == CameraStyle.Locking)
        {
            Vector3 direction = lockOnEnemy.transform.position - _playerTransform.position;

            Vector3 playerDir = direction;
            playerDir.y = 0;

            _playerBodyTransform.forward = Vector3.Slerp(_playerBodyTransform.forward, playerDir.normalized, Time.fixedDeltaTime * _rotationSpeed);
            //_playerOrientationTransform.LookAt(lockOnEnemy.transform.position);
            _playerTransform.forward = Vector3.Slerp(_playerTransform.forward, playerDir.normalized, Time.fixedDeltaTime * _rotationSpeed);
            _playerOrientationTransform.forward = Vector3.Slerp(_playerOrientationTransform.forward, direction.normalized, Time.fixedDeltaTime * _rotationSpeed);
            //_lockingCamera.LookAt(lockOnEnemy.transform.position);

            /*Vector3 directionToFightingLookAt = fightingLookAt.position - new Vector3(transform.position.x, fightingLookAt.position.y, transform.position.z);
            playerOrientation.forward = directionToFightingLookAt.normalized;

            playerBody.forward = directionToFightingLookAt.normalized;*/
        } 
    }
    #endregion

    #region Defini Lock-On
    public GameObject BestTargetInView(Transform playerPosition, float radius, LayerMask enemyMask)
    {
        GameObject bestEnemy = null;
        float bestScore = -1f;

        Collider[] enemies = Physics.OverlapSphere(playerPosition.position, radius, enemyMask);
        if(enemies.Length == 0)
        {
            return null;
        }

        Vector3 camForward = transform.forward;

        foreach (var enemyCollider in enemies)
        {
            Vector3 enemyPoint = enemyCollider.bounds.center;
            float cameraToEnemy = Vector3.Distance(transform.position, enemyPoint);

            Vector3 rayOrigin = transform.position + transform.forward * 0.1f;
            Vector3 dirToEnemy = (enemyPoint - rayOrigin).normalized;


            Ray ray = new Ray(rayOrigin, dirToEnemy);

            Debug.DrawRay(rayOrigin, dirToEnemy * cameraToEnemy, Color.red, 2f);
            Debug.DrawLine(transform.position, enemyPoint, Color.green, 2f);
            Debug.DrawRay(transform.position, transform.forward * 3f, Color.blue, 2f);

            if(Physics.Raycast(ray, out RaycastHit hit, cameraToEnemy))
            {
                if(hit.collider.gameObject == enemyCollider.gameObject)
                {
                    float dot = Vector3.Dot(camForward, dirToEnemy);

                    if(dot < 0.05f)
                    {
                        continue;
                    }

                    float distanceToEnemy = Vector3.Distance(_playerTransform.position, enemyPoint);
                    float score = dot - (distanceToEnemy * 0.01f);

                    if(score > bestScore)
                    {
                        bestScore = score;
                        bestEnemy = enemyCollider.gameObject;
                    }
                }
            }
        }

        if(bestEnemy == null)
        {
            return null;
        }
        
        Debug.Log(bestEnemy.name);
        return bestEnemy;
    }

    /*
    public GameObject BestTargetInView(Transform playerPosition, float radius, LayerMask enemyMask)
    {
        GameObject _nearestEnemy = null;
        float _nearestDistance = float.MaxValue;
        float _distance;

        Collider[] enemies = Physics.OverlapSphere(playerPosition.position, radius, enemyMask);
        if(enemies.Length == 0)
        {
            return null;
        }

        Vector3 camForward = transform.forward;

        foreach (var enemyCollider in enemies)
        {
            Vector3 dirToEnemy = (enemyCollider.transform.position - transform.position).normalized;

            Vector3 _offset = enemyCollider.transform.position - playerPosition.position;
            _distance = _offset.sqrMagnitude;

            if(_distance < _nearestDistance)
            {
                _nearestDistance = _distance;
                _nearestEnemy = enemyCollider.gameObject;
            }
        }
        Debug.Log(_nearestEnemy.name);
        

        return _nearestEnemy.gameObject;

        //var target = lockingCamera.Target;

        //target.LookAtTarget = _nearestEnemy.transform;
    }
    */
    #endregion
}
