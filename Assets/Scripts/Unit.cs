using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

abstract public class Unit : MonoBehaviour
{
    #region Static

    static HexPathfinder pathfinder = new HexPathfinder();

    static public Unit Create(Unit prefab, HexCell cell)
    {
        Unit obj = Instantiate(prefab);
        obj.Initialize(cell);
        return obj;
    }

    #endregion

    public string UnitName { get { return unitName; } }

    [SerializeField] string unitName;

    [SerializeField] UnitAnimation unitAnimation;

    protected HexCell Location {
        get {
            return _location;
        }
        set {
            if (_location != null) {
                _location.units.Remove(this);
            }
            _location = value;
            _location.units.Add(this);
        }
    }
    HexCell _location;

    protected bool IsMoving {
        get {
            return _isMoving;
        }
        set {
            if (value != _isMoving) {
                var animationType = value ? UnitAnimationType.MOVE : UnitAnimationType.IDLE;
                unitAnimation.SetAnimation(animationType);
                _isMoving = value;
            }
        }
    }
    bool _isMoving;


    protected virtual void Awake()
    {
        var meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        meshCollider.sharedMesh = GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        //IsMoving = false;
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
        Select();
    }

    public virtual void Select()
    {
        var material = GetComponentInChildren<SkinnedMeshRenderer>().material;
        var originalColor = material.color;
        material.color = Color.green;
        UIInGame.ActiveInGameUI.ShowUI(UnitName, "A Unit!", () => { material.color = originalColor; onBlur(); });
    }

    protected virtual void onBlur() { }

    protected IEnumerator TravelToCell(IList<HexCell> path)
    {
        foreach (var cell in path.Skip(1)) {
            var lastLocation = Location;
            Location = cell;
            var travelSpeed = .5f;
            for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed) {
                var rotation = t * 2;
                if (rotation < 1f) {
                    Quaternion fromRotation = transform.localRotation;
                    Quaternion toRotation =
                        Quaternion.LookRotation(cell.Center - transform.localPosition);
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

    protected IList<HexCell> GetNewTravelPath()
    {
        return pathfinder.FindNearest(
            Location,
            c => c != Location &&
            c.Coordinates.DistanceTo(Location.Coordinates) >= 5 &&
            c.GetNeighbors().FirstOrDefault(n => n.Elevation == 0) != null);
    }

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