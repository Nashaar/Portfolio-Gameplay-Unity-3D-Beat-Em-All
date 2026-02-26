using Unity.Cinemachine;
using UnityEngine;

public class CameraMode : MonoBehaviour
{
    #region Declarations
    [Header("References")]
    [SerializeField] private ThirdPersonCam _thirdPersonCam;
    [SerializeField] private InputController _inputController;
    [SerializeField] private CinemachineCamera _walkingCamera;
    [SerializeField] private CinemachineCamera _lockingCamera;
    [SerializeField] private Transform _playerTransform;

    [Space]

    [Header("Variables")]
    [SerializeField] private LayerMask _scanMask;
    [SerializeField] private LayerMask _visibilityMask;
    [SerializeField] private float _scanRadius;
    private bool _isWalking, _isLocking;
    #endregion

    #region Unity Functions
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isWalking = true;
        _isLocking = false;

        _thirdPersonCam.cameraStyle = ThirdPersonCam.CameraStyle.Walking;
        _walkingCamera.Priority = 1;
        _lockingCamera.Priority = 0;
    }

    // Update is called once per frame
    void Update()
    {
        DefineCameraMode();
    }
    #endregion

    #region Cam mode functions
    private void DefineCameraMode()
    {
        if(!_isWalking && _inputController.InputPressed(_inputController.lockAction) && _isLocking)
        {
            _isWalking = true;
            _isLocking = false;

            _thirdPersonCam.cameraStyle = ThirdPersonCam.CameraStyle.Walking;

            _walkingCamera.Priority = 1;
            _lockingCamera.Priority = 0;
        }
        else if (_isWalking && _inputController.InputPressed(_inputController.lockAction))
        {
            _isWalking = false;
            _isLocking = true;

            _thirdPersonCam.lockOnEnemy = _thirdPersonCam.BestTargetInView(_playerTransform, _scanRadius, _scanMask, _visibilityMask);

            if(_thirdPersonCam.lockOnEnemy == null)
            {
                _isWalking = true;
                _isLocking = false;

                _thirdPersonCam.cameraStyle = ThirdPersonCam.CameraStyle.Walking;

                _walkingCamera.Priority = 1;
                _lockingCamera.Priority = 0;
                return;
            }

            _thirdPersonCam.cameraStyle = ThirdPersonCam.CameraStyle.Locking;

            _walkingCamera.Priority = 0;
            _lockingCamera.Priority = 1;
        }
        else
        {
            return;
        }
    }
    #endregion
}
