using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    protected HexPathfinder pathfinder = new HexPathfinder();

    abstract protected float TravelSpeed { get; }

    public string UnitName { get { return _unitName; } }
    [SerializeField] string _unitName;

    public float AttackPower { get { return _attackPower; } }
    [SerializeField] float _attackPower;

    public float MaxHealth { get { return AttackPower * 5; } }

    public float Health { get; private set; }

    [SerializeField] UnitAnimation unitAnimation;

    SkinnedMeshRenderer skinnedMeshRender;

    int id;

    public HexCell Location {
        get { return _location; }
        set {
            if (_location != null)
                _location.units.Remove(this);
            if (value != null)
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

    protected virtual void Awake()
    {
        var meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        skinnedMeshRender = GetComponentInChildren<SkinnedMeshRenderer>();
        meshCollider.sharedMesh = skinnedMeshRender.sharedMesh;
        originalColor = skinnedMeshRender.material.color;
        StartCoroutine(StartIdleAnimation());
    }

    void Update()
    {
        if (IsDead) {
            // Stay dead.
        } else if (CombatOpponent != null) {
            CombatOpponent.TakeDamage(Time.deltaTime * AttackPower * Random.value);
            if (CombatOpponent.IsDead) {
                var exOponent = CombatOpponent;
                CombatOpponent = null;
                SetAnimation(UnitAnimationType.IDLE);
                CombatWon(CombatOpponent);
            }
        } else {
            TakeAction();
        }
    }

    /// <summary>
    /// Cleans up all references to the unit upon its removal.
    /// </summary>
    void OnDestroy()
    {
        if ((object)UIInGame.ActiveInGameUI.selection == this) {
            UIInGame.ActiveInGameUI.SetSelected(null);
        }
        Location = null;
    }

    protected virtual void CombatWon(Unit opponent) { }

    protected abstract void TakeAction();

    protected virtual void Initialize(HexCell cell)
    {
        id = ++maxId;
        name = string.Format("{0} [id={1}]", UnitName, id);
        Location = cell;
        transform.localPosition = cell.Center;
        transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        Health = MaxHealth;
    }

    protected virtual void OnMouseDown()
    {
        Debug.LogFormat("You clicked {0}", name);
        UIInGame.ActiveInGameUI.SetSelected(this);
    }

    #region ISelectable

    public virtual string InGameUITitle {
        get {
            return string.Format("{0} (health: {1:0.0}/{2:0.0})", UnitName, Health, MaxHealth);
        }
    }

    public virtual string InGameUIDescription {
        get {
            return "";
        }
    }

    public virtual void OnFocus()
    {
        skinnedMeshRender.material.color = Color.green;
    }

    public virtual void OnBlur()
    {
        skinnedMeshRender.material.color = originalColor;
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
        transform.LookAt(attacker.transform);
        SetAnimation(UnitAnimationType.FIGHT);
    }

    public void TakeDamage(float damage)
    {
        if (!IsDead) {
            Debug.LogFormat(this, "{0} attacked {1} for {2} dmg", this, CombatOpponent, damage);
            Health -= damage;
            if (Health <= 0) {
                Die();
            }
        }
    }

    public void Die()
    {
        Debug.LogFormat(this, "{0} is dead!", this);
        SetAnimation(UnitAnimationType.DEATH);
        IsDead = true;
        Destroy(gameObject, DECOMPOSE_TIME);
    }

    protected IEnumerator TravelToCell(IList<HexCell> path)
    {
        if (path == null || path.Count < 2) {
            Debug.LogWarningFormat(this, "{0} has invalid TravelToCell path: {0}", name, path);
            yield break;
        }
        IsMoving = true;
        foreach (var cell in path.Skip(1)) {
            var lastLocation = Location;
            Location = cell;
            for (float t = 0f; t < 1f; t += Time.deltaTime * TravelSpeed) {
                var rotation = t * 2;
                if (rotation < 1f) {
                    var fromRotation = transform.localRotation;
                    var toRotation = Quaternion.LookRotation(cell.Center - transform.localPosition);
                    transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, rotation);
                }
                transform.localPosition = Vector3.Lerp(lastLocation.Center, cell.Center, t);
                yield return null;
                if (!IsMoving) {
                    Debug.Log("Stopping, attacked!", gameObject);
                    yield break;
                }
            }
        }
        // Reset any vertical rotation so unit is level on map.
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.ScaledBy(Vector3.up));
        IsMoving = false;
        ArrivedAtCell();
    }

    protected virtual void ArrivedAtCell() { }

    /// <summary>
    /// Wait a short random interval before starting the idle animation loop,
    /// to prevent all units from having the exact same idle animation schedule.
    /// </summary>
    IEnumerator StartIdleAnimation()
    {
        var secs = Random.Range(0f, 1f);
        yield return new WaitForSeconds(secs);
        if (!IsMoving) {
            SetAnimation(UnitAnimationType.IDLE);
        }
    }

    protected void SetAnimation(UnitAnimationType animationType)
    {
        unitAnimation.SetAnimation(animationType);
    }

}