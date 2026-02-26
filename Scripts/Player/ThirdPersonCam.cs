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
            _playerTransform.forward = Vector3.Slerp(_playerTransform.forward, playerDir.normalized, Time.fixedDeltaTime * _rotationSpeed);
            _playerOrientationTransform.forward = Vector3.Slerp(_playerOrientationTransform.forward, direction.normalized, Time.fixedDeltaTime * _rotationSpeed);
        } 
    }
    #endregion

    #region Defini Lock-On
    public GameObject BestTargetInView(Transform playerPosition, float radius, LayerMask enemyMask, LayerMask collisionMask)
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

            Vector3 rayCameraOrigin = transform.position + transform.forward * 0.1f;
            Vector3 camDirToEnemy = (enemyPoint - rayCameraOrigin).normalized;

            float cameraToEnemy = Vector3.Distance(rayCameraOrigin, enemyPoint);

            Ray camRay = new Ray(rayCameraOrigin, camDirToEnemy);

            if(!Physics.Raycast(camRay, out RaycastHit cameraHit, cameraToEnemy, collisionMask))
            {
                continue;
            }

            if(cameraHit.collider.gameObject != enemyCollider.gameObject)
            {
                continue;
            }

            float dot = Vector3.Dot(camForward, camDirToEnemy);

            if(dot < 0.05f)
            {
                continue;
            }

            float distanceToEnemy = Vector3.Distance(_playerTransform.position, enemyPoint);
            float score = dot - (distanceToEnemy * 0.01f);

            if(score <= bestScore)
            {
                continue;
            }

            bestScore = score;
            bestEnemy = enemyCollider.gameObject;
        }

        if(bestEnemy == null)
        {
            return null;
        }
        
        Debug.Log(bestEnemy.name);
        return bestEnemy;
    }
    #endregion
}
