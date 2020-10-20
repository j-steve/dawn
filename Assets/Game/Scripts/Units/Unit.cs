using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
abstract public class Unit : MonoBehaviour, ISelectable
{
    #region Static

    static public Unit Create(Unit prefab, HexCell cell)
    {
        Unit obj = Instantiate(prefab);
        obj.Initialize(cell);
        return obj;
    }

    static int maxId = 0;

    const float DECOMPOSE_TIME = 10;

    #endregion

    #region Properties & Fields

    protected readonly HexPathfinder pathfinder;

    abstract protected float calculateMovementCost(HexCell c1, HexCell c2);

    abstract protected float TravelSpeed { get; }

    public string UnitName { get { return _unitName; } }
    [SerializeField] protected string _unitName;

    public float AttackPower { get { return _attackPower; } }
    [SerializeField] protected float _attackPower;

    public float MaxHealth { get { return AttackPower * 5; } }

    public float Health { get; private set; }

    public float Hunger {
        get { return _hunger; }
        protected set { _hunger = Mathf.Clamp(value, 0, 100); }
    }
    float _hunger;

    public float Thirst {
        get { return _thirst; }
        protected set { _thirst = Mathf.Clamp(value, 0, 100); }
    }
    float _thirst;

    SkinnedMeshRenderer skinnedMeshRender;

    int id;

    public HexCell Location {
        get { return _location; }
        set {
            if (_location)
                _location.units.Remove(this);
            if (value)
                value.units.Add(this);
            _location = value;
        }
    }
    HexCell _location;

    public bool IsDead { get; private set; }

    protected virtual bool IsMoving {
        get { return _isMoving; }
        set {
            if (value != _isMoving) {
                var animationType = value ? UnitAnimationType.MOVE : UnitAnimationType.IDLE;
                SetAnimation(animationType);
                _isMoving = value;
            }
        }
    }
    bool _isMoving;

    public Unit CombatOpponent { get; private set; }

    Color originalColor;

    #endregion

    public Unit() {pathfinder = new HexPathfinder(calculateMovementCost);}

    protected virtual void Initialize(HexCell cell)
    {
        id = ++maxId;
        name = string.Format("{0} (U{1:000})", UnitName, id);
        Location = cell;
        transform.localPosition = cell.Center;
        transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        Health = MaxHealth;
        Hunger = 100;
        Thirst = 100;
    }

    #region Unity Event Handlers

    protected virtual void Awake()
    {
        // Identify the skinned mesh renderer to change its color when unit is clicked.
        skinnedMeshRender = GetComponentInChildren<SkinnedMeshRenderer>();
        originalColor = skinnedMeshRender.material.color;
        // Add mesh collider so unit can respond to OnMouseDown events.
        var meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = skinnedMeshRender.sharedMesh;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        // Start the idle animation sequence after a random delay.
        StartCoroutine(StartIdleAnimation());
    }

    protected virtual void Update()
    {
        if (IsDead || IsMoving) {
            // Do nothing.
        } else if (CombatOpponent) {
            CombatOpponent.TakeDamage(Time.deltaTime * AttackPower * Random.value);
            if (CombatOpponent == null || CombatOpponent.IsDead) {
                var exOponent = CombatOpponent;
                SetAnimation(UnitAnimationType.IDLE);
                CombatOpponent = null;
            }
        } else {
            TakeAction();
        }
    }

    protected virtual void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            InGameUI.Instance.SetSelected(this);
        }
    }

    /// <summary>
    /// Cleans up all references to the unit upon its removal.
    /// </summary>
    void OnDestroy()
    {
        // Deselect the unit if it is the current selection.
        if (InGameUI.Instance.IsSelected(this)) {
            InGameUI.Instance.SetSelected(null);
        }
        // Remove the current location cell's reference to this unit.
        Location = null;
    }

    #endregion

    protected abstract void TakeAction();

    #region ISelectable

    public virtual void OnFocus(InGameUI ui)
    {
        skinnedMeshRender.material.color = Color.green;
        ui.selectionInfoPanel.SetActive(true);
    }

    public virtual void OnBlur(InGameUI ui)
    {
        skinnedMeshRender.material.color = originalColor;
        ui.selectionInfoPanel.SetActive(false);
    }

    public virtual void OnUpdateWhileSelected(InGameUI ui)
    {
        ui.labelTitle.text = string.Format("{0} ({1:0.0}/{2:0.0})", UnitName, Health, MaxHealth);
    }

    #endregion

    public void AttackedBy(Unit attacker)
    {
        if (IsMoving) {
            IsMoving = false;
            // Ensure centered & level on current tile.
            //transform.localPosition = Location.Center;
            //transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.ScaledBy(Vector3.up));
        }
        CombatOpponent = attacker;
        LookAt(attacker.transform.position);
        SetAnimation(UnitAnimationType.FIGHT);
    }

    /// <summary>
    /// Points the unit at the given transform while keeping the unit level on the tile
    /// (e.g., ignoring the transform's elevation).
    /// </summary>
    protected void LookAt(Vector3 target)
    {
        target.y = transform.position.y;
        transform.LookAt(target);
    }

    public void TakeDamage(float damage)
    {
        if (!IsDead) {
            //Debug.LogFormat(this, "{0} attacked {1} for {2} dmg", this, CombatOpponent, damage);
            Health -= damage;
            if (Health <= 0) {
                Die();
            }
        }
    }

    public void Die()
    {
        //Debug.LogFormat(this, "{0} is dead!", this);
        SetAnimation(UnitAnimationType.DEATH);
        IsDead = true;
        CombatOpponent.CombatOpponent = null;
        Destroy(gameObject, DECOMPOSE_TIME);
    }

    protected IEnumerator TravelToCell(IList<HexCell> path)
    {
        if (path == null || path.Count < 2) {
            Debug.LogWarningFormat(this, "{0} has invalid TravelToCell path (length={1})", name, path.Count);
            yield break;
        }
        IsMoving = true;
        foreach (var cell in path.Skip(1)) {
            // Look towards next cell but then clear any vertical rotation.
            LookAt(cell.Center); 
            //transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.ScaledBy(Vector3.up));

            if (cell.units.Count > 0) {
                Debug.Log("Stopping, next cell occupied!", gameObject);
                yield break;
            }
            Debug.LogFormat("arrowing at {0}", Location);
            var arrow = MovementArrow.Create(Location, cell);
            for (float t = 0f; t < 1f; t += Time.deltaTime * TravelSpeed) {
                arrow.SetCompletion(t);
                yield return null;
            }
            arrow.Remove();
            //yield return AnimateMovement(Location, cell);
            transform.localPosition = cell.Center;

            Location = cell;
        }
        IsMoving = false;
        ArrivedAtCell();
    }

    protected virtual void ArrivedAtCell() { }

    //private IEnumerator AnimateMovement(HexCell fromCell, HexCell toCell)
    //{
    //    for (float t = 0f; t < 1f; t += .1f) {
    //        var rotation = t * 2;
    //        if (rotation < 1f) {
    //            var fromRotation = transform.localRotation;
    //            var toRotation = Quaternion.LookRotation(toCell.Center - transform.localPosition);
    //            transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, rotation);
    //        }
    //        transform.localPosition = Vector3.Lerp(fromCell.Center, toCell.Center, t);
    //        yield return null;
    //        if (!IsMoving) {
    //            Debug.Log("Stopping, attacked!", gameObject);
    //            yield break;
    //        }
    //    }
    //    yield return null;
    //}

    /// <summary>
    /// Wait a short random interval before starting the idle animation loop,
    /// to prevent all units from having the exact same idle animation schedule.
    /// </summary>
    IEnumerator StartIdleAnimation()
    {
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        if (!IsMoving)
            SetAnimation(UnitAnimationType.IDLE);
    }

    static readonly int triggerMoving = Animator.StringToHash("Moving");
    static readonly int triggerIdle = Animator.StringToHash("Idle");
    static readonly int triggerEating = Animator.StringToHash("Eating");
    static readonly int triggerFighting = Animator.StringToHash("Fighting");
    static readonly int triggerDeath = Animator.StringToHash("Death");
    static readonly int triggerWorking = Animator.StringToHash("Working");

    int GetTriggerName(UnitAnimationType animationType)
    {
        switch (animationType) {
            case UnitAnimationType.EAT:
            case UnitAnimationType.DRINK:
                return triggerEating;
            case UnitAnimationType.MOVE:
            case UnitAnimationType.STALK:
                return triggerMoving;
            case UnitAnimationType.FIGHT:
                return triggerFighting;
            case UnitAnimationType.DEATH:
                return triggerDeath;
            case UnitAnimationType.IDLE:
            default:
                return triggerIdle;
        }
    }

    protected virtual void SetAnimation(UnitAnimationType animationType)
    {
        GetComponent<Animator>().SetTrigger(GetTriggerName(animationType));
    }
}