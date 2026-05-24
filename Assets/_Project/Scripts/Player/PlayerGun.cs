using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] private Raycaster _raycaster;
    [SerializeField] private InputActionReference _fireActionReference;
    [SerializeField] private InputActionReference _reloadActionReference;

    [SerializeField] private float _fireRate = 0.1f;
    [SerializeField] private int _maxAmmo = 2;       // Ammo is usually an int
    [SerializeField] private int _currentAmmo = 0;
    [SerializeField] private float _damage = 40;
    [SerializeField] private float _reloadTime = 1.5f; // Temp timer until you get animations
    [SerializeField] private VisualEffect _muzzleFlash;
    [SerializeField] private GameObject _bloodSplatterVFX;

    private float _lastFireTime = 0;
    private bool _isReloading = false; // Replaces 'canfire' for cleaner logic

    private void Start()
    {
        
        _fireActionReference.action.Enable();
        _reloadActionReference.action.Enable();
        _currentAmmo = _maxAmmo;
    }

    private void OnDestroy()
    {
        _fireActionReference.action.Disable();
        _reloadActionReference.action.Disable();
    }

    private void Update()
    {
        // If we are currently reloading, block shooting and further reload inputs
        if (_isReloading) return;

        HandleFire();
        HandleReload();
    }

    private void HandleFire()
    {
        // 1. Check if enough time has passed for the fire rate
        if (Time.time - _lastFireTime >= _fireRate)
        {
            // 2. Did the player press fire?
            if (_fireActionReference.action.WasPressedThisFrame())
            {
                // 3. Do we have ammo?
                if (_currentAmmo > 0)
                {
                    Shoot();
                }
                else
                {
                    // Optional: Auto-reload if they click while empty
                    StartCoroutine(ReloadRoutine());
                }
            }
        }
    }

    private void Shoot()
    {
        _currentAmmo--;
        _muzzleFlash.Play();
        _lastFireTime = Time.time;

        // Removed the invalid _raycaster.HitAnimal != null check
        if (_raycaster.IsHittingAnimal)
        {
            Debug.Log("Hit an animal! Attempting to apply damage...");
            // If you need to be absolutely sure the object still exists before getting the component:
            if (_raycaster.HitAnimal.transform.TryGetComponent<IDamageables>(out var animal))
            {
                Debug.Log("Applying damage to the animal...");
                animal.TakeDamage(_damage);
                if (_bloodSplatterVFX != null)
                {
                    GameObject splatter = Instantiate(_bloodSplatterVFX, _raycaster.HitAnimal.point, Quaternion.identity);
                    Destroy( splatter,2f );
                }
            }
        }
        
    }

    private void HandleReload()
    {
        // Check manual reload input
        if (_reloadActionReference.action.WasPerformedThisFrame() && _currentAmmo < _maxAmmo)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    // Simulates an animation delay. Later, you can remove this Coroutine 
    // and just let an Animation Event call ReloadGun() directly.
    private IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        Debug.Log("Reloading...");

        // Wait for the temporary reload time
        yield return new WaitForSeconds(_reloadTime);

        ReloadGun();
    }

    // You can hook this exact method up to your Animation Events later
    public void ReloadGun()
    {
        _currentAmmo = _maxAmmo;
        _isReloading = false;
        Debug.Log("Reload Complete!");
    }

}
