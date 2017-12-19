using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

abstract public class Unit : MonoBehaviour, ISelectable
{
    #region Static

    static protected HexPathfinder pathfinder = new HexPathfinder();

    static public Unit Create(Unit prefab, HexCell cell)
    {
        Unit obj = Instantiate(prefab);
        obj.Initialize(cell);
        return obj;
    }

    #endregion

    abstract protected float TravelSpeed { get; }

    public string UnitName { get { return _unitName; } }
    [SerializeField] string _unitName;

    public float AttackPower { get { return _attackPower; } }
    [SerializeField] float _attackPower;

    [SerializeField] UnitAnimation unitAnimation;

    SkinnedMeshRenderer skinnedMeshRender;

    protected HexCell Location {
        get { return _location; }
        set {
            if (_location != null)
                _location.units.Remove(this);
            _location = value;
            _location.units.Add(this);
        }
    }
    HexCell _location;

    protected bool IsMoving {
        get { return _isMoving; }
        set {
            if (value != _isMoving) {
                var animationType = value ? UnitAnimationType.MOVE : UnitAnimationType.IDLE;
                unitAnimation.SetAnimation(animationType);
                _isMoving = value;
            }
        }
    }
    bool _isMoving;

    Color originalColor;

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

    protected virtual void Initialize(HexCell cell)
    {

        Location = cell;
        transform.localPosition = cell.Center;
        transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    protected virtual void OnMouseDown()
    {
        Debug.LogFormat("You clicked {0}", name);
        UIInGame.ActiveInGameUI.SetSelected(this);
    }

    #region ISelectable

    public virtual string InGameUITitle {
        get {
            return UnitName;
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

    protected IEnumerator TravelToCell(IList<HexCell> path)
    {
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
            unitAnimation.SetAnimation(UnitAnimationType.IDLE);
        }
    }
}