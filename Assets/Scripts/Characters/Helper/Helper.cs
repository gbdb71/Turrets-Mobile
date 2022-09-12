using Assets.Scripts.StateMachine;
using UnityEngine;
using UnityEngine.AI;

public class Helper : MonoBehaviour
{
    [SerializeField] private int _maxAmmo = 10;

    private int _currentAmmo = 0;

    public HelperStateMachine StateMachine { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public bool InventoryFull => _currentAmmo == _maxAmmo;
    public bool InventoyEmpty => _currentAmmo <= 0;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        StateMachine = new HelperStateMachine(this);
    }
    private void Update()
    {
        StateMachine.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_currentAmmo >= _maxAmmo)
            return;

        if (other.CompareTag("Ammunition"))
        {
            if (other.TryGetComponent(out Ammunition ammunition))
            {

                _currentAmmo++;


                //REMOVE: 
                Destroy(other.gameObject);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (_currentAmmo <= 0) return;

        if (other.CompareTag("Turret"))
        {
            if (other.TryGetComponent(out BaseTurret turret))
            {
                turret.Charge();
                _currentAmmo--;
            }
        }

    }
}
