using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Unit : MonoBehaviour
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

    HexCell location {
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

    bool isMoving = false;

    float timeTilDeparture;

    protected virtual void Awake()
    {
        var meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        meshCollider.sharedMesh = GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        SetMovement(false);
    }

    void Initialize(HexCell cell)
    {
        StartCoroutine(StartIdleAnimation());
        location = cell;
        transform.localPosition = cell.Center;
        transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    void OnMouseDown()
    {
        Debug.LogFormat("You clicked {0}", name);
        Select();
    }

    public void Select()
    {
        var material = GetComponentInChildren<SkinnedMeshRenderer>().material;
        var originalColor = material.color;
        material.color = Color.green;
        UIInGame.ActiveInGameUI.ShowUI(UnitName, "A Unit!", () => material.color = originalColor);
    }

    void Update()
    {
        if (!isMoving) {
            timeTilDeparture -= Time.deltaTime;
            if (timeTilDeparture <= 0) {
                var path = GetNewTravelPath();
                if (path != null) {
                    SetMovement(true);
                    StopAllCoroutines();
                    StartCoroutine(TravelToCell(path));
                }
            }
        }
    }

    IEnumerator TravelToCell(IList<HexCell> path)
    {
        foreach (var cell in path.Skip(1)) {
            var lastLocation = location;
            location = cell;
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
        SetMovement(false);
        timeTilDeparture = Random.Range(5f, 20f);
    }

    IList<HexCell> GetNewTravelPath()
    {
        return pathfinder.FindNearest(
            location,
            c => c != location &&
            c.Coordinates.DistanceTo(location.Coordinates) >= 5 &&
            c.GetNeighbors().FirstOrDefault(n => n.Elevation == 0) != null);
    }

    /// <summary>
    /// Wait a short random interval before starting the idle animation loop,
    /// to prevent all units from having the exact same idle animation schedule.
    /// </summary>
    IEnumerator StartIdleAnimation()
    {
        var secs = Random.Range(0f, 2f);
        yield return new WaitForSeconds(secs);
        if (!isMoving) {
            SetMovement(false);
        }
    }

    IEnumerator TriggerMoveAaimation()
    {
        while (true) {
            var secs = Random.Range(0f, 10f);
            yield return new WaitForSeconds(secs);
            SetMovement(!isMoving);
        }
    }

    void SetMovement(bool moving)
    {
        var animationType = moving ? UnitAnimationType.MOVE : UnitAnimationType.IDLE;
        unitAnimation.SetAnimation(animationType);
        isMoving = moving;
    }
}